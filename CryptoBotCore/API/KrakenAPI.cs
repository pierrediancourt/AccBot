﻿
using CryptoBotCore.Models;
using CryptoExchange.Net.Authentication;
using Kraken.Net.Clients;
using Kraken.Net.Objects;
using Microsoft.Extensions.Logging;


namespace CryptoBotCore.API
{
    class KrakenAPI : ICryptoExchangeAPI
    {

        public ILogger Log { get; set; }

        public string pair_quote { get; set; }
        public string pair_base { get; set; }

        public KrakenClient client { get; set; }

        public string withdrawal_keyname { get; set; }

        public KrakenAPI(string pair, string withdrawalKeyName, Dictionary<ExchangeCredentialType, string> credentials, ILogger log)
        {
            this.pair_base = pair.Split('_')[0].ToUpper();
            this.pair_quote = pair.Split('_')[1].ToUpper();

            this.withdrawal_keyname = withdrawalKeyName;


            this.Log = log;

            var key = credentials[ExchangeCredentialType.Kraken_Key];
            var secret = credentials[ExchangeCredentialType.Kraken_Secret];

            client = new KrakenClient(new KrakenClientOptions()
            {
                // Specify options for the client
                ApiCredentials = new ApiCredentials(key, secret)
            });

        }

        private async Task<decimal> getCurrentPrice()
        {
            var callResult = await client.SpotApi.ExchangeData.GetOrderBookAsync($"{pair_base}{pair_quote}", 0);
            // Make sure to check if the call was successful
            if (!callResult.Success)
            {
                // Call failed, check callResult.Error for more info
                throw new Exception(callResult.Error?.Message);
            }
            else
            {
                // Call succeeded, callResult.Data will have the resulting data
                return callResult.Data.Asks.First().Price;
            }
        }

        public async Task<string> buyOrderAsync(decimal amount)
        {
            var baseAmount = amount / (await getCurrentPrice());

            var callResult = await client.SpotApi.Trading.PlaceOrderAsync($"{pair_base}{pair_quote}", 
                                                                            Kraken.Net.Enums.OrderSide.Buy, 
                                                                            Kraken.Net.Enums.OrderType.Market, 
                                                                            quantity: baseAmount);
            // Make sure to check if the call was successful
            if (!callResult.Success)
            {
                // Call failed, check callResult.Error for more info
                throw new Exception(callResult.Error?.Message);
            }
            else
            {
                // Call succeeded, callResult.Data will have the resulting data
                return callResult.Data.OrderIds.First();
            }
        }

        public async Task<List<WalletBalances>> getBalancesAsync()
        {
            var callResult = await client.SpotApi.Account.GetAvailableBalancesAsync();
            // Make sure to check if the call was successful
            if (!callResult.Success)
            {
                // Call failed, check callResult.Error for more info
                throw new Exception(callResult.Error?.Message);
            }
            else
            {

                var wallets = new List<WalletBalances>();
                // Call succeeded, callResult.Data will have the resulting data
                var balancesQuote = callResult.Data[this.pair_quote];
                var balancesBase = callResult.Data[this.pair_base];

                wallets.Add(new WalletBalances(this.pair_quote, balancesQuote.Available));
                wallets.Add(new WalletBalances(this.pair_base, balancesBase.Available));

                return wallets;
            }
        }

        public async Task<decimal> getTakerFee()
        {
            var callResult = await client.SpotApi.ExchangeData.GetSymbolsAsync(new List<string> { $"{pair_base}{pair_quote}" });

            // Make sure to check if the call was successful
            if (!callResult.Success)
            {
                // Call failed, check callResult.Error for more info
                throw new Exception(callResult.Error?.Message);
            }
            else
            {
                // Call succeeded, callResult.Data will have the resulting data
                var takerFee = callResult.Data[$"{pair_base}{pair_quote}"].Fees.Where(x => x.Volume == 0).First().FeePercentage;
                return takerFee;
            }
        }

        public async Task<decimal> getWithdrawalFeeAsync(decimal? amount = null, string? destinationAddress = null)
        {
            var callResult = await client.SpotApi.Account.GetWithdrawInfoAsync(this.pair_base, this.withdrawal_keyname, (amount??0m));

            // Make sure to check if the call was successful
            if (!callResult.Success)
            {
                // Call failed, check callResult.Error for more info
                throw new Exception(callResult.Error?.Message);
            }
            else
            {
                // Call succeeded, callResult.Data will have the resulting data
                var withdrawInfo = callResult.Data.Fee;
                return withdrawInfo;
            }
        }

        public async Task<WithdrawalStateEnum> withdrawAsync(decimal amount, string destinationAddress)
        {
            var callResult = await client.SpotApi.Account.WithdrawAsync(this.pair_base, this.withdrawal_keyname, amount);

            // Make sure to check if the call was successful
            if (!callResult.Success)
            {
                // Call failed, check callResult.Error for more info
                throw new Exception(callResult.Error?.Message);
            }
            else
            {
                // Call succeeded, callResult.Data will have the resulting data
                return WithdrawalStateEnum.OK;
            }
        }
    }
}

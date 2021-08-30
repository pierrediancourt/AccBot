﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoBotCore.Models
{
    public enum CryptoExchangeAPIEnum
    {
        Coinmate = 1, Binance = 2, Huobi = 3, Coinbase = 4, Kraken = 5, FTX = 6
    }

    public enum ExchangeCredentialType
    {
        Coinmate_ClientId = 1, Coinmate_PublicKey = 2, Coinmate_PrivateKey = 3,
        Huobi_Key = 4, Huobi_Secret = 5, 
        Binance_Key = 6, Binance_Secret = 7, 
        Kraken_Key = 8, Kraken_Secret = 9,
        FTX_Key = 10, FTX_Secret = 11
    }

    public enum WithdrawalStateEnum
    {
        OK = 1, InsufficientKeyPrivilages = 2, UNKNOWN_FAIL = 3
    }


    public enum OrderType
    {
        MarginMarket,
        MarginLimit,
        MarginStop,
        MarginTrailingStop,
        ExchangeMarket,
        ExchangeLimit,
        ExchangeStop,
        ExchangeTrailingStop
    }
    public enum OrderSide
    {
        Buy,
        Sell
    }
    public enum OrderExchange
    {
        Bitfinex,
        Bitstamp,
        All
    }

    public enum MessageTypeEnum
    {
        Information,
        Warning,
        Error
    }

}

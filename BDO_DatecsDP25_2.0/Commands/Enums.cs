using System;
using System.Collections.Generic;
using System.Globalization;
using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    public enum PaymentMode
    {
        Cash = 0,
        Card = 1,
        Credit = 2,
        Tare = 3
    }

    public enum Cash
    {
        In = 0,
        Out = 1
    }

    public enum FiscalEntryInfoType
    {
        CashDebit = 0,
        CashCredit = 1,
        CashFreeDebit = 2,
        CashFreeCredit = 3
    }

    public enum ReceiptType
    {
        Sale = 0,
        Return = 1
    }

    public enum ReportType
    {
        X = 1,
        Z = 2
    }

    public enum CommandType
    {
        P = 0,
        R = 1
    }

    public enum PriceType
    {
        FixedPrice = 0,
        FreePrice = 1,
        MaxPrice = 2
    }

    public enum TaxGr
    {
        A,
        B,
        C
    }

    public enum TaxCode
    {
        A = 1,
        B = 2,
        C = 3
    }

    public enum DiscountType
    {
        NoDiscount = 0,
        SurchargeByPercentage = 1,
        DiscountByPercentage = 2,
        SurchargeBySum = 3,
        DiscountBySum = 4
    }
}

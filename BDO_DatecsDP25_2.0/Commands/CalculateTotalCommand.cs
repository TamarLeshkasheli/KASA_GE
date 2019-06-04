using System;
using System.Collections.Generic;
using System.Globalization;
using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class CalculateTotalCommand : WrappedMessage
    {
        public CalculateTotalCommand(int paymentMode, decimal cashMoney, int paymentMode2, bool total)
        {
            NumberFormatInfo Nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            var cashMoneyParam = (cashMoney == 0 ? string.Empty : cashMoney.ToString(Nfi));
            Command = 53;
            if (total == false)
                Data = (new object[] { paymentMode, cashMoneyParam, paymentMode2 }).StringJoin("\t");
            else
                Data = (new object[] { paymentMode, cashMoneyParam }).StringJoin("\t");
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
    public enum PaymentMode
    {
        Cash = 0,
        Card = 1,
        Credit = 2,
        Tare = 3
    }
}

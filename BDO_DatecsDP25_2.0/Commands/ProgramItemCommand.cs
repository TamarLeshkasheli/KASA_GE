﻿using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class ProgramItemCommand : WrappedMessage
    {
        public ProgramItemCommand(string name, int plu, TaxGr taxGr, int dep, int group, decimal price, decimal quantity = 9999, PriceType priceType = PriceType.FixedPrice)
        {
            Command = 107;
            Data = (new object[] { "P", plu, taxGr, dep, group, (int)priceType, price, "", quantity, "", "", "", "", name }).StringJoin("\t");
        }
        public override int Command { get; set; }

        public override string Data { get; set; }
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
}

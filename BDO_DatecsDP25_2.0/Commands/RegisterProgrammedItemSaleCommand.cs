﻿using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class RegisterProgrammedItemSaleCommand : WrappedMessage
    {
        public RegisterProgrammedItemSaleCommand(int pluCode, decimal qty)
        {
            Command = 58;
            Data = (new object[] { pluCode, qty, string.Empty, string.Empty, string.Empty }).StringJoin("\t");
        }
        public RegisterProgrammedItemSaleCommand(int pluCode, decimal qty, decimal price, int discountType, decimal discountValue)
        {
            Command = 58;
            Data = (new object[] { pluCode, qty, price, discountType, discountValue }).StringJoin("\t");
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

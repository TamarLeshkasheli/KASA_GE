using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class CashInCashOutCommand : WrappedMessage
    {
        public CashInCashOutCommand(int type, decimal amount)
        {
            Command = 70;
            Data = (new object[] { type, amount }).StringJoin("\t");
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }

    public enum Cash
    {
        In = 0,
        Out = 1
    }
}

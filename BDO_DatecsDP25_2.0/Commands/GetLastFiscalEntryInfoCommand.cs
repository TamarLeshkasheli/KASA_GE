using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class GetLastFiscalEntryInfoCommand : WrappedMessage
    {
        public GetLastFiscalEntryInfoCommand(int type)
        {
            Command = 64;
            Data = type + "\t";
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
    public enum FiscalEntryInfoType
    {
        CashDebit = 0,
        CashCredit = 1,
        CashFreeDebit = 2,
        CashFreeCredit = 3
    }
}

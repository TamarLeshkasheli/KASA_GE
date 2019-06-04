using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class OperatorsReportCommand : WrappedMessage
    {
        public OperatorsReportCommand(string firstOper, string lastOper, string clear)
        {
            Command = 105;
            Data = (new object[] { firstOper, lastOper, clear }).StringJoin("\t");
        }

        public override int Command { get; set; }

        public override string Data { get; set; }
    }
}

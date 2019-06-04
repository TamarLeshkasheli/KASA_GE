using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class PrintReportCommand : WrappedMessage
    {
        public PrintReportCommand(string type)
        {
            Command = 69;
            Data = type + "\t";
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }

    public enum ReportType
    {
        X = 1,
        Z = 2
    }
}

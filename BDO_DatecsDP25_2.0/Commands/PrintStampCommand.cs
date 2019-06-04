using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class PrintStampCommand : WrappedMessage
    {
        public PrintStampCommand(int type, string name)
        {
            Command = 127;
            Data = (new object[] { type, name }).StringJoin("\t");
        }

        public override int Command { get; set; }
        public override string Data { get; set; }
    }
    public enum CommandType
    {
        P = 0,
        R = 1
    }
}

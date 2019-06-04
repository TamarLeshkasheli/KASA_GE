using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class ProgrammingCommand : WrappedMessage
    {
        public ProgrammingCommand(string name, string index, string value)
        {
            Command = 255;
            Data = (new object[] { name, index, value }).StringJoin("\t");
        }

        public override int Command { get; set; }

        public override string Data { get; set; }
    }
}

using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class ReadErrorCommand : WrappedMessage
    {
        public ReadErrorCommand(string errorCode)
        {
            Command = 100;
            Data = errorCode + "\t";
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class ReadDateTimeCommand : WrappedMessage
    {
        public ReadDateTimeCommand()
        {
            Command = 62;
            Data = string.Empty;
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

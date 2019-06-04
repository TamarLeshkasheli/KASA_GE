using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class SubTotalCommand : WrappedMessage
    {
        public SubTotalCommand()
        {
            Command = 51;
            Data = string.Empty;
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

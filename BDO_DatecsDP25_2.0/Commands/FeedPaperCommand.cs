using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class FeedPaperCommand : WrappedMessage
    {
        public FeedPaperCommand(int lines)
        {
            Command = 44;
            Data = lines + "\t";
        }

        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class PlaySoundCommand : WrappedMessage
    {
        public PlaySoundCommand(int frequency, int interval)
        {
            Command = 80;
            Data = (new object[] { frequency, interval }).StringJoin("\t");
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

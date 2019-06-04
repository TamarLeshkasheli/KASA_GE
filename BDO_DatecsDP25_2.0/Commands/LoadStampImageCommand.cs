using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;


namespace BDO_DatecsDP25.Commands
{
    internal class LoadStampImageCommand : WrappedMessage
    {
        public LoadStampImageCommand(string Parameter)
        {
            Command = 203;
            Data = Parameter  + "\t";
        }

        public override int Command { get; set; }

        public override string Data { get; set; }
    }
   
}

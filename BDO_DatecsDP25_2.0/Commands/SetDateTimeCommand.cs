using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;
using System;

namespace BDO_DatecsDP25.Commands
{
    internal class SetDateTimeCommand : WrappedMessage
    {
        public SetDateTimeCommand(DateTime dateTime)
        {
            Command = 61;
            Data = dateTime.ToString("dd-MM-yy HH:mm:ss") + "\t";
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

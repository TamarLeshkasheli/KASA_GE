﻿using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class OpenDrawerCommand : WrappedMessage
    {
        public OpenDrawerCommand(int impulseLength)
        {
            Command = 106;
            Data = impulseLength + "\t";
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

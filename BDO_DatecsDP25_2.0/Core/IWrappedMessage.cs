using System;
using System.Collections.Generic;
using System.Text;

namespace BDO_DatecsDP25.Core
{
    public interface IWrappedMessage
    {
        int Command { get; set; }
        string Data { get; set; }
        byte[] GetBytes(int sequence);
    }
}

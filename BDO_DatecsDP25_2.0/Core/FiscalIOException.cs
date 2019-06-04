using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BDO_DatecsDP25.Core
{
    public class FiscalIOException : IOException
    {
        public FiscalIOException(string message) : base(message)
        {
        }
    }
}

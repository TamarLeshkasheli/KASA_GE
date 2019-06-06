using BDO_DatecsDP25.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDO_DatecsDP25.Responses
{
    public class EmptyFiscalResponse : FiscalResponse
    {
        public EmptyFiscalResponse(FP700Result buffer) : base(buffer) { }
    }
}

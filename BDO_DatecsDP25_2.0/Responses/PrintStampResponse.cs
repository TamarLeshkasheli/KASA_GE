using BDO_DatecsDP25.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BDO_DatecsDP25.Responses
{
    public class PrintStampResponse : FiscalResponse
    {
        public PrintStampResponse(FP700Result buffer)
            : base(buffer)
        {
            var values = GetDataValues();
            if (values.Length == 0) return;
        }

       
    }
}

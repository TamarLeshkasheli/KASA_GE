using BDO_DatecsDP25.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BDO_DatecsDP25.Responses
{
    public class LoadStampImageResponse : FiscalResponse
    {
        public LoadStampImageResponse(FP700Result buffer)
            : base(buffer)
        {
            var values = GetDataValues();
            if (values.Length == 0) return;

            if (values.Length == 1)
                Error = values[0];
            else
            {
                Error = values[0];
                Chechsum = int.Parse(values[1]);
            }
        }

        /// <summary>
        /// Chechsum - Sum of decoded base64 data
        /// </summary>
        public int Chechsum { get; set; }
        public string Error { get; set; }

    }
}
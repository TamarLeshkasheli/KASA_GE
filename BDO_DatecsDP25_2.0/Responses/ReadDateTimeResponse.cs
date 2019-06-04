using BDO_DatecsDP25.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BDO_DatecsDP25.Responses
{
    public class ReadDateTimeResponse : FiscalResponse
    {
        public ReadDateTimeResponse(byte[] buffer) : base(buffer)
        {
            var values = GetDataValues();
            if (values.Length == 0) return;
            DateTime = DateTime.ParseExact(values[0], "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Current Date and time in ECR
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}

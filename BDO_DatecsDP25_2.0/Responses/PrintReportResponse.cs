using BDO_DatecsDP25.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BDO_DatecsDP25.Responses
{
    public class PrintReportResponse : FiscalResponse
    {
        public PrintReportResponse(FP700Result buffer) : base(buffer)
        {
            var values = GetDataValues();
            if (values.Length == 0) return;
            nRep = int.Parse(values[0]);
            TotX = decimal.Parse(values[1], CultureInfo.InvariantCulture);
            TotNegX = decimal.Parse(values[2], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Number of Z-report 
        /// </summary>
        public int nRep { get; set; }
        /// <summary>
        /// Turnovers of VAT group X in debit (cash and cashfree) receipts 
        /// </summary>
        public decimal TotX { get; set; }
        /// <summary>
        /// Turnovers of VAT group X in credit (cash and cashfree) receipts 
        /// </summary>
        public decimal TotNegX { get; set; }
    }
}

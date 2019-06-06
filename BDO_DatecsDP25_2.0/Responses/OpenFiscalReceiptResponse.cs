using BDO_DatecsDP25.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDO_DatecsDP25.Responses
{
    public class OpenFiscalReceiptResponse : FiscalResponse
    {
        public OpenFiscalReceiptResponse(FP700Result buffer) : base(buffer)
        {
            var values = GetDataValues();
            if (values.Length == 0) return;
            int result;
            if (int.TryParse(values[0], out result))
                SlipNumber = result;
            if (int.TryParse(values[1], out result))
                DocNumber = result;
        }
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt 
        /// </summary>
        public int SlipNumber { get; set; }
        /// <summary>
		/// Global number of all documents 
		/// </summary>
		public int DocNumber { get; set; }
    }
}

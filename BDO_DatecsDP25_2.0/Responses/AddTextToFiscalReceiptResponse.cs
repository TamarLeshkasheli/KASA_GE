using BDO_DatecsDP25.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDO_DatecsDP25.Responses
{
    public class AddTextToFiscalReceiptResponse : FiscalResponse
    {
        public AddTextToFiscalReceiptResponse(FP700Result buffer) : base(buffer)
        {
            var values = GetDataValues();
            if (values.Length == 0) return;
            SlipNumber = int.Parse(values[0]);
            DocNumber = int.Parse(values[1]);
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

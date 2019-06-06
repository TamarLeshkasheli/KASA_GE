using BDO_DatecsDP25.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BDO_DatecsDP25.Core
{
    public abstract class FiscalResponse : IFiscalResponse
    {
        protected FiscalResponse(FP700Result result)
        {
            if (result.data[0] != 0x30)
                ErrorCode = result.data.GetString();
            else
                Data = result.data.Skip(2).Take(result.data.Length - 2).ToArray();
        }

        public bool CommandPassed { get { return string.IsNullOrEmpty(ErrorCode); } }
        public string ErrorCode { get; set; }
        protected byte[] Data { get; set; }
        protected string[] GetDataValues()
        {
            if (CommandPassed)
                return Data.GetString().Split('\t');
            return new string[0];
        }
    }
}

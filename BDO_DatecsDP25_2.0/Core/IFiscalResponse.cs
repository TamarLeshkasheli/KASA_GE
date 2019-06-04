using System;
using System.Collections.Generic;
using System.Text;

namespace BDO_DatecsDP25.Core
{
    public interface IFiscalResponse
    {
        bool CommandPassed { get; }
        string ErrorCode { get; set; }
    }
}

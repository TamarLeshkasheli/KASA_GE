using BDO_DatecsDP25.Core;

namespace BDO_DatecsDP25.Commands
{
    internal class VoidOpenFiscalReceiptCommand : WrappedMessage
    {
        public VoidOpenFiscalReceiptCommand()
        {
            Command = 60;
            Data = string.Empty;
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

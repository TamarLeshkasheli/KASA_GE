using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class AddTextToFiscalReceiptCommand : WrappedMessage
    {
        public AddTextToFiscalReceiptCommand(string text)
        {
            Command = 54;
            Data = text + "\t";
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

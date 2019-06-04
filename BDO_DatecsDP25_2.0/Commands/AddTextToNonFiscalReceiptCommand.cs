using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Utils;

namespace BDO_DatecsDP25.Commands
{
    internal class AddTextToNonFiscalReceiptCommand : WrappedMessage
    {
        public AddTextToNonFiscalReceiptCommand(string text)
        {
            Command = 42;
            Data = text + "\t";
        }
        public override int Command { get; set; }
        public override string Data { get; set; }
    }
}

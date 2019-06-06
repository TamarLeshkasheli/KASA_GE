using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using BDO_DatecsDP25.Commands;
using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Responses;

namespace BDO_DatecsDP25
{
    /// <summary>
    /// ECR Device DP25 Implementation (API)
    /// </summary>
    public class Dp25 : IDisposable
    {
        private SerialPort _port;
        private int _sequence = 32;
        private bool _innerReadStatusExecuted;
        private readonly Queue<byte> _queue;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="portName"></param>
        public Dp25(string portName)
        {
            _queue = new Queue<byte>();
            _port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            _port.Open();
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_port == null) return;
            if (_port.IsOpen)
                _port.Close();
            _port.Dispose();
            _port = null;
        }


        private bool ReadByte()
        {
            var b = _port.ReadByte();
            _queue.Enqueue((byte)b);
            return b != 0x03;
        }

        private IFiscalResponse SendMessage(IWrappedMessage msg, Func<byte[], IFiscalResponse> responseFactory)
        {
            if (_innerReadStatusExecuted)
                return _SendMessage(msg, responseFactory);

            _SendMessage(new ReadStatusCommand(), bytes => null);
            _innerReadStatusExecuted = true;
            return _SendMessage(msg, responseFactory);
        }

        private IFiscalResponse _SendMessage(IWrappedMessage msg, Func<byte[], IFiscalResponse> responseFactory)
        {
            IFiscalResponse response = null;
            byte[] lastStatusBytes = null;
            var packet = msg.GetBytes(_sequence);
            for (var r = 0; r < 3; r++)
            {
                try
                {
                    _port.Write(packet, 0, packet.Length);
                    var list = new List<byte>();

                    while (ReadByte())
                    {
                        var b = _queue.Dequeue();
                        if (b == 22)
                        {
                            continue;
                        }
                        if (b == 21)
                            throw new IOException("Invalid packet checksum or form of messsage.");
                        list.Add(b);
                    }

                    list.Add(_queue.Dequeue());
                    var buffer = list.ToArray();
                    response = responseFactory(buffer);
                    lastStatusBytes = list.Skip(list.IndexOf(0x04) + 1).Take(6).ToArray();
                    break;
                }
                catch (Exception)
                {
                    if (r >= 2)
                        throw;
                    _queue.Clear();
                }
            }
            _sequence += 1;
            if (_sequence > 254)
                _sequence = 32;
            if (msg.Command != 74)
                CheckStatusOnErrors(lastStatusBytes);
            return response;
        }

        private void CheckStatusOnErrors(byte[] statusBytes)
        {
            if (statusBytes == null)
                throw new ArgumentNullException("statusBytes");
            if (statusBytes.Length == 0)
                throw new ArgumentException("Argument is empty collection", "statusBytes");
            if ((statusBytes[0] & 0x20) > 0)
                throw new FiscalIOException("General error - this is OR of all errors marked with #");
            if ((statusBytes[0] & 0x2) > 0)
                throw new FiscalIOException("# Command code is invalid.");
            if ((statusBytes[0] & 0x1) > 0)
                throw new FiscalIOException("# Syntax error.");
            if ((statusBytes[1] & 0x2) > 0)
                throw new FiscalIOException("# Command is not permitted.");
            if ((statusBytes[1] & 0x1) > 0)
                throw new FiscalIOException("# Overflow during command execution.");
            if ((statusBytes[2] & 0x1) > 0)
                throw new FiscalIOException("# End of paper.");
            if ((statusBytes[4] & 0x20) > 0)
                throw new FiscalIOException(" OR of all errors marked with ‘*’ from Bytes 4 and 5.");
            if ((statusBytes[4] & 0x10) > 0)
                throw new FiscalIOException("* Fiscal memory is full.");
            if ((statusBytes[4] & 0x1) > 0)
                throw new FiscalIOException("* Error while writing in FM.");
        }


        /// <summary> 
        /// Changes port name at runtime. 
        /// </summary> 
        /// <param name="portName">Name of the serial port.</param> 
        public void ChangePort(string portName)
        {
            _port.Close();
            _port.PortName = portName;
            _port.Open();
        }



        /// <summary>
        /// Executes custom command implementation and returns predefined custom response.
        /// </summary>
        /// <typeparam name="T">Response Type. Must be a child of an abstract class FiscalResponse</typeparam>
        /// <param name="cmd">Command to execute. Must be a child of an abstract class WrappedMessage</param>
        /// <returns>T</returns>
        public T ExecuteCustomCommand<T>(WrappedMessage cmd) where T : FiscalResponse
        {
            return (T)SendMessage(cmd,
                bytes => (FiscalResponse)Activator.CreateInstance(typeof(T), new object[] { bytes }));
        }

        private Fp700Printer printer;

        #region NonFiscalCommands
        /// <summary>
        /// Opens non fiscal text receipt.
        /// </summary>
        /// <returns>CommonFiscalResponse</returns>
        public CommonFiscalResponse OpenNonFiscalReceipt()
        {
            return (CommonFiscalResponse)SendMessage(new OpenNonFiscalReceiptCommand()
                , bytes => new CommonFiscalResponse(bytes));
        }

        /// <summary>
        /// Printing of free text in a non-fiscal text receipt
        /// </summary>
        /// <param name="text">Up to 30 symbols.</param>
        /// <returns></returns>
        public CommonFiscalResponse AddTextToNonFiscalReceipt(string text)
        {
            return (CommonFiscalResponse)SendMessage(new AddTextToNonFiscalReceiptCommand(text)
                , bytes => new CommonFiscalResponse(bytes));
        }

        /// <summary>
        /// Closes non fiscal text receipt.
        /// </summary>
        /// <returns>CommonFiscalResponse</returns>
        public CommonFiscalResponse CloseNonFiscalReceipt()
        {
            return (CommonFiscalResponse)SendMessage(new CloseNonFiscalReceiptCommand()
                , bytes => new CommonFiscalResponse(bytes));
        }

        #endregion

        #region FiscalCommands



        public SubTotalResponse SubTotal()
        {
            return (SubTotalResponse)SendMessage(new SubTotalCommand(), bytes => new SubTotalResponse(bytes));
        }

        /// <summary>
        /// Opens Sales Fiscal Receipt
        /// </summary>
        /// <param name="opCode">Operator code</param>
        /// <param name="opPwd">Operator password</param>
        /// <returns>OpenFiscalReceiptResponse</returns>
        public OpenFiscalReceiptResponse OpenFiscalReceipt(string opCode, string opPwd)
        {
            return (OpenFiscalReceiptResponse)SendMessage(new OpenFiscalReceiptCommand(opCode, opPwd)
                , bytes => new OpenFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Opens Fiscal Receipt
        /// </summary>
        /// <param name="opCode">Operator code</param>
        /// <param name="opPwd">Operator password</param>
        /// <param name="type">Receipt type</param>
        /// <returns>OpenFiscalReceiptResponse</returns>
        public OpenFiscalReceiptResponse OpenFiscalReceipt(string opCode, string opPwd, ReceiptType type)
        {
            return (OpenFiscalReceiptResponse)SendMessage(new OpenFiscalReceiptCommand(opCode, opPwd, (int)type)
                , bytes => new OpenFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Opens Fiscal Receipt
        /// </summary>
        /// <param name="opCode">Operator code</param>
        /// <param name="opPwd">Operator password</param>
        /// <param name="type">Receipt type</param>
        /// <param name="tillNumber">Number of point of sale (1...999). Default: Logical number of the ECR in the workplace; </param>
        /// <returns>OpenFiscalReceiptResponse</returns>
        public OpenFiscalReceiptResponse OpenFiscalReceipt(string opCode, string opPwd, ReceiptType type, int tillNumber)
        {
            return (OpenFiscalReceiptResponse)SendMessage(new OpenFiscalReceiptCommand(opCode, opPwd, (int)type, tillNumber)
                , bytes => new OpenFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluName">Item name (up to 32 symbols)</param>
        /// <param name="price">Product price. With sign '-' at void operations;</param>
        /// <param name="departmentNumber">Between 1 and 16.</param>
        /// <param name="quantity"> Quantity. NOTE: Max value: {Quantity} * {Price} is 9999999.99</param>
        /// <param name="taxCode">Optional Parameter. Tax code: 1-A, 2-B, 3-C; default = TaxCode.A</param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterSale(string pluName, decimal price, decimal quantity, int departmentNumber, TaxCode taxCode = TaxCode.A)
        {
            return (RegisterSaleResponse)SendMessage(
                new RegisterSaleCommand(pluName
                                        , (int)taxCode
                                        , price
                                        , departmentNumber
                                        , quantity)
                , bytes => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluName">Item name (up to 32 symbols)</param>
        /// <param name="price">Product price. With sign '-' at void operations;</param>
        /// <param name="departmentNumber">Between 1 and 16.</param>
        /// <param name="quantity"> Quantity. NOTE: Max value: {Quantity} * {Price} is 9999999.99</param>
        /// <param name="discountType">Type of the discount.</param>
        /// <param name="discountValue">Discount Value. Percentage ( 0.00 - 100.00 ) for percentage operations; Amount ( 0.00 - 9999999.99 ) for value operations; Note: If {DiscountType} is given, {DiscountValue} must contain value. </param>
        /// <param name="taxCode">Optional Parameter. Tax code: 1-A, 2-B, 3-C; default = TaxCode.A</param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterSale(string pluName, decimal price, decimal quantity, int departmentNumber, DiscountType discountType, decimal discountValue, TaxCode taxCode = TaxCode.A)
        {
            return (RegisterSaleResponse)SendMessage(
                new RegisterSaleCommand(pluName
                                        , (int)taxCode
                                        , price
                                        , departmentNumber
                                        , quantity
                                        , (int)discountType
                                        , discountValue)
                , bytes => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluCode">The code of the item (1 - 100000). With sign '-' at void operations; </param>
        /// <param name="quantity"> Quantity of the item (0.001 - 99999.999) </param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterProgrammedItemSale(int pluCode, decimal quantity)
        {
            return (RegisterSaleResponse)SendMessage(new RegisterProgrammedItemSaleCommand(pluCode, quantity)
                , bytes => new RegisterSaleResponse(bytes));
        }
        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluCode">The code of the item (1 - 100000). With sign '-' at void operations; </param>
        /// <param name="price"> Price of the item (0.01 - 9999999.99). Default: programmed price of the item; </param>
        /// <param name="quantity"> Quantity of the item (0.001 - 99999.999) </param>
        /// <param name="discountType">Type of the discount.</param>
        /// <param name="discountValue">Discount Value. Percentage ( 0.00 - 100.00 ) for percentage operations; Amount ( 0.00 - 9999999.99 ) for value operations; Note: If {DiscountType} is given, {DiscountValue} must contain value. </param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterProgrammedItemSale(int pluCode, decimal quantity, decimal price,
            DiscountType discountType, decimal discountValue)
        {
            return (RegisterSaleResponse)SendMessage(
                new RegisterProgrammedItemSaleCommand(pluCode, quantity, price, (int)discountType, discountValue)
                , bytes => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Payments and calculation of the total sum
        /// </summary>
        /// <param name="paymentMode"> Type of payment. Default: 'Cash' </param>
        /// <returns>CalculateTotalResponse</returns>
        public CalculateTotalResponse Total(PaymentMode paymentMode = PaymentMode.Cash)
        {
            return (CalculateTotalResponse)SendMessage(new CalculateTotalCommand((int)paymentMode, 0, (int)PaymentMode.Cash, true)
                , bytes => new CalculateTotalResponse(bytes));
        }

        /// <summary>
		/// Payments and calculation of the total sum
		/// </summary>
		/// <param name="paymentMode"> Type of payment. </param>
        /// <param name="paymentMode"> Amount to pay (0.00 - 9999999.99). Default: the residual sum of the receipt; </param>
		/// <returns>CalculateTotalResponse</returns>
		public CalculateTotalResponse Total(PaymentMode paymentMode1, decimal cashMoney, PaymentMode paymentMode2)
        {
            return (CalculateTotalResponse)SendMessage(new CalculateTotalCommand((int)paymentMode1, cashMoney, (int)paymentMode2, false)
                , bytes => new CalculateTotalResponse(bytes));
        }

        /// <summary>
        /// All void of a fiscal receipt. <br/>
        /// <bold>Note:The receipt will be closed as a non fiscal receipt. The slip number (unique number of the fiscal receipt) will not be increased.</bold>
        /// </summary>
        /// <returns>VoidOpenFiscalReceiptResponse</returns>
        public VoidOpenFiscalReceiptResponse VoidOpenFiscalReceipt()
        {
            return (VoidOpenFiscalReceiptResponse)SendMessage(new VoidOpenFiscalReceiptCommand()
                , bytes => new VoidOpenFiscalReceiptResponse(bytes));
        }


        public AddTextToFiscalReceiptResponse AddTextToFiscalReceipt(string text)
        {
            return (AddTextToFiscalReceiptResponse)SendMessage(new AddTextToFiscalReceiptCommand(text)
                , bytes => new AddTextToFiscalReceiptResponse(bytes));
        }

        /// <summary>
        ///  Closes open fiscal receipt.
        /// </summary>
        /// <returns>CloseFiscalReceiptResponse</returns>
        public CloseFiscalReceiptResponse CloseFiscalReceipt()
        {
            return (CloseFiscalReceiptResponse)SendMessage(new CloseFiscalReceiptCommand()
                , bytes => new CloseFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Get the information on the last fiscal entry.
        /// </summary>
        /// <param name="type">FiscalEntryInfoType. Default: FiscalEntryInfoType.CashDebit</param>
        /// <returns>GetLastFiscalEntryInfoResponse</returns>
        public GetLastFiscalEntryInfoResponse GetLastFiscalEntryInfo(FiscalEntryInfoType type = FiscalEntryInfoType.CashDebit)
        {
            return (GetLastFiscalEntryInfoResponse)SendMessage(new GetLastFiscalEntryInfoCommand((int)type)
                , bytes => new GetLastFiscalEntryInfoResponse(bytes));
        }

        /// <summary>
        /// Cash in and Cash out operations
        /// </summary>
        /// <param name="operationType">Type of operation</param>
        /// <param name="amount">The sum</param>
        /// <returns>CashInCashOutResponse</returns>
        public CashInCashOutResponse CashInCashOutOperation(Cash operationType, decimal amount)
        {
            return (CashInCashOutResponse)SendMessage(new CashInCashOutCommand((int)operationType, amount)
                , bytes => new CashInCashOutResponse(bytes));
        }
        #endregion

        #region other
        /// <summary>
        /// Reads the status of the device.
        /// </summary>
        /// <returns>ReadStatusResponse</returns>
        public ReadStatusResponse ReadStatus()
        {
            return (ReadStatusResponse)SendMessage(new ReadStatusCommand()
                , bytes => new ReadStatusResponse(bytes));
        }

        /// <summary>
        /// Feeds blank paper.
        /// </summary>
        /// <param name="lines">Line Count 1 to 99; default =  1;</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse FeedPaper(int lines = 1)
        {
            return (EmptyFiscalResponse)SendMessage(new FeedPaperCommand(lines)
                , bytes => new EmptyFiscalResponse(bytes));
        }


        /// <summary>
        /// Prints buffer
        /// </summary>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse PrintBuffer()
        {
            return (EmptyFiscalResponse)SendMessage(new FeedPaperCommand(0)
                , bytes => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Reads an error code  explanation from ECR.
        /// </summary>
        /// <param name="errorCode">Code of the error</param>
        /// <returns>ReadErrorResponse</returns>
        public ReadErrorResponse ReadError(string errorCode)
        {
            return (ReadErrorResponse)SendMessage(new ReadErrorCommand(errorCode)
                , bytes => new ReadErrorResponse(bytes));
        }

        /// <summary>
        /// ECR beeps with given interval and frequency.
        /// </summary>
        /// <param name="frequency">in hertzes</param>
        /// <param name="interval">in milliseconds</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse PlaySound(int frequency, int interval)
        {
            return (EmptyFiscalResponse)SendMessage(new PlaySoundCommand(frequency, interval)
                , bytes => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Prints X or Z Report and returns some stats.
        /// </summary>
        /// <param name="type">ReportType</param>
        /// <returns>PrintReportResponse</returns>
        public PrintReportResponse PrintReport(ReportType type)
        {
            return (PrintReportResponse)SendMessage(new PrintReportCommand(type.ToString())
                , bytes => new PrintReportResponse(bytes));
        }

        /// <summary>
        /// 105 (69h) - Operators report
        /// </summary>
        /// <param name="firstOper">First operator in the report ( 1...30 ). Default: 1</param>
        /// <param name="lastOper">Last operator in the report ( 1...30 ). Default: 30</param>
        /// <param name="clear">Type of report. Default: 0. '0' - Operators report, '1' - Operators report with clearing the operators registers</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse OperatorsReport(string firstOper = "1", string lastOper = "30", string clear = "0")
        {
            return (EmptyFiscalResponse)SendMessage(new OperatorsReportCommand(firstOper, lastOper, clear)
                , bytes => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Opens the cash drawer if such is connected.
        /// </summary>
        /// <param name="impulseLength"> The length of the impulse in milliseconds. </param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse OpenDrawer(int impulseLength)
        {
            return (EmptyFiscalResponse)SendMessage(new OpenDrawerCommand(impulseLength)
                , bytes => new EmptyFiscalResponse(bytes));
        }
    
        /// <summary>
        /// Sets date and time in ECR.
        /// </summary>
        /// <param name="dateTime">DateTime to set.</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse SetDateTime(DateTime dateTime)
        {
            return (EmptyFiscalResponse)SendMessage(new SetDateTimeCommand(dateTime)
                , bytes => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Reads current date and time from ECR.
        /// </summary>
        /// <returns>ReadDateTimeResponse</returns>
        public ReadDateTimeResponse ReadDateTime()
        {
            return (ReadDateTimeResponse)SendMessage(new ReadDateTimeCommand()
                , bytes => new ReadDateTimeResponse(bytes));
        }

        /// <summary>
        /// Gets the status of current or last receipt 
        /// </summary>
        /// <returns>GetStatusOfCurrentReceiptResponse</returns>
        public GetStatusOfCurrentReceiptResponse GetStatusOfCurrentReceipt()
        {
            return
                (GetStatusOfCurrentReceiptResponse)
                    SendMessage(new GetStatusOfCurrentReceiptCommand()
                        , bytes => new GetStatusOfCurrentReceiptResponse(bytes));
        }

        /// <summary>
        /// Defines items in ECR
        /// </summary>
        /// <param name="name"></param>
        /// <param name="plu"></param>
        /// <param name="taxGr"></param>
        /// <param name="dep"></param>
        /// <param name="group"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="priceType"></param>
        /// <returns></returns>
        public EmptyFiscalResponse ProgramItem(string name, int plu, TaxGr taxGr, int dep, int group, decimal price, decimal quantity = 9999, PriceType priceType = PriceType.FixedPrice)
        {
            return (EmptyFiscalResponse)SendMessage(new ProgramItemCommand(name, plu, taxGr, dep, group, price, quantity, priceType)
                , bytes => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Programming. #255
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="index">Used for index if variable is array. If variable is not array, "Index" must be left blank</param>
        /// <param name="value">If this parameter is blank, ECR will return current value (Answer(2)). If the value is set, then ECR will program this value (Answer(1))</param>
        /// <returns>ProgrammingResponse</returns>
        public ProgrammingResponse Programming(string name, string index, string value)
        {
            return (ProgrammingResponse)SendMessage(
                new ProgrammingCommand(name
                                        , index
                                        , value)
                , bytes => new ProgrammingResponse(bytes));
        }

        /// <summary>
        /// 127 (7Fh) - Stamp operations
        /// </summary>
        /// <param name="type"> '0' - Print stamp;'1' - Rename loaded stamp with command 203. Default: '0'</param>
        /// <param name="name"> Name of stamp as filename in format 8.3;. Default: ""</param>
        /// <returns>ErrorCode - Indicates an error code. If command passed, ErrorCode is 0</returns>
        public PrintStampResponse PrintStamp(CommandType type, string name)
        {
            return (PrintStampResponse)SendMessage(
                 new PrintStampCommand((int)type, name)
                 , bytes => new PrintStampResponse(bytes));
        }

        /// <summary>
        /// 203 (CAh) - Stamp image loading
        /// </summary>
        /// <param name="Parameter">  type of operation: START - Praparation for data loading; Answer(1); STOP - End of data; Answer(2); YmFzZTY0ZGF0YQ== - base64 coded data of the grahpic logo; Answer(2) </param>
        /// <returns> Answer(1): ErrorCode - Indicates an error code. If command passed, ErrorCode is 0</returns>
        /// <returns> Answer(2): ErrorCode - Indicates an error code. If command passed, ErrorCode is 0; Chechsum - Sum of decoded base64 data;</returns>
        public LoadStampImageResponse LoadStampImage(String Parameter)
        {
            return (LoadStampImageResponse)SendMessage(
                 new LoadStampImageCommand(Parameter)
                 , bytes => new LoadStampImageResponse(bytes));
        }

        #endregion
    }

    internal class FP700Result
    {
        public readonly byte seq;
        public readonly int cmd;
        public readonly string data;
        public readonly byte[] status;

        public FP700Result(byte seq, int cmd, string data, byte[] status)
        {
            this.seq = seq;
            this.cmd = cmd;
            this.data = data;
            this.status = status;
        }
    }

    internal class Fp700Printer : IDisposable
    {
        private const int Nak = 0x15;
        private const int Syn = 0x16;
        private static readonly Dictionary<char, char> GeoToRusDict = new Dictionary<char, char>
        {
            {'ა', 'а'},
            {'ბ', 'б'},
            {'გ', 'в'},
            {'დ', 'г'},
            {'ე', 'д'},
            {'ვ', 'е'},
            {'ზ', 'ж'},
            {'თ', 'з'},
            {'ი', 'и'},
            {'კ', 'й'},
            {'ლ', 'к'},
            {'მ', 'л'},
            {'ნ', 'м'},
            {'ო', 'н'},
            {'პ', 'о'},
            {'ჟ', 'п'},
            {'რ', 'р'},
            {'ს', 'с'},
            {'ტ', 'т'},
            {'უ', 'у'},
            {'ფ', 'ф'},
            {'ქ', 'х'},
            {'ღ', 'ц'},
            {'ყ', 'ч'},
            {'შ', 'ш'},
            {'ჩ', 'щ'},
            {'ც', 'ъ'},
            {'ძ', 'ы'},
            {'წ', 'ь'},
            {'ჭ', 'э'},
            {'ხ', 'ю'},
            {'ჯ', 'я'},
            {'ჰ', 'ё'}
        };

        private SerialPort serialPort;
        private FP700Result lastResult;

        public Fp700Printer(string portName)
        {
            serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            serialPort.Open();
            lastResult = UnWrapMessage(ReTry(3, () => Send(WrapCommand(32, 74, string.Empty))));

        }

        public FP700Result Exec(int cmd, string data)
        {
            var seq = lastResult.seq == 0xFE ? 33 : lastResult.seq + 1;
            lastResult = UnWrapMessage(ReTry(3, () => Send(WrapCommand((byte)seq, cmd, data))));
            return lastResult;
        }

        public void Dispose()
        {
            using (serialPort)
            {
                if (serialPort.IsOpen) serialPort.Close();
            }
        }

        private byte[] Send(byte[] message)
        {
            serialPort.Write(message, 0, message.Length);
            var bytes = new List<byte>();
            while (true)
            {
                var b = (byte)serialPort.ReadByte();
                if (b == Syn) continue;
                if (b == Nak) throw new InvalidOperationException("nak");
                bytes.Add(b);
                if (b == 0x03) return bytes.ToArray();
            }
        }

        private static byte[] WrapCommand(byte seq, int cmd, string data)
        {
            var body = Quarterize(data.Length + 10 + 0x20)
                .Concat(new[] { seq })
                .Concat(Quarterize(cmd))
                .Concat(Encoding.GetEncoding(1251).GetBytes(Convert(GeoToRusDict, data)))
                .Concat(new byte[] { 0x05 })
                .ToArray();
            return new byte[] { 0x01 }
                .Concat(body)
                .Concat(Quarterize(body.Sum(b => b)))
                .Concat(new byte[] { 0x03 })
                .ToArray();
        }

        private static FP700Result UnWrapMessage(byte[] message)
        {
            if (!(
                message.Length >= 25 &&
                message[0] == 0x01 &&
                message[message.Length - 1] == 0x03 &&
                message[message.Length - 6] == 0x05 &&
                message[message.Length - 15] == 0x04 &&
                message.Slice(1, -5).Sum(b => b) == UnQuarterize(message.Slice(-5, -1))
            )) throw new ArgumentException("message");
            var seq = message[5];
            var cmd = message.Slice(6, 10);
            var data = message.Slice(10, -15);
            var status = message.Slice(-14, -6);
            return new FP700Result(seq, UnQuarterize(cmd), Encoding.GetEncoding(1251).GetString(data), status);
        }

        private static byte[] Quarterize(int value)
        {
            var buffer = new byte[4];
            for (var i = 0; i < 4; i++)
                buffer[i] = (byte)(((value >> ((3 - i) * 4)) & 0xF) | 0x30);
            return buffer;
        }

        private static int UnQuarterize(byte[] bytes)
        {
            var value = 0;
            for (var i = 0; i < 4; i++)
                value |= (bytes[i] & 0x0F) << ((3 - i) * 4);
            return value;
        }

        private static T ReTry<T>(int count, Func<T> f)
        {
            var i = 0;
            while (true)
                try
                {
                    i++;
                    return f();
                }
                catch (Exception)
                {
                    if (i > count) throw;
                }
        }

        private static string Convert(Dictionary<char, char> dict, string source)
        {
            var result = string.Empty;
            foreach (var c in source)
            {
                if (dict.ContainsKey(c))
                    result += dict[c];
                else
                    result += c;
            }
            return result;
        }
    }

    internal static class ByteArrayExtensions
    {
        public static byte[] Slice(this byte[] x)
        {
            return x.Slice(0);
        }

        public static byte[] Slice(this byte[] xs, int begin)
        {
            return xs.Slice(begin, xs.Length);
        }

        public static byte[] Slice(this byte[] xs, int begin, int end)
        {
            begin = begin < 0 ? xs.Length + begin : begin;
            end = end < 0 ? xs.Length + end : end;
            if (!(xs.Length >= end && end >= begin)) throw new InvalidOperationException();
            var len = end - begin;
            var rez = new byte[len];
            Array.Copy(xs, begin, rez, 0, len);
            return rez;
        }
    }

}

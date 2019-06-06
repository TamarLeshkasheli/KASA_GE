using BDO_DatecsDP25.Commands;
using BDO_DatecsDP25.Core;
using BDO_DatecsDP25.Responses;
using BDO_DatecsDP25.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BDO_DatecsDP25
{
    /// <summary>
    /// ECR Device DP25 Implementation (API)
    /// </summary>
    public class Dp25 : IDisposable
    {
        private FP700Printer printer;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="portName"></param>
        public Dp25(string portName)
        {
            printer = new FP700Printer(portName);
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            using (printer) { }
        }
        private List<Action<string>> loggers = new List<Action<string>>();

        public void AddLogger(Action<string> logger) {
            loggers.Add(logger);
        }

        public void RemoveLogger(Action<string> logger) {
            loggers.Remove(logger);
        }

        public FP700Result SendMessage(int cmd, string data)
        {
            loggers.ForEach(log => log(cmd + "->" + data));
            var result = printer.Exec(cmd, data);
            loggers.ForEach(log => log(
                cmd + "<- " +
                "0(" + Convert.ToString(result.status[0], 2) + ")" +
                "1(" + Convert.ToString(result.status[1], 2) + ")" +
                "2(" + Convert.ToString(result.status[2], 2) + ")" +
                "3(" + Convert.ToString(result.status[3], 2) + ")" +
                "4(" + Convert.ToString(result.status[4], 2) + ")" +
                "5(" + Convert.ToString(result.status[5], 2) + ")"));
            ThrowOnStatusError(result.status);
            return result;
        }

        private void ThrowOnStatusError(byte[] statusBytes)
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
            printer.Dispose();
            printer = new FP700Printer(portName);
        }


        #region NonFiscalCommands
        /// <summary>
        /// Opens non fiscal text receipt.
        /// </summary>
        /// <returns>CommonFiscalResponse</returns>
        public CommonFiscalResponse OpenNonFiscalReceipt()
        {
            var Command = 38;
            var Data = string.Empty;
            return new CommonFiscalResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Printing of free text in a non-fiscal text receipt
        /// </summary>
        /// <param name="text">Up to 30 symbols.</param>
        /// <returns></returns>
        public CommonFiscalResponse AddTextToNonFiscalReceipt(string text)
        {
            var Command = 42;
            var Data = text + "\t";
            return new CommonFiscalResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Closes non fiscal text receipt.
        /// </summary>
        /// <returns>CommonFiscalResponse</returns>
        public CommonFiscalResponse CloseNonFiscalReceipt()
        {
            var Command = 39;
            var Data = string.Empty;
            return new CommonFiscalResponse(SendMessage(Command, Data));
        }
        #endregion

        #region FiscalCommands



        public SubTotalResponse SubTotal()
        {
            var Command = 51;
            var Data = string.Empty;
            return new SubTotalResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Opens Sales Fiscal Receipt
        /// </summary>
        /// <param name="opCode">Operator code</param>
        /// <param name="opPwd">Operator password</param>
        /// <returns>OpenFiscalReceiptResponse</returns>
        public OpenFiscalReceiptResponse OpenFiscalReceipt(string opCode, string opPwd)
        {
            var Command = 48;
            var Data = (new object[] { opCode, opPwd, string.Empty, 0 }).StringJoin("\t");
            return new OpenFiscalReceiptResponse(SendMessage(Command, Data));
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
            var Command = 48;
            var Data = (new object[] { opCode, opPwd, string.Empty, type }).StringJoin("\t");
            return new OpenFiscalReceiptResponse(SendMessage(Command, Data));
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
            var Command = 48;
            var Data = (new object[] { opCode, opPwd, tillNumber, type }).StringJoin("\t");
            return new OpenFiscalReceiptResponse(SendMessage(Command, Data));
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
            var Command = 49;
            var Data = (new object[] { pluName, taxCode, price, quantity, 0, string.Empty, departmentNumber }).StringJoin("\t");
            return new RegisterSaleResponse(SendMessage(Command, Data));
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
            var Command = 49;
            var Data = (new object[] { pluName, taxCode, price, quantity, discountType, discountValue, departmentNumber }).StringJoin("\t");
            return new RegisterSaleResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluCode">The code of the item (1 - 100000). With sign '-' at void operations; </param>
        /// <param name="quantity"> Quantity of the item (0.001 - 99999.999) </param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterProgrammedItemSale(int pluCode, decimal quantity)
        {
            var Command = 58;
            var Data = (new object[] { pluCode, quantity, string.Empty, string.Empty, string.Empty }).StringJoin("\t");
            return new RegisterSaleResponse(SendMessage(Command, Data));
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
            var Command = 58;
            var Data = (new object[] { pluCode, quantity, price, discountType, discountValue }).StringJoin("\t");
            return new RegisterSaleResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Payments and calculation of the total sum
        /// </summary>
        /// <param name="paymentMode"> Type of payment. Default: 'Cash' </param>
        /// <returns>CalculateTotalResponse</returns>
        public CalculateTotalResponse Total(PaymentMode paymentMode = PaymentMode.Cash)
        {
            NumberFormatInfo Nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            var cashMoneyParam =string.Empty ;
            var Command = 53;
            var Data = (new object[] { paymentMode, cashMoneyParam }).StringJoin("\t");
            return new CalculateTotalResponse(SendMessage(Command, Data));
        }

        /// <summary>
		/// Payments and calculation of the total sum
		/// </summary>
		/// <param name="paymentMode"> Type of payment. </param>
        /// <param name="paymentMode"> Amount to pay (0.00 - 9999999.99). Default: the residual sum of the receipt; </param>
		/// <returns>CalculateTotalResponse</returns>
		public CalculateTotalResponse Total(PaymentMode paymentMode1, decimal cashMoney, PaymentMode paymentMode2)
        {
            NumberFormatInfo Nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            var cashMoneyParam = cashMoney.ToString(Nfi);
            var Command = 53;
            var Data = (new object[] { paymentMode1, cashMoneyParam, paymentMode2 }).StringJoin("\t");
            return new CalculateTotalResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// All void of a fiscal receipt. <br/>
        /// <bold>Note:The receipt will be closed as a non fiscal receipt. The slip number (unique number of the fiscal receipt) will not be increased.</bold>
        /// </summary>
        /// <returns>VoidOpenFiscalReceiptResponse</returns>
        public VoidOpenFiscalReceiptResponse VoidOpenFiscalReceipt()
        {
            var Command = 60;
            var Data = string.Empty;
            return new VoidOpenFiscalReceiptResponse(SendMessage(Command, Data));
        }


        public AddTextToFiscalReceiptResponse AddTextToFiscalReceipt(string text)
        {
            var Command = 54;
            var Data = text + "\t";
            return new AddTextToFiscalReceiptResponse(SendMessage(Command, Data));
        }

        /// <summary>
        ///  Closes open fiscal receipt.
        /// </summary>
        /// <returns>CloseFiscalReceiptResponse</returns>
        public CloseFiscalReceiptResponse CloseFiscalReceipt()
        {
            var Command = 56;
            var Data = string.Empty;
            return new CloseFiscalReceiptResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Get the information on the last fiscal entry.
        /// </summary>
        /// <param name="type">FiscalEntryInfoType. Default: FiscalEntryInfoType.CashDebit</param>
        /// <returns>GetLastFiscalEntryInfoResponse</returns>
        public GetLastFiscalEntryInfoResponse GetLastFiscalEntryInfo(FiscalEntryInfoType type = FiscalEntryInfoType.CashDebit)
        {
            var Command = 64;
            var Data = type + "\t";
            return new GetLastFiscalEntryInfoResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Cash in and Cash out operations
        /// </summary>
        /// <param name="operationType">Type of operation</param>
        /// <param name="amount">The sum</param>
        /// <returns>CashInCashOutResponse</returns>
        public CashInCashOutResponse CashInCashOutOperation(Cash operationType, decimal amount)
        {
            var Command = 70;
            var Data = (new object[] { operationType, amount }).StringJoin("\t");
            return new CashInCashOutResponse(SendMessage(Command, Data));
        }
        #endregion

        #region other
        /// <summary>
        /// Reads the status of the device.
        /// </summary>
        /// <returns>ReadStatusResponse</returns>
        public ReadStatusResponse ReadStatus()
        {
            var Command = 74;
            var Data = string.Empty;
            return new ReadStatusResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Feeds blank paper.
        /// </summary>
        /// <param name="lines">Line Count 1 to 99; default =  1;</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse FeedPaper(int lines = 1)
        {
            var Command = 44;
            var Data = lines + "\t";
            return new EmptyFiscalResponse(SendMessage(Command, Data));
        }


        /// <summary>
        /// Prints buffer
        /// </summary>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse PrintBuffer()
        {
            return FeedPaper(0);
        }

        /// <summary>
        /// Reads an error code  explanation from ECR.
        /// </summary>
        /// <param name="errorCode">Code of the error</param>
        /// <returns>ReadErrorResponse</returns>
        public ReadErrorResponse ReadError(string errorCode)
        {
            var Command = 100;
            var Data = errorCode + "\t";
            return new ReadErrorResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// ECR beeps with given interval and frequency.
        /// </summary>
        /// <param name="frequency">in hertzes</param>
        /// <param name="interval">in milliseconds</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse PlaySound(int frequency, int interval)
        {
            var Command = 80;
            var Data = (new object[] { frequency, interval }).StringJoin("\t");
            return new EmptyFiscalResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Prints X or Z Report and returns some stats.
        /// </summary>
        /// <param name="type">ReportType</param>
        /// <returns>PrintReportResponse</returns>
        public PrintReportResponse PrintReport(ReportType type)
        {
            var Command = 69;
            var Data = type + "\t";
            return new PrintReportResponse(SendMessage(Command, Data));
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
            var Command = 105;
            var Data = (new object[] { firstOper, lastOper, clear }).StringJoin("\t");
            return new EmptyFiscalResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Opens the cash drawer if such is connected.
        /// </summary>
        /// <param name="impulseLength"> The length of the impulse in milliseconds. </param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse OpenDrawer(int impulseLength)
        {
            var Command = 106;
            var Data = impulseLength + "\t";
            return new EmptyFiscalResponse(SendMessage(Command, Data));
        }
    
        /// <summary>
        /// Sets date and time in ECR.
        /// </summary>
        /// <param name="dateTime">DateTime to set.</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse SetDateTime(DateTime dateTime)
        {
            var Command = 61;
            var Data = dateTime.ToString("dd-MM-yy HH:mm:ss") + "\t";
            return new EmptyFiscalResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Reads current date and time from ECR.
        /// </summary>
        /// <returns>ReadDateTimeResponse</returns>
        public ReadDateTimeResponse ReadDateTime()
        {
            var Command = 62;
            var Data = string.Empty;
            return new ReadDateTimeResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// Gets the status of current or last receipt 
        /// </summary>
        /// <returns>GetStatusOfCurrentReceiptResponse</returns>
        public GetStatusOfCurrentReceiptResponse GetStatusOfCurrentReceipt()
        {
            var Command = 76;
            var Data = string.Empty;
            return new GetStatusOfCurrentReceiptResponse(SendMessage(Command, Data));
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
            var Command = 107;
            var Data = (new object[] { "P", plu, taxGr, dep, group, (int)priceType, price, "", quantity, "", "", "", "", name }).StringJoin("\t");
            return new EmptyFiscalResponse(SendMessage(Command, Data));
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
            var Command = 255;
            var Data = (new object[] { name, index, value }).StringJoin("\t");
            return new ProgrammingResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// 127 (7Fh) - Stamp operations
        /// </summary>
        /// <param name="type"> '0' - Print stamp;'1' - Rename loaded stamp with command 203. Default: '0'</param>
        /// <param name="name"> Name of stamp as filename in format 8.3;. Default: ""</param>
        /// <returns>ErrorCode - Indicates an error code. If command passed, ErrorCode is 0</returns>
        public PrintStampResponse PrintStamp(CommandType type, string name)
        {
            var Command = 127;
            var Data = (new object[] { type, name }).StringJoin("\t");
            return new PrintStampResponse(SendMessage(Command, Data));
        }

        /// <summary>
        /// 203 (CAh) - Stamp image loading
        /// </summary>
        /// <param name="Parameter">  type of operation: START - Praparation for data loading; Answer(1); STOP - End of data; Answer(2); YmFzZTY0ZGF0YQ== - base64 coded data of the grahpic logo; Answer(2) </param>
        /// <returns> Answer(1): ErrorCode - Indicates an error code. If command passed, ErrorCode is 0</returns>
        /// <returns> Answer(2): ErrorCode - Indicates an error code. If command passed, ErrorCode is 0; Chechsum - Sum of decoded base64 data;</returns>
        public LoadStampImageResponse LoadStampImage(String Parameter)
        {
            var Command = 203;
            var Data = Parameter + "\t";
            return new LoadStampImageResponse(SendMessage(Command, Data));
        }

        #endregion
    }

    }

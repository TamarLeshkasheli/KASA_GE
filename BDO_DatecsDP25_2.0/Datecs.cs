using BDO_DatecsDP25.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BDO_DatecsDP25
{
    public class Datecs
    {
        //private ObservableCollection<string> PortNames { get; set; }
        private Dp25 _Dp25;

        /// <summary>
        /// Dp25 ობიექტის ინიციალიზაცია
        /// </summary>
        /// <param name="portName_1c"> პორტის სახელი, რომელიც უკავია ფისკალურს </param>
        /// <returns></returns>
        public string InitDp25(string portName_1c, ref string ErrorText)
        {
            try
            {
                _Dp25 = new Dp25(portName_1c);
                return "";
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ex.ToString();
            }
        }

        public string Dispose(ref string ErrorText)
        {
            try
            {
                _Dp25.Dispose();
                return "";
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ex.ToString();
            }
        }

        /// <summary>
        /// პორტის ცვლილება
        /// </summary>
        /// <param name="ComPortName_1c"></param>
        public string ChangePort(string ComPortName_1c, ref string ErrorText)
        {
            try
            {
                _Dp25.ChangePort(ComPortName_1c);
                return "";
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ex.ToString();
            }
        }

        /// <summary>
        /// პორტები, რომლებიდანაც უნდა ავირჩიოთ ის პორტი სადაც შეერთებული გვაქვს მოწყობილობა.
        /// </summary>
        /// <returns>აბრუნებს პორტების სახელებს</returns>
        //public string GetPortNames(ref string ErrorText)
        //{
        //    try
        //    {
        //        PortNames = new ObservableCollection<string>(SerialPort.GetPortNames());
        //        StringBuilder stringBuilder = new StringBuilder();
        //        for (int i = 0; i < PortNames.Count; i++)
        //        {
        //            stringBuilder.AppendLine(PortNames[i]);
        //        }
        //        return stringBuilder.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorText = ex.Message;
        //        return ex.ToString();
        //    }
        //}

        #region NonFiscalCommands

        /// <summary>
        ///OpenNonFiscalReceipt 
        /// </summary>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] OpenNonFiscalReceipt(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.OpenNonFiscalReceipt();

                ArrayResult = new string[3];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }


        /// <summary>
        /// AddTextToNonFiscalReceipt
        /// </summary>
        /// <param name="text_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] AddTextToNonFiscalReceipt(string text_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.AddTextToNonFiscalReceipt(text_1c);

                ArrayResult = new string[3];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

     
        /// <summary>
        /// CloseNonFiscalReceipt
        /// </summary>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] CloseNonFiscalReceipt(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.CloseNonFiscalReceipt();

                ArrayResult = new string[3];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        #endregion

        #region FiscalCommands

        /// <summary>
        /// SubTotal
        /// </summary>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] SubTotal(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.SubTotal();
                ArrayResult = new string[6];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();
                ArrayResult[4] = response.SubTotal.ToString();
                ArrayResult[5] = response.TaxX != null ? response.TaxX.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// OpenFiscalReceipt
        /// </summary>
        /// <param name="opCode_1c"></param>
        /// <param name="opPwd_1c"></param>
        /// <returns></returns>
        public string[] OpenFiscalReceipt(string opCode_1c, string opPwd_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.OpenFiscalReceipt(opCode_1c, opPwd_1c);

                ArrayResult = new string[4];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// OpenFiscalReceipt_Type
        /// </summary>
        /// <param name="opCode_1c"></param>
        /// <param name="opPwd_1c"></param>
        /// <param name="type_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] OpenFiscalReceipt_TYPE(string opCode_1c, string opPwd_1c, string type_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            ReceiptType type = ReceiptType.Sale;

            switch (type_1c)
            {
                case "Return":
                    type = ReceiptType.Return;
                    break;
            }

            try
            {
                var response = _Dp25.OpenFiscalReceipt(opCode_1c, opPwd_1c, type);

                ArrayResult = new string[4];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// OpenFiscalReceipt_TYPE_TIILNUM
        /// </summary>
        /// <param name="opCode_1c"></param>
        /// <param name="opPwd_1c"></param>
        /// <param name="type_1c"></param>
        /// <param name="tillNumber_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] OpenFiscalReceipt_TYPE_TIILNUM(string opCode_1c, string opPwd_1c, string type_1c, int tillNumber_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            ReceiptType type = ReceiptType.Sale;

            switch (type_1c)
            {
                case "Return":
                    type = ReceiptType.Return;
                    break;
            }

            try
            {
                var response = _Dp25.OpenFiscalReceipt(opCode_1c, opPwd_1c, type, tillNumber_1c);

                ArrayResult = new string[4];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// RegisterSale
        /// </summary>
        /// <param name="pluName_1c"></param>
        /// <param name="price_1c"></param>
        /// <param name="quantity_1c"></param>
        /// <param name="departmentNumber_1c"></param>
        /// <param name="taxCode_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] RegisterSale(string pluName_1c, string price_1c, string quantity_1c, int departmentNumber_1c, string taxCode_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;

            decimal price = Convert.ToDecimal(price_1c, CultureInfo.InvariantCulture);
            decimal quantity = Convert.ToDecimal(quantity_1c, CultureInfo.InvariantCulture);
            TaxCode taxCode = TaxCode.A;
            switch (taxCode_1c)
            {
                case "B":
                    taxCode = TaxCode.B;
                    break;
                case "C":
                    taxCode = TaxCode.C;
                    break;
            }

            if (HasValues(pluName_1c, price, quantity))
            {
                try
                {
                    var response = _Dp25.RegisterSale(pluName_1c, price, quantity, departmentNumber_1c, taxCode);

                    ArrayResult = new string[4];
                    ArrayResult[0] = response.CommandPassed.ToString();
                    ArrayResult[1] = response.DocNumber.ToString();
                    ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                    ArrayResult[3] = response.SlipNumber.ToString();

                    return ArrayResult;
                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                    return ArrayResult;
                }
            }
            else
            {
                ErrorText = "დასახელება, რაოდენობა ან ფასი არ არის შევსებული!";
                return ArrayResult;
            }
        }

        /// <summary>
        /// RegisterSale_Discount
        /// </summary>
        /// <param name="pluName_1c"></param>
        /// <param name="price_1c"></param>
        /// <param name="quantity_1c"></param>
        /// <param name="departmentNumber_1c"></param>
        /// <param name="discountType_1c"></param>
        /// <param name="discountValue_1c"></param>
        /// <param name="taxCode_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] RegisterSale_Discount(string pluName_1c, string price_1c, string quantity_1c, int departmentNumber_1c, string discountType_1c, string discountValue_1c, string taxCode_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;

            decimal price = Convert.ToDecimal(price_1c, CultureInfo.InvariantCulture);
            decimal quantity = Convert.ToDecimal(quantity_1c, CultureInfo.InvariantCulture);
            DiscountType discountType = DiscountType.DiscountBySum;
            switch (discountType_1c)
            {
                case "DiscountByPercentage":
                    discountType = DiscountType.DiscountByPercentage;
                    break;
                case "SurchargeByPercentage":
                    discountType = DiscountType.SurchargeByPercentage;
                    break;
                case "SurchargeBySum":
                    discountType = DiscountType.SurchargeBySum;
                    break;
            }

            decimal discountValue = Convert.ToDecimal(discountValue_1c, CultureInfo.InvariantCulture);

            TaxCode taxCode = TaxCode.A;
            switch (taxCode_1c)
            {
                case "B":
                    taxCode = TaxCode.B;
                    break;
                case "C":
                    taxCode = TaxCode.C;
                    break;
            }

            if (HasValues(pluName_1c, price, quantity))
            {
                try
                {
                    var response = _Dp25.RegisterSale(pluName_1c, price, quantity, departmentNumber_1c, discountType, discountValue, taxCode);

                    ArrayResult = new string[4];
                    ArrayResult[0] = response.CommandPassed.ToString();
                    ArrayResult[1] = response.DocNumber.ToString();
                    ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                    ArrayResult[3] = response.SlipNumber.ToString();

                    return ArrayResult;
                }
                catch (Exception ex)
                {
                    ErrorText = ex.Message;
                    return ArrayResult;
                }
            }
            else
            {
                ErrorText = "დასახელება, რაოდენობა ან ფასი არ არის შევსებული!";
                return ArrayResult;
            }
        }

        /// <summary>
        /// RegisterProgrammedItemSale
        /// </summary>
        /// <param name="pluCode_1c"></param>
        /// <param name="quantity_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] RegisterProgrammedItemSale(int pluCode_1c, string quantity_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            decimal quantity = Convert.ToDecimal(quantity_1c, CultureInfo.InvariantCulture);

            try
            {
                var response = _Dp25.RegisterProgrammedItemSale(pluCode_1c, quantity);

                ArrayResult = new string[4];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// RegisterProgrammedItemSale_Discount
        /// </summary>
        /// <param name="pluCode_1c"></param>
        /// <param name="price_1c"></param>
        /// <param name="quantity_1c"></param>
        /// <param name="discountType_1c"></param>
        /// <param name="discountValue_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] RegisterProgrammedItemSale_Discount(int pluCode_1c, string quantity_1c, string price_1c, string discountType_1c, string discountValue_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            decimal quantity = Convert.ToDecimal(quantity_1c, CultureInfo.InvariantCulture);
            decimal price = Convert.ToDecimal(price_1c, CultureInfo.InvariantCulture);
            DiscountType discountType = DiscountType.DiscountBySum;
            switch (discountType_1c)
            {
                case "DiscountByPercentage":
                    discountType = DiscountType.DiscountByPercentage;
                    break;
                case "SurchargeByPercentage":
                    discountType = DiscountType.SurchargeByPercentage;
                    break;
                case "SurchargeBySum":
                    discountType = DiscountType.SurchargeBySum;
                    break;
            }
            decimal discountValue = Convert.ToDecimal(discountValue_1c, CultureInfo.InvariantCulture);

            try
            {
                var response = _Dp25.RegisterProgrammedItemSale(pluCode_1c, quantity, price, discountType, discountValue);

                ArrayResult = new string[4];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// Total
        /// </summary>
        /// <param name="paymentMode1_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] Total(string paymentMode1_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;

            PaymentMode paymentMode1 = PaymentMode.Cash;
            switch (paymentMode1_1c)
            {
                case "Cash":
                    paymentMode1 = PaymentMode.Cash;
                    break;
                case "Card":
                    paymentMode1 = PaymentMode.Card;
                    break;
                case "Credit":
                    paymentMode1 = PaymentMode.Credit;
                    break;
                case "Tare":
                    paymentMode1 = PaymentMode.Tare;
                    break;
            }

            try
            {
                var response = _Dp25.Total(paymentMode1);
                ArrayResult = new string[6];
                ArrayResult[0] = response.Amount.ToString();
                ArrayResult[1] = response.CommandPassed.ToString();
                ArrayResult[2] = response.DocNumber.ToString();
                ArrayResult[3] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[4] = response.SlipNumber.ToString();
                ArrayResult[5] = response.Status != null ? response.Status.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// Total_CashMoney
        /// </summary>
        /// <param name="paymentMode1_1c"></param>
        /// <param name="cashMoney_1c"></param>
        /// <param name="paymentMode2_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] Total_CashMoney(string paymentMode1_1c, string cashMoney_1c, string paymentMode2_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            decimal cashMoney = Convert.ToDecimal(cashMoney_1c, CultureInfo.InvariantCulture);

            PaymentMode paymentMode1 = PaymentMode.Cash;
            switch (paymentMode1_1c)
            {
                case "Cash":
                    paymentMode1 = PaymentMode.Cash;
                    break;
                case "Card":
                    paymentMode1 = PaymentMode.Card;
                    break;
                case "Credit":
                    paymentMode1 = PaymentMode.Credit;
                    break;
                case "Tare":
                    paymentMode1 = PaymentMode.Tare;
                    break;
            }

            PaymentMode paymentMode2 = PaymentMode.Cash;
            switch (paymentMode2_1c)
            {
                case "Cash":
                    paymentMode2 = PaymentMode.Cash;
                    break;
                case "Card":
                    paymentMode2 = PaymentMode.Card;
                    break;
                case "Credit":
                    paymentMode2 = PaymentMode.Credit;
                    break;
                case "Tare":
                    paymentMode2 = PaymentMode.Tare;
                    break;
            }

            try
            {
                var response = _Dp25.Total(paymentMode1, cashMoney, paymentMode2);
                ArrayResult = new string[6];
                ArrayResult[0] = response.Amount.ToString();
                ArrayResult[1] = response.CommandPassed.ToString();
                ArrayResult[2] = response.DocNumber.ToString();
                ArrayResult[3] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[4] = response.SlipNumber.ToString();
                ArrayResult[5] = response.Status != null ? response.Status.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// VoidOpenFiscalReceipt
        /// </summary>
        /// <returns></returns>
        public string[] VoidOpenFiscalReceipt(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.VoidOpenFiscalReceipt();

                ArrayResult = new string[4];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// AddTextToFiscalReceipt
        /// </summary>
        /// <param name="text_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] AddTextToFiscalReceipt(string text_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.AddTextToFiscalReceipt(text_1c);

                ArrayResult = new string[4];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// CloseFiscalReceipt
        /// </summary>
        /// <returns></returns>
        public string[] CloseFiscalReceipt(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.CloseFiscalReceipt();

                ArrayResult = new string[5];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DocNumber.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.SlipNumber.ToString();
                ArrayResult[4] = response.SlipNumberOfThisType.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// GetLastFiscalEntryInfo
        /// </summary>
        /// <param name="EntryInfoType_1c"></param>
        /// <returns></returns>
        public string[] GetLastFiscalEntryInfo(string EntryInfoType_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            FiscalEntryInfoType fiscalEntryInfoType = FiscalEntryInfoType.CashDebit;
            switch (EntryInfoType_1c)
            {
                case "CashCredit":
                    fiscalEntryInfoType = FiscalEntryInfoType.CashCredit;
                    break;
                case "CashFreeDebit":
                    fiscalEntryInfoType = FiscalEntryInfoType.CashFreeDebit;
                    break;
                case "CashFreeCredit":
                    fiscalEntryInfoType = FiscalEntryInfoType.CashFreeCredit;
                    break;
            }
            try
            {
                var response = _Dp25.GetLastFiscalEntryInfo(fiscalEntryInfoType);

                ArrayResult = new string[6];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.Date.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.nRep.ToString();
                ArrayResult[4] = response.Sum.ToString();
                ArrayResult[5] = response.Vat.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// CashInCashOut
        /// </summary>
        /// <param name="CashAmount_1c"></param>
        /// <param name="operationType_1c"></param>
        /// <returns></returns>
        public string[] CashInCashOut(string CashAmount_1c, string operationType_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                Cash operationType = Cash.In;

                if (operationType_1c == "Out")
                {
                    operationType = Cash.Out;
                }

                Decimal amount = Convert.ToDecimal(CashAmount_1c, CultureInfo.InvariantCulture);

                var response = _Dp25.CashInCashOutOperation(operationType, amount);

                ArrayResult = new string[6];
                ArrayResult[0] = response.CashIn.ToString();
                ArrayResult[1] = response.CashOut.ToString();
                ArrayResult[2] = response.CashSum.ToString();
                ArrayResult[3] = response.CommandPassed.ToString();
                ArrayResult[4] = response.DocNumber.ToString();
                ArrayResult[5] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        #endregion

        #region other

        /// <summary>
        /// ReadStatus
        /// </summary>
        /// <returns></returns>
        public string[] ReadStatus(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.ReadStatus();

                ArrayResult = new string[3];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < response.Status.Length; i++)
                {
                    stringBuilder.AppendLine(response.Status[i]);
                }

                ArrayResult[2] = stringBuilder != null ? stringBuilder.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }

        }

        /// <summary>
        /// FeedPaper
        /// </summary>
        /// <param name="lines_1c"></param>
        /// <returns></returns>
        public string[] FeedPaper(int lines_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.FeedPaper(lines_1c > 0 ? lines_1c : 1);

                ArrayResult = new string[2];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// PrintBuffer
        /// </summary>
        /// <returns></returns>
        public string[] PrintBuffer(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.PrintBuffer();

                ArrayResult = new string[2];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// ReadError
        /// </summary>
        /// <param name="errorCode_1c"></param>
        /// <returns></returns>
        public string[] ReadError(string errorCode_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.ReadError(errorCode_1c);

                ArrayResult = new string[4];
                ArrayResult[0] = response.Code != null ? response.Code.ToString() : "";
                ArrayResult[1] = response.CommandPassed.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[3] = response.ErrorMessage != null ? response.ErrorMessage.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// PlaySound
        /// </summary>
        /// <param name="frequency_1c">ჰერცებში</param>
        /// <param name="interval_1c">მილიწამებში</param>
        /// <returns></returns>
        public string[] PlaySound(int frequency_1c, int interval_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.PlaySound(frequency_1c, interval_1c * 1000);

                ArrayResult = new string[2];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }
 
        /// <summary>
        /// LoadStampImage
        /// </summary>
        /// <param name="type_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] LoadStampImage(string type_1c, ref string ErrorText)
        {
            string[] ArrayResult = null; 
           
            try
            {
                var response = _Dp25.LoadStampImage(type_1c);
                ArrayResult = new string[5];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
               
                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// PrintStamp
        /// </summary>
        /// <param name="type_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] PrintStamp(string type_1c, string name, ref string ErrorText)
        {
            string[] ArrayResult = null;
            CommandType type = CommandType.P;
            switch (type_1c)
            {
                case "P":
                    type = CommandType.P;
                    break;
                case "R":
                    type = CommandType.R;
                    break;
            }
            try
            {
                var response = _Dp25.PrintStamp(type, name);
                ArrayResult = new string[5];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
               
                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }


        /// <summary>
        /// PrintReport
        /// </summary>
        /// <param name="type_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] PrintReport(string type_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            ReportType type = ReportType.Z;
            switch (type_1c)
            {
                case "X":
                    type = ReportType.X;
                    break;
                case "x":
                    type = ReportType.X;
                    break;
            }
            try
            {
                var response = _Dp25.PrintReport(type);
                ArrayResult = new string[5];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[2] = response.nRep.ToString();
                ArrayResult[3] = response.TotNegX.ToString();
                ArrayResult[4] = response.TotX.ToString();

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// OperatorsReport
        /// </summary>
        /// <param name="firstOper_1c">First operator in the report ( 1...30 ). Default: 1</param>
        /// <param name="lastOper_1c">Last operator in the report ( 1...30 ). Default: 30</param>
        /// <param name="clear_1c">Type of report. Default: 0. '0' - Operators report, '1' - Operators report with clearing the operators registers</param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] OperatorsReport(string firstOper_1c, string lastOper_1c, string clear_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.OperatorsReport(firstOper_1c, lastOper_1c, clear_1c);

                ArrayResult = new string[2];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// OpenDrawer
        /// </summary>
        /// <param name="impulseLength_1c"></param>
        /// <returns></returns>
        public string[] OpenDrawer(int impulseLength_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.OpenDrawer(impulseLength_1c);

                ArrayResult = new string[2];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// SetDateTime
        /// </summary>
        /// <param name="dateTime_1c"></param>
        /// <returns></returns>
        public string[] SetDateTime(string dateTime_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            DateTime dateTime = DateTime.Parse(dateTime_1c, CultureInfo.InvariantCulture);

            try
            {
                var response = _Dp25.SetDateTime(dateTime);

                ArrayResult = new string[2];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// ReadDateTime
        /// </summary>
        /// <returns></returns>
        public string[] ReadDateTime(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.ReadDateTime();
                ArrayResult = new string[3];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.DateTime.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// GetStatusOfCurrentReceipt
        /// </summary>
        /// <returns></returns>
        public string[] GetStatusOfCurrentReceipt(ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.GetStatusOfCurrentReceipt();
                ArrayResult = new string[8];
                ArrayResult[0] = response.Amount.ToString();
                ArrayResult[1] = response.CommandPassed.ToString();
                ArrayResult[2] = response.DocNumber.ToString();
                ArrayResult[3] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";
                ArrayResult[4] = response.Items.ToString();
                ArrayResult[5] = response.SlipNumber.ToString();
                ArrayResult[6] = response.Sum.ToString();
                ArrayResult[7] = response.Value != null ? response.Value.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// ProgramItem
        /// </summary>
        /// <param name="name_1c"></param>
        /// <param name="plu_1c"></param>
        /// <param name="taxGr_1c"></param>
        /// <param name="dep_1c"></param>
        /// <param name="group_1c"></param>
        /// <param name="price_1c"></param>
        /// <param name="quantity_1c"></param>
        /// <param name="priceType_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] ProgramItem(string name_1c, int plu_1c, string taxGr_1c, int dep_1c, int group_1c, string price_1c, string quantity_1c, string priceType_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                TaxGr taxGr = TaxGr.A;
                switch (taxGr_1c)
                {
                    case "B":
                        taxGr = TaxGr.B;
                        break;
                    case "C":
                        taxGr = TaxGr.C;
                        break;
                }
                Decimal price = Convert.ToDecimal(price_1c, CultureInfo.InvariantCulture);
                Decimal quantity = Convert.ToDecimal(quantity_1c, CultureInfo.InvariantCulture);
                PriceType priceType = PriceType.FixedPrice;
                switch (priceType_1c)
                {
                    case "FreePrice":
                        priceType = PriceType.FreePrice;
                        break;
                    case "MaxPrice":
                        priceType = PriceType.MaxPrice;
                        break;
                }
                var response = _Dp25.ProgramItem(name_1c, plu_1c, taxGr, dep_1c, group_1c, price, quantity, priceType);
                ArrayResult = new string[2];
                ArrayResult[0] = response.CommandPassed.ToString();
                ArrayResult[1] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        /// <summary>
        /// Programming
        /// </summary>
        /// <param name="name_1c"></param>
        /// <param name="index_1c"></param>
        /// <param name="value_1c"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public string[] Programming(string name_1c, string index_1c, string value_1c, ref string ErrorText)
        {
            string[] ArrayResult = null;
            try
            {
                var response = _Dp25.Programming(name_1c, index_1c, value_1c);
                ArrayResult = new string[3];

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < response.Answer.Length; i++)
                {
                    stringBuilder.AppendLine(response.Answer[i]);
                }

                ArrayResult[0] = stringBuilder != null ? stringBuilder.ToString() : "";
                ArrayResult[1] = response.CommandPassed.ToString();
                ArrayResult[2] = response.ErrorCode != null ? response.ErrorCode.ToString() : "";

                return ArrayResult;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return ArrayResult;
            }
        }

        #endregion

        /// <summary>
        /// ამოწმებს გაყიდვის დროს დასახელება, ფასი და რაოდენობა არ იყოს ცარიელი
        /// </summary>
        /// <param name="name"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private bool HasValues(string name, decimal price, decimal quantity)
        {
            return !string.IsNullOrEmpty(name) && price > 0 && quantity > 0;
        }


    }
}

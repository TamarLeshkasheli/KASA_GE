using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace BDO_DatecsDP25
{
    public class Datecs
    {
        private SerialPort serial = null;
        private DatecProtocol datec = null;
        public string[] InitDp25(string portName)
        {
            try
            {
                serial = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout = 1500,
                    WriteTimeout = 1500
                };
                serial.Open();
                datec = new DatecProtocol(serial.Write, serial.Read);
                return datec.Exec(74);
            }
            catch (Exception ex)
            {
                Dispose();
                return new string[] { ex.Message, "" };
            }
        }
        public void Dispose()
        {
            var s = serial;
            serial = null;
            datec = null;
            if (s.IsOpen) s.Close();
            s.Dispose();
        }
        public string[] ChangePort(string ComPortName_1c)
        {
            Dispose();
            return InitDp25(ComPortName_1c);
        }
        public string[] Exec(int cmd, params string[] prms)
        {
            try
            {
                return datec.Exec(cmd, prms);
            }
            catch (Exception ex) { return new[] { ex.Message, "" }; }
        }
        public string[] ReadStatus()
        {
            if (datec == null) return new string[] { "Driver not initialized" };
            var list = new List<string>();
            long status = datec.Status;

            if ((status & (1 << 0)) > 0) list.Add("# Syntax error.");
            if ((status & (1 << 1)) > 0) list.Add("# Command code is invalid.");
            if ((status & (1 << 5)) > 0) list.Add("General error - this is OR of all errors marked with #");

            status >>= 8;
            if ((status & (1 << 0)) > 0) list.Add("# Overflow during command execution.");
            if ((status & (1 << 1)) > 0) list.Add("# Command is not permitted.");

            status >>= 8;
            if ((status & (1 << 0)) > 0) list.Add("# End of paper.");
            if ((status & (1 << 2)) > 0) list.Add("EJ is full.");
            if ((status & (1 << 3)) > 0) list.Add("Fiscal receipt is open.");
            if ((status & (1 << 4)) > 0) list.Add("EJ nearly full.");
            if ((status & (1 << 5)) > 0) list.Add("Nonfiscal receipt is open.");

            status >>= 16;
            if ((status & (1 << 0)) > 0) list.Add("* Error while writing in FM.");
            if ((status & (1 << 1)) > 0) list.Add("Tax number is set.");
            if ((status & (1 << 2)) > 0) list.Add("Serial number and number of FM are set.");
            if ((status & (1 << 3)) > 0) list.Add("There is space for less then 50 reports in Fiscal memory.");
            if ((status & (1 << 4)) > 0) list.Add("* Fiscal memory is full.");
            if ((status & (1 << 5)) > 0) list.Add("OR of all errors marked with ‘*’ from Bytes 4 и 5.");

            status >>= 8;
            if ((status & (1 << 1)) > 0) list.Add("FM is formated.");
            if ((status & (1 << 3)) > 0) list.Add("ECR is fiscalized.");
            if ((status & (1 << 4)) > 0) list.Add("VAT are set at least once.");

            return list.ToArray();
        }
    }
}

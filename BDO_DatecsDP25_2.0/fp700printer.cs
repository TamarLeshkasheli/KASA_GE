using BDO_DatecsDP25.Utils;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace BDO_DatecsDP25
{
    public class FP700Result
    {
        public readonly byte seq;
        public readonly int cmd;
        public readonly byte[] data;
        public readonly byte[] status;

        public FP700Result(byte seq, int cmd, byte[] data, byte[] status)
        {
            this.seq = seq;
            this.cmd = cmd;
            this.data = data;
            this.status = status;
        }
    }

    public class FP700Printer : IDisposable
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

        public FP700Printer(string portName)
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
            return new FP700Result(seq, UnQuarterize(cmd), data, status);
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

}

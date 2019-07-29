using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BDO_DatecsDP25
{
    public class DatecProtocol
    {
        private Action<byte[], int, int> writeFun;
        private Func<byte[], int, int, int> readFun;

        private long status;
        private byte seq;
        private int cmd;
        private readonly byte[] buffer;

        public long Status { get { return status; } }

        public DatecProtocol(Action<byte[], int, int> writeFun, Func<byte[], int, int, int> readFun, int size = 4096)
        {
            buffer = new byte[size];
            this.writeFun = writeFun;
            this.readFun = readFun;
            seq = 0x20;
            Exec(0x4A); // read status
        }

        public string[] Exec(int cmd, params string[] prms)
        {
            writeFun(
                buffer,
                0,
                Encode(buffer, (byte)(seq == 0xFE ? 32 : seq + 1), cmd, prms));
            var offset = 0;
            while (true)
            {
                if (buffer.Length <= offset) throw new InvalidProgramException("buffer overflow");
                offset += readFun(buffer, offset, buffer.Length - offset);
                if (buffer[offset - 1] == 0x15) throw new InvalidOperationException("nak");
                if (buffer[offset - 1] == 0x03) break;
                while (offset > 0 && buffer[offset - 1] == 0x16) offset--;
            }
            return Decode(buffer, offset, out seq, out this.cmd, out status);
        }

        static int Encode(byte[] buffer, byte seq, int cmd, params string[] list)
        {
            var paramsStr = (list.Length == 0) ? string.Empty : ToASCIIString(string.Join("\t", list)) + '\t';
            var chrArray = paramsStr.ToCharArray();
            var pos = 0;
            buffer[pos++] = 0x01;
            pos += Quarterize(chrArray.Length + 10 + 0x20, buffer, pos);
            buffer[pos++] = seq;
            pos += Quarterize(cmd, buffer, pos);
            pos += Encoding.GetEncoding(1251).GetBytes(chrArray, 0, chrArray.Length, buffer, pos);
            buffer[pos++] = 0x05;
            var checkSum = 0;
            for (int i = 1; i < pos; i++) checkSum += buffer[i];
            pos += Quarterize(checkSum, buffer, pos);
            buffer[pos++] = 0x03;
            return 1 + (4 + 1 + 4 + chrArray.Length + 1) + 4 + 1;
        }

        static string[] Decode(byte[] message, int msgLength, out byte seq, out int cmd, out long status)
        {
            var pos = 0;
            if (message[pos++] != 0x01) throw new ArgumentException("Preamble");
            var len = UnQuarterize(message, pos) - 0x20;
            pos += 4;
            if (len + 4 > msgLength) throw new ArgumentException("Preamble");
            var x = 0;
            for (var i = 0; i < len; i++) x += message[1 + i];
            if (x != UnQuarterize(message, len + 1)) throw new ArgumentException("Checksum");
            seq = message[pos++];
            cmd = UnQuarterize(message, pos);
            pos += 4;
            var data = Encoding.GetEncoding(1251).GetString(message, pos, len - 19);
            pos += data.Length;
            if (message[pos++] != 0x04) throw new ArgumentException("Separator");
            status = BitConverter.ToInt64(message, pos);
            return data.Split('\t');
        }

        static int Quarterize(int value, byte[] buffer, int start)
        {
            for (var i = 0; i < 4; i++)
                buffer[start + i] = (byte)(((value >> ((3 - i) * 4)) & 0xF) | 0x30);
            return 4;
        }

        static int UnQuarterize(byte[] bytes, int start)
        {
            var value = 0;
            for (var i = 0; i < 4; i++)
                value |= (bytes[start + i] & 0x0F) << ((3 - i) * 4);
            return value;
        }

        static string ToASCIIString(string source)
        {
            var result = string.Empty;
            foreach (var c in source)
                result += GeoToRusDict.ContainsKey(c) ? GeoToRusDict[c] : c;
            return result;
        }

        static readonly Dictionary<char, char> GeoToRusDict = new Dictionary<char, char>
        {
            {'\t', ';'},
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
    }
}

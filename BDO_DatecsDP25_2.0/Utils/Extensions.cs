using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BDO_DatecsDP25.Utils
{
    internal static class Extensions
    {
        private static readonly NumberFormatInfo Nfi;
        static Extensions()
        {
            Nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };
        }
        internal static string StringJoin(this IEnumerable<object> enumerable, string separator)
        {
            return
                string.Join(""
                , enumerable.Select(x => {
                    string result;
                    if (x.GetType() == typeof(decimal))
                        result = ((decimal)x).ToString(Nfi);
                    else
                        result = x.ToString();
                    return result + separator;
                }).ToArray());
        }
        public static string GetString(this byte[] buffer)
        {
            return Encoding.GetEncoding(1251).GetString(buffer);
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

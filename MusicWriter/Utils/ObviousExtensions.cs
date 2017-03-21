using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class ObviousExtensions
    {
        public static string ToString<T>(this T obj) => obj.ToString();

        public static T EnumParse<T>(this string source) =>
            (T)Enum.Parse(typeof(T), source);
    }
}

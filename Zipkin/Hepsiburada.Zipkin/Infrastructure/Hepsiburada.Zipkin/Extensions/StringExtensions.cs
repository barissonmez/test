using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hepsiburada.Zipkin.Extensions
{
    public static class StringExtensions
    {  /// Checks if string value can be converted to 128bit (Guid) or 64bit (long)
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
        internal static bool IsParsableTo128Or64Bit(this string value)
        {
            if (value.IsParsableToGuid()) return true;
            return value.IsParsableToLong();
        }

        /// <summary>
        /// Checks if hex string value is parsable to Guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsParsableToGuid(this string value)
        {
            Guid result;
            return Guid.TryParseExact(value, "N", out result);
        }

        /// <summary>
        /// Checks if hex string value is parsable to long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsParsableToLong(this string value)
        {
            long result;
            return !string.IsNullOrWhiteSpace(value) && long.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
        }
    }
}

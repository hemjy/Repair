using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.Helpers
{
    internal static class OrderHelper
    {
        private static readonly Random _random = new();

        public static string GenerateSixDigitCode()
        {
            char[] digits = new char[6];

            for (int i = 0; i < 6; i++)
            {
                digits[i] = (char)('1' + _random.Next(0, 9)); // generates '1' to '9'
            }

            return new string(digits);
        }
        private static bool TryParseOrderNumber(string input, out string prefix, out int number)
        {
            prefix = null;
            number = 0;

            var parts = input?.Split('-');
            if (parts?.Length != 2) return false;

            if (parts[0].Length != 3 || !IsAllUpperAlpha(parts[0])) return false;

            if (!int.TryParse(parts[1], out number)) return false;

            prefix = parts[0];
            return true;
        }

        private static bool IsAllUpperAlpha(string input)
        {
            foreach (var c in input)
            {
                if (c < 'A' || c > 'Z') return false;
            }
            return true;
        }

        public static string GetNextOrderNumber(string lastOrderNumber)
        {
            // Split prefix and number

            if (!TryParseOrderNumber(lastOrderNumber, out string prefix, out int number))
            {
                //reset
                lastOrderNumber = "AAA-00000000";
                prefix = "AAA";
                number = 0;
            }
            if (number < 99999999)
            {
                number++;
            }
            else
            {
                number = 1;
                prefix = IncrementPrefix(prefix);
            }

            return $"{prefix}-{number:D8}";
        }
        private static string IncrementPrefix(string prefix)
        {
            char[] chars = prefix.ToCharArray();

            for (int i = chars.Length - 1; i >= 0; i--)
            {
                if (chars[i] < 'Z')
                {
                    chars[i]++;
                    for (int j = i + 1; j < chars.Length; j++)
                    {
                        chars[j] = 'A';
                    }
                    return new string(chars);
                }
            }

            throw new InvalidOperationException("Prefix overflowed beyond ZZZ");
        }

        //  private static int _counter = 0; // A counter to avoid collisions in rapid calls

        //public static string GenerateOrderNumber()
        //{
        //    string datePart = DateTime.UtcNow.ToString("yyMMdd");

        //    long timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

        //    string counterPart = (_counter++ % 1000).ToString("D3"); // Keeps counter within 3 digits 
        //    string orderNumber = $"{counterPart}{datePart}{timestamp}";

        //    return orderNumber;
        //}
    }
}

using System;
using System.Collections.Generic;

namespace iznakurnoz.Bot.Extensions
{
    /// <summary>
    /// Методы расшрения для работы со строками.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly HashSet<char> _allowMacAddressChars = new HashSet<char>()
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '-', ':',
            'A', 'B', 'C', 'D', 'E', 'F'
        };

        private static readonly HashSet<char> _delimiterMacAddressChars = new HashSet<char>()
        {
            '-', ':'
        };

        private static readonly HashSet<char> _delimiterDeviceNameChars = new HashSet<char>()
        {
            '-', '_'
        };

        /// <summary>
        /// Безопасное конвертирование строки в наименование устройства.
        /// </summary>
        /// <param name="inString">Входная строка, содержащая MAC адрес.</param>
        /// <param name="deviceName">Строка, содержащая наименование устройства.</param>
        /// <returns>Истину, если конвертация прошла успешно.</returns>
        public static bool TryConvertToDeviceName(this string inString, out string deviceName)
        {
            deviceName = null;
            var result = string.Empty;
            inString = inString.ToLower();
            var inEnumerator = inString.GetEnumerator();

            while (inEnumerator.MoveNext())
            {
                var inChar = inEnumerator.Current;

                if (char.IsLetterOrDigit(inChar) || _delimiterDeviceNameChars.Contains(inChar))
                {
                    result += inChar;
                }
            }

            if (result.Length > 0)
            {
                deviceName = result;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Безопасное конвертирование строки в MAC адрес.
        /// </summary>
        /// <param name="inString">Входная строка, содержащая MAC адрес.</param>
        /// <param name="macAddress">Строка, содержащая форматированный MAC адрес.</param>
        /// <returns>Истину, если конвертация прошла успешно.</returns>
        public static bool TryConvertToMacAddress(this string inString, out string macAddress)
        {
            macAddress = null;
            var result = string.Empty;
            var inEnumerator = inString.GetEnumerator();

            while (TryGetNext(inEnumerator, out var inChar))
            {
                if (result.Length > 0)
                {
                    result += ':';
                }

                result += inChar;

                if (!TryGetNext(inEnumerator, out inChar))
                {
                    break;
                }

                result += inChar;
            }

            // Строка, содержащая корректный MAC адрес должна содержать ровно 17 символов.
            if (result.Length == 17)
            {
                macAddress = result;
                return true;
            }

            return false;
        }

        private static bool TryGetNext(IEnumerator<char> inEnumerator, out char nextChar)
        {
            nextChar = '\0';

            if (!inEnumerator.MoveNext())
            {
                return false;
            }

            var inChar = Char.ToUpper(inEnumerator.Current);

            if (!_allowMacAddressChars.Contains(inChar) || _delimiterMacAddressChars.Contains(inChar))
            {
                return TryGetNext(inEnumerator, out nextChar);
            }

            nextChar = inChar;
            return true;
        }
    }
}
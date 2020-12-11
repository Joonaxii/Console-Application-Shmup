using System;
using System.Globalization;
using System.IO;

namespace Joonaxii.ConsoleBulletHell
{
    public static class Extensions
    {    
        private static NumberFormatInfo _nfiFloat;

        static Extensions()
        {
            _nfiFloat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();          
        }

        public static float ToFloat(this string str, float defaultValue = 0)
        {
            str = str.Replace(",", ".").Trim();
            _nfiFloat.NumberDecimalDigits = 4;
            return float.TryParse(str, NumberStyles.Any, _nfiFloat, out float val) ? val : defaultValue;
        }

        public static bool ContainsID(this ICollideable[] arr, int id, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (arr[i].GetEntity().ID == id) { return true; }
            }
            return false;
        }

        public static void ValidateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static int Sum(this int[] input)
        {
            int sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                sum += input[i];
            }
            return sum;
        }

        public static string AddSpaces(this int input)
        {
            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            nfi.NumberDecimalDigits = 0;
            return input.ToString("n", nfi);
        }

        public static string AddSpaces(this long input)
        {
            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            nfi.NumberDecimalDigits = 0;
            return input.ToString("n", nfi);
        }

        public static string GetCentered(string input)
        {
            int length = input.Length / 2;

            int ll = Renderer.CurrentWidth / 2 - length;
            string str = new string(' ', ll);

            return $"{str}{input}{str}";
        }
    }
}
using System.Globalization;

namespace Library
{
    public static class StringExtensions
    {
        /// <summary>
        /// To test if the string 'input' contains the string 'value' inside it
        /// </summary>
        /// <param name="input">the original string</param>
        /// <param name="value">string that may/may not be present inside the 'input' string</param>
        /// <returns>true if the string 'input' contains the string 'value' otherwise false </returns>
        public static bool ContainsIgnoreCase(this string input, string value)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(input, value, CompareOptions.IgnoreCase) >= 0;
        }
    }
}

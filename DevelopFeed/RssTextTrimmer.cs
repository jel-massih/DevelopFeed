using System;
using System.Windows.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;

namespace DevelopFeed
{
    public class RssTextTrimmer : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            int maxLength = 200;
            int strLength = 0;
            string fixedString = "";

            //Remove HTML Tags
            fixedString = Regex.Replace(value.ToString(), "<[^>]+>", string.Empty);

            //Remove Newline Characters
            fixedString = fixedString.Replace("\r", "").Replace("\n", "");

            //Remove Encoded CHaracters
            fixedString = HttpUtility.HtmlDecode(fixedString);

            strLength = fixedString.ToString().Length;

            if (strLength == 0)
            {
                return null;
            }
            else if (strLength >= maxLength)
            {
                fixedString = fixedString.Substring(0, maxLength);
                fixedString = fixedString.Substring(0, fixedString.LastIndexOf(" "));
                fixedString += "...";
            }


            return fixedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

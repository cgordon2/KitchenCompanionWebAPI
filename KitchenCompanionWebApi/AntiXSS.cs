using System.Net;
using System.Text.RegularExpressions;

namespace KitchenCompanionWebApi
{
    public class AntiXSS
    {
        public static bool ContainsHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = WebUtility.HtmlDecode(input);

            return Regex.IsMatch(input, @"<\s*[a-z][^>]*>", RegexOptions.IgnoreCase);
        }
    }
}

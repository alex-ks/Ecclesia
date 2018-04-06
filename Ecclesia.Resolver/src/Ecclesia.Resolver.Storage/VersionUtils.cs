using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Ecclesia.Resolver.Storage
{
    static class VersionUtils
    {
        private const string VersionRegex = @"^(\d+)(\.(\d+)(\.(\d+))?)?(-([\w\d]+))?$";

        public const string DefaultVersion = "1.0.0";

        public static bool TryParse(string str, out int major, out int? middle, out int? minor, out string name)
        {
            var regex = new Regex(VersionRegex);
            var match = regex.Match(str);
            
            if (!match.Success)
            {
                major = 0;
                middle = null;
                minor = null;
                name = null;
                return false;
            }

            var majorStr = match.Groups[1].Value;
            var middleStr = match.Groups[3].Value;
            var minorStr = match.Groups[5].Value;
            name = match.Groups[7].Value;

            major = int.Parse(majorStr);

            if (int.TryParse(middleStr, out int m))
                middle = m;
            else
                middle = null;

            if (int.TryParse(minorStr, out m))
                minor = m;
            else
                minor = null;

            if (name == string.Empty)
                name = null;

            return true;
        }

        public static string Serialize(int major, int? middle, int? minor, string name)
        {
            var builder = new StringBuilder();
            builder.Append(major);
            if (middle != null)
                builder.Append('.').Append(middle);
            if (minor != null)
                builder.Append('.').Append(minor);
            if (name != null)
                builder.Append('-').Append(name);
            return builder.ToString();
        }
    }
}
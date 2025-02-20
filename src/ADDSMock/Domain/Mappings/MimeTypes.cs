using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.Regex;

#pragma warning disable SYSLIB1045

namespace ADDSMock.Domain.Mappings
{
    public static class MimeTypes
    {
        private static readonly Dictionary<string, List<string>> _apacheMimeTypes = new();
        public const string DefaultType = "application/octet-stream";

        static MimeTypes()
        {
            var allApacheMimeTypes = ApacheMimeTypes.AllMimeTypes;

            using var stringReader = new StringReader(allApacheMimeTypes);

            while (stringReader.ReadLine() is { } line)
            {
                //Remove comments
                var currentLine = Replace(line, @"\s*#.*|^\s*|\s*$/g", "");

                //split them by whitespace
                var stripWhiteSpaceRegEx = new Regex(@"\s+", RegexOptions.None);

                if (string.IsNullOrEmpty(currentLine))
                {
                    continue;
                }

                var matches = stripWhiteSpaceRegEx.Split(currentLine);

                //add the mime type and extension to the dictionary
                //mime-type is the key and value is the list of extensions it is associated with
                //e.g. {"application/mathematica":["ma","nb","mb"]}

                _apacheMimeTypes.Add(matches.First(), matches.Skip(1).ToList());
            }
        }

        public static string Lookup(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            //return default type if there is no extension
            if (string.IsNullOrEmpty(extension))
            {
                return DefaultType;
            }

            //remove dot from extenstion to lookup in the dictionary
            extension = extension.Substring(1);

            // Get an exact match if possible
            var mimeType = _apacheMimeTypes.FirstOrDefault(x => x.Value.Exists(m => m == extension)).Key;
            if (!string.IsNullOrEmpty(mimeType))
            {
                return mimeType;
            }

            // Get a close match
            mimeType = _apacheMimeTypes.FirstOrDefault(x => x.Value.Exists(m => m.Contains(extension))).Key;

            return string.IsNullOrEmpty(mimeType) ? DefaultType : mimeType;
        }

        public static List<string> Extension(string mimeType)
        {
            var extensions = _apacheMimeTypes.FirstOrDefault(x => x.Key.Equals(mimeType)).Value;

            return extensions ?? [];
        }
    }
}

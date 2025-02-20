// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;

namespace ReferenceTest
{
    public static class StringUtils
    {
        /// <summary>
        ///     Extracts a string from between a pair of delimiters. Only the first
        ///     instance is found.
        /// </summary>
        /// <param name="source">Input String to work on</param>
        /// <param name="beginDelimiter">Beginning delimiter</param>
        /// <param name="endDelimiter">ending delimiter</param>
        /// <param name="caseSensitive">Determines whether the search for delimiters is case-sensitive</param>
        /// <param name="allowMissingEndDelimiter"></param>
        /// <param name="returnDelimiters"></param>
        /// <returns>Extracted string or string.Empty on no match</returns>
        public static string ExtractString(this string source, string beginDelimiter, string endDelimiter, bool caseSensitive = false, bool allowMissingEndDelimiter = false, bool returnDelimiters = false)
        {
            int at1, at2;

            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            if (caseSensitive)
            {
                at1 = source.IndexOf(beginDelimiter, StringComparison.CurrentCulture);

                if (at1 == -1)
                {
                    return string.Empty;
                }

                at2 = source.IndexOf(endDelimiter, at1 + beginDelimiter.Length, StringComparison.CurrentCulture);
            }
            else
            {
                at1 = source.IndexOf(beginDelimiter, 0, source.Length, StringComparison.OrdinalIgnoreCase);

                if (at1 == -1)
                {
                    return string.Empty;
                }

                at2 = source.IndexOf(endDelimiter, at1 + beginDelimiter.Length, StringComparison.OrdinalIgnoreCase);
            }

            if (allowMissingEndDelimiter && at2 < 0)
            {
                if (!returnDelimiters)
                {
                    return source[(at1 + beginDelimiter.Length)..];
                }

                return source[at1..];
            }

            if (at1 > -1 && at2 > 1)
            {
                if (!returnDelimiters)
                {
                    return source.Substring(at1 + beginDelimiter.Length, at2 - at1 - beginDelimiter.Length);
                }

                return source.Substring(at1, at2 - at1 + endDelimiter.Length);
            }

            return string.Empty;
        }


        /// <summary>
        ///     Replicates an input string n number of times
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public static string Replicate(string input, int charCount)
        {
            var sb = new StringBuilder(input.Length * charCount);

            for (var i = 0; i < charCount; i++)
            {
                sb.Append(input);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Replaces a substring within a string with another substring with optional case sensitivity turned off.
        /// </summary>
        /// <param name="originalString">String to do replacements on</param>
        /// <param name="findString">The string to find</param>
        /// <param name="replaceString">The string to replace found string with</param>
        /// <param name="caseInsensitive">If true case-insensitive search is performed</param>
        /// <returns>updated string or original string if no matches</returns>
        public static string ReplaceString(string originalString, string findString, string replaceString, bool caseInsensitive)
        {
            var at1 = 0;
            while (true)
            {
                if (caseInsensitive)
                {
                    at1 = originalString.IndexOf(findString, at1, originalString.Length - at1, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    at1 = originalString.IndexOf(findString, at1, StringComparison.Ordinal);
                }

                if (at1 == -1)
                {
                    break;
                }

                originalString = originalString[..at1] + replaceString + originalString[(at1 + findString.Length)..];

                at1 += replaceString.Length;
            }

            return originalString;
        }
    }
}

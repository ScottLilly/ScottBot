using System;
using System.Linq;

namespace ScottBot.Models
{
    public static class ExtensionMethods
    {
        public static bool IncludesTheWords(this string text, params string[] words)
        {
            if(string.IsNullOrWhiteSpace(text) ||
               words.Length == 0 ||
               words.All(string.IsNullOrWhiteSpace))
            {
                return false;
            }

            return words.All(word => text.Contains(word, StringComparison.CurrentCultureIgnoreCase));
        }
        
        public static string RemoveText(this string text, string textToRemove)
        {
            while(text.Contains(textToRemove, StringComparison.CurrentCultureIgnoreCase))
            {
                text = text.Replace(textToRemove, "", StringComparison.CurrentCultureIgnoreCase);
            }

            return text;
        }
    }
}
namespace SkyrimPotionRecipes.Extensions
{
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class Extensions
    {
        public static bool AreEqual<T1,T2>(this List<T1> first, List<T1> second, Func<T1,T2> KeyProperty)
        {
            if (first.Count != second.Count)
                return false;

            bool result = true;

            for (int i = 0; i < first.Count; i++)
            {
                var key1 = KeyProperty.Invoke(first[i]);
                var key2 = KeyProperty.Invoke(second[i]);
                if (key1 != null && !key1.Equals(key2))
                {
                    result = false;
                    break;
                }
                else if (key1 == null && key2 != null)
                {
                    result = false;
                    break;
                }
                
            }

            return result;
        }

        public static string ToTitleCase(this string str)
        {
            string[] substrings = Regex.Split(str, @"\w+\s+\w+");
            List<string> ostring = new List<string>();
            foreach (string substring in substrings)
            {
                string osubstring = $"{new string(substring[0], 1).ToUpper()}{substring.Substring(1).ToLower()}";
                ostring.Add(osubstring);
            }

            return string.Join(" ", ostring.ToArray());
        }
    }
}

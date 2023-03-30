namespace SkyrimPotionRecipes.Extensions
{
    using System.Collections;

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
    }
}

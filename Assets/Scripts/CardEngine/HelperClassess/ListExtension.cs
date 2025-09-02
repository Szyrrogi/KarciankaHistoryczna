using System.Collections.Generic;
using System.Security.Cryptography;
namespace CardEngine.HelperClassess
{
    public static class ListExtensions
    {
        public static void ShuffleCrypto<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static T Pop<T>(this List<T> list, int index = 0)
        {
            T r = list[index];
            list.RemoveAt(index);
            return r;
        }
    }
}
using System;
using System.Linq;
using System.Collections;

namespace PersonProfile_DF.Utilities
{
    public static class Tools
    {
        public static byte[] ObjectToByteArray(Object obj)
        {
            IEnumerable en = (IEnumerable)obj;
            byte[] myBytes = en.OfType<byte>().ToArray();
            return myBytes;
        }

        public static int LockdownPageSize(int originalPageSize)
        {
            int[] allowedPageSizes = new int[] { 10, 20, 50, 75, 100 };
            return allowedPageSizes.Contains(originalPageSize) ? originalPageSize : 50;
        }

        public static string LimitCharacters(string originalString, int limit)
        {
            if (!string.IsNullOrEmpty(originalString) && originalString.Length > limit)
            {
                return originalString.Substring(0, limit) + ".....";
            }
            else
            {
                return originalString;
            }
        }
    }

}


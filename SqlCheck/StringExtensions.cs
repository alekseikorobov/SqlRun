using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCheck
{
    public static class StringExtensions
    {
        public static bool Eq(this string strA, string strB,bool ignoreCase = true)
        {
            return string.Compare(strA, strB, ignoreCase) == 0;
        }
    }
}

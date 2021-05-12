using System;
using System.Collections.Generic;
using System.Text;
using DxCK.Utils.Text;

namespace DxCK.Utils
{
    public static class FastStringUtils
    {
        public static int CompareFast(/*this */string str1, string str2)
        {
            if (str1 == null)
            {
                throw new ArgumentNullException("str1");
            }

            if (str2 == null)
            {
                throw new ArgumentNullException("str2");
            }

            int shortLength;

            if (str1.Length < str2.Length)
            {
                shortLength = str1.Length;
            }
            else
            {
                shortLength = str2.Length;
            }

            for (int i = 0; i < shortLength; i++)
            {
                int compareResult = str1[i] - str2[i];

                if (compareResult != 0)
                {
                    return compareResult;
                }
            }

            return str1.Length - str2.Length;
        }
    }
}

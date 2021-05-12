using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DxCK.Utils
{
    public static class ArrayUtils
    {
        public static bool Equals<T>(T[] arr1, T[] arr2)
            where T : class
        {
            if (arr1 == arr2)
            {
                return true;
            }

            if (arr1 == null || arr2 == null)
            {
                return false;
            }

            if (arr1.Length != arr2.Length)
            {
                return false;
            }

            for (int i = 0; i < arr1.Length; i++)
            {
                if (!arr1[i].Equals(arr2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool EqualsByte(byte[] arr1, byte[] arr2)
        {
            if (arr1 == arr2)
            {
                return true;
            }

            if (arr1 == null || arr2 == null)
            {
                return false;
            }

            if (arr1.Length != arr2.Length)
            {
                return false;
            }

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static void Reverse<T>(/*this*/ T[] array, int index, int length)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if ((index < array.GetLowerBound(0)) || (length < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "length", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if ((array.Length - (index - array.GetLowerBound(0))) < length)
            {
                throw new ArgumentException("Argument_InvalidOffLen");
            }
            if (array.Rank != 1)
            {
                throw new RankException("Rank_MultiDimNotSupported");
            }

            int num = index;
            int num2 = (index + length) - 1;

            while (num < num2)
            {
                T obj2 = array[num];
                array[num] = array[num2];
                array[num2] = obj2;
                num++;
                num2--;
            }
        }

        public static bool IsPalindrome<T>(/*this*/ T[] array, int index, int length)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if ((index < array.GetLowerBound(0)) || (length < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "length", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if ((array.Length - (index - array.GetLowerBound(0))) < length)
            {
                throw new ArgumentException("Argument_InvalidOffLen");
            }
            if (array.Rank != 1)
            {
                throw new RankException("Rank_MultiDimNotSupported");
            }

            int num = index;
            int num2 = (index + length) - 1;

            while (num < num2)
            {
                if (!array[num].Equals(array[num2]))
                {
                    return false;
                }
                num++;
                num2--;
            }

            return true;
        }

        //slow method
        /*public static void Reverse<T>(this IList<T> array, int index, int length)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if ((index < 0) || (length < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "length", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if ((array.Count - index) < length)
            {
                throw new ArgumentException("Argument_InvalidOffLen");
            }

            int num = index;
            int num2 = (index + length) - 1;

            while (num < num2)
            {
                T obj2 = array[num];
                array[num] = array[num2];
                array[num2] = obj2;
                num++;
                num2--;
            }
        }*/

        public static void Copy<T>(T[] arrSource, T[] arrDest, int count)
        {
            for (int i = 0; i < count; i++)
            {
                arrDest[i] = arrSource[i];
            }
        }

        public static void Copy<T>(T[] arrSource, int indexSource, T[] arrDest, int indexDest, int count)
        {
            if (indexSource >= indexDest)
            {
                int sourceIndexPlusCount = indexSource + count;

                for (int sourceI = indexSource, destI = indexDest; sourceI < sourceIndexPlusCount; sourceI++, destI++)
                {
                    arrDest[destI] = arrSource[sourceI];
                }
            }
            else
            {
                for (int sourceI = indexSource + count - 1, destI = indexDest + count - 1; sourceI >= indexSource; sourceI--, destI--)
                {
                    arrDest[destI] = arrSource[sourceI];
                }
            }
        }

        public static void Clear<T>(T[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = default(T);
            }
        }

        public static void Clear<T>(/*this*/ T[] arr, int index, int count)
        {
            int indexPlusCount = index + count;

            for (int i = index; i < indexPlusCount; i++)
            {
                arr[i] = default(T);
            }
        }

        public static T[] ChangeArraySize<T>(T[] arr, int newSize)
        {
            T[] newArr = new T[newSize];
            Copy(arr, newArr, Math.Min(arr.Length, newSize));
            return newArr;
        }
    }
}

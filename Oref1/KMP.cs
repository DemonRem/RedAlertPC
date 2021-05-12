using System;
using System.Collections.Generic;
using System.Text;

namespace DxCK.Utils.Text
{
    public class KMP
    {
        //private string _query;
        private char[] _query;
        private int _queryLength;
        private int[] _prefix;

        public KMP(string query)
        {
            _query = query.ToCharArray();
            _queryLength = _query.Length;

            _prefix = ComputePrefix();
        }

        private int[] ComputePrefix()
        {
            int[] prefix = new int[_queryLength];

            prefix[0] = 0;
            int k = 0;
            for (int q = 1; q < _queryLength; q++)
            {
                while (k > 0 && _query[k] != _query[q])
                {
                    k = prefix[k - 1];
                }

                if (_query[k] == _query[q])
                {
                    k++;
                }

                prefix[q] = k;
            }

            return prefix;
        }

        public bool ExistsIn(string text)
        {
            int q = 0;

            int textLength = text.Length;
            for (int i = 0; i < textLength; i++)
            {
                /*while (q > 0 && _query[q] != text[i])
                {
                    q = _prefix[q - 1];
                }

                if (_query[q] == text[i])
                {
                    q++;

                    if (q == _query.Length)
                    {
                        return true;
                        q = _prefix[q - 1];
                    }
                }*/

                while (true)
                {
                    if (_query[q] == text[i])
                    {
                        q++;

                        if (q == _queryLength)
                        {
                            return true;
                            //q = _prefix[q - 1];
                        }

                        break;
                    }
                    else if (q > 0)
                    {
                        q = _prefix[--q];
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return false;
        }

        public int IndexIn(string text)
        {
            int q = 0;

            int textLength = text.Length;
            for (int i = 0; i < textLength; i++)
            {
                /*while (q > 0 && _query[q] != text[i])
                {
                    q = _prefix[q - 1];
                }

                if (_query[q] == text[i])
                {
                    q++;

                    if (q == _query.Length)
                    {
                        return i - q + 1;
                        q = _prefix[q - 1];
                    }
                }*/

                while (true)
                {
                    if (_query[q] == text[i])
                    {
                        q++;

                        if (q == _queryLength)
                        {
                            return i - q + 1;
                            //q = _prefix[q - 1];
                        }

                        break;
                    }
                    else if (q > 0)
                    {
                        q = _prefix[--q];
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return -1;
        }
    }
}

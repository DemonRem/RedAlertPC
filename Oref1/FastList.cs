using System;
using System.Collections.Generic;
using System.Text;

namespace DxCK.Utils.Collections.Generic
{
    public class FastList<T> : IList<T>, IFastList<T>
    {
        public const int DEFAULT_CAPABILITY = 10;

        private T[] _arr;
        private int _count;
        private bool _clearRequired;

        public FastList()
        {
            Initialize(DEFAULT_CAPABILITY, 0);
        }

        public FastList(int capability)
        {
            Initialize(capability, 0);
        }

        public FastList(ICollection<T> collection)
        {
            Initialize(collection.Count, collection.Count);
            collection.CopyTo(_arr, 0);
        }

        public FastList(FastList<T> fastList)
        {
            Initialize(fastList.Count, fastList.Count);
            fastList.CopyTo(_arr, 0);
        }

        public FastList(int capability, ICollection<T> collection)
        {
            Initialize(Math.Max(capability, collection.Count), collection.Count);
            collection.CopyTo(_arr, 0);
        }

        public FastList(int capability, FastList<T> fastList)
        {
            Initialize(Math.Max(capability, fastList.Count), fastList.Count);
            fastList.CopyTo(_arr, 0);
        }

        public FastList(IEnumerable<T> enumerable)
        {
            Initialize(DEFAULT_CAPABILITY, 0);
            AddRange(enumerable);
        }

        public FastList(int capability, IEnumerable<T> enumerable)
        {
            Initialize(capability, 0);
            AddRange(enumerable);
        }

        private void Initialize(int capability, int count)
        {
            _arr = new T[capability];
            _count = count;
            _clearRequired = !(typeof(T).IsPrimitive && typeof(T).IsValueType);
        }

        public void AddRange(IEnumerable<T> enumerable)
        {
            InsertRange(_count, enumerable);
        }

        public void InsertRange(int index, IEnumerable<T> enumerable)
        {
            int currIndex = index;
            foreach (T obj in enumerable)
            {
                Insert(currIndex, obj);
            }
        }

        public void AddRange(ICollection<T> collection)
        {
            InsertRange(_count, collection);
        }

        public void AddRange(FastList<T> fastList)
        {
            InsertRange(_count, fastList);
        }

        public void AddRange(IList<T> collection, int index, int count)
        {
            int AfterLastIndex = index + count;
            if (AfterLastIndex > collection.Count)
            {
                throw new ArgumentOutOfRangeException("index+count");
            }

            int targetCount = _count + count;

            if (_arr.Length < targetCount)
            {
                T[] newArr = new T[targetCount + count];

                if (index > 0)
                {
                    ArrayUtils.Copy(_arr, newArr, count);
                }

                _arr = newArr;
            }

            for (int i = index; i < AfterLastIndex; i++)
            {
                _arr[_count++] = collection[i];
            }

            _count = targetCount;
        }

        public void AddRange(FastList<T> fastList, int index, int count)
        {
            int AfterLastIndex = index + count;
            if (AfterLastIndex > fastList.Count)
            {
                throw new ArgumentOutOfRangeException("index+count");
            }

            int targetCount = _count + fastList.Count;

            if (_arr.Length < targetCount)
            {
                T[] newArr = new T[targetCount + 3];

                if (index > 0)
                {
                    ArrayUtils.Copy(_arr, newArr, index);
                }

                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index, newArr, index + fastList.Count, _count - index);
                }

                _arr = newArr;
            }
            else
            {
                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index, _arr, index + fastList.Count, _count - index);
                }
            }

            for (int i = index; i < AfterLastIndex; i++)
            {
                _arr[_count++] = fastList[i];
            }

            _count = targetCount;
        }

        public void InsertRange(int index, ICollection<T> collection)
        {
            if (index > _count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            int targetCount = _count + collection.Count;

            if (_arr.Length < targetCount)
            {
                T[] newArr = new T[targetCount + 3];

                if (index > 0)
                {
                    ArrayUtils.Copy(_arr, newArr, index);
                }

                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index, newArr, index + collection.Count, _count - index);
                }

                _arr = newArr;
            }
            else
            {
                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index, _arr, index + collection.Count, _count - index);
                }
            }

            collection.CopyTo(_arr, index);

            _count = targetCount;
        }

        public void InsertRange(int index, FastList<T> fastList)
        {
            if (index > _count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            int targetCount = _count + fastList.Count;

            if (_arr.Length < targetCount)
            {
                T[] newArr = new T[targetCount + 3];

                if (index > 0)
                {
                    ArrayUtils.Copy(_arr, newArr, index);
                }

                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index, newArr, index + fastList.Count, _count - index);
                }

                _arr = newArr;
            }
            else
            {
                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index, _arr, index + fastList.Count, _count - index);
                }
            }

            fastList.CopyTo(_arr, index);

            _count = targetCount;
        }

        public void AddRepeat(T value, int count)
        {
            //InsertRepeat(_count, value, count);

            //bugfix
            if (count > 0)
            {
                int targetCount = _count + count;

                if (_arr.Length < targetCount)
                {
                    T[] newArr = new T[targetCount + 3];

                    if (_count > 0)
                    {
                        ArrayUtils.Copy(_arr, newArr, _count);
                    }

                    _arr = newArr;
                }

                if (!(_arr.Length < targetCount && object.Equals(value, default(T))))
                {
                    int insertCount = _count + count;
                    for (int i = _count; i < insertCount; i++)
                    {
                        _arr[i] = value;
                    }
                }

                _count = targetCount;
            }
        }

        public void InsertRepeat(int index, T value, int count)
        {
            if (index > _count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            //bugfix
            if (count > 0)
            {
                int targetCount = _count + count;

                if (_arr.Length < targetCount)
                {
                    T[] newArr = new T[targetCount + 3];

                    if (index > 0)
                    {
                        ArrayUtils.Copy(_arr, newArr, index);
                    }

                    if (index < _count)
                    {
                        ArrayUtils.Copy(_arr, index, newArr, index + count, _count - index);
                    }

                    _arr = newArr;
                }
                else
                {
                    if (index < _count)
                    {
                        ArrayUtils.Copy(_arr, index, _arr, index + count, _count - index);
                    }
                }

                if (!(_arr.Length < targetCount && object.Equals(value, default(T))))
                {
                    int insertCount = index + count;
                    for (int i = index; i < insertCount; i++)
                    {
                        _arr[i] = value;
                    }
                }

                _count = targetCount;
            }
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return Array.FindIndex<T>(_arr, 0, _count, delegate(T obj)
            {
                return object.Equals(item, obj);
            });
        }

        public void Insert(int index, T item)
        {
            if (index > _count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            int targetCount = _count + 1;

            if (_arr.Length < targetCount)
            {
                T[] newArr = new T[targetCount + 3];

                if (index > 0)
                {
                    ArrayUtils.Copy(_arr, newArr, index);
                }

                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index, newArr, index + 1, _count - index);
                }

                _arr = newArr;
            }
            else
            {
                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index, _arr, index + 1, _count - index);
                }
            }

            _arr[index] = item;

            _count = targetCount;
        }

        public void RemoveAt(int index)
        {
            if (index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            
            _count--;
            ArrayUtils.Copy(_arr, index + 1, _arr, index, _count - index);
            _arr[_count] = default(T);
        }

        public T this[int index]
        {
            get
            {
                //this check slowing down
                /*if (index >= _count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }*/

                return _arr[index];
            }
            set
            {
                //this check slowing down
                /*if (index >= _count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }*/

                _arr[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            //Insert(_count, item); //this slower

            int targetCount = _count + 1;

            if (_arr.Length < targetCount)
            {
                T[] newArr = new T[targetCount + 3]; //3 spares

                if (_count > 0)
                {
                    ArrayUtils.Copy(_arr, newArr, _count);
                }

                _arr = newArr;
            }

            _arr[_count] = item;

            _count = targetCount;
        }

        public void Clear()
        {
            if (_clearRequired)
            {
                ArrayUtils.Clear(_arr, 0, _count);
            }
            _count = 0;
        }

        public bool Contains(T item)
        {
            return (IndexOf(item) != -1);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            //old bug
            //_arr.CopyTo(array, arrayIndex);
            //ArrayTools.Copy(_arr, 0, array, arrayIndex, _count);

            for (int i = 0, targetI = arrayIndex; i < _count; i++, arrayIndex++)
            {
                array[arrayIndex] = _arr[i];
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < _count; i++)
            {
                yield return _arr[i];
            }
        }

        #endregion

        /*#region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion*/

        public void RemoveRange(int index, int count)
        {
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index": "count");
            }
            
            if ((_count - index) < count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            
            if (count > 0)
            {
                _count -= count;
                
                if (index < _count)
                {
                    ArrayUtils.Copy(_arr, index + count, _arr, index, _count - index);
                }

                if (_clearRequired)
                {
                    ArrayUtils.Clear(_arr, _count, count);
                }
            }
        }

        public int Capability
        {
            get { return _arr.Length; }
            set
            {
                if (value < _count)
                {
                    throw new ArgumentOutOfRangeException("Capability");
                }

                T[] newArr = new T[value];

                ArrayUtils.Copy(_arr, newArr, _count);

                _arr = newArr;
            }
        }

        public void Reverse()
        {
            ArrayUtils.Reverse(_arr, 0, _count); //this faster
            //Array.Reverse(_arr, 0, _count);
        }

        /*public void EnsureCapability(int requiredCount)
        {
            if (_arr.Length < requiredCount)
            {
                T[] newArr = new T[requiredCount];
                ArrayTools.Copy(_arr, newArr, _count);
                _arr = newArr;
            }
        }*/

        public T[] ToArray()
        {
            T[] arr = new T[_count];
            ArrayUtils.Copy(_arr, arr, _count);
            return arr;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        public T[] GetInternalBuffer()
        {
            return _arr;
        }
    }
}

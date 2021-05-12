using System;
using System.Collections.Generic;
namespace DxCK.Utils.Collections.Generic
{
    public interface IFastList<T> : IList<T>
    {
        void AddRange(System.Collections.Generic.IList<T> collection, int index, int count);
        void AddRange(System.Collections.Generic.ICollection<T> collection);
        void AddRange(System.Collections.Generic.IEnumerable<T> enumerable);
        void AddRepeat(T value, int count);
        int Capability { get; }
        void InsertRange(int index, System.Collections.Generic.IEnumerable<T> enumerable);
        void InsertRange(int index, System.Collections.Generic.ICollection<T> collection);
        void InsertRepeat(int index, T value, int count);
        void RemoveRange(int index, int count);
        void Reverse();
        T[] ToArray();
    }
}

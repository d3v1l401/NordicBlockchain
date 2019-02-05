using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Extensions
{
    public static class EList {
        public static T[] ToArray<T>(this List<T> _list) {
            if (_list.Count == 0)
                return null;

            T[] _array = new T[_list.Count];

            for (int i = 0; i < _list.Count; i++)
                _array[i] = _list[i];

            return _array;
        }
    }
}

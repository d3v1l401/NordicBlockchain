using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Extensions
{
    public static class EObject {
        public static T Cast<T>(this object o) {
            return (T)o;
        }
    }
}

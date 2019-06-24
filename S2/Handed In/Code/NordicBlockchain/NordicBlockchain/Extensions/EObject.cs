using System;
using System.Collections.Generic;
using System.Text;

namespace Nordic.Extensions
{
    using System.Text;
    public static class EObject {
        public static T Cast<T>(this object o) {
            return (T)o;
        }
    }
}

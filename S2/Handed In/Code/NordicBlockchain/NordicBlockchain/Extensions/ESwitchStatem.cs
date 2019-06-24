using System;
using System.Collections.Generic;
using System.Text;

// https://stackoverflow.com/questions/708911/using-case-switch-and-gettype-to-determine-the-object
// &&
// https://stackoverflow.com/questions/7252186/switch-case-on-type-c-sharp/7301514#7301514

namespace Nordic.Extensions {

    public class Switch {
        public Switch(Object o) {
            Object = o;
        }

        public Object Object { get; private set; }
    }

    public static class SwitchExtensions {
        public static Switch Case<T>(this Switch s, Action<T> a) where T : class 
            => Case(s, o => true, a, false);
        

        public static Switch Case<T>(this Switch s, Action<T> a, bool fallThrough) where T : class
            => Case(s, o => true, a, fallThrough);

        public static Switch Case<T>(this Switch s, Func<T, bool> c, Action<T> a) where T : class
            => Case(s, c, a, false);

        public static Switch Case<T>(this Switch s, Func<T, bool> c, Action<T> a, bool fallThrough) where T : class {
            if (s == null)
                return null;

            T t = s.Object as T;
            if (t != null)
                if (c(t)) {
                    a(t);
                    return fallThrough ? s : null;
                }

            return s;
        }
    }
}

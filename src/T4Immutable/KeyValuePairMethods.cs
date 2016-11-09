using System;
using System.Collections.Generic;
using System.Reflection;

namespace T4Immutable {
  internal class KeyValuePairMethods {
    public const string ClassFullName = "System.Collections.Generic.KeyValuePair";

    private static readonly object KeyValuePairMethodsDictionaryLock = new object();

    private static readonly Dictionary<Type, KeyValuePairMethods> KeyValuePairMethodsDictionary =
      new Dictionary<Type, KeyValuePairMethods>();

    private static readonly object[] NoParams = new object[0];

    private static string StripGenericFromFullName(string name) {
      var i = name.IndexOf("`", StringComparison.Ordinal);
      if (i <= 0) {
        return name;
      }
      return name.Substring(0, i);
    }

    public static Tuple<object, object> TryExtractKeyValuePair(object obj) {
      var methods = GetForType(obj.GetType());
      return methods == null ? null : new Tuple<object, object>(methods.GetKey(obj), methods.GetValue(obj));
    }

    public static KeyValuePairMethods GetForType(Type type) {
      if (!type.IsGenericType || (StripGenericFromFullName(type.FullName) != ClassFullName)) {
        return null;
      }

      // TODO: this probably could be more performant by using a concurrent dictionary
      lock (KeyValuePairMethodsDictionaryLock) {
        KeyValuePairMethods kvpm;
        if (!KeyValuePairMethodsDictionary.TryGetValue(type, out kvpm)) {
          kvpm = new KeyValuePairMethods(type);
          KeyValuePairMethodsDictionary.Add(type, kvpm);
        }
        return kvpm;
      }
    }

    public KeyValuePairMethods(Type type) {
      KeyMethodInfo = type.GetProperty("Key").GetGetMethod();
      ValueMethodInfo = type.GetProperty("Value").GetGetMethod();
    }

    private MethodInfo KeyMethodInfo { get; }
    private MethodInfo ValueMethodInfo { get; }

    public object GetKey(object o) {
      return KeyMethodInfo.Invoke(o, NoParams);
    }

    public object GetValue(object o) {
      return ValueMethodInfo.Invoke(o, NoParams);
    }
  }
}

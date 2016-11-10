using System;

namespace T4Immutable {
  internal static class KeyValuePairHelper {
    public const string ClassFullName = "System.Collections.Generic.KeyValuePair";

    private static string StripGenericFromFullName(string name) {
      var i = name.IndexOf("`", StringComparison.Ordinal);
      return i <= 0 ? name : name.Substring(0, i);
    }

    public static Tuple<object, object> TryExtractKeyValuePair(object obj) {
      if (!IsKeyValuePair(obj.GetType())) {
        return null;
      }
      dynamic dobj = obj;
      return new Tuple<object, object>(dobj.Key, dobj.Value);
    }

    public static bool IsKeyValuePair(Type type) {
      return type.IsGenericType && (StripGenericFromFullName(type.FullName) == ClassFullName);
    }
  }
}

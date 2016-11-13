using System;
using System.Collections.Generic;

namespace T4Immutable {
  internal static class KeyValuePairHelper {
    public static Tuple<object, object> TryExtractKeyValuePair(object obj) {
      if (!IsKeyValuePair(obj.GetType())) {
        return null;
      }
      dynamic dobj = obj;
      return new Tuple<object, object>(dobj.Key, dobj.Value);
    }

    public static bool IsKeyValuePair(Type type) {
      if (!type.IsGenericType || type.IsGenericTypeDefinition) {
        return false;
      }
      var genericType = type.GetGenericTypeDefinition();
      return ReferenceEquals(genericType, typeof(KeyValuePair<,>));
    }
  }
}

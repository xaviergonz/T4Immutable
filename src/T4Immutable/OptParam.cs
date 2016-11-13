using System;

namespace T4Immutable {
  /// <summary>
  /// Struct used to pass parameters to With/Builder methods.
  /// It is a struct because structs are not nullable and then we can use null and get it transformed to the value type.
  /// </summary>
  /// <typeparam name="T">Value type</typeparam>
  [Serializable]
  public struct OptParam<T> {
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private bool _hasValue;
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private T _value;

    /// <summary>
    /// Actual value passed.
    /// </summary>
    public T Value {
      get {
        if (!HasValue) {
          throw new InvalidOperationException("No value set");
        }
        return _value;
      }
    }

    /// <summary>
    /// true if it holds a value, false otherwise.
    /// </summary>
    public bool HasValue => _hasValue;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">Value to hold.</param>
    public OptParam(T value) {
      _value = value;
      _hasValue = true;
    }

    /// <summary>
    /// Converts automatically between this class and the inner parameter type.
    /// </summary>
    /// <param name="val"></param>
    public static implicit operator OptParam<T>(T val) {
      return new OptParam<T>(val);
    }

    /// <summary>
    /// Converts this class to the inner parameter type.
    /// </summary>
    /// <param name="val"></param>
    public static explicit operator T(OptParam<T> val) {
      return val.Value;
    }

    /// <summary>
    /// Gets the value or default if none.
    /// </summary>
    /// <returns></returns>
    public T GetValueOrDefault() {
      return Value;
    }

    /// <summary>
    /// Checks for equality.
    /// </summary>
    /// <param name="obj">The other object.</param>
    /// <returns>true if they are equals, false otherwise.</returns>
    public override bool Equals(object obj) {
      var otherOptParam = obj as OptParam<T>?;

      // if it doesn't have a value then the other object must be the same type and not have a value
      if (!HasValue) {
        if (otherOptParam == null) {
          return false;
        }
        return !otherOptParam.Value.HasValue;
      }

      // this has a value

      if (otherOptParam != null) {
        return otherOptParam.Value.HasValue && Helpers.BasicAreEqual(Value, otherOptParam.Value.Value);
      }
      return Helpers.BasicAreEqual(Value, obj);
    }

    /// <summary>
    /// Get the hash code.
    /// </summary>
    /// <returns>A hash code.</returns>
    public override int GetHashCode() {
      if (!HasValue) {
        return -1;
      }
      if (Value == null) {
        return 0;
      }
      return Value.GetHashCode();
    }

    /// <summary>
    /// To string.
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      if (!HasValue) {
        return "No value";
      }
      if (Value == null) {
        return "null";
      }
      return Value.ToString();
    }
  }

  public static class OptParam {
    /// <summary>
    /// If the type provided is not an OptParam returns null.
    /// Otherwise returns the underlying type of the OptParam type.
    /// </summary>
    /// <param name="optParamType"></param>
    /// <returns></returns>
    public static Type GetUnderlyingType(Type optParamType) {
      if (optParamType == null) {
        throw new ArgumentNullException(nameof(optParamType));
      }

      if (!optParamType.IsGenericType || optParamType.IsGenericTypeDefinition) {
        return null;
      }

      var genericType = optParamType.GetGenericTypeDefinition();
      return ReferenceEquals(genericType, typeof(OptParam<>)) ? optParamType.GetGenericArguments()[0] : null;
    }
  }
}

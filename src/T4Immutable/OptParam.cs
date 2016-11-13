namespace T4Immutable {
  /// <summary>
  /// Struct used to pass parameters to With/Builder methods.
  /// It is a struct because structs are not nullable and then we can use null and get it transformed to the value type.
  /// </summary>
  /// <typeparam name="T">Value type</typeparam>
  public struct OptParam<T> {
    /// <summary>
    /// Actual value passed.
    /// </summary>
    public T Value { get; private set; }

    /// <summary>
    /// true if it holds a value, false otherwise.
    /// </summary>
    public bool HasValue { get; private set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">Value to hold.</param>
    public OptParam(T value) {
      Value = value;
      HasValue = true;
    }

    /// <summary>
    /// Clears the value as if it was never assigned.
    /// </summary>
    public void Clear() {
      Value = default(T);
      HasValue = false;
    }

    /// <summary>
    /// Converts automatically between this class and the inner parameter type.
    /// </summary>
    /// <param name="val"></param>
    public static implicit operator OptParam<T>(T val) {
      return new OptParam<T>(val);
    }
  }
}

namespace T4Immutable {
  /// <summary>
  /// Struct used to pass parameters to With methods.
  /// It is a struct because structs are not nullable and then we can use null and get it transformed to the value type.
  /// </summary>
  /// <typeparam name="T">Value type</typeparam>
  public struct WithParam<T> {
    /// <summary>
    /// Actual value passed.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// true if it holds a value, false otherwise.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">Value to hold.</param>
    public WithParam(T value) {
      Value = value;
      HasValue = true;
    }

    /// <summary>
    /// Converts automatically between the WithParam class and the inner parameter type.
    /// </summary>
    /// <param name="val"></param>
    public static implicit operator WithParam<T>(T val) {
      return new WithParam<T>(val);
    }
  }
}

namespace T4Immutable {
  /// <summary>
  /// Struct used to pass parameters to With methods.
  /// It is a struct because structs are not nullable and then we can use null and get it transformed to the value type.
  /// </summary>
  /// <typeparam name="T">Value type</typeparam>
  public struct WithParam<T>
  {
    public T Value { get; }
    public bool HasValue { get; }

    public WithParam(T value) {
      Value = value;
      HasValue = true;
    }

    public static implicit operator WithParam<T>(T val) {
      return new WithParam<T>(val);
    }
  }
}
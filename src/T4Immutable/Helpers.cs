namespace T4Immutable
{
  /// <summary>
  /// Collection of helper methods for T4Immutable.
  /// </summary>
  public static class Helpers
  {
    /// <summary>
    /// Check if two objects are equal.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="a">First object.</param>
    /// <param name="b">Second object.</param>
    /// <returns>true if they are equal, false otherwise.</returns>
    public static bool AreEqual<T>(T a, T b) {
      bool aIsNull = ReferenceEquals(a, null), bIsNull = ReferenceEquals(b, null);
      if (aIsNull && bIsNull) return true;
      if (aIsNull || bIsNull) return false;
      return a.Equals(b);
    }

    /// <summary>
    /// Returns the ToString() of an object or "null" if the object is null.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>The ToString() of an object or "null" if the object is null.</returns>
    public static string ToString(object obj) {
      return obj?.ToString() ?? "null";
    }
  }
}
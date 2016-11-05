namespace T4Immutable
{
  public static class Helpers
  {
    public static bool AreEqual<T>(T a, T b) {
      bool aIsNull = ReferenceEquals(a, null), bIsNull = ReferenceEquals(b, null);
      if (aIsNull && bIsNull) return true;
      if (aIsNull || bIsNull) return false;
      return a.Equals(b);
    }

    public static string ToString(object obj) {
      return obj?.ToString() ?? "null";
    }
  }
}
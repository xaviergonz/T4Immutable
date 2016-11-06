using System;

namespace T4Immutable {
  /// <summary>
  /// Generate a not null check at the beginning of the constructor.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class PreNotNullCheckAttribute : Attribute
  {
  }
}
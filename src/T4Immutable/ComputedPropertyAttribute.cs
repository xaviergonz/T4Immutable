using System;

namespace T4Immutable
{
  /// <summary>
  /// Marks a property as computed, effectively making T4Immutable ignore it.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ComputedPropertyAttribute : Attribute
  {
  }
}
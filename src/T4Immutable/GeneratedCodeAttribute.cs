using System;

namespace T4Immutable
{
  /// <summary>
  /// Attribute used internally by T4Immutable to mark generated code. Not for public usage.
  /// </summary>
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  public sealed class GeneratedCodeAttribute : Attribute
  {
  }
}

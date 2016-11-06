using System;

namespace T4Immutable
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  public sealed class GeneratedCodeAttribute : Attribute
  {
  }
}

using System;

namespace T4Immutable
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ImmutableClassAttribute : Attribute
  {
    public ImmutableClassOptions Options { get; set; } = ImmutableClassOptions.None;
    public ConstructorAccessLevel ConstructorAccessLevel { get; set; } = ConstructorAccessLevel.Public;
  }
}
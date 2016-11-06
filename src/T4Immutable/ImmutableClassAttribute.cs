using System;

namespace T4Immutable
{
  /// <summary>
  /// Marks a class so it can be processed by T4Immutable.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ImmutableClassAttribute : Attribute
  {
    /// <summary>
    /// Immutable class generation options.
    /// </summary>
    public ImmutableClassOptions Options { get; set; } = ImmutableClassOptions.None;
    /// <summary>
    /// Generated constructor access level (modifier).
    /// </summary>
    public ConstructorAccessLevel ConstructorAccessLevel { get; set; } = ConstructorAccessLevel.Public;
  }
}
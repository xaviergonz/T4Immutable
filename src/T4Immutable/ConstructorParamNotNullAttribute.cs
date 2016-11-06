using System;

namespace T4Immutable {
  /// <summary>
  /// Adds a JetBrains.Annotations.NotNull attribute to the constructor parameter.
  /// Also enables a not null precheck implicitely.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ConstructorParamNotNullAttribute : Attribute
  {
  }
}
using System;

namespace T4Immutable {
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ConstructorParamNotNullAttribute : Attribute
  {
  }
}
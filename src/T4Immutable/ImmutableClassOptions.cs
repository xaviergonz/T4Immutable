using System;

namespace T4Immutable {
  [Flags]
  public enum ImmutableClassOptions {
    None = 0,
    DisableEquals = 1,
    DisableGetHashCode = 2,
    EnableOperatorEquals = 4,
    DisableToString = 8,
    DisableWith = 16
  }
}
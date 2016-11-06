using System;

namespace T4Immutable {
  /// <summary>
  /// Immutable class generation options for T4Immutable.
  /// </summary>
  [Flags]
  public enum ImmutableClassOptions {
    /// <summary>
    /// Default.
    /// </summary>
    None = 0,
    /// <summary>
    /// Do not generate Equals() implementation or add the IEquatable interface.
    /// </summary>
    DisableEquals = 1,
    /// <summary>
    /// Do not generate a GetHashCode() implementation.
    /// </summary>
    DisableGetHashCode = 2,
    /// <summary>
    /// Generate operator == and operator !=.
    /// </summary>
    EnableOperatorEquals = 4,
    /// <summary>
    /// Do not generated a ToString() implementation.
    /// </summary>
    DisableToString = 8,
    /// <summary>
    /// Do not generate a With() implementation.
    /// </summary>
    DisableWith = 16
  }
}
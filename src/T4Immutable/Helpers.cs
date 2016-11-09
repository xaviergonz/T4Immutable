using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T4Immutable
{
  /// <summary>
  /// Collection of helper methods for T4Immutable.
  /// </summary>
  public static class Helpers
  {
    /// <summary>
    /// Check if two objects are equal.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="a">First object.</param>
    /// <param name="b">Second object.</param>
    /// <returns>true if they are equal, false otherwise.</returns>
    public static bool AreEqual<T>(T a, T b) {
      bool aIsNull = ReferenceEquals(a, null), bIsNull = ReferenceEquals(b, null);
      if (aIsNull && bIsNull) return true;
      if (aIsNull || bIsNull) return false;
      var equals = a.Equals(b);
      if (equals) return true;

      // one extra check for enumerables
      
      // first a fastcheck for collection size
      var aCollection = a as ICollection;
      var bCollection = b as ICollection;
      if (aCollection != null && bCollection != null)
      {
        if (aCollection.Count != bCollection.Count) return false;
      }

      var aEnumerable = a as IEnumerable;
      var bEnumerable = b as IEnumerable;
      if (aEnumerable == null || bEnumerable == null) return false;

      var aEnum = aEnumerable.GetEnumerator();
      var bEnum = bEnumerable.GetEnumerator();

      while (aEnum.MoveNext())
      {
        if (!bEnum.MoveNext()) return false;
        object aCurrent = aEnum.Current, bCurrent = bEnum.Current;
        if (!AreEqual(aCurrent, bCurrent)) return false;
      }

      // all items so far are the same, but does b have one more?
      return !bEnum.MoveNext();
    }

    /// <summary>
    /// Gets the hashcode of a single object. If the object is an enumerable it will make a hashcode of the enumeration.
    /// </summary>
    /// <param name="o">Object to make the hashcode for.</param>
    /// <returns>A hashcode.</returns>
    public static int GetHashCodeForSingleObject(object o)
    {
      if (ReferenceEquals(o, null)) return 0;

      var oEnumerable = o as IEnumerable;
      if (oEnumerable == null) return o.GetHashCode();

      // make a hash of the items if it is an enumerable
      var oEnum = oEnumerable.GetEnumerator();

      var list = new List<object>();
      while (oEnum.MoveNext())
      {
        list.Add(oEnum.Current);
      }

      return GetHashCodeFor(list.ToArray());
    }

    /// <summary>
    /// Returns the hash code of the combination of a series of objects.
    /// </summary>
    /// <param name="objs">Objects to make the hash code for.</param>
    /// <returns>A hashcode.</returns>
    public static int GetHashCodeFor(params object[] objs)
    {
      if (objs.LongLength == 0) return 0;

      const int prime = 486187739;
      // overflow is fine
      unchecked {
        int hash = 17;
        foreach (var o in objs)
        {
          hash = hash*prime + GetHashCodeForSingleObject(o);
        }
        return hash;
      }
    }

    /// <summary>
    /// Returns the string representation of an object, null if null or [items] if a collection.
    /// </summary>
    /// <param name="o"></param>
    /// <returns>The string representation.</returns>
    public static string ToStringForSingleObject(object o)
    {
      if (ReferenceEquals(o, null)) return "null";

      var oEnumerable = o as IEnumerable;
      if (oEnumerable == null) return o.ToString();

      // make a list of the items if it is an enumerable
      var oEnum = oEnumerable.GetEnumerator();

      var list = new List<object>();
      while (oEnum.MoveNext()) {
        list.Add(oEnum.Current);
      }

      var sb = new StringBuilder();
      sb.Append("[ ");
      // TODO: this could be optimized by not using select and using the append on each object instead
      sb.Append(string.Join(", ", list.Select(ToStringForSingleObject)));
      sb.Append(" ]");
      return sb.ToString();
    }

    /// <summary>
    /// Returns the string representation of an immutable object.
    /// </summary>
    /// <param name="name">Immutable object type name.</param>
    /// <param name="objs">Properties of the immutable objects (name and value).</param>
    /// <returns>The string representation.</returns>
    public static string ToStringFor(string name, params Tuple<string, object>[] objs)
    {
      var sb = new StringBuilder();
      sb.Append(name + " { ");
      // TODO: this could be optimized by not using select and using the append on each object instead
      sb.Append(string.Join(", ", objs.Select(v => v.Item1 + "=" + ToStringForSingleObject(v.Item2))));
      sb.Append(" }");
      return sb.ToString();
    }
  }
}
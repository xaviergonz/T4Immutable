# T4Immutable
[![NuGet package](https://img.shields.io/nuget/v/T4Immutable.svg)](https://nuget.org/packages/T4Immutable)

T4Immutable is a T4 template for C# .NET apps that generates code for immutable classes.

Creating proper immutable objects in C# requires a lot boilerplate code. The aim of this project is to reduce this to a minimum by means of automatic code generation via T4 templates. For instance, given the following class:
```c#
[ImmutableClass(Options = ImmutableClassOptions.EnableOperatorEquals)]
class Person {
  private const int AgeDefaultValue = 18;

  public string FirstName { get; }
  public string LastName { get; }
  public int Age { get; }
  
  [ComputedProperty]
  public string FullName {
    get {
      return FirstName + " " + LastName;
    }
  }
}
```

It will automatically generate for you in a separate partial class file the following:
* A constructor such as ```public Person(string firstName, string lastName, int age = 18)``` that will initialize the values.
* Working implementations for ```Equals(object other)``` and ```Equals(Person other)```. Also it will add the ```IEquatable<Person>``` interface for you.
* Working implementations for ```operator==``` and ```operator!=```
* A working implementation of ```GetHashCode()```
* A better ```ToString()``` with output such as ```"Person { FirstName=John, LastName=Doe, Age=21 }"```
* A ```Person With(...)``` method that can be used to generate a new immutable clone with 0 or more properties changed (e.g. ```var janeDoe = johnDoe.With(firstName: "Jane", age: 20)```

## How do I start?
Just install the T4Immutable nuget package.

## What's needed to make an immutable class?
Just use the ```[ImmutableClass]``` attribute over the class. The class will be auto-checked to meet the following constraints before code generation takes place:
* Any properties NOT marked as ```ComputedProperty``` will need to be either auto properites or have a non-public setter.
* It cannot have any custom constructors since one will be auto-generated, however please check the "Constructor overrides" section below to see ways to overcome this limitation.
* Any default values (see "How to specify property default values?") will be checked to have the same type than the properties.
* It cannot be static.
* It cannot have any extra partials besides the generated one (this support is still TODO).
* It cannot have a base class (probably to be lifted in a future update if anybody can show a proper use case), it can however have any interfaces.

Besides those checks it is your responsibility to make the immutable object behave correctly. For example you should use ImmutableList instead of List and so on. This project is just made to reduce the boilerplate after all, not ensure correctness.

## I don't want X. Can I control what gets generated?
You sure can, just add to the ImmutableClass attribute something like this:
```
[ImmutableClass(Options = 
  ImmutableClassOptions.DisableEquals | 
  ImmutableClassOptions.DisableGetHashCode | 
  ImmutableClassOptions.EnableOperatorEquals | 
  ImmutableClassOptions.DisableToString | 
  ImmutableClassOptions.DisableWith)]
```
The names should be pretty explanatory. Note that even if you disable for example the Equals implementation you can still use them internally by invoking the ```private bool ImmutableEquals(...)``` implementation. This is done in case you might want to write your own ```Equals(...)``` yet still use the generated one as a base.

## Can I control the access level (public/private/...) of the constructor?
Yes. Do something like this:
```
[ImmutableClass(ConstructorAccessLevel = ConstructorAccessLevel.Private)]
```

## Constructor post-initalization / validation
If you need to do extra initialization / validation on the generated constructor just define a ```private void PostConstructor()``` method and do your work there. It will be invoked after all assignations are done inside the generated constructor.

Alternatively it is of course also possible to do validation inside the properties private/protected setters. E.g:
```c#
private int _Age;
public int Age {
  get { return _Age; }
  set {
    if (value < 18) throw new Exception("You are too young!");
    _Age = value;
  }
}
```

## Can I add extra attributes to each constructor parameter?
Yes, use the following when defining a property:
```c#
// T4Immutable.PreConstructorParam: [JetBrains.Annotations.NotNull]
public string FirstName { get; }
```
Bear in mind the comment must be ABOVE any attributes the property might use and that if you use it to specify attributes they must have the full name (including namespace) or else there would be compilation errors.

## How do I enforce automatic null checking for the constructor parameters? What about for the properties?
If you use this:
```c#
[PreNotNullCheck, PostNotNullCheck]
public string FirstName { get; }
```
The constructor will be this:
```c#
public Person(string firstName) {
  // pre not null check
  if (firstName == null) throw new ArgumentNullException(nameof(firstName));
  
  // assignations + PostConstructor() if needed
  
  // post not null check
  if (this.FirstName == null) throw new NullReferenceException(nameof(this.FirstName));
}
```

Having said this, if you use JetBrains Annotations for null checking, you can also do this:
```c#
[JetBrains.Annotations.NotNull, ConstructorParamNotNull]
public string FirstName { get; }
```
And the constructor will be this:
```c#
public Person([JetBrains.Annotations.NotNull] string firstName) {
  // pre not null check is implied by ConstructorParamNotNull
  if (firstName == null) throw new ArgumentNullException(nameof(firstName));
  
  // assignations + PostConstructor() if needed
  
  // post not null check implied by JetBrains.Annotations.NotNull on the property
  if (this.FirstName == null) throw new NullReferenceException(nameof(this.FirstName));
}
```

## Constructor overrides
If you need to do alternate constructors (for example having a Point\<T>(T x, T y) Immutable class and you want to generate a point from a distance and an angle) then you can do something like:
```c#
public static Point<T> FromAngleAndDistance(T distance, double angle) {
  // your code here
  return new Point(x, y);
}
```

## How do I change the order of the arguments in the generated constructor?
Just change the order of the properties.

## How to specify property default values?
If you want a property to have a given default value on the auto-generated constructor there are two ways. Say that you have a property named int Age, and you want it to have the default value of 18:
* Way 1: ```(private/protected/public/whatever) const int AgeDefaultValue = 18;```
* Way 2: ```(private/protected/public/whatever) static readonly int AgeDefaultValue = 18;```

If you wonder why there are two alternatives it is because sometimes it is possible to add stuff such as ```new Foo()``` as a default parameter for constructors and that expression works as a readonly but does not work as a const.

Please note that default values, like in a constructor, should not have gaps. This is, if you have int x, int y then you should have a default value for y or for x and y. If you want a default value for x then move it to the end.

## Does it work with generic classes? Custom methods? Nested classes?
It sure does!

## What if I want to make the class smarter though not strictly immutable, like caching a point distance after it has been requested the first time?
This is more about reducing boilerplate than ensuring immutability, so you can. E.g.:
```c#
[ImmutableClass]
class Point {
  public double X { get; }
  public double Y { get; }
  
  private double _Distance;
  
  [ComputedProperty]
  public double Distance {
    get {
      if (_Distance == null) _Distance = Math.Sqrt(X*X + Y*Y);
      return _Distance.Value;
    }
  }
}
```
However if you do stuff like this, then since internally it has become mutable-ish you will need to use a lock or some other method if you want it to work properly when the object is used concurrently. A probably better solution would be to initialize the ```_Distance``` member inside the ```PostConstructor()```. It all depends on your use case.

## How do I rebuild the auto-generated files once I make a change in my code?
There are plugins out there that auto-run T4 templates once code changes, but if you don't want/need one then just use ¨Build - Transform All T4 Templates¨.

## Does Intellisense and all that stuff work after using this?
Absolutely, since the generated files are .cs files Intellisense will pick the syntax without problems.

## Why doesn't it generate a builder?
Why would you need one when you can set all parameters at once by using the constructor or change as many parameters as you want at once with a single ```With(...)``` invocation? That being said, please let me know if you think otherwise.

## Can I suggest new features or whatever?
Please do!

## Can I see the extra code generated for the very first example?
Here you go (excluding some redundant attributes):
```c#
using System;

partial class Person : IEquatable<Person> {
  public Person(string firstName, string lastName, int age = 18) {
    this.FirstName = firstName;
    this.LastName = lastName;
    this.Age = age;
    _ImmutableHashCode = new { this.FirstName, this.LastName, this.Age }.GetHashCode();
  }
  
  private bool ImmutableEquals(Person obj) {
    if (ReferenceEquals(this, obj)) return true;
    if (ReferenceEquals(obj, null)) return false;
    return T4Immutable.Helpers.AreEqual(this.FirstName, obj.FirstName) && T4Immutable.Helpers.AreEqual(this.LastName, obj.LastName) && T4Immutable.Helpers.AreEqual(this.Age, obj.Age);
  }
  
  public override bool Equals(object obj) {
    return ImmutableEquals(obj as Person);
  }
  
  public bool Equals(Person obj) {
    return ImmutableEquals(obj);
  }
  
  public static bool operator ==(Person a, Person b) {
    return T4Immutable.Helpers.AreEqual(a, b);
  }
  
  public static bool operator !=(Person a, Person b) {
    return !T4Immutable.Helpers.AreEqual(a, b);
  }
  
  private int _ImmutableHashCode;
  
  private int ImmutableGetHashCode() {
    return _ImmutableHashCode;
  }
  
  public override int GetHashCode() {
    return ImmutableGetHashCode();
  }
  
  private string ImmutableToString() {
    var sb = new System.Text.StringBuilder();
    sb.Append(nameof(Person) + " { ");
    
    var values = new string[] {
      nameof(this.FirstName) + "=" + T4Immutable.Helpers.ToString(this.FirstName),
      nameof(this.LastName) + "=" + T4Immutable.Helpers.ToString(this.LastName),
      nameof(this.Age) + "=" + T4Immutable.Helpers.ToString(this.Age),
    };
    
    sb.Append(string.Join(", ", values) + " }");
    return sb.ToString();
  }
  
  public override string ToString() {
    return ImmutableToString();
  }
  
  private Person ImmutableWith(T4Immutable.WithParam<string> firstName = default(T4Immutable.WithParam<string>), T4Immutable.WithParam<string> lastName = default(T4Immutable.WithParam<string>), T4Immutable.WithParam<int> age = default(T4Immutable.WithParam<int>)) {
    return new Person(
      !firstName.HasValue ? this.FirstName : firstName.Value,
      !lastName.HasValue ? this.LastName : lastName.Value,
      !age.HasValue ? this.Age : age.Value
    );
  }
  
  public Person With(T4Immutable.WithParam<string> firstName = default(T4Immutable.WithParam<string>), T4Immutable.WithParam<string> lastName = default(T4Immutable.WithParam<string>), T4Immutable.WithParam<int> age = default(T4Immutable.WithParam<int>)) {
    return ImmutableWith(firstName, lastName, age);
  }
  
}

```

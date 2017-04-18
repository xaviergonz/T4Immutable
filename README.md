# T4Immutable
### T4Immutable is a T4 template for C# .NET apps that generates code for immutable classes.

[![NuGet package](https://img.shields.io/nuget/v/T4Immutable.svg)](https://nuget.org/packages/T4Immutable)

## Table of contents
* [Why use this?](#why)
* [How do I start?](#starting)
* [What's needed to make an immutable class?](#basics)
* [How are collection (Array, List, Set, Dictionary... plus their Immutable versions) based properties handled?](#collections)
* [Do generated classes serialize/deserialize correctly with JSON.NET / Protobuf.NET / others?](#serialization)
* [I don't want X. Can I control what gets generated?](#codegen-options)
* [Can I control the access level (public/private/...) of the constructor or the builder?](#constructor-builder-access-level)
* [Constructor post-initalization / validation](#constructor-init-validation)
* [Can I add extra attributes to each constructor parameter?](#constructor-param-attribs)
* [How do I enforce automatic null checking for the constructor parameters? What about for the properties?](#null-checks)
* [Constructor overrides](#constructor-overrides)
* [How do I change the order of the arguments in the generated constructor?](#constructor-argument-order)
* [How to specify property default values?](#default-values)
* [Does it work with generic classes? Custom methods? Nested classes?](#supported-features)
* [What if I want to make the class smarter though not strictly immutable, like caching a point distance after it has been requested the first time?](#non-immutability)
* [Does Intellisense and all that stuff work after using this?](#intellisense)
* [Can I suggest new features or whatever?](#suggestions)
* [Can I see the extra code generated for the very first example?](#generated-code-sample)

#### Release notes
* **[v1.3.3]** ImmutableEquals now uses ImmutableGetHashCode as a speed optimization.
* **[v1.3.2]** Fixed the generated equals operator (sometimes it would crash when the first item was null).
* **[v1.3.1]** Made the library portable, however please check the notes inside 'How do I start?' about portable projects.
* **[v1.2.1]** Now supports generating ToBuilder() and a better OptParam implementation.
* **[v1.2.0]** Now supports generating builders.
* **[v1.2.0]** WithParam class is now called OptParam.
* **[v1.1.5]** Added a PreConstructor option to write code such as atributtes before generated constructors.
* **[v1.1.5]** Added ExcludeConstructor and AllowCustomConstructors options.
* **[v1.1.4]** Collection special cases are done when they inherit from ICollection instead of IEnumerable.
* **[v1.1.3]** Using the dynamic keyword instead of reflection for faster KeyValuePair handling.
* **[v1.1.2]** Generated Equals, GetHashCode and ToString now properly support collections as long as they implement IEnumerator. This means that arrays, List, Set, Dictionary, plus its Immutable variants are properly handled.
* **[v1.1.0]** ImmutableClassOptions.EnableXXX/DisableXXX have been renamed to ImmutableClassOptions.IncludeXXX/ExcludeXXX
* **[v1.1.0]** preConstructorParam code comment has been changed to the PreConstructorParam attribute

## <a name="why"></a>Why use this?
Creating proper immutable objects in C# requires a lot boilerplate code. The aim of this project is to reduce this to a minimum by means of automatic code generation via T4 templates. For instance, given the following class:
```c#
[ImmutableClass(Options = ImmutableClassOptions.IncludeOperatorEquals)]
class Person {
  private const int AgeDefaultValue = 18;

  public string FirstName { get; }
  public string LastName { get; }
  public int Age { get; }
  
  [ComputedProperty]
  public string FullName => $"{FirstName} {LastName}";
}
```

It will automatically generate for you in a separate partial class file the following:
* A constructor such as `public Person(string firstName, string lastName, int age = 18)` that will initialize the values.
* Working implementations for `Equals(object other)` and `Equals(Person other)`.
* Working implementations for `operator==` and `operator!=`
* A working implementation of `GetHashCode()`.
* A better `ToString()` with output such as `"Person { FirstName=John, LastName=Doe, Age=21 }"`
* A `Person With(...)` method that can be used to generate a new immutable clone with 0 or more properties changed (e.g. `var janeDoe = johnDoe.With(firstName: "Jane", age: 20)`
* A `Builder` subclass that can be used to create the objects with the builder pattern such as:
```c#
var builder = new Person.Builder().With(firstName: "John").With(lastName: "Doe"); // fluid way
builder.Age = 21; // or via properties
// that can be read back
string firstName = builder.FirstName; // "John"
var lastName = builder.LastName.Value; // "Doe"
Person johnDoe = b.Build();
var janeDoe = johnDoe.ToBuilder().With(firstName: "Jane", age: 20).Build(); // back and forth
```

## <a name="starting"></a>How do I start?
#### Old-style projects (.NET Framework)
1. Install the T4Immutable nuget package
2. Use "**Build - Transform All T4 Templates**" or right click on the _T4Immutable/T4Immutable.tt_ file and click "**Run custom tool**". 

*Remember to do this everytime you update the package or any of your immutable classes change.* If you want to automate it there are plugins out there that auto-run T4 templates before build such as [AutoT4](https://github.com/bennor/AutoT4).

#### New-style projects (.NET Core, .NET Standard, UWP, Portable Class Libraries...)
Since right now NuGet does not support copying files to the project itself you will need to follow the next steps:

1. Install the T4Immutable nuget package
2. Create a folder in the root of your project named "T4Immutable"
3. Download the T4Immutable sources at https://github.com/xaviergonz/T4Immutable/archive/master.zip
4. Create a folder in your project named _T4Immutable_ and make sure it is empty
5. Extract the folder _src/content/T4Immutable_ (inside the ZIP) to the _T4Immutable_ folder of your project

"**Build - Transform All T4 Templates**" **won't work**, so you will need to right click the file _T4Immutable/T4Immutable.tt_ and click on "**Run custom tool**" whenever you want to regenerate your templates.

_You will need to do 2 and 3 everytime you update this nuget package._

As soon as NuGet gets better support for this (or someone tells me how to do it) for new-style projects there will be an update to do this automatically like it is done for old-style .NET projects.

## <a name="basics"></a>What's needed to make an immutable class?
Just mark the class with the use the `[ImmutableClass]` attribute. The class will be auto-checked to meet the following constraints before code generation takes place:
* Any properties _not_ marked as `ComputedProperty` will need to be either auto properites or have a non-public setter.
* It should not have any custom constructors since one will be auto-generated, however please check the "Constructor overrides" section below to see ways to overcome this limitation.
* Any default values (see "How to specify property default values?") will be checked to have the same type than the properties.
* It cannot be static.
* It cannot have any extra partials besides the generated one (this support is still TODO).
* It cannot have a base class (probably to be lifted in a future update if anybody can show a proper use case), it can however have any interfaces.

Besides those checks it is your responsibility to make the immutable object behave correctly. For example you should use ImmutableList instead of List and so on. This project is just made to reduce the boilerplate after all, not ensure correctness.

## <a name="collections"></a>How are collection (Array, List, Set, Dictionary... plus their Immutable versions) based properties handled?
They just work as long as they inherit from `ICollection` (as all of the basic ones do). The generated `Equals()` will check they are equivalent by checking their contents, as well as the generated `GetHashCode()`. Nested collections are not a problem as well.

## <a name="serialization"></a>Do generated classes serialize/deserialize correctly with JSON.NET / Protobuf.NET / others?
#### JSON.NET
* If you use a *public generated constructor* just add `JsonIgnore` to computed properties.
* If you use a *non-public generated constructor* then:

Use `PreConstructor = "[Newtonsoft.Json.JsonConstructor]"` inside the `ImmutableClass` attribute. (Recommended over the next option)

Alternatively:
  1. Add the `ImmutableClassOptions.AllowCustomConstructors` to the `Options` parameter of the `ImmutableClass` attribute.
  2. Add a constructor with no arguments.
  3. Add a private/protected setter to all your non-computed properties if they didn't have any.
  4. Add `JsonIgnore` to computed properties.
  5. NB: In this case JSON.net will call the constructor and then _later_ set the properties one by one.

#### Protobuf.NET
1. Mark your class as `[ProtoContract]`.
2. Add the `ImmutableClassOptions.AllowCustomConstructors` to the `Options` parameter of the `ImmutableClass` attribute.
3. Add a constructor with no arguments.
4. Mark the non-computed properties with `[ProtoMember(unique number)]`.
5. Add to all non-computed properties a private/protected setter if they didn't have any.
6. NB: In this case Protobuf.NET will call the constructor and then _later_ set the properties one by one.

#### Others
* Let me know :)

## <a name="codegen-options"></a>I don't want X. Can I control what gets generated?
You sure can, just add to the ImmutableClass attribute something like this:
```c#
[ImmutableClass(Options = 
  ImmutableClassOptions.ExcludeEquals | // do not generate an Equals() method
  ImmutableClassOptions.ExcludeGetHashCode | // do not generate a GetHashCode() method
  ImmutableClassOptions.IncludeOperatorEquals | // generate operator== and operator!= methods
  ImmutableClassOptions.ExcludeToString | // do not generate a ToString() method
  ImmutableClassOptions.ExcludeWith | // do not generate a With() method
  ImmutableClassOptions.ExcludeConstructor | // do not generate a constructor
  ImmutableClassOptions.ExcludeBuilder | // do not generate a builder or ImmutableToBuilder() - implies ExcludeToBuilder (usually used alongside ExcludeConstructor)
  ImmutableClassOptions.ExcludeToBuilder | // do not generate a builder or ToBuilder() method
  ImmutableClassOptions.AllowCustomConstructors)] // allow custom constructors
```
Note that even if you exclude for example the `Equals()` method implementation you can still use them internally by invoking the `private bool ImmutableEquals(...)` implementation. This is done in case you might want to write your own `Equals()` yet still use the generated one as a base.
Take care you do *not* use "using Foo = ImmutableClassOptions" to save some typing. Due to limitations with T4 it won't work.

## <a name="constructor-builder-access-level"></a>Can I control the access level (public/private/...) of the constructor or the builder?
Yes. Do something like this:
```
[ImmutableClass(ConstructorAccessLevel = ConstructorAccessLevel.Private, BuilderAccessLevel = BuilderAccessLevel.Protected)]
```
Valid options are `Public`, `Protected`, `ProtectedInternal`, `Internal` and `Private`.

## <a name="constructor-init-validation"></a>Constructor post-initalization / validation
If you need to do extra initialization / validation on the generated constructor just define a `void PostConstructor()` method (access modifier doesn't matter) and do your work there. It will be invoked inside the generated constructor after all assignations are done.

Alternatively (and recommended) it is of course also possible to do validation inside the properties private/protected setters. E.g:
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

## <a name="constructor-param-attribs"></a>Can I add extra attributes to each constructor parameter?
Yes, use the following when defining a property:
```c#
[PreConstructorParam("[JetBrains.Annotations.NotNull]")]
public string FirstName { get; }
```
Bear in mind that if you use it to specify attributes they must have the full name (including namespace) or else there would be compilation errors. Also bear in mind that due to T4 limitations the string has to be constant, this is, it shouldn't depend on other const values.

## <a name="null-checks"></a>How do I enforce automatic null checking for the constructor parameters? What about for the properties?
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

## <a name="constructor-overrides"></a>Constructor overrides
If you need to do alternate constructors (for example having a `Point<T>(T x, T y)` immutable class and you want to generate a point from a distance and an angle) then you can do something like:
```c#
public static Point<T> FromAngleAndDistance(T distance, double angle) {
  // your code here
  return new Point(x, y);
}
```
Still, if you still aren't satisfied by this you can enable the `ImmutableOptions.AllowCustomConstructors` and create your own alternate constructor to your own risk.

## <a name="constructor-argument-order"></a>How do I change the order of the arguments in the generated constructor?
Just change the order of the properties.

## <a name="default-values"></a>How to specify property default values?
If you want a property to have a given default value on the auto-generated constructor there are two ways. Say that you have a property named int Age, and you want it to have the default value of 18:
* Way 1: private/protected/public/whatever `const int AgeDefaultValue = 18;`
* Way 2: private/protected/public/whatever `static readonly int AgeDefaultValue = 18;`

If you wonder why there are two alternatives it is because sometimes it is possible to add stuff such as `new Foo()` as a default parameter for constructors and that expression works as a readonly but does not work as a const.

Please note that default values, like in a constructor, should not have gaps.
This is, if you have int x, int y then you should have a default value for y or for x and y.
If you want a default value for x then move it to the end.

## <a name="supported-features"></a>Does it work with generic classes? Custom methods? Nested classes?
It sure does!

## <a name="non-immutability"></a>What if I want to make the class smarter though not strictly immutable, like caching a point distance after it has been requested the first time?
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
However if you do stuff like this, then since internally it has become mutable-ish you will need to use a lock or some other method if you want it to work properly when the object is used concurrently. A probably better solution would be to initialize the ```_Distance``` member inside the `PostConstructor()`. It all depends on your use case.

## <a name="intellisense"></a>Does Intellisense and all that stuff work after using this?
Absolutely, since the generated files are .cs files Intellisense will pick the syntax without problems after the T4 template is built.

## <a name="suggestions"></a>Can I suggest new features or whatever?
Please do!

## <a name="generated-code-sample"></a>Can I see the extra code generated for the very first example?
Here you go (excluding some redundant attributes):
```c#
using System;

partial class Person : IEquatable<Person> {
  public Person(string firstName, string lastName, int age = 18) {
    this.FirstName = firstName;
    this.LastName = lastName;
    this.Age = age;
    _ImmutableHashCode = T4Immutable.Helpers.GetHashCodeFor(this.FirstName, this.LastName, this.Age);
  }
  
  private bool ImmutableEquals(Person obj) {
    if (ReferenceEquals(this, obj)) return true;
    if (ReferenceEquals(obj, null)) return false;
    if (ImmutableGetHashCode() !== obj.ImmutableGetHashCode()) return false;
    return T4Immutable.Helpers.AreEqual(this.FirstName, obj.FirstName) && T4Immutable.Helpers.AreEqual(this.LastName, obj.LastName) && T4Immutable.Helpers.AreEqual(this.Age, obj.Age);
  }
  
  public override bool Equals(object obj) {
    return ImmutableEquals(obj as Person);
  }
  
  public bool Equals(Person obj) {
    return ImmutableEquals(obj);
  }
  
  public static bool operator ==(Person a, Person b) {
    return T4Immutable.Helpers.BasicAreEqual(a, b);
  }
  
  public static bool operator !=(Person a, Person b) {
    return !T4Immutable.Helpers.BasicAreEqual(a, b);
  }
  
  private readonly int _ImmutableHashCode;
  
  private int ImmutableGetHashCode() {
    return _ImmutableHashCode;
  }
  
  public override int GetHashCode() {
    return ImmutableGetHashCode();
  }
  
  private string ImmutableToString() {
    return T4Immutable.Helpers.ToStringFor(nameof(Person), new System.Tuple<string, object>(nameof(this.FirstName), this.FirstName), new System.Tuple<string, object>(nameof(this.LastName), this.LastName), new System.Tuple<string, object>(nameof(this.Age), this.Age));
  }
  
  public override string ToString() {
    return ImmutableToString();
  }
  
  private Person ImmutableWith(T4Immutable.OptParam<string> firstName = default(T4Immutable.OptParam<string>), T4Immutable.OptParam<string> lastName = default(T4Immutable.OptParam<string>), T4Immutable.OptParam<int> age = default(T4Immutable.OptParam<int>)) {
    return new Person(
      firstName.HasValue ? firstName.Value : this.FirstName,
      lastName.HasValue ? lastName.Value : this.LastName,
      age.HasValue ? age.Value : this.Age
    );
  }
  
  public Person With(T4Immutable.OptParam<string> firstName = default(T4Immutable.OptParam<string>), T4Immutable.OptParam<string> lastName = default(T4Immutable.OptParam<string>), T4Immutable.OptParam<int> age = default(T4Immutable.OptParam<int>)) {
    return ImmutableWith(firstName, lastName, age);
  }
  
  public Person.Builder ToBuilder() {
    return ImmutableToBuilder();
  }
  
  private Person.Builder ImmutableToBuilder() {
    return new Person.Builder().With(
      new T4Immutable.OptParam<string>(this.FirstName),
      new T4Immutable.OptParam<string>(this.LastName),
      new T4Immutable.OptParam<int>(this.Age)
    );
  }
  
  public class Builder {
    public T4Immutable.OptParam<string> FirstName { get; set; }
    public T4Immutable.OptParam<string> LastName { get; set; }
    public T4Immutable.OptParam<int> Age { get; set; }
    
    public Builder With(T4Immutable.OptParam<string> firstName = default(T4Immutable.OptParam<string>), T4Immutable.OptParam<string> lastName = default(T4Immutable.OptParam<string>), T4Immutable.OptParam<int> age = default(T4Immutable.OptParam<int>)) {
      if (firstName.HasValue) this.FirstName = firstName;
      if (lastName.HasValue) this.LastName = lastName;
      if (age.HasValue) this.Age = age;
      return this;
    }
    
    public Person Build() {
      if (!this.FirstName.HasValue) throw new InvalidOperationException("Builder property 'FirstName' cannot be left unassigned");
      if (!this.LastName.HasValue) throw new InvalidOperationException("Builder property 'LastName' cannot be left unassigned");
      if (!this.Age.HasValue) this.Age = 18;
      if (!this.Age.HasValue) throw new InvalidOperationException("Builder property 'Age' cannot be left unassigned");
      return new Person(this.FirstName.Value, this.LastName.Value, this.Age.Value);
    }
  }  
}
```

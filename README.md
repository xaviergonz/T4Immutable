# T4Immutable
T4Immutable is a T4 template for C# .NET apps that generates code for immutable classes.

Creating proper immutable objects in C# requires a lot boilerplate code. The aim of this project is to reduce this to a minimum by means of automatic code generation via T4 templates. For instance, given the following class:
```c#
[ImmutableClass(Options = ImmutableClassOptions.EnableOperatorEquals)]
class Person {
  private const int AgeDefaultValue = 18;

  public string FirstName { get; }
  public string LastName { get; }
  public ing Age { get; }
  
  [ComputedProperty]
  public string FullName => FirstName + " " + LastName;
}
```

It will automatically generate for you in a separate partial class file the following:
* A constructor such as ```public Person(string firstName, string lastName, int age = 18)``` that will initialize the values.
* Working implementations for ```Equals(object other)``` and ```Equals(Person other)```
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

## Can I control what gets generated?
You sure can, just add to the ImmutableClass attribute something like this:
```
[ImmutableClass(Options = ImmutableClassDisableEquals | DisableGetHashCode | EnableOperatorEquals | DisableToString| DisableWith)]
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

## Are there any other nice tricks I should know about?
About not null checking for ReSharper users here is a nice trick. Install the JetBrains.Annotations package from NuGet and then do something like:
```c#
[NotNull]
public string FirstName { get; }
```

Then the generated constructor will have this automatically for you:
```c#
public Person([NotNull] string firstName) {
  if (firstName == null) throw new ArgumentNullException(nameof(firstName));
  this.FirstName = firstName;
}
```

## Constructor overrides
If you need to do alternate constructors (for example having a Point\<T>(T x, T y) Immutable class and you want to generate a point from a distance and an angle) then you can do something like:
```c#
public static Point\<T> FromAngleAndDistance(T distance, double angle) {
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

## Does it work with generic classes? Custom methods?
It sure does!

## How do I rebuild the auto-generated files once I make a change in my code?
There are plugins out there that auto-run T4 templates once code changes, but if you don't want/need one then just use ¨Build - Transform All T4 Templates¨.

## Does Intellisense and all that stuff work after using this?
Absolutely, since the generated files are .cs files Intellisense will pick the syntax without problems.

## Can I suggest new features or whatever?
Please do!

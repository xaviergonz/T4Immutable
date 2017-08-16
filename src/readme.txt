===========
T4Immutable
===========

Please run the generator by using "Build - Transform All T4 Templates" or if that doesn't work (e.g.
.NET Core/Standard/etc projects) right click the file "T4Immutable/T4Immutable.tt" and click on 
"Run custom tool" whenever you want to  regenerate your templates.

You should also do this after each time you update this nuget package.

For .NET Framework projects the files will be generated inside "T4Immutable" as children of "T4Immutable.tt".
For .NET Core/Standard/etc projects they will be generated in a folder named "T4Immutable_generated".

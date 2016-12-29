===========
T4Immutable
===========

Note for old-style projects (.NET Framework)
--------------------------------------------

Please run the generator by using "Build - Transform All T4 Templates".
You should do this after each time you update this nuget package.


Note for new-style projects (.NET Core, .NET Standard, UWP, Portable Class Libraries...)
----------------------------------------------------------------------------------------

Since right now NuGet does not support copying files to the project itself you will need 
to follow the next steps:

1. Create a folder in the root of your project named "T4Immutable"
2. Download the T4Immutable sources at 
   https://github.com/xaviergonz/T4Immutable/archive/master.zip
3. Create a folder in your project named T4Immutable and make sure it is empty
4. Extract the folder src/content/T4Immutable (inside the ZIP) to the T4Immutable folder 
   of your project

"Build - Transform All T4 Templates" won't work, so you will need to right click the
file "T4Immutable/T4Immutable.tt" and click on "Run custom tool" whenever you want to 
regenerate your templates.

You will need to do steps 2 to 4 everytime you update this nuget package.

As soon as NuGet gets better support for this (or someone tells me how to do it) for 
new-style projects there will be an update to do this automatically like it is done for 
old-style .NET projects.

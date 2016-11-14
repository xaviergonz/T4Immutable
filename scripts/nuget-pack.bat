@echo off
cd ..
md output
cd src\T4Immutable-portable
nuget pack T4Immutable-portable.nuspec -build -properties Configuration=Release -outputdirectory ..\..\output
cd ..\..\scripts

@echo off
cd ..
md output
cd src\T4Immutable
nuget pack T4Immutable.csproj -build -properties Configuration=Release -outputdirectory ..\..\output
cd ..\..\scripts

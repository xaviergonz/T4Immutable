@echo off
cd ..
md output
cd src\T4Immutable-netstd2
nuget pack T4Immutable.nuspec -build -properties Configuration=Release -outputdirectory ..\..\output
cd ..\..\scripts

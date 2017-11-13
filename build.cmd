@echo off

dotnet build -c "Debug"
dotnet publish -c "Debug"
nuget pack AzureExtension.FullMinify.nuspec


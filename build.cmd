@echo off

dotnet build -c "Release"
dotnet publish -c "Release"
nuget pack AzureExtension.FullMinify.nuspec


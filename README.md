# RazorEngineCore
NETCore 3.1 Razor Template Engine

[![NuGet](https://img.shields.io/nuget/dt/RazorEngineCore.svg?style=flat-square)](https://www.nuget.org/packages/RazorEngineCore)
[![NuGet](https://img.shields.io/nuget/v/RazorEngineCore.svg?style=flat-square)](https://www.nuget.org/packages/RazorEngineCore)

## NuGet
```
Install-Package RazorEngineCore
```

## Examples

#### Basic usage
```cs
RazorEngine razorEngine = new RazorEngine();
RazorEngineCompiledTemplate template = razorEngine.Compile("Hello @Model.Name");

string result = template.Run(new
{
    Name = "Alexander"
});

Console.WriteLine(result);
```

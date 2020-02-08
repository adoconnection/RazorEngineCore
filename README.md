# RazorEngineCore
NETCore 3.1.1 Razor Template Engine

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

#### Save / Load compiled templates
Most expensive task is to compile template, you should not compile template every time you need to run it
```cs
RazorEngine razorEngine = new RazorEngine();
RazorEngineCompiledTemplate template = razorEngine.Compile("Hello @Model.Name");

// save to file
template.SaveToFile("myTemplate.dll");

//save to stream
MemoryStream memoryStream = new MemoryStream();
template.SaveToStream(memoryStream);
```

```cs
RazorEngineCompiledTemplate template1 = RazorEngineCompiledTemplate.LoadFromFile("myTemplate.dll");
RazorEngineCompiledTemplate template2 = RazorEngineCompiledTemplate.LoadFromStream(myStream);
```

#### Template functions
ASP.NET Core 3 way of defining template functions:
```
<area>
    @{ RecursionTest(3); }
</area>

@{
  void RecursionTest(int level)
  {
	if (level <= 0)
	{
		return;
	}

	<div>LEVEL: @level</div>
	@{ RecursionTest(level - 1); }
  }
}
```

#### Helpers
TBD

#### Custom references
TBD


#### Credits
This package is inspired by [Simon Mourier SO post](https://stackoverflow.com/a/47756437/267736)

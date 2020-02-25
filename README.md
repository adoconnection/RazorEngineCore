# RazorEngineCore
NETCore 3.1.1 Razor Template Engine. No legacy code.

[![NuGet](https://img.shields.io/nuget/dt/RazorEngineCore.svg?style=flat-square)](https://www.nuget.org/packages/RazorEngineCore)
[![NuGet](https://img.shields.io/nuget/v/RazorEngineCore.svg?style=flat-square)](https://www.nuget.org/packages/RazorEngineCore)

Every single star makes maintainer happy! ⭐

## NuGet
```
Install-Package RazorEngineCore
```

## Articles
* [CodeProject: Building String Razor Template Engine with Bare Hands](https://www.codeproject.com/Articles/5260233/Building-String-Razor-Template-Engine-with-Bare-Ha)

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

#### Simplest thread safe caching pattern

```cs
private static ConcurrentDictionary<string, RazorEngineCompiledTemplate> TemplateCache = new ConcurrentDictionary<string, RazorEngineCompiledTemplate>();
```

```cs
private string RenderTemplate(string template, object model)
{
    int hashCode = template.GetHashCode();

    RazorEngineCompiledTemplate compiledTemplate = TemplateCache.GetOrAdd(hashCode, i =>
    {
        RazorEngine razorEngine = new RazorEngine();
        return razorEngine.Compile(Content);
    });

    return compiledTemplate.Run(model);
}
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
output:
```
<div>LEVEL: 3</div>
<div>LEVEL: 2</div>
<div>LEVEL: 1</div>
```

#### Helpers and custom members
```cs
string content = @"Hello @A, @B, @Decorator(123)";

RazorEngine razorEngine = new RazorEngine();
RazorEngineCompiledTemplate<CustomModel> template = razorEngine.Compile<CustomModel>(content);

string result = template.Run(instance =>
{
    instance.A = 10;
    instance.B = "Alex";
});

Console.WriteLine(result);
```
```cs
public class CustomModel : RazorEngineTemplateBase
{
    public int A { get; set; }
    public string B { get; set; }

    public string Decorator(object value)
    {
        return "-=" + value + "=-";
    }
}
```



#### Credits
This package is inspired by [Simon Mourier SO post](https://stackoverflow.com/a/47756437/267736)


#### Changelog
* 2020.2.4
	* null values in model correct handling
	* null model fix
	* netstandard2 insted of netcore3.1
* 2020.2.3
	* Html attribute rendering fix
	* Html attribute rendering tests

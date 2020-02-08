dotnet build -c Release
dotnet test -c Release 
dotnet pack -c Release -o artifacts RazorEngineCore\RazorEngineCore.csproj 
dotnet nuget push artifacts\RazorEngineCore.2020.2.1.nupkg --source https://www.nuget.org/api/v2/package
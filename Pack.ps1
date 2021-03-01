dotnet build -c Release
dotnet test
dotnet pack -c Release -o artifacts RazorEngineCore\RazorEngineCore.csproj
dotnet nuget push artifacts\RazorEngineCore.2021.03.1.nupkg --source https://www.nuget.org/api/v2/package -k KEY
dotnet build -c Release
dotnet test
dotnet pack -c Release -o artifacts RazorEngineCore\RazorEngineCore.csproj -p:symbolPackageFormat=snupkg --include-symbols
dotnet nuget push artifacts\RazorEngineCore.2022.8.1.nupkg --source https://www.nuget.org/api/v2/package -k KEY
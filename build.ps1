$version = "1.0.0"
$name = "DownloadMusic"


$today = Get-Date
$date = $today.ToString('yyyy-MM-dd')
Remove-Item ./bin/* -Force -Recurse

Set-Location ./$name
$fileData = @"
namespace $name {
    public static class Version {
        public const string version = "$version";
        public const string date = "$date";
    }
}
"@
$fileData | Out-File -Encoding utf8 ./src/Version.cs

dotnet publish -c release -o ../bin/$name-win-x64 -r win-x64
dotnet publish -c release -o ../bin/$name-osx-x64 -r osx-x64
dotnet publish -c release -o ../bin/$name-linux-x64 -r linux-x64


Compress-Archive ../bin/$name-win-x64 ../bin/$name-$version-win-x64.zip -Force
Compress-Archive ../bin/$name-osx-x64 ../bin/$name-$version-osx-x64.zip -Force
Compress-Archive ../bin/$name-linux-x64 ../bin/$name-$version-linux-x64.zip -Force

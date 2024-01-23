cd /d %~dp0

echo Cleaning old build directory...
rd /s /q win64

echo Building project...
dotnet build ./src/Luban/Luban.csproj -c Release -r win-x64 --self-contained true -o win64

echo Build complete.
pause

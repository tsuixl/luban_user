rd /s /q win64


dotnet build  ./src/Luban/Luban.csproj -c Release -r win-x64 --self-contained true -o win64

# dotnet build  ./src/Luban/Luban.csproj -c Release -o ../../unity-project/LubanDemo/Design/Tools/Luban
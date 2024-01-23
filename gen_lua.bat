cd /d %~dp0

.\win64\Luban.exe ^
-t client ^
-c lua-bin ^
-d sg-lua ^
--conf D:\Moon\work\luban\luban_user\sample\locationtest\luban.conf ^
--validationFailAsError ^
-x outputCodeDir=D:\Moon\work\luban\luban_user\sample\locationtest\output\lua\code ^
-x outputDataDir=D:\Moon\work\luban\luban_user\sample\locationtest\output\lua\data ^
-x outputCustomDir=D:\Moon\work\luban\luban_user\sample\locationtest\custom ^
-x outputAllType=D:\Moon\work\luban\luban_user\sample\locationtest\output\custom\all_type_output.txt ^
-x locationFile=D:\Moon\work\luban\luban_user\sample\locationtest\output\custom\location.txt ^
-x locationExportLanguage=all ^
-x locationDefaultLanguage=zh
pause

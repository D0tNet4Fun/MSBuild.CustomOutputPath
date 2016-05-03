@echo off
@echo Deleting before-after targets...
del ..\14.0\Microsoft.Common.Targets\ImportBefore\Before.CustomOutputPath.targets
del ..\14.0\Microsoft.Common.Targets\ImportAfter\After.CustomOutputPath.targets
del ..\12.0\Microsoft.Common.Targets\ImportBefore\Before.CustomOutputPath.targets
del ..\12.0\Microsoft.Common.Targets\ImportAfter\After.CustomOutputPath.targets
del ..\11.0\Microsoft.Common.Targets\ImportBefore\Before.CustomOutputPath.targets
del ..\11.0\Microsoft.Common.Targets\ImportAfter\After.CustomOutputPath.targets

echo. 
echo Killing MSBuild.exe instances using TaskKill.exe...
TaskKill /IM msbuild.exe -f
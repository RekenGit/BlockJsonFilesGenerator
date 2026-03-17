@echo off
setlocal

set CONFIG=Release
set RUNTIME=win-x64
set OUTDIR=publish-portable

echo Publishing portable build...
dotnet publish MinecraftJsonGenerator.csproj -c %CONFIG% -r %RUNTIME% --self-contained false -o %OUTDIR%

echo.
echo Done. Output: %OUTDIR%
pause

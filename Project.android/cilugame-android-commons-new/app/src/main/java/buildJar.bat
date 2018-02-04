setlocal EnableDelayedExpansion

:: set "abc=%cd%"

%~d0

set a=%~dp0
for %%a in ("%a%") do (
        set ok=%%~dpa
        for /f "delims=" %%b in ("!ok:~0,-1!") do (
		set name=%%~nb
        )
)

cd %~dp0bin

echo 开始打包 %name%

jar -cvf %name%.jar *
pause
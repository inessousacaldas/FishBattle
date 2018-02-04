@echo

set patchProfile=%1%
::Xlsj_Release
::set apkFileName=H1_0.7.7_tsi.apk
set workspace=%2%
::D:\Workspace\H1\client\v0.7

@echo %patchProfile% BuildAPK开始

set patchApkfile=%3%
::APK/Xlsj/Release/tsi/%apkFileName%
set patchCopyDllToDir=BuildAPK/%patchProfile%/target/dlls

CD %workspace%
ant -DpatchProfile=%patchProfile% -DpatchApkfile=%patchApkfile% -DpatchCopyDllToDir=%patchCopyDllToDir% -DforceUpdate=true -Dencrypt=true apk-with-patch

@echo %patchProfile% BuildAPK成功

pause
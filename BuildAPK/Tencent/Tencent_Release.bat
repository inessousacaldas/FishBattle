@echo

set patchProfile=Tencent_Release
set apkFileName=H1_0.6.6_yyb_Tencent.apk
set workspace=D:\Workspace\H1\client\v0.6

@echo %patchProfile% BuildAPK开始

set patchApkfile=APK/Tencent/Release/yyb/%apkFileName%
set patchCopyDllToDir=BuildAPK/%patchProfile%/target/dlls

CD %workspace%
ant -DpatchProfile=%patchProfile% -DpatchApkfile=%patchApkfile% -DpatchCopyDllToDir=%patchCopyDllToDir% -DforceUpdate=true -Dencrypt=true apk-with-patch

@echo %patchProfile% BuildAPK成功

pause
set packPath=D:\work\H5\client\branches\channel-tools\ApkPackTools

copy demiproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\demi\jarlib\demiproxy.jar
copy shoumengproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\shoumeng\jarlib\shoumengproxy.jar
copy ucproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\uc\jarlib\ucproxy.jar
copy yijieproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\yijie\jarlib\yijieproxy.jar
copy qihooproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\qihoo\jarlib\qihooproxy.jar
copy huaweiproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\huawei\jarlib\huaweiproxy.jar
copy oppoproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\oppo\jarlib\oppoproxy.jar
copy acfunproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\acfun\jarlib\acfunproxy.jar
copy nubiyaproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\nubiya\jarlib\nubiyaproxy.jar
copy shunwangproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\shunwang\jarlib\shunwangproxy.jar
copy vivoproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\vivo\jarlib\vivoproxy.jar
copy xiaomiproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\xiaomi\jarlib\xiaomiproxy.jar
copy kaopuproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\kaopu\jarlib\kaopuproxy.jar
copy 4399proxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\4399\jarlib\4399proxy.jar
copy haimaproxy\build\intermediates\bundles\release\classes.jar %packPath%\platforms\haima\jarlib\haimaproxy.jar

copy demiframe\build\intermediates\bundles\release\classes.jar %packPath%\platforms\demiframe.jar

copy app\build\intermediates\bundles\release\classes.jar ..\..\Assets\Plugins\Android\CiluSdk\libs\com.cilugame.cilusdk.jar
copy demiframe\build\intermediates\bundles\release\classes.jar ..\..\Assets\Plugins\Android\libs\demiframe.jar

pause
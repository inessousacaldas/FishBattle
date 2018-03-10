public class ServerNameGetter
{
    private static readonly string serverNameColor = "[c7f6f7]";


    //维护【d4d4d4】
    //新服【00deff】
    //流畅【22f322】
    //火爆【ff0000】
    //繁忙【ff9c00】

    //服务器状态颜色
    private static readonly string[] serverStateColor = new string[5]
    {
        "[d4d4d4]",
        "[00deff]",
        "[22f322]",
        "[ff0000]",
        "[ff9c00]"
    };


    //颜色结束符
    private static string endColorString = "[-]";

    //服务器状态信息
    private static string[] serverStateInfo = new string[4]
    {
        "empty", //"(空闲)",  //0
        "full", //"(忙碌)",  //1
        "boom", //"(拥挤)",  //2
        "fix" //"(维护)",  //3
    };

    //中文数字
    private static string[] serverIDStringArray = new string[10]
    {
        "零", "一", "二", "三", "四", "五", "六", "七", "八", "九"
    };

    //获取服务器ID中文名
    private static string GetServerId(int id)
    {
        int serverID = id;
        string number = string.Empty;

        while (serverID > 0)
        {
            int n = serverID%10;

            number = serverIDStringArray[n] + number;

            serverID = serverID/10;
        }

        number += "服 ";
        return number;
    }


    /// <summary>
    ///     获取服务器名字
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string GetServerName(GameServerInfo info)
    {
        //		int runState = 0;
        //		
        //		//如果不是维护状态
        //		if( message.dboState != 2 )
        //		{
        //			//如果是推荐状态
        //			if( message.recommend == 1 )
        //			{
        //				runState = 1;
        //			}
        //			else
        //			{
        //				//流畅
        //				if( message.runState == 0 )
        //				{
        //					runState = 2;
        //				}
        //				//火爆
        //				else if ( message.runState == 1 )
        //				{
        //					runState = 3;
        //				}
        //				
        //				//繁忙
        //				else
        //				{
        //					runState = 4;
        //				}
        //			}
        //		}
        //		
        //        return string.Format("{0}({1}) {2}{3} {4}{5}{6}",
        //								serverNameColor,
        //								message.serviceId,
        //								message.name,
        //								endColorString,					
        //			
        //								serverStateColor[runState],
        //								serverStateInfo[runState],
        //								endColorString );

        return info.name;
    }

    /// <summary>
    ///     根据状态返回指定状态的图片名
    /// </summary>
    /// <returns>The service state sprite name.</returns>
    /// <param name="message">Message.</param>
    public static string GetServiceStateSpriteName(GameServerInfo info)
    {
        return serverStateInfo[info.runState];
    }


    public static string GetSelectServerName(GameServerInfo message)
    {
        int runState = 0;

        //如果不是维护状态
        if (message.dboState != 2)
        {
            //如果是推荐状态
			if (message.recommendType > 0 || message.newServer)
            {
                runState = 1;
            }
            else
            {
                if (message.runState == 0)
                {
                    runState = 2;
                }
                else if (message.runState == 1)
                {
                    runState = 3;
                }
                else
                {
                    runState = 4;
                }
            }
        }

        return string.Format("{0}{1} {2}{3}",
            serverStateColor[runState],
            message.name,
            serverStateInfo[runState],
            endColorString);
    }
}
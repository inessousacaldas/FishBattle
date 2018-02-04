using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventObject
{
	public const string Event_Leave = "Event_Leave";
	public const string Event_Message = "Event_Message";
	public const string Event_Close = "Event_Close";
	public const string Event_Joined = "Event_Joined";
	public const string Event_Service = "Event_Service";
	public const string Event_State = "Event_State";
	public const string Event_Timeout = "Event_Timeout";
	public const string Event_SSL_OPEN_RES = "Event_SSL_OPEN_RES";
	public const string Event_VER = "Event_VER";
	
	public string type = "event";

	/** 0未知 */
	public const uint Leave_status_unkonwn = 0;
	/** 1主动退出 */
	public const uint Leave_status_logout = 1;
	/** 2被动退出 */
	public const uint Leave_status_disconnect = 2;
	/** 3踢出 */
	public const uint Leave_status_kickout = 3;
	/**4 维护 */
	public const uint Leave_status_destroy = 4;
	/** 5重复登录 */
	public const uint Leave_status_duplicate = 5;

	/** 0未知 */
	public const uint Close_status_unkonwn = 0;
	/** 1 接收数据包长度小于20 */
	public const uint Close_status_MinOfSize = 1;
	/** 2 接收数据包长度超出 */
	public const uint Close_status_OutOfSize = 2;
	/** 3 请求连接超时 */
	public const uint Close_status_ConnectOutTime = 3;
	/** 4 keepalive超时 */
	public const uint Close_status_KeepaliveOutTime = 4;
	/** 5 创建连接时socket断开 */
	public const uint Close_status_StarConnectClose = 5;
	/** 6 游戏时socket断开 */
	public const uint Close_status_GameConnectClose = 6;
	/** 7 接收数据为0 */
	public const uint Close_status_ReceiveNull = 7;
	/** 8 ThreadAbortException */
	public const uint Close_status_ThreadAbortException = 8;
	/** 9 SocketException */
	public const uint Close_status_SocketException = 9;
	/** 10 OtherException */
	public const uint Close_status_OtherException = 10;

	public ByteArray byteArray;
	public List<ServiceInfo> services;
	public string msg = "";
	public uint state = 0;
    public bool compress;

	public EventObject(string _type)
	{
		type = _type;
	}
}


	/**
	 * ha.net返回版本响应信息，版本匹配后，尝试接入ha.net中
	 */

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



	public class VerAckInstruction : MarshalableObject
{

		private string family;
		private uint currentVersion;
		private uint leastVersion;

		public VerAckInstruction() {
		}

		public PacketHeader getPacketHeader()  {
			return header;
		}
		
		public DirectMessageHeader getDirectMessageHeader()   {
			return null;
		}
		
		public  GroupMessageHeader getGroupMessageHeader() {
			return null;			
		}
		
		public BroadcastMessageHeader getBroadcastMessageHeader()  {
			return null;
		}
		
		public ProtoByteArray getBodyMessage()   {
			return null;
		}
		
		public void execute( HaConnector haconn) {
			EventObject obj = new EventObject(EventObject.Event_VER);
			haconn.AddEventObj(obj);
		}

		public ProtoByteArray toBytes() {
			return null;
		}

		public void fromBytes( ProtoByteArray bytes )
		{
			recivedBytes(bytes);

			family = readString();
			currentVersion = readUnsignedInt32();
			leastVersion = readUnsignedInt32();
		}

	}


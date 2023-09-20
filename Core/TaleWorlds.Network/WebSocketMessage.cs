using System;
using System.IO;
using System.Text;

namespace TaleWorlds.Network
{
	[Obsolete]
	public class WebSocketMessage
	{
		public byte[] Payload { get; set; }

		public WebSocketMessage()
		{
			this.MessageInfo = new MessageInfo();
		}

		public void SetTextPayload(string payload)
		{
			this.Payload = WebSocketMessage.Encoding.GetBytes(payload);
		}

		public MessageInfo MessageInfo { get; set; }

		public int Cursor { get; set; }

		public MessageTypes MessageType { get; set; }

		public void WriteTo(bool fromServer, Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			this.MessageInfo.WriteTo(stream, fromServer);
			binaryWriter.Write(this.Payload.Length);
			binaryWriter.Write(this.Payload, 0, this.Payload.Length);
			binaryWriter.Write(this.Cursor);
			binaryWriter.Write((byte)this.MessageType);
		}

		public static WebSocketMessage ReadFrom(bool fromServer, byte[] payload)
		{
			WebSocketMessage webSocketMessage;
			using (MemoryStream memoryStream = new MemoryStream(payload))
			{
				webSocketMessage = WebSocketMessage.ReadFrom(fromServer, memoryStream);
			}
			return webSocketMessage;
		}

		public static WebSocketMessage ReadFrom(bool fromServer, Stream stream)
		{
			WebSocketMessage webSocketMessage = new WebSocketMessage();
			BinaryReader binaryReader = new BinaryReader(stream);
			webSocketMessage.MessageInfo = MessageInfo.ReadFrom(stream, fromServer);
			int num = binaryReader.ReadInt32();
			webSocketMessage.Payload = binaryReader.ReadBytes(num);
			webSocketMessage.Cursor = binaryReader.ReadInt32();
			webSocketMessage.MessageType = (MessageTypes)binaryReader.ReadByte();
			return webSocketMessage;
		}

		public static WebSocketMessage CreateCursorMessage(int cursor)
		{
			return new WebSocketMessage
			{
				MessageType = MessageTypes.Cursor,
				MessageInfo = 
				{
					DestinationPostBox = ""
				},
				Payload = BitConverter.GetBytes(cursor)
			};
		}

		public static WebSocketMessage CreateCloseMessage()
		{
			return new WebSocketMessage
			{
				MessageType = MessageTypes.Close
			};
		}

		public int GetCursor()
		{
			if (this.MessageType == MessageTypes.Cursor)
			{
				return BitConverter.ToInt32(this.Payload, 0);
			}
			throw new Exception("not a cursor message");
		}

		public static Encoding Encoding = Encoding.UTF8;
	}
}

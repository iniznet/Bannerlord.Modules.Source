using System;
using System.IO;
using System.Text;

namespace TaleWorlds.Network
{
	// Token: 0x0200002A RID: 42
	[Obsolete]
	public class WebSocketMessage
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000138 RID: 312 RVA: 0x0000514E File Offset: 0x0000334E
		// (set) Token: 0x06000139 RID: 313 RVA: 0x00005156 File Offset: 0x00003356
		public byte[] Payload { get; set; }

		// Token: 0x0600013A RID: 314 RVA: 0x0000515F File Offset: 0x0000335F
		public WebSocketMessage()
		{
			this.MessageInfo = new MessageInfo();
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00005172 File Offset: 0x00003372
		public void SetTextPayload(string payload)
		{
			this.Payload = WebSocketMessage.Encoding.GetBytes(payload);
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00005185 File Offset: 0x00003385
		// (set) Token: 0x0600013D RID: 317 RVA: 0x0000518D File Offset: 0x0000338D
		public MessageInfo MessageInfo { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600013E RID: 318 RVA: 0x00005196 File Offset: 0x00003396
		// (set) Token: 0x0600013F RID: 319 RVA: 0x0000519E File Offset: 0x0000339E
		public int Cursor { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000140 RID: 320 RVA: 0x000051A7 File Offset: 0x000033A7
		// (set) Token: 0x06000141 RID: 321 RVA: 0x000051AF File Offset: 0x000033AF
		public MessageTypes MessageType { get; set; }

		// Token: 0x06000142 RID: 322 RVA: 0x000051B8 File Offset: 0x000033B8
		public void WriteTo(bool fromServer, Stream stream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			this.MessageInfo.WriteTo(stream, fromServer);
			binaryWriter.Write(this.Payload.Length);
			binaryWriter.Write(this.Payload, 0, this.Payload.Length);
			binaryWriter.Write(this.Cursor);
			binaryWriter.Write((byte)this.MessageType);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00005214 File Offset: 0x00003414
		public static WebSocketMessage ReadFrom(bool fromServer, byte[] payload)
		{
			WebSocketMessage webSocketMessage;
			using (MemoryStream memoryStream = new MemoryStream(payload))
			{
				webSocketMessage = WebSocketMessage.ReadFrom(fromServer, memoryStream);
			}
			return webSocketMessage;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00005250 File Offset: 0x00003450
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

		// Token: 0x06000145 RID: 325 RVA: 0x000052A2 File Offset: 0x000034A2
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

		// Token: 0x06000146 RID: 326 RVA: 0x000052CC File Offset: 0x000034CC
		public static WebSocketMessage CreateCloseMessage()
		{
			return new WebSocketMessage
			{
				MessageType = MessageTypes.Close
			};
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000052DA File Offset: 0x000034DA
		public int GetCursor()
		{
			if (this.MessageType == MessageTypes.Cursor)
			{
				return BitConverter.ToInt32(this.Payload, 0);
			}
			throw new Exception("not a cursor message");
		}

		// Token: 0x0400007A RID: 122
		public static Encoding Encoding = Encoding.UTF8;
	}
}

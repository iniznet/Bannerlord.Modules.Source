using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.ChatSystem.Library
{
	// Token: 0x02000003 RID: 3
	public class ChatClient
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020A7 File Offset: 0x000002A7
		// (set) Token: 0x06000005 RID: 5 RVA: 0x000020AF File Offset: 0x000002AF
		public ChatClientState State { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020B8 File Offset: 0x000002B8
		// (set) Token: 0x06000007 RID: 7 RVA: 0x000020C0 File Offset: 0x000002C0
		public Guid RoomId { get; private set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000008 RID: 8 RVA: 0x000020CC File Offset: 0x000002CC
		// (remove) Token: 0x06000009 RID: 9 RVA: 0x00002104 File Offset: 0x00000304
		public event ChatClient.MessageReceivedDelegate OnMessageReceived;

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002139 File Offset: 0x00000339
		public bool IsConnected
		{
			get
			{
				return this.State == ChatClientState.Connected;
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002144 File Offset: 0x00000344
		public ChatClient(string endpoint, string userName, string displayName, Guid roomId)
		{
			this._endpoint = endpoint;
			this._receivedMessages = new ConcurrentQueue<ChatMessage>();
			this._socket = new ClientWebSocket();
			this.State = ChatClientState.Created;
			this.RoomId = roomId;
			this._userName = userName;
			this._displayName = displayName;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002194 File Offset: 0x00000394
		public async void Connect()
		{
			try
			{
				this.State = ChatClientState.Connecting;
				string text = Base64Helper.Base64UrlEncode(AesHelper.Encrypt(Common.SerializeObject(new UserInfo(this._userName, this._displayName)), Parameters.Key, Parameters.InitializationVector));
				await this._socket.ConnectAsync(new Uri(this._endpoint + "?user=" + text), CancellationToken.None);
				this.State = ChatClientState.Connected;
				await this.Receive();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
				this.State = ChatClientState.Disconnected;
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021D0 File Offset: 0x000003D0
		public void OnTick()
		{
			if (this.State == ChatClientState.Disconnected)
			{
				if (this.reconnectTimer == null)
				{
					this.reconnectTimer = new long?(DateTime.Now.Ticks);
				}
				if (this.reconnectTimer != null && DateTime.Now.Ticks - this.reconnectTimer.Value > 100000000L)
				{
					this.Connect();
					this.reconnectTimer = null;
				}
			}
			ChatMessage chatMessage;
			if (this._receivedMessages.TryDequeue(out chatMessage))
			{
				ChatClient.MessageReceivedDelegate onMessageReceived = this.OnMessageReceived;
				if (onMessageReceived == null)
				{
					return;
				}
				onMessageReceived(this, chatMessage);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000226C File Offset: 0x0000046C
		public async Task Stop()
		{
			this.State = ChatClientState.Stopped;
			if (this._socket.State == WebSocketState.Open || this._socket.State == WebSocketState.Aborted)
			{
				await this._socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022B4 File Offset: 0x000004B4
		public async Task Send(ChatMessage message)
		{
			if (this.IsConnected)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
				await this._socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002304 File Offset: 0x00000504
		private async Task Receive()
		{
			ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
			for (;;)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					WebSocketReceiveResult webSocketReceiveResult;
					do
					{
						webSocketReceiveResult = await this._socket.ReceiveAsync(buffer, CancellationToken.None);
						ms.Write(buffer.Array, buffer.Offset, webSocketReceiveResult.Count);
					}
					while (!webSocketReceiveResult.EndOfMessage);
					if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
					{
						await this.Stop();
						break;
					}
					ms.Seek(0L, SeekOrigin.Begin);
					using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
					{
						foreach (ChatMessage chatMessage in JsonConvert.DeserializeObject<ChatMessage[]>(await reader.ReadToEndAsync()))
						{
							this._receivedMessages.Enqueue(chatMessage);
						}
					}
					StreamReader reader = null;
				}
				MemoryStream ms = null;
			}
		}

		// Token: 0x04000001 RID: 1
		private ConcurrentQueue<ChatMessage> _receivedMessages;

		// Token: 0x04000002 RID: 2
		private string _endpoint;

		// Token: 0x04000003 RID: 3
		private ClientWebSocket _socket;

		// Token: 0x04000007 RID: 7
		private string _userName;

		// Token: 0x04000008 RID: 8
		private string _displayName;

		// Token: 0x04000009 RID: 9
		private long? reconnectTimer;

		// Token: 0x0400000A RID: 10
		private const long ReconnectInterval = 100000000L;

		// Token: 0x0200000A RID: 10
		// (Invoke) Token: 0x06000022 RID: 34
		public delegate void MessageReceivedDelegate(ChatClient client, ChatMessage message);
	}
}

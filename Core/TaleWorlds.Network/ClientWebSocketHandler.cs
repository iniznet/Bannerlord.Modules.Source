using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
	// Token: 0x02000002 RID: 2
	[Obsolete]
	public class ClientWebSocketHandler
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		// (remove) Token: 0x06000002 RID: 2 RVA: 0x00002080 File Offset: 0x00000280
		public event ClientWebSocketHandler.MessageReceivedDelegate MessageReceived;

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000020B5 File Offset: 0x000002B5
		public bool IsConnected
		{
			get
			{
				return ClientWebSocketHandler._webSocket.State == WebSocketState.Open;
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000004 RID: 4 RVA: 0x000020C4 File Offset: 0x000002C4
		// (remove) Token: 0x06000005 RID: 5 RVA: 0x000020FC File Offset: 0x000002FC
		public event ClientWebSocketHandler.OnErrorDelegate OnError;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000006 RID: 6 RVA: 0x00002134 File Offset: 0x00000334
		// (remove) Token: 0x06000007 RID: 7 RVA: 0x0000216C File Offset: 0x0000036C
		public event ClientWebSocketHandler.DisconnectedDelegate Disconnected;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000008 RID: 8 RVA: 0x000021A4 File Offset: 0x000003A4
		// (remove) Token: 0x06000009 RID: 9 RVA: 0x000021DC File Offset: 0x000003DC
		public event ClientWebSocketHandler.ConnectedDelegate Connected;

		// Token: 0x0600000A RID: 10 RVA: 0x00002214 File Offset: 0x00000414
		public ClientWebSocketHandler()
		{
			this._outgoingSocketMessageQueue = new ConcurrentQueue<WebSocketMessage>();
			this._outgoingSocketMessageLog = new ConcurrentQueue<WebSocketMessage>();
			this.Connected += this.ClientWebSocketConnected;
			ClientWebSocketHandler._webSocket = new ClientWebSocket();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002278 File Offset: 0x00000478
		public async Task Connect(string uri, string token, List<KeyValuePair<string, string>> headers = null)
		{
			try
			{
				if (ClientWebSocketHandler._webSocket.State == WebSocketState.Closed || ClientWebSocketHandler._webSocket.State == WebSocketState.Aborted)
				{
					ClientWebSocketHandler._webSocket.Dispose();
					ClientWebSocketHandler._webSocket = new ClientWebSocket();
				}
				if (ClientWebSocketHandler._webSocket.State == WebSocketState.None)
				{
					ClientWebSocketHandler._webSocket.Options.SetRequestHeader("Authorization", "Bearer " + token);
					if (headers != null)
					{
						foreach (KeyValuePair<string, string> keyValuePair in headers)
						{
							ClientWebSocketHandler._webSocket.Options.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
				await ClientWebSocketHandler._webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
				if (ClientWebSocketHandler._webSocket.State == WebSocketState.Open)
				{
					if (this.Connected != null)
					{
						await this.Connected(this);
					}
					Debug.Print("WebSocket connected", 0, Debug.DebugColor.White, 17592186044416UL);
				}
				await Task.WhenAll(new Task[]
				{
					this.Receive(ClientWebSocketHandler._webSocket),
					this.Send(ClientWebSocketHandler._webSocket)
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: {0}", ex);
				this.OnError(this, ex);
			}
			finally
			{
				Console.WriteLine("WebSocket closed.");
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022D8 File Offset: 0x000004D8
		private async Task Receive(ClientWebSocket webSocket)
		{
			ArraySegment<byte> inputSegment = new ArraySegment<byte>(new byte[65536]);
			using (MemoryStream ms = new MemoryStream())
			{
				while (webSocket.State == WebSocketState.Open)
				{
					int num = 0;
					try
					{
						WebSocketReceiveResult webSocketReceiveResult = await webSocket.ReceiveAsync(inputSegment, CancellationToken.None);
						if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
						{
							try
							{
								if (this.Disconnected != null)
								{
									await this.Disconnected(this, true);
								}
								await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Endpoint demanded closure", CancellationToken.None);
								Console.WriteLine("Endpoint demanded closure");
							}
							catch
							{
							}
							return;
						}
						ms.Write(inputSegment.Array, inputSegment.Offset, webSocketReceiveResult.Count);
						if (webSocketReceiveResult.EndOfMessage)
						{
							ms.GetBuffer();
							ms.Seek(0L, SeekOrigin.Begin);
							WebSocketMessage webSocketMessage = WebSocketMessage.ReadFrom(true, ms);
							Console.WriteLine(string.Concat(new object[] { "Message:", ms.Length, " ", webSocketMessage }));
							if (this.MessageReceived != null)
							{
								this.MessageReceived(webSocketMessage, this);
							}
							ms.Seek(0L, SeekOrigin.Begin);
						}
					}
					catch (WebSocketException)
					{
						num = 1;
					}
					if (num == 1)
					{
						if (this.Disconnected != null)
						{
							await this.Disconnected(this, false);
						}
						return;
					}
				}
			}
			MemoryStream ms = null;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002328 File Offset: 0x00000528
		private async Task Send(ClientWebSocket webSocket)
		{
			while (webSocket.State == WebSocketState.Open)
			{
				if (this._outgoingSocketMessageQueue.Count > 0)
				{
					WebSocketMessage webSocketMessage;
					if (this._outgoingSocketMessageQueue.TryDequeue(out webSocketMessage))
					{
						MemoryStream memoryStream = new MemoryStream();
						webSocketMessage.WriteTo(false, memoryStream);
						await webSocket.SendAsync(new ArraySegment<byte>(memoryStream.GetBuffer()), WebSocketMessageType.Binary, true, CancellationToken.None);
						this._messageSentCursor = webSocketMessage.Cursor;
						this.AddMessageToBuffer(webSocketMessage);
						Debug.Print("message sent to: " + ((webSocketMessage.MessageInfo.DestinationPostBox != null) ? webSocketMessage.MessageInfo.DestinationPostBox : webSocketMessage.MessageInfo.DestinationClientId.ToString()), 0, Debug.DebugColor.White, 17592186044416UL);
					}
					webSocketMessage = null;
				}
				await Task.Delay(10);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002378 File Offset: 0x00000578
		private void AddMessageToBuffer(WebSocketMessage message)
		{
			this._outgoingSocketMessageLog.Enqueue(message);
			while (this._outgoingSocketMessageLog.Count > this.logBufferSize)
			{
				WebSocketMessage webSocketMessage = null;
				this._outgoingSocketMessageLog.TryDequeue(out webSocketMessage);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000023B8 File Offset: 0x000005B8
		private void ResetMessageQueueByCursor(int serverCursor)
		{
			while (this._outgoingSocketMessageLog.Count > 0)
			{
				WebSocketMessage webSocketMessage = null;
				if (this._outgoingSocketMessageLog.TryDequeue(out webSocketMessage) && webSocketMessage.Cursor > serverCursor)
				{
					this._outgoingSocketMessageQueue.Enqueue(webSocketMessage);
				}
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000023FB File Offset: 0x000005FB
		private Task ClientWebSocketConnected(ClientWebSocketHandler sender)
		{
			this.SendCursorMessage();
			return Task.FromResult<int>(0);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000240C File Offset: 0x0000060C
		private void SendCursorMessage()
		{
			WebSocketMessage webSocketMessage = WebSocketMessage.CreateCursorMessage(this._lastReceivedMessage);
			this._outgoingSocketMessageQueue.Enqueue(webSocketMessage);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002434 File Offset: 0x00000634
		public async Task Disconnect(string reason, bool onDisconnectCommand)
		{
			try
			{
				if (ClientWebSocketHandler._webSocket != null && (ClientWebSocketHandler._webSocket.State == WebSocketState.Open || ClientWebSocketHandler._webSocket.State == WebSocketState.Connecting))
				{
					await ClientWebSocketHandler._webSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, reason, CancellationToken.None);
				}
			}
			catch (ObjectDisposedException ex)
			{
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
			}
			if (this.Disconnected != null)
			{
				await this.Disconnected(this, onDisconnectCommand);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000248C File Offset: 0x0000068C
		public void SendTextMessage(string postBoxId, string text)
		{
			this._messageQueueCursor++;
			WebSocketMessage webSocketMessage = new WebSocketMessage();
			webSocketMessage.SetTextPayload(text);
			webSocketMessage.Cursor = this._messageQueueCursor;
			webSocketMessage.MessageType = MessageTypes.Rest;
			webSocketMessage.MessageInfo.DestinationPostBox = postBoxId;
			this._outgoingSocketMessageQueue.Enqueue(webSocketMessage);
		}

		// Token: 0x04000005 RID: 5
		private int _messageSentCursor = -1;

		// Token: 0x04000006 RID: 6
		private int _messageQueueCursor = -1;

		// Token: 0x04000007 RID: 7
		private int _lastReceivedMessage = -1;

		// Token: 0x04000008 RID: 8
		private ConcurrentQueue<WebSocketMessage> _outgoingSocketMessageQueue;

		// Token: 0x04000009 RID: 9
		private ConcurrentQueue<WebSocketMessage> _outgoingSocketMessageLog;

		// Token: 0x0400000A RID: 10
		private int logBufferSize = 100;

		// Token: 0x0400000B RID: 11
		private static object consoleLock = new object();

		// Token: 0x0400000C RID: 12
		private const int sendChunkSize = 256;

		// Token: 0x0400000D RID: 13
		private const int receiveChunkSize = 256;

		// Token: 0x0400000E RID: 14
		private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000.0);

		// Token: 0x0400000F RID: 15
		private static ClientWebSocket _webSocket = null;

		// Token: 0x0200002D RID: 45
		// (Invoke) Token: 0x0600015B RID: 347
		public delegate void MessageReceivedDelegate(WebSocketMessage message, ClientWebSocketHandler socket);

		// Token: 0x0200002E RID: 46
		// (Invoke) Token: 0x0600015F RID: 351
		public delegate void OnErrorDelegate(ClientWebSocketHandler sender, Exception ex);

		// Token: 0x0200002F RID: 47
		// (Invoke) Token: 0x06000163 RID: 355
		public delegate Task DisconnectedDelegate(ClientWebSocketHandler sender, bool onDisconnectCommand);

		// Token: 0x02000030 RID: 48
		// (Invoke) Token: 0x06000167 RID: 359
		public delegate Task ConnectedDelegate(ClientWebSocketHandler sender);
	}
}

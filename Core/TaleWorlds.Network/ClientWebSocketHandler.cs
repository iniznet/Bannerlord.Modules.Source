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
	[Obsolete]
	public class ClientWebSocketHandler
	{
		public event ClientWebSocketHandler.MessageReceivedDelegate MessageReceived;

		public bool IsConnected
		{
			get
			{
				return ClientWebSocketHandler._webSocket.State == WebSocketState.Open;
			}
		}

		public event ClientWebSocketHandler.OnErrorDelegate OnError;

		public event ClientWebSocketHandler.DisconnectedDelegate Disconnected;

		public event ClientWebSocketHandler.ConnectedDelegate Connected;

		public ClientWebSocketHandler()
		{
			this._outgoingSocketMessageQueue = new ConcurrentQueue<WebSocketMessage>();
			this._outgoingSocketMessageLog = new ConcurrentQueue<WebSocketMessage>();
			this.Connected += this.ClientWebSocketConnected;
			ClientWebSocketHandler._webSocket = new ClientWebSocket();
		}

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

		private void AddMessageToBuffer(WebSocketMessage message)
		{
			this._outgoingSocketMessageLog.Enqueue(message);
			while (this._outgoingSocketMessageLog.Count > this.logBufferSize)
			{
				WebSocketMessage webSocketMessage = null;
				this._outgoingSocketMessageLog.TryDequeue(out webSocketMessage);
			}
		}

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

		private Task ClientWebSocketConnected(ClientWebSocketHandler sender)
		{
			this.SendCursorMessage();
			return Task.FromResult<int>(0);
		}

		private void SendCursorMessage()
		{
			WebSocketMessage webSocketMessage = WebSocketMessage.CreateCursorMessage(this._lastReceivedMessage);
			this._outgoingSocketMessageQueue.Enqueue(webSocketMessage);
		}

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

		private int _messageSentCursor = -1;

		private int _messageQueueCursor = -1;

		private int _lastReceivedMessage = -1;

		private ConcurrentQueue<WebSocketMessage> _outgoingSocketMessageQueue;

		private ConcurrentQueue<WebSocketMessage> _outgoingSocketMessageLog;

		private int logBufferSize = 100;

		private static object consoleLock = new object();

		private const int sendChunkSize = 256;

		private const int receiveChunkSize = 256;

		private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000.0);

		private static ClientWebSocket _webSocket = null;

		public delegate void MessageReceivedDelegate(WebSocketMessage message, ClientWebSocketHandler socket);

		public delegate void OnErrorDelegate(ClientWebSocketHandler sender, Exception ex);

		public delegate Task DisconnectedDelegate(ClientWebSocketHandler sender, bool onDisconnectCommand);

		public delegate Task ConnectedDelegate(ClientWebSocketHandler sender);
	}
}

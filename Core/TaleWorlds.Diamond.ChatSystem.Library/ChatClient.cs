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
	public class ChatClient
	{
		public ChatClientState State { get; private set; }

		public Guid RoomId { get; private set; }

		public event ChatClient.MessageReceivedDelegate OnMessageReceived;

		public bool IsConnected
		{
			get
			{
				return this.State == ChatClientState.Connected;
			}
		}

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

		public async Task Stop()
		{
			this.State = ChatClientState.Stopped;
			if (this._socket.State == WebSocketState.Open || this._socket.State == WebSocketState.Aborted)
			{
				await this._socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
			}
		}

		public async Task Send(ChatMessage message)
		{
			if (this.IsConnected)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
				await this._socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}

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

		private ConcurrentQueue<ChatMessage> _receivedMessages;

		private string _endpoint;

		private ClientWebSocket _socket;

		private string _userName;

		private string _displayName;

		private long? reconnectTimer;

		private const long ReconnectInterval = 100000000L;

		public delegate void MessageReceivedDelegate(ChatClient client, ChatMessage message);
	}
}

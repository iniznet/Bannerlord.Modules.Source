using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Diamond.ChatSystem.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000F3 RID: 243
	public class ChatManager
	{
		// Token: 0x06000439 RID: 1081 RVA: 0x00006035 File Offset: 0x00004235
		public ChatManager(IChatClientHandler chatClientHandler)
		{
			this._clients = new Dictionary<string, ChatClient>();
			this._rooms = new Dictionary<Guid, ChatRoomInformationForClient>();
			this._chatClientHandler = chatClientHandler;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0000605C File Offset: 0x0000425C
		public void OnJoinChatRoom(ChatRoomInformationForClient info, PlayerId playerId, string playerName)
		{
			if (this._rooms.ContainsKey(info.RoomId))
			{
				return;
			}
			if (!this._clients.ContainsKey(info.Endpoint))
			{
				ChatClient chatClient = new ChatClient(info.Endpoint, playerId.ToString(), playerName, info.RoomId);
				this._clients.Add(info.Endpoint, chatClient);
				chatClient.OnMessageReceived += this.ClientOnMessageReceived;
				chatClient.Connect();
			}
			this._rooms.Add(info.RoomId, info);
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x000060EC File Offset: 0x000042EC
		public void OnTick()
		{
			if (!this._isCleaningUp)
			{
				List<string> list = null;
				foreach (string text in this._clients.Keys)
				{
					ChatClient chatClient = this._clients[text];
					chatClient.OnTick();
					if (chatClient.State == ChatClientState.Stopped)
					{
						if (list == null)
						{
							list = new List<string>(1);
						}
						list.Add(text);
					}
				}
				if (list != null)
				{
					foreach (string text2 in list)
					{
						ChatClient chatClient2 = this._clients[text2];
						this._rooms.Remove(chatClient2.RoomId);
						this._clients.Remove(text2);
					}
				}
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x000061E0 File Offset: 0x000043E0
		public List<ChatRoomInformationForClient> Rooms
		{
			get
			{
				return this._rooms.Values.ToList<ChatRoomInformationForClient>();
			}
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x000061F4 File Offset: 0x000043F4
		public void OnChatRoomClosed(Guid roomId)
		{
			ChatRoomInformationForClient room;
			if (this._rooms.TryGetValue(roomId, out room))
			{
				room = this._rooms[roomId];
				this._rooms.Remove(roomId);
				if (!this._rooms.Any((KeyValuePair<Guid, ChatRoomInformationForClient> item) => room.Endpoint == item.Value.Endpoint))
				{
					this._clients[room.Endpoint].Stop();
					this._clients.Remove(room.Endpoint);
				}
			}
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x00006288 File Offset: 0x00004488
		private void ClientOnMessageReceived(ChatClient client, ChatMessage message)
		{
			ChatRoomInformationForClient chatRoomInformationForClient;
			if (this._rooms.TryGetValue(message.RoomId, out chatRoomInformationForClient))
			{
				this._chatClientHandler.OnChatMessageReceived(message.RoomId, chatRoomInformationForClient.Name, message.Name, message.Text, chatRoomInformationForClient.RoomColor, message.Type);
			}
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x000062DC File Offset: 0x000044DC
		public async void SendMessage(Guid roomId, string message)
		{
			ChatRoomInformationForClient chatRoomInformationForClient;
			if (this._rooms.TryGetValue(roomId, out chatRoomInformationForClient))
			{
				string endpoint = chatRoomInformationForClient.Endpoint;
				ChatClient chatClient;
				if (this._clients.TryGetValue(endpoint, out chatClient))
				{
					await chatClient.Send(new ChatMessage
					{
						RoomId = roomId,
						Text = message
					});
				}
			}
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00006328 File Offset: 0x00004528
		public async void Cleanup()
		{
			this._isCleaningUp = true;
			foreach (KeyValuePair<string, ChatClient> keyValuePair in this._clients)
			{
				await keyValuePair.Value.Stop();
			}
			Dictionary<string, ChatClient>.Enumerator enumerator = default(Dictionary<string, ChatClient>.Enumerator);
			this._clients.Clear();
			this._rooms.Clear();
			this._isCleaningUp = false;
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x00006364 File Offset: 0x00004564
		public ChatManager.GetChatRoomResult TryGetChatRoom(string command)
		{
			if (!command.StartsWith("/"))
			{
				return ChatManager.GetChatRoomResult.CreateFailed(new TextObject("{=taPBAd4c}Given command does not start with /", null));
			}
			string text = command.ToLower().Split(new char[] { '/' }).Last<string>();
			List<ChatRoomInformationForClient> list = new List<ChatRoomInformationForClient>();
			foreach (ChatRoomInformationForClient chatRoomInformationForClient in this._rooms.Values)
			{
				if (chatRoomInformationForClient.Name.ToLower().StartsWith(text))
				{
					list.Add(chatRoomInformationForClient);
				}
			}
			if (list.Count == 1)
			{
				return ChatManager.GetChatRoomResult.CreateSuccessful(list[0]);
			}
			if (list.Count == 0)
			{
				TextObject textObject = new TextObject("{=YOYvBVu1}No chat room found matching {COMMAND}", null);
				textObject.SetTextVariable("COMMAND", command);
				return ChatManager.GetChatRoomResult.CreateFailed(textObject);
			}
			TextObject textObject2 = new TextObject("{=6doiovtH}Disambiguation: {CHATROOMS}", null);
			string text2 = "";
			for (int i = 0; i < list.Count; i++)
			{
				text2 += list[i];
				if (i != list.Count - 1)
				{
					text2 += ", ";
				}
			}
			textObject2.SetTextVariable("CHATROOMS", text2);
			return ChatManager.GetChatRoomResult.CreateFailed(textObject2);
		}

		// Token: 0x040001D6 RID: 470
		private readonly Dictionary<string, ChatClient> _clients;

		// Token: 0x040001D7 RID: 471
		private readonly Dictionary<Guid, ChatRoomInformationForClient> _rooms;

		// Token: 0x040001D8 RID: 472
		private readonly IChatClientHandler _chatClientHandler;

		// Token: 0x040001D9 RID: 473
		private bool _isCleaningUp;

		// Token: 0x02000171 RID: 369
		public class GetChatRoomResult
		{
			// Token: 0x1700031C RID: 796
			// (get) Token: 0x060008F1 RID: 2289 RVA: 0x0000FC25 File Offset: 0x0000DE25
			// (set) Token: 0x060008F2 RID: 2290 RVA: 0x0000FC2D File Offset: 0x0000DE2D
			public bool Successful { get; private set; }

			// Token: 0x1700031D RID: 797
			// (get) Token: 0x060008F3 RID: 2291 RVA: 0x0000FC36 File Offset: 0x0000DE36
			// (set) Token: 0x060008F4 RID: 2292 RVA: 0x0000FC3E File Offset: 0x0000DE3E
			public ChatRoomInformationForClient Room { get; private set; }

			// Token: 0x1700031E RID: 798
			// (get) Token: 0x060008F5 RID: 2293 RVA: 0x0000FC47 File Offset: 0x0000DE47
			// (set) Token: 0x060008F6 RID: 2294 RVA: 0x0000FC4F File Offset: 0x0000DE4F
			public TextObject ErrorMessage { get; private set; }

			// Token: 0x060008F7 RID: 2295 RVA: 0x0000FC58 File Offset: 0x0000DE58
			public GetChatRoomResult(bool successful, ChatRoomInformationForClient room, TextObject error)
			{
				this.Successful = successful;
				this.Room = room;
				this.ErrorMessage = error;
			}

			// Token: 0x060008F8 RID: 2296 RVA: 0x0000FC75 File Offset: 0x0000DE75
			public static ChatManager.GetChatRoomResult CreateSuccessful(ChatRoomInformationForClient room)
			{
				return new ChatManager.GetChatRoomResult(true, room, new TextObject("", null));
			}

			// Token: 0x060008F9 RID: 2297 RVA: 0x0000FC89 File Offset: 0x0000DE89
			public static ChatManager.GetChatRoomResult CreateFailed(TextObject error)
			{
				return new ChatManager.GetChatRoomResult(false, null, error);
			}
		}
	}
}

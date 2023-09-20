using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Diamond.ChatSystem.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class ChatManager
	{
		public ChatManager(IChatClientHandler chatClientHandler)
		{
			this._clients = new Dictionary<string, ChatClient>();
			this._rooms = new Dictionary<Guid, ChatRoomInformationForClient>();
			this._chatClientHandler = chatClientHandler;
		}

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

		public List<ChatRoomInformationForClient> Rooms
		{
			get
			{
				return this._rooms.Values.ToList<ChatRoomInformationForClient>();
			}
		}

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

		private void ClientOnMessageReceived(ChatClient client, ChatMessage message)
		{
			ChatRoomInformationForClient chatRoomInformationForClient;
			if (this._rooms.TryGetValue(message.RoomId, out chatRoomInformationForClient))
			{
				this._chatClientHandler.OnChatMessageReceived(message.RoomId, chatRoomInformationForClient.Name, message.Name, message.Text, chatRoomInformationForClient.RoomColor, message.Type);
			}
		}

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

		private readonly Dictionary<string, ChatClient> _clients;

		private readonly Dictionary<Guid, ChatRoomInformationForClient> _rooms;

		private readonly IChatClientHandler _chatClientHandler;

		private bool _isCleaningUp;

		public class GetChatRoomResult
		{
			public bool Successful { get; private set; }

			public ChatRoomInformationForClient Room { get; private set; }

			public TextObject ErrorMessage { get; private set; }

			public GetChatRoomResult(bool successful, ChatRoomInformationForClient room, TextObject error)
			{
				this.Successful = successful;
				this.Room = room;
				this.ErrorMessage = error;
			}

			public static ChatManager.GetChatRoomResult CreateSuccessful(ChatRoomInformationForClient room)
			{
				return new ChatManager.GetChatRoomResult(true, room, new TextObject("", null));
			}

			public static ChatManager.GetChatRoomResult CreateFailed(TextObject error)
			{
				return new ChatManager.GetChatRoomResult(false, null, error);
			}
		}
	}
}

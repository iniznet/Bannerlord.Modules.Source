using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class LobbyNotification
	{
		public int Id { get; }

		public NotificationType Type { get; }

		public DateTime Date { get; }

		public string Message { get; }

		public Dictionary<string, string> Parameters { get; }

		public LobbyNotification(NotificationType type, DateTime date, string message)
		{
			this.Id = -1;
			this.Type = type;
			this.Date = date;
			this.Message = message;
			this.Parameters = new Dictionary<string, string>();
		}

		public LobbyNotification(int id, NotificationType type, DateTime date, string message, string serializedParameters)
		{
			this.Id = id;
			this.Type = type;
			this.Date = date;
			this.Message = message;
			try
			{
				this.Parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedParameters);
			}
			catch (Exception)
			{
				this.Parameters = new Dictionary<string, string>();
			}
		}

		public string GetParametersAsString()
		{
			string text = "{}";
			try
			{
				text = JsonConvert.SerializeObject(this.Parameters, 0);
			}
			catch (Exception)
			{
			}
			return text;
		}

		public TextObject GetTextObjectOfMessage()
		{
			TextObject textObject;
			if (!GameTexts.TryGetText(this.Message, out textObject, null))
			{
				textObject = new TextObject("{=!}" + this.Message, null);
			}
			return textObject;
		}

		public const string BadgeIdParameterName = "badge_id";

		public const string FriendRequesterParameterName = "friend_requester";
	}
}

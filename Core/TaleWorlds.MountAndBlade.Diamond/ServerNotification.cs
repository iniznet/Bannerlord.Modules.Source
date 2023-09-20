using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ServerNotification
	{
		public ServerNotificationType Type { get; }

		public string Message { get; }

		public ServerNotification(ServerNotificationType type, string message)
		{
			this.Type = type;
			this.Message = message;
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
	}
}

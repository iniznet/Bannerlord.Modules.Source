using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200012A RID: 298
	[Serializable]
	public class LobbyNotification
	{
		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000706 RID: 1798 RVA: 0x0000B344 File Offset: 0x00009544
		public int Id { get; }

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000707 RID: 1799 RVA: 0x0000B34C File Offset: 0x0000954C
		public NotificationType Type { get; }

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000708 RID: 1800 RVA: 0x0000B354 File Offset: 0x00009554
		public DateTime Date { get; }

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000709 RID: 1801 RVA: 0x0000B35C File Offset: 0x0000955C
		public string Message { get; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x0600070A RID: 1802 RVA: 0x0000B364 File Offset: 0x00009564
		public Dictionary<string, string> Parameters { get; }

		// Token: 0x0600070B RID: 1803 RVA: 0x0000B36C File Offset: 0x0000956C
		public LobbyNotification(NotificationType type, DateTime date, string message)
		{
			this.Id = -1;
			this.Type = type;
			this.Date = date;
			this.Message = message;
			this.Parameters = new Dictionary<string, string>();
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0000B39C File Offset: 0x0000959C
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

		// Token: 0x0600070D RID: 1805 RVA: 0x0000B3FC File Offset: 0x000095FC
		public string GetParametersAsString()
		{
			string text = "{}";
			try
			{
				text = JsonConvert.SerializeObject(this.Parameters, Formatting.None);
			}
			catch (Exception)
			{
			}
			return text;
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0000B434 File Offset: 0x00009634
		public TextObject GetTextObjectOfMessage()
		{
			TextObject textObject;
			if (!GameTexts.TryGetText(this.Message, out textObject, null))
			{
				textObject = new TextObject("{=!}" + this.Message, null);
			}
			return textObject;
		}

		// Token: 0x0400032D RID: 813
		public const string BadgeIdParameterName = "badge_id";

		// Token: 0x0400032E RID: 814
		public const string FriendRequesterParameterName = "friend_requester";
	}
}

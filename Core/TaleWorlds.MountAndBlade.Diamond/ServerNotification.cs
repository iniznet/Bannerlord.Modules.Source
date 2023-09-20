using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200014F RID: 335
	[Serializable]
	public class ServerNotification
	{
		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600085C RID: 2140 RVA: 0x0000E2EF File Offset: 0x0000C4EF
		public ServerNotificationType Type { get; }

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600085D RID: 2141 RVA: 0x0000E2F7 File Offset: 0x0000C4F7
		public string Message { get; }

		// Token: 0x0600085E RID: 2142 RVA: 0x0000E2FF File Offset: 0x0000C4FF
		public ServerNotification(ServerNotificationType type, string message)
		{
			this.Type = type;
			this.Message = message;
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x0000E318 File Offset: 0x0000C518
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

﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	// Token: 0x020002E6 RID: 742
	public class RebellionStartedLogEntry : LogEntry, IChatNotification
	{
		// Token: 0x06002AEE RID: 10990 RVA: 0x000B5B9A File Offset: 0x000B3D9A
		internal static void AutoGeneratedStaticCollectObjectsRebellionStartedLogEntry(object o, List<object> collectedObjects)
		{
			((RebellionStartedLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002AEF RID: 10991 RVA: 0x000B5BA8 File Offset: 0x000B3DA8
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Settlement);
		}

		// Token: 0x06002AF0 RID: 10992 RVA: 0x000B5BBD File Offset: 0x000B3DBD
		internal static object AutoGeneratedGetMemberValueSettlement(object o)
		{
			return ((RebellionStartedLogEntry)o).Settlement;
		}

		// Token: 0x06002AF1 RID: 10993 RVA: 0x000B5BCA File Offset: 0x000B3DCA
		internal static object AutoGeneratedGetMemberValue_isVisibleNotification(object o)
		{
			return ((RebellionStartedLogEntry)o)._isVisibleNotification;
		}

		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x06002AF2 RID: 10994 RVA: 0x000B5BDC File Offset: 0x000B3DDC
		public bool IsVisibleNotification
		{
			get
			{
				return this._isVisibleNotification;
			}
		}

		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06002AF3 RID: 10995 RVA: 0x000B5BE4 File Offset: 0x000B3DE4
		public override ChatNotificationType NotificationType
		{
			get
			{
				return base.MilitaryNotification(null, this.Settlement.OwnerClan);
			}
		}

		// Token: 0x06002AF4 RID: 10996 RVA: 0x000B5BF8 File Offset: 0x000B3DF8
		public RebellionStartedLogEntry(Settlement settlement, Clan oldOwnerCLan)
		{
			this.Settlement = settlement;
			this._isVisibleNotification = oldOwnerCLan == Clan.PlayerClan || (oldOwnerCLan.Kingdom != null && oldOwnerCLan.Kingdom == Clan.PlayerClan.Kingdom);
		}

		// Token: 0x06002AF5 RID: 10997 RVA: 0x000B5C35 File Offset: 0x000B3E35
		public override string ToString()
		{
			return this.GetNotificationText().ToString();
		}

		// Token: 0x06002AF6 RID: 10998 RVA: 0x000B5C42 File Offset: 0x000B3E42
		public TextObject GetNotificationText()
		{
			TextObject textObject = new TextObject("{=fbsFZWhb}Rebels in {SETTLEMENT} have taken the ownership of the settlement.", null);
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.Name);
			return textObject;
		}

		// Token: 0x04000CF5 RID: 3317
		[SaveableField(310)]
		public readonly Settlement Settlement;

		// Token: 0x04000CF6 RID: 3318
		[SaveableField(311)]
		private readonly bool _isVisibleNotification;
	}
}

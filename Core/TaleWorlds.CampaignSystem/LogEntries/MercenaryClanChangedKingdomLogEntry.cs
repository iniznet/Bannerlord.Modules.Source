﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	// Token: 0x020002E0 RID: 736
	public class MercenaryClanChangedKingdomLogEntry : LogEntry, IChatNotification, IWarLog
	{
		// Token: 0x06002AA5 RID: 10917 RVA: 0x000B4A2B File Offset: 0x000B2C2B
		internal static void AutoGeneratedStaticCollectObjectsMercenaryClanChangedKingdomLogEntry(object o, List<object> collectedObjects)
		{
			((MercenaryClanChangedKingdomLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002AA6 RID: 10918 RVA: 0x000B4A39 File Offset: 0x000B2C39
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Clan);
			collectedObjects.Add(this.OldKingdom);
			collectedObjects.Add(this.NewKingdom);
		}

		// Token: 0x06002AA7 RID: 10919 RVA: 0x000B4A66 File Offset: 0x000B2C66
		internal static object AutoGeneratedGetMemberValueClan(object o)
		{
			return ((MercenaryClanChangedKingdomLogEntry)o).Clan;
		}

		// Token: 0x06002AA8 RID: 10920 RVA: 0x000B4A73 File Offset: 0x000B2C73
		internal static object AutoGeneratedGetMemberValueOldKingdom(object o)
		{
			return ((MercenaryClanChangedKingdomLogEntry)o).OldKingdom;
		}

		// Token: 0x06002AA9 RID: 10921 RVA: 0x000B4A80 File Offset: 0x000B2C80
		internal static object AutoGeneratedGetMemberValueNewKingdom(object o)
		{
			return ((MercenaryClanChangedKingdomLogEntry)o).NewKingdom;
		}

		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x06002AAA RID: 10922 RVA: 0x000B4A8D File Offset: 0x000B2C8D
		public bool IsVisibleNotification
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002AAB RID: 10923 RVA: 0x000B4A90 File Offset: 0x000B2C90
		public MercenaryClanChangedKingdomLogEntry(Clan clan, Kingdom oldKingdom, Kingdom newKingdom)
		{
			this.Clan = clan;
			this.OldKingdom = oldKingdom;
			this.NewKingdom = newKingdom;
		}

		// Token: 0x06002AAC RID: 10924 RVA: 0x000B4AB0 File Offset: 0x000B2CB0
		public bool IsRelatedToWar(StanceLink stance, out IFaction effector, out IFaction effected)
		{
			IFaction faction = stance.Faction1;
			IFaction faction2 = stance.Faction2;
			Kingdom newKingdom = this.NewKingdom;
			effector = ((newKingdom != null) ? newKingdom.MapFaction : null);
			Kingdom oldKingdom = this.OldKingdom;
			effected = ((oldKingdom != null) ? oldKingdom.MapFaction : null);
			return this.NewKingdom == faction2 || this.NewKingdom == faction;
		}

		// Token: 0x06002AAD RID: 10925 RVA: 0x000B4B07 File Offset: 0x000B2D07
		public override string ToString()
		{
			return this.GetNotificationText().ToString();
		}

		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x06002AAE RID: 10926 RVA: 0x000B4B14 File Offset: 0x000B2D14
		public override ChatNotificationType NotificationType
		{
			get
			{
				return base.MilitaryNotification(this.NewKingdom, this.OldKingdom);
			}
		}

		// Token: 0x06002AAF RID: 10927 RVA: 0x000B4B28 File Offset: 0x000B2D28
		public TextObject GetNotificationText()
		{
			if (this.OldKingdom == null && this.NewKingdom != null)
			{
				TextObject textObject = GameTexts.FindText("str_notification_mercenary_contract", null);
				textObject.SetTextVariable("CLAN", this.Clan.Name);
				textObject.SetTextVariable("KINGDOM", this.NewKingdom.InformalName);
				return textObject;
			}
			if (this.OldKingdom != null && this.NewKingdom == null)
			{
				TextObject textObject2 = GameTexts.FindText("str_notification_mercenary_contract_end", null);
				textObject2.SetTextVariable("CLAN", this.Clan.Name);
				textObject2.SetTextVariable("KINGDOM", this.OldKingdom.InformalName);
				return textObject2;
			}
			if (this.OldKingdom != null && this.NewKingdom != null)
			{
				TextObject textObject3 = GameTexts.FindText("str_notification_mercenary_contract", null);
				textObject3.SetTextVariable("CLAN", this.Clan.Name);
				textObject3.SetTextVariable("KINGDOM", this.NewKingdom.InformalName);
				return textObject3;
			}
			return TextObject.Empty;
		}

		// Token: 0x04000CDE RID: 3294
		[SaveableField(250)]
		public readonly Clan Clan;

		// Token: 0x04000CDF RID: 3295
		[SaveableField(251)]
		public readonly Kingdom OldKingdom;

		// Token: 0x04000CE0 RID: 3296
		[SaveableField(252)]
		public readonly Kingdom NewKingdom;
	}
}

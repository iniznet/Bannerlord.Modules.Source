﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	// Token: 0x02000261 RID: 609
	public class SettlementRebellionMapNotification : InformationData
	{
		// Token: 0x06001F6F RID: 8047 RVA: 0x00088A57 File Offset: 0x00086C57
		internal static void AutoGeneratedStaticCollectObjectsSettlementRebellionMapNotification(object o, List<object> collectedObjects)
		{
			((SettlementRebellionMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06001F70 RID: 8048 RVA: 0x00088A65 File Offset: 0x00086C65
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.RebelliousSettlement);
		}

		// Token: 0x06001F71 RID: 8049 RVA: 0x00088A7A File Offset: 0x00086C7A
		internal static object AutoGeneratedGetMemberValueRebelliousSettlement(object o)
		{
			return ((SettlementRebellionMapNotification)o).RebelliousSettlement;
		}

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x06001F72 RID: 8050 RVA: 0x00088A87 File Offset: 0x00086C87
		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=pgbC8UkU}Rebellion", null);
			}
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x06001F73 RID: 8051 RVA: 0x00088A94 File Offset: 0x00086C94
		public override string SoundEventPath
		{
			get
			{
				return "event:/ui/notification/settlement_rebellion";
			}
		}

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x06001F74 RID: 8052 RVA: 0x00088A9B File Offset: 0x00086C9B
		// (set) Token: 0x06001F75 RID: 8053 RVA: 0x00088AA3 File Offset: 0x00086CA3
		[SaveableProperty(1)]
		public Settlement RebelliousSettlement { get; private set; }

		// Token: 0x06001F76 RID: 8054 RVA: 0x00088AAC File Offset: 0x00086CAC
		public SettlementRebellionMapNotification(Settlement settlement, TextObject descriptionText)
			: base(descriptionText)
		{
			this.RebelliousSettlement = settlement;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	public class SettlementUnderSiegeMapNotification : InformationData
	{
		internal static void AutoGeneratedStaticCollectObjectsSettlementUnderSiegeMapNotification(object o, List<object> collectedObjects)
		{
			((SettlementUnderSiegeMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.BesiegerParty);
			collectedObjects.Add(this.BesiegedSettlement);
		}

		internal static object AutoGeneratedGetMemberValueBesiegerParty(object o)
		{
			return ((SettlementUnderSiegeMapNotification)o).BesiegerParty;
		}

		internal static object AutoGeneratedGetMemberValueBesiegedSettlement(object o)
		{
			return ((SettlementUnderSiegeMapNotification)o).BesiegedSettlement;
		}

		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=siegeevent}Siege", null);
			}
		}

		public override string SoundEventPath
		{
			get
			{
				return "";
			}
		}

		[SaveableProperty(10)]
		public MobileParty BesiegerParty { get; private set; }

		[SaveableProperty(20)]
		public Settlement BesiegedSettlement { get; private set; }

		public SettlementUnderSiegeMapNotification(SiegeEvent siegeEvent, TextObject descriptionText)
			: base(descriptionText)
		{
			this.BesiegerParty = siegeEvent.BesiegerCamp.BesiegerParty;
			this.BesiegedSettlement = siegeEvent.BesiegedSettlement;
		}
	}
}

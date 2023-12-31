﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	public class PartyLeaderChangeNotification : InformationData
	{
		internal static void AutoGeneratedStaticCollectObjectsPartyLeaderChangeNotification(object o, List<object> collectedObjects)
		{
			((PartyLeaderChangeNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Party);
		}

		internal static object AutoGeneratedGetMemberValueParty(object o)
		{
			return ((PartyLeaderChangeNotification)o).Party;
		}

		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=nFl0ufe3}A party without a leader", null);
			}
		}

		public override string SoundEventPath
		{
			get
			{
				return "event:/ui/notification/death";
			}
		}

		[SaveableProperty(1)]
		public MobileParty Party { get; private set; }

		public PartyLeaderChangeNotification(MobileParty party, TextObject descriptionText)
			: base(descriptionText)
		{
			this.Party = party;
		}

		public override bool IsValid()
		{
			MobileParty party = this.Party;
			return party != null && party.IsActive;
		}
	}
}

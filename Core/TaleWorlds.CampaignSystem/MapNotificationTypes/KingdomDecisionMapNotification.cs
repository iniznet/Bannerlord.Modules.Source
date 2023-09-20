﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	public class KingdomDecisionMapNotification : InformationData
	{
		internal static void AutoGeneratedStaticCollectObjectsKingdomDecisionMapNotification(object o, List<object> collectedObjects)
		{
			((KingdomDecisionMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.KingdomOfDecision);
			collectedObjects.Add(this.Decision);
		}

		internal static object AutoGeneratedGetMemberValueKingdomOfDecision(object o)
		{
			return ((KingdomDecisionMapNotification)o).KingdomOfDecision;
		}

		internal static object AutoGeneratedGetMemberValueDecision(object o)
		{
			return ((KingdomDecisionMapNotification)o).Decision;
		}

		public override TextObject TitleText
		{
			get
			{
				return this.Decision.GetGeneralTitle();
			}
		}

		public override string SoundEventPath
		{
			get
			{
				return "event:/ui/notification/kingdom_decision";
			}
		}

		[SaveableProperty(1)]
		public Kingdom KingdomOfDecision { get; private set; }

		[SaveableProperty(2)]
		public KingdomDecision Decision { get; private set; }

		public KingdomDecisionMapNotification(Kingdom kingdom, KingdomDecision decision, TextObject descriptionText)
			: base(descriptionText)
		{
			this.KingdomOfDecision = kingdom;
			this.Decision = decision;
		}
	}
}
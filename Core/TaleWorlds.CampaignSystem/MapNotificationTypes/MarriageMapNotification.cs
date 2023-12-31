﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	public class MarriageMapNotification : InformationData
	{
		internal static void AutoGeneratedStaticCollectObjectsMarriageMapNotification(object o, List<object> collectedObjects)
		{
			((MarriageMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Suitor);
			collectedObjects.Add(this.Maiden);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.CreationTime, collectedObjects);
		}

		internal static object AutoGeneratedGetMemberValueSuitor(object o)
		{
			return ((MarriageMapNotification)o).Suitor;
		}

		internal static object AutoGeneratedGetMemberValueMaiden(object o)
		{
			return ((MarriageMapNotification)o).Maiden;
		}

		internal static object AutoGeneratedGetMemberValueCreationTime(object o)
		{
			return ((MarriageMapNotification)o).CreationTime;
		}

		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=bJZJI867}Marriage", null);
			}
		}

		public override string SoundEventPath
		{
			get
			{
				return "event:/ui/notification/marriage";
			}
		}

		[SaveableProperty(1)]
		public Hero Suitor { get; private set; }

		[SaveableProperty(2)]
		public Hero Maiden { get; private set; }

		[SaveableProperty(3)]
		public CampaignTime CreationTime { get; private set; }

		public MarriageMapNotification(Hero firstHero, Hero secondHero, TextObject descriptionText, CampaignTime creationTime)
			: base(descriptionText)
		{
			this.Suitor = (firstHero.IsFemale ? secondHero : firstHero);
			this.Maiden = (firstHero.IsFemale ? firstHero : secondHero);
			this.CreationTime = creationTime;
		}
	}
}

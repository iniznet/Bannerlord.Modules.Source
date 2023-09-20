﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	public class MarriageOfferMapNotification : InformationData
	{
		internal static void AutoGeneratedStaticCollectObjectsMarriageOfferMapNotification(object o, List<object> collectedObjects)
		{
			((MarriageOfferMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Suitor);
			collectedObjects.Add(this.Maiden);
		}

		internal static object AutoGeneratedGetMemberValueSuitor(object o)
		{
			return ((MarriageOfferMapNotification)o).Suitor;
		}

		internal static object AutoGeneratedGetMemberValueMaiden(object o)
		{
			return ((MarriageOfferMapNotification)o).Maiden;
		}

		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=1OQubYTT}Marriage Offer", null);
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

		public MarriageOfferMapNotification(Hero firstHero, Hero secondHero, TextObject descriptionText)
			: base(descriptionText)
		{
			this.Suitor = (firstHero.IsFemale ? secondHero : firstHero);
			this.Maiden = (firstHero.IsFemale ? firstHero : secondHero);
		}

		public override bool IsValid()
		{
			return Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(this.Suitor, this.Maiden);
		}
	}
}
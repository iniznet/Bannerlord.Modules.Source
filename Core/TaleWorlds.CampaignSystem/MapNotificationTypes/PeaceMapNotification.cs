﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	public class PeaceMapNotification : InformationData
	{
		internal static void AutoGeneratedStaticCollectObjectsPeaceMapNotification(object o, List<object> collectedObjects)
		{
			((PeaceMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.FirstFaction);
			collectedObjects.Add(this.SecondFaction);
		}

		internal static object AutoGeneratedGetMemberValueFirstFaction(object o)
		{
			return ((PeaceMapNotification)o).FirstFaction;
		}

		internal static object AutoGeneratedGetMemberValueSecondFaction(object o)
		{
			return ((PeaceMapNotification)o).SecondFaction;
		}

		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=yTOVeqYe}Declaration of Peace", null);
			}
		}

		public override string SoundEventPath
		{
			get
			{
				return "event:/ui/notification/peace";
			}
		}

		[SaveableProperty(1)]
		public IFaction FirstFaction { get; private set; }

		[SaveableProperty(2)]
		public IFaction SecondFaction { get; private set; }

		public PeaceMapNotification(IFaction firstFaction, IFaction secondFaction, TextObject descriptionText)
			: base(descriptionText)
		{
			this.FirstFaction = firstFaction;
			this.SecondFaction = secondFaction;
		}
	}
}
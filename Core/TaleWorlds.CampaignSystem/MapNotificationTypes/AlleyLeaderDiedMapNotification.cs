﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	public class AlleyLeaderDiedMapNotification : InformationData
	{
		internal static void AutoGeneratedStaticCollectObjectsAlleyLeaderDiedMapNotification(object o, List<object> collectedObjects)
		{
			((AlleyLeaderDiedMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Alley);
		}

		internal static object AutoGeneratedGetMemberValueAlley(object o)
		{
			return ((AlleyLeaderDiedMapNotification)o).Alley;
		}

		[SaveableProperty(10)]
		public Alley Alley { get; private set; }

		public AlleyLeaderDiedMapNotification(Alley alley, TextObject description)
			: base(description)
		{
			this.Alley = alley;
		}

		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=6QoSHiWC}An alley without a leader", null);
			}
		}

		public override string SoundEventPath
		{
			get
			{
				return "event:/ui/notification/death";
			}
		}
	}
}

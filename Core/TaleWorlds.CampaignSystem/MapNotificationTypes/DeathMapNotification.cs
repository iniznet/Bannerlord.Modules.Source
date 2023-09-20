﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.MapNotificationTypes
{
	public class DeathMapNotification : InformationData
	{
		internal static void AutoGeneratedStaticCollectObjectsDeathMapNotification(object o, List<object> collectedObjects)
		{
			((DeathMapNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.VictimHero);
			collectedObjects.Add(this.KillerHero);
		}

		internal static object AutoGeneratedGetMemberValueVictimHero(object o)
		{
			return ((DeathMapNotification)o).VictimHero;
		}

		internal static object AutoGeneratedGetMemberValueKillerHero(object o)
		{
			return ((DeathMapNotification)o).KillerHero;
		}

		internal static object AutoGeneratedGetMemberValueKillDetail(object o)
		{
			return ((DeathMapNotification)o).KillDetail;
		}

		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=W73My5KO}Death", null);
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
		public Hero VictimHero { get; private set; }

		[SaveableProperty(2)]
		public Hero KillerHero { get; private set; }

		[SaveableProperty(3)]
		public KillCharacterAction.KillCharacterActionDetail KillDetail { get; private set; }

		public DeathMapNotification(Hero victimHero, Hero killerHero, TextObject descriptionText, KillCharacterAction.KillCharacterActionDetail detail)
			: base(descriptionText)
		{
			this.VictimHero = victimHero;
			this.KillerHero = killerHero;
			this.KillDetail = detail;
		}
	}
}

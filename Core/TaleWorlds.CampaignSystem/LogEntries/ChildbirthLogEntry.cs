﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class ChildbirthLogEntry : LogEntry, IEncyclopediaLog, IChatNotification
	{
		internal static void AutoGeneratedStaticCollectObjectsChildbirthLogEntry(object o, List<object> collectedObjects)
		{
			((ChildbirthLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Mother);
			collectedObjects.Add(this.NewbornHero);
		}

		internal static object AutoGeneratedGetMemberValueNewbornHero(object o)
		{
			return ((ChildbirthLogEntry)o).NewbornHero;
		}

		internal static object AutoGeneratedGetMemberValueMother(object o)
		{
			return ((ChildbirthLogEntry)o).Mother;
		}

		[SaveableProperty(143)]
		public Hero NewbornHero { get; private set; }

		public bool IsVisibleNotification
		{
			get
			{
				return true;
			}
		}

		public override ChatNotificationType NotificationType
		{
			get
			{
				return base.CivilianNotification(this.Mother.Clan);
			}
		}

		public override CampaignTime KeepInHistoryTime
		{
			get
			{
				return CampaignTime.Days(Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays * 2f);
			}
		}

		public bool NeedsNewLogEntryForTwin { get; private set; }

		public Hero NewLogTwin { get; private set; }

		public ChildbirthLogEntry(Hero mother, Hero newbornHero)
		{
			this.Mother = mother;
			this.NewbornHero = newbornHero;
		}

		public override string ToString()
		{
			return this.GetEncyclopediaText().ToString();
		}

		public TextObject GetNotificationText()
		{
			return this.GetEncyclopediaText();
		}

		public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
		{
			return obj == this.Mother;
		}

		public TextObject GetEncyclopediaText()
		{
			TextObject textObject;
			if (this.NewbornHero != null)
			{
				textObject = GameTexts.FindText("str_notification_give_birth_single_child", null);
			}
			else
			{
				textObject = GameTexts.FindText("str_notification_give_birth_single_stillborn", null);
			}
			StringHelpers.SetCharacterProperties("MOTHER", this.Mother.CharacterObject, textObject, false);
			return textObject;
		}

		[SaveableField(140)]
		public readonly Hero Mother;
	}
}

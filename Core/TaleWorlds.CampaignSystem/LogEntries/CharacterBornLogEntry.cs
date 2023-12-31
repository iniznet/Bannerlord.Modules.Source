﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class CharacterBornLogEntry : LogEntry, IEncyclopediaLog, IChatNotification
	{
		internal static void AutoGeneratedStaticCollectObjectsCharacterBornLogEntry(object o, List<object> collectedObjects)
		{
			((CharacterBornLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.BornCharacter);
		}

		internal static object AutoGeneratedGetMemberValueBornCharacter(object o)
		{
			return ((CharacterBornLogEntry)o).BornCharacter;
		}

		public bool IsVisibleNotification
		{
			get
			{
				return true;
			}
		}

		public override CampaignTime KeepInHistoryTime
		{
			get
			{
				return CampaignTime.Years((float)(Campaign.Current.Models.AgeModel.BecomeOldAge / 2));
			}
		}

		public CharacterBornLogEntry(Hero bornCharacter)
		{
			this.BornCharacter = bornCharacter;
		}

		public override ChatNotificationType NotificationType
		{
			get
			{
				return base.PoliticalNotification(this.BornCharacter.Clan);
			}
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
			return obj == this.BornCharacter;
		}

		public TextObject GetEncyclopediaText()
		{
			TextObject textObject = GameTexts.FindText("str_notification_character_born", null);
			StringHelpers.SetCharacterProperties("HERO", this.BornCharacter.CharacterObject, textObject, false);
			StringHelpers.SetCharacterProperties("MOTHER", this.BornCharacter.Mother.CharacterObject, textObject, false);
			StringHelpers.SetCharacterProperties("FATHER", this.BornCharacter.Father.CharacterObject, textObject, false);
			return textObject;
		}

		[SaveableField(100)]
		public readonly Hero BornCharacter;
	}
}

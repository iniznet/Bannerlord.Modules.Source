﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class DefeatCharacterLogEntry : LogEntry
	{
		internal static void AutoGeneratedStaticCollectObjectsDefeatCharacterLogEntry(object o, List<object> collectedObjects)
		{
			((DefeatCharacterLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.WinnerHero);
			collectedObjects.Add(this.LoserHero);
		}

		internal static object AutoGeneratedGetMemberValueWinnerHero(object o)
		{
			return ((DefeatCharacterLogEntry)o).WinnerHero;
		}

		internal static object AutoGeneratedGetMemberValueLoserHero(object o)
		{
			return ((DefeatCharacterLogEntry)o).LoserHero;
		}

		public DefeatCharacterLogEntry(Hero winner, Hero loser)
		{
			this.WinnerHero = winner;
			this.LoserHero = loser;
		}

		public override ImportanceEnum GetImportanceForClan(Clan clan)
		{
			if (this.WinnerHero.Clan == clan || this.LoserHero.Clan == clan)
			{
				return ImportanceEnum.SlightlyImportant;
			}
			return ImportanceEnum.Zero;
		}

		public override void GetConversationScoreAndComment(Hero talkTroop, bool findString, out string comment, out ImportanceEnum score)
		{
			score = ImportanceEnum.Zero;
			comment = "";
			if (this.WinnerHero == Hero.MainHero)
			{
				if (this.LoserHero == talkTroop)
				{
					score = ImportanceEnum.SlightlyImportant;
					if (findString)
					{
						comment = "str_comment_you_defeated_me_in_single_combat";
						return;
					}
				}
			}
			else if (this.LoserHero == Hero.MainHero && this.WinnerHero == talkTroop)
			{
				score = ImportanceEnum.SlightlyImportant;
				if (findString)
				{
					comment = "str_comment_i_defeated_you_in_single_combat";
				}
			}
		}

		public override string ToString()
		{
			TextObject textObject = new TextObject("{=!}{LOSER.NAME} defeated by {WINNER.NAME}.", null);
			StringHelpers.SetCharacterProperties("WINNER", this.WinnerHero.CharacterObject, textObject, false);
			StringHelpers.SetCharacterProperties("LOSER", this.LoserHero.CharacterObject, textObject, false);
			return textObject.ToString();
		}

		[SaveableField(200)]
		public readonly Hero WinnerHero;

		[SaveableField(201)]
		public readonly Hero LoserHero;
	}
}
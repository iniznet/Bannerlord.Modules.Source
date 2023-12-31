﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class SiegeAftermathLogEntry : LogEntry
	{
		internal static void AutoGeneratedStaticCollectObjectsSiegeAftermathLogEntry(object o, List<object> collectedObjects)
		{
			((SiegeAftermathLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._capturedSettlement);
			collectedObjects.Add(this._decisionMaker);
		}

		internal static object AutoGeneratedGetMemberValue_siegeAftermath(object o)
		{
			return ((SiegeAftermathLogEntry)o)._siegeAftermath;
		}

		internal static object AutoGeneratedGetMemberValue_capturedSettlement(object o)
		{
			return ((SiegeAftermathLogEntry)o)._capturedSettlement;
		}

		internal static object AutoGeneratedGetMemberValue_decisionMaker(object o)
		{
			return ((SiegeAftermathLogEntry)o)._decisionMaker;
		}

		internal static object AutoGeneratedGetMemberValue_playerWasInvolved(object o)
		{
			return ((SiegeAftermathLogEntry)o)._playerWasInvolved;
		}

		public SiegeAftermathLogEntry(MobileParty leaderParty, IEnumerable<MobileParty> attackers, Settlement settlement, SiegeAftermathAction.SiegeAftermath siegeAftermath)
		{
			this._siegeAftermath = siegeAftermath;
			this._decisionMaker = leaderParty.LeaderHero;
			this._capturedSettlement = settlement;
			this._playerWasInvolved = false;
			using (IEnumerator<MobileParty> enumerator = attackers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == MobileParty.MainParty)
					{
						this._playerWasInvolved = true;
						break;
					}
				}
			}
		}

		public override ImportanceEnum GetImportanceForClan(Clan clan)
		{
			return ImportanceEnum.Zero;
		}

		public override void GetConversationScoreAndComment(Hero talkTroop, bool findString, out string comment, out ImportanceEnum score)
		{
			score = ImportanceEnum.Zero;
			comment = "";
			if (this._playerWasInvolved && Hero.MainHero.CurrentSettlement == this._capturedSettlement && Hero.OneToOneConversationHero.IsNotable)
			{
				score = ImportanceEnum.VeryImportant;
				if (this._siegeAftermath == SiegeAftermathAction.SiegeAftermath.ShowMercy)
				{
					comment = "str_comment_endplayerbattle_you_stormed_this_city_showed_mercy";
				}
				if (this._siegeAftermath == SiegeAftermathAction.SiegeAftermath.Devastate)
				{
					comment = "str_comment_endplayerbattle_you_stormed_this_city_devastated";
				}
				if (this._siegeAftermath == SiegeAftermathAction.SiegeAftermath.Pillage)
				{
					comment = "str_comment_endplayerbattle_you_stormed_this_city";
				}
			}
		}

		public override string ToString()
		{
			return this.GetNotificationText().ToString();
		}

		public TextObject GetNotificationText()
		{
			TextObject textObject = null;
			if (this._siegeAftermath == SiegeAftermathAction.SiegeAftermath.ShowMercy)
			{
				textObject = new TextObject("{=wTh00qoj}{HERO.NAME} has showed mercy to {SETTLEMENT}.", null);
			}
			if (this._siegeAftermath == SiegeAftermathAction.SiegeAftermath.Devastate)
			{
				textObject = new TextObject("{=NeTp63aU}{HERO.NAME} has devastated {SETTLEMENT}.", null);
			}
			if (this._siegeAftermath == SiegeAftermathAction.SiegeAftermath.Pillage)
			{
				textObject = new TextObject("{=VzAqZucZ}{HERO.NAME} has pillaged {SETTLEMENT}.", null);
			}
			if (this._decisionMaker != null)
			{
				textObject.SetCharacterProperties("HERO", this._decisionMaker.CharacterObject, false);
			}
			textObject.SetTextVariable("SETTLEMENT", this._capturedSettlement.Name);
			return textObject;
		}

		[SaveableField(10)]
		private readonly SiegeAftermathAction.SiegeAftermath _siegeAftermath;

		[SaveableField(20)]
		private readonly Settlement _capturedSettlement;

		[SaveableField(30)]
		private readonly Hero _decisionMaker;

		[SaveableField(40)]
		private readonly bool _playerWasInvolved;
	}
}

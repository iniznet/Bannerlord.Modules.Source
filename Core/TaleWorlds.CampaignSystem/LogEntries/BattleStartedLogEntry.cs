﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class BattleStartedLogEntry : LogEntry, IEncyclopediaLog, IChatNotification
	{
		internal static void AutoGeneratedStaticCollectObjectsBattleStartedLogEntry(object o, List<object> collectedObjects)
		{
			((BattleStartedLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._settlement);
			collectedObjects.Add(this._attackerName);
			collectedObjects.Add(this._defenderName);
			collectedObjects.Add(this._attackerFaction);
			collectedObjects.Add(this._attackerLord);
		}

		internal static object AutoGeneratedGetMemberValue_notificationTextColor(object o)
		{
			return ((BattleStartedLogEntry)o)._notificationTextColor;
		}

		internal static object AutoGeneratedGetMemberValue_settlement(object o)
		{
			return ((BattleStartedLogEntry)o)._settlement;
		}

		internal static object AutoGeneratedGetMemberValue_attackerName(object o)
		{
			return ((BattleStartedLogEntry)o)._attackerName;
		}

		internal static object AutoGeneratedGetMemberValue_defenderName(object o)
		{
			return ((BattleStartedLogEntry)o)._defenderName;
		}

		internal static object AutoGeneratedGetMemberValue_isVisibleNotification(object o)
		{
			return ((BattleStartedLogEntry)o)._isVisibleNotification;
		}

		internal static object AutoGeneratedGetMemberValue_battleDetail(object o)
		{
			return ((BattleStartedLogEntry)o)._battleDetail;
		}

		internal static object AutoGeneratedGetMemberValue_attackerFaction(object o)
		{
			return ((BattleStartedLogEntry)o)._attackerFaction;
		}

		internal static object AutoGeneratedGetMemberValue_attackerLord(object o)
		{
			return ((BattleStartedLogEntry)o)._attackerLord;
		}

		internal static object AutoGeneratedGetMemberValue_attackerPartyHasArmy(object o)
		{
			return ((BattleStartedLogEntry)o)._attackerPartyHasArmy;
		}

		public bool IsVisibleNotification
		{
			get
			{
				return this._isVisibleNotification;
			}
		}

		public override ChatNotificationType NotificationType
		{
			get
			{
				return base.AdversityNotification(null, null);
			}
		}

		public BattleStartedLogEntry(PartyBase attackerParty, PartyBase defenderParty, object subject)
		{
			this._notificationTextColor = defenderParty.MapFaction.LabelColor;
			this._attackerName = ((attackerParty.IsMobile && attackerParty.MobileParty.Army != null) ? attackerParty.MobileParty.ArmyName : attackerParty.Name);
			this._defenderName = ((defenderParty.IsMobile && defenderParty.MobileParty.Army != null) ? defenderParty.MobileParty.ArmyName : defenderParty.Name);
			this._attackerFaction = attackerParty.MapFaction;
			this._attackerPartyHasArmy = attackerParty.IsMobile && attackerParty.MobileParty.Army != null && attackerParty.MobileParty.LeaderHero != null;
			if (attackerParty.IsMobile && attackerParty.MobileParty.LeaderHero != null)
			{
				this._attackerLord = attackerParty.MobileParty.LeaderHero;
			}
			this._settlement = subject as Settlement;
			if (this._settlement != null)
			{
				this._isVisibleNotification = true;
				if (this._settlement.IsVillage)
				{
					this._battleDetail = (this._settlement.MapFaction.IsAtWarWith(attackerParty.MapFaction) ? 1U : 0U);
					return;
				}
				if (this._settlement.IsFortification)
				{
					this._battleDetail = (this._settlement.MapFaction.IsAtWarWith(attackerParty.MapFaction) ? 3U : 2U);
					return;
				}
			}
			else if (subject is Army)
			{
				this._battleDetail = 4U;
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
			return obj == this._settlement;
		}

		public TextObject GetEncyclopediaText()
		{
			TextObject textObject = TextObject.Empty;
			TextObject textObject2 = ((this._settlement != null) ? this._settlement.EncyclopediaLinkWithName : TextObject.Empty);
			switch (this._battleDetail)
			{
			case 0U:
				textObject = GameTexts.FindText("str_army_defend_from_raiding_news", null);
				textObject.SetTextVariable("PARTY", this._attackerName);
				textObject.SetTextVariable("SETTLEMENT", textObject2);
				break;
			case 1U:
				if (this._attackerFaction == null || this._attackerLord == null)
				{
					textObject = new TextObject("{=xss7eP0f}{PARTY} is raiding {SETTLEMENT}", null);
					textObject.SetTextVariable("PARTY", this._attackerName);
					textObject.SetTextVariable("SETTLEMENT", textObject2);
				}
				else
				{
					if (this._attackerPartyHasArmy)
					{
						textObject = GameTexts.FindText("str_army_raiding_news", null);
					}
					else
					{
						textObject = GameTexts.FindText("str_party_raiding_news", null);
					}
					StringHelpers.SetCharacterProperties("LORD", this._attackerLord.CharacterObject, textObject, false);
					textObject.SetTextVariable("FACTION_NAME", this._attackerFaction.EncyclopediaLinkWithName);
					textObject.SetTextVariable("SETTLEMENT", textObject2);
				}
				break;
			case 2U:
				textObject = GameTexts.FindText("str_defend_from_assault_news", null);
				textObject.SetTextVariable("PARTY", this._attackerName);
				textObject.SetTextVariable("SETTLEMENT", textObject2);
				break;
			case 3U:
				textObject = GameTexts.FindText("str_assault_news", null);
				textObject.SetTextVariable("PARTY", this._attackerName);
				textObject.SetTextVariable("SETTLEMENT", textObject2);
				break;
			case 4U:
				textObject = GameTexts.FindText("str_army_battle_news", null);
				textObject.SetTextVariable("PARTY1", this._attackerName);
				textObject.SetTextVariable("PARTY2", this._defenderName);
				break;
			}
			return textObject;
		}

		[SaveableField(40)]
		private readonly uint _notificationTextColor;

		[SaveableField(41)]
		private readonly Settlement _settlement;

		[SaveableField(42)]
		private readonly TextObject _attackerName;

		[SaveableField(43)]
		private readonly TextObject _defenderName;

		[SaveableField(44)]
		private readonly bool _isVisibleNotification;

		[SaveableField(45)]
		private readonly uint _battleDetail;

		[SaveableField(46)]
		private readonly IFaction _attackerFaction;

		[SaveableField(47)]
		private readonly Hero _attackerLord;

		[SaveableField(48)]
		private readonly bool _attackerPartyHasArmy;

		private enum BattleDetail
		{
			AttackerPartyDefendingVillage,
			AttackerPartyRaidingVillage,
			AttackerPartyDefendingFortification,
			AttackerPartyBesiegingFortification,
			ArmyBattle
		}
	}
}

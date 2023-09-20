﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class DeclareWarLogEntry : LogEntry, IEncyclopediaLog, IChatNotification, IWarLog
	{
		internal static void AutoGeneratedStaticCollectObjectsDeclareWarLogEntry(object o, List<object> collectedObjects)
		{
			((DeclareWarLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Faction1);
			collectedObjects.Add(this.Faction2);
			collectedObjects.Add(this.Faction1Leader);
		}

		internal static object AutoGeneratedGetMemberValueFaction1(object o)
		{
			return ((DeclareWarLogEntry)o).Faction1;
		}

		internal static object AutoGeneratedGetMemberValueFaction2(object o)
		{
			return ((DeclareWarLogEntry)o).Faction2;
		}

		internal static object AutoGeneratedGetMemberValueFaction1Leader(object o)
		{
			return ((DeclareWarLogEntry)o).Faction1Leader;
		}

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
				return base.AdversityNotification(this.Faction1, this.Faction2);
			}
		}

		public DeclareWarLogEntry(IFaction faction1, IFaction faction2)
		{
			this.Faction1 = faction1;
			this.Faction2 = faction2;
			this.Faction1Leader = faction1.Leader;
		}

		public bool IsRelatedToWar(StanceLink stance, out IFaction effector, out IFaction effected)
		{
			IFaction faction = stance.Faction1;
			IFaction faction2 = stance.Faction2;
			effector = faction;
			effected = faction2;
			return (faction == this.Faction1 && faction2 == this.Faction2) || (faction == this.Faction2 && faction2 == this.Faction1);
		}

		public TextObject GetNotificationText()
		{
			Hero hero = this.Faction1Leader ?? this.Faction1.Leader;
			TextObject textObject;
			if (hero != null)
			{
				textObject = GameTexts.FindText("str_factions_declare_war_news", null);
				textObject.SetTextVariable("RULER_NAME", hero.Name);
			}
			else
			{
				textObject = GameTexts.FindText("str_factions_declare_war_news_direct", null);
			}
			textObject.SetTextVariable("FACTION1_NAME", this.Faction1.EncyclopediaLinkWithName);
			textObject.SetTextVariable("FACTION2_NAME", this.Faction2.EncyclopediaLinkWithName);
			return textObject;
		}

		public override void GetConversationScoreAndComment(Hero talkTroop, bool findString, out string comment, out ImportanceEnum score)
		{
			score = ImportanceEnum.Zero;
			comment = "";
			if (!this.Faction1.IsEliminated && this.Faction1.Leader.Clan.IsRebelClan && talkTroop.Clan == this.Faction1)
			{
				score = ImportanceEnum.MatterOfLifeAndDeath;
				if (findString)
				{
					comment = "str_comment_we_have_rebelled";
				}
			}
		}

		public override int GetAsRumor(Settlement talkSettlement, ref TextObject comment)
		{
			int num = 0;
			if (this.Faction1 == talkSettlement.MapFaction)
			{
				comment = new TextObject("{=mrmxEklL}So looks like it's war with {ENEMY_NAME}. Well, I don't deny they deserve it, but it will fall hardest on the poor folk like us.", null);
				comment.SetTextVariable("ENEMY_NAME", FactionHelper.GetTermUsedByOtherFaction(this.Faction2, talkSettlement.MapFaction, false));
				return 10;
			}
			if (this.Faction2 == talkSettlement.MapFaction)
			{
				comment = new TextObject("{=SVebFiHQ}So looks like {ENEMY_NAME} want war with us. Well, I say we show the bastards who we are!", null);
				comment.SetTextVariable("ENEMY_NAME", FactionHelper.GetTermUsedByOtherFaction(this.Faction1, talkSettlement.MapFaction, false));
				return 10;
			}
			return num;
		}

		public override string ToString()
		{
			return this.GetEncyclopediaText().ToString();
		}

		public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
		{
			return obj == this.Faction1 || obj == this.Faction2;
		}

		public TextObject GetEncyclopediaText()
		{
			return this.GetNotificationText();
		}

		[SaveableField(190)]
		public readonly IFaction Faction1;

		[SaveableField(191)]
		public readonly IFaction Faction2;

		[SaveableField(192)]
		public readonly Hero Faction1Leader;
	}
}

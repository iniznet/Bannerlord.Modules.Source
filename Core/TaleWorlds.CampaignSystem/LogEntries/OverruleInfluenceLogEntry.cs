﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class OverruleInfluenceLogEntry : LogEntry
	{
		internal static void AutoGeneratedStaticCollectObjectsOverruleInfluenceLogEntry(object o, List<object> collectedObjects)
		{
			((OverruleInfluenceLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._liege);
		}

		internal static object AutoGeneratedGetMemberValue_liege(object o)
		{
			return ((OverruleInfluenceLogEntry)o)._liege;
		}

		public override CampaignTime KeepInHistoryTime
		{
			get
			{
				return CampaignTime.Weeks(240f);
			}
		}

		public OverruleInfluenceLogEntry(Hero liege, Hero wrongedLord)
		{
			this._liege = liege;
		}

		public override TextObject GetHistoricComment(Hero talkTroop)
		{
			TextObject textObject = GameTexts.FindText("str_overrule_influence_historic_comment", null);
			textObject.SetTextVariable("HERO", this._liege.Name);
			textObject.SetTextVariable("FACTION", this._liege.Clan.Name);
			return textObject;
		}

		public override int GetValueAsPoliticsAbuseOfPower(Hero referenceTroop, Hero liege)
		{
			if (liege == this._liege)
			{
				return 3;
			}
			return 0;
		}

		public override string ToString()
		{
			return "OverruleInfluenceLogEntry";
		}

		[SaveableField(260)]
		private readonly Hero _liege;
	}
}

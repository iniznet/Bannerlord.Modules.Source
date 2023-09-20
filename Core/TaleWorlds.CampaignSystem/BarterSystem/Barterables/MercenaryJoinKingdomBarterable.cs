﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	public class MercenaryJoinKingdomBarterable : Barterable
	{
		public override string StringID
		{
			get
			{
				return "mercenary_join_faction_barterable";
			}
		}

		public MercenaryJoinKingdomBarterable(Hero owner, PartyBase ownerParty, Kingdom targetKingdom)
			: base(owner, ownerParty)
		{
			this._targetKingdom = targetKingdom;
		}

		public override TextObject Name
		{
			get
			{
				TextObject textObject = new TextObject("{=PaG0Blui}Become a mercenary for {TARGET_FACTION}", null);
				textObject.SetTextVariable("TARGET_FACTION", this._targetKingdom.Name);
				return textObject;
			}
		}

		public override int GetUnitValueForFaction(IFaction faction)
		{
			float num = 0f;
			if (this._targetKingdom == faction.MapFaction)
			{
				num += Campaign.Current.Models.DiplomacyModel.GetScoreOfKingdomToHireMercenary(this._targetKingdom, base.OriginalOwner.Clan);
			}
			else if (faction == base.OriginalOwner.Clan)
			{
				if (base.OriginalOwner.Clan.Kingdom != null)
				{
					num += Campaign.Current.Models.DiplomacyModel.GetScoreOfMercenaryToLeaveKingdom(base.OriginalOwner.Clan, base.OriginalOwner.Clan.Kingdom);
				}
				num += Campaign.Current.Models.DiplomacyModel.GetScoreOfMercenaryToJoinKingdom(base.OriginalOwner.Clan, this._targetKingdom);
			}
			return (int)num;
		}

		public override void CheckBarterLink(Barterable linkedBarterable)
		{
		}

		public override ImageIdentifier GetVisualIdentifier()
		{
			return null;
		}

		public override string GetEncyclopediaLink()
		{
			return this._targetKingdom.EncyclopediaLink;
		}

		public override void Apply()
		{
			ChangeKingdomAction.ApplyByJoinFactionAsMercenary(base.OriginalOwner.Clan, this._targetKingdom, Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(base.OriginalOwner.Clan, this._targetKingdom, false), true);
		}

		internal static void AutoGeneratedStaticCollectObjectsMercenaryJoinKingdomBarterable(object o, List<object> collectedObjects)
		{
			((MercenaryJoinKingdomBarterable)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._targetKingdom);
		}

		internal static object AutoGeneratedGetMemberValue_targetKingdom(object o)
		{
			return ((MercenaryJoinKingdomBarterable)o)._targetKingdom;
		}

		[SaveableField(700)]
		private readonly Kingdom _targetKingdom;
	}
}
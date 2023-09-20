﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	public class GoldBarterable : Barterable
	{
		public override string StringID
		{
			get
			{
				return "gold_barterable";
			}
		}

		public override int MaxAmount
		{
			get
			{
				return this._maxGold;
			}
		}

		public override TextObject Name
		{
			get
			{
				return GameTexts.FindText("str_char_denar_tooltip", null);
			}
		}

		public GoldBarterable(Hero owner, Hero other, PartyBase ownerParty, PartyBase otherParty, int val)
			: base(owner, ownerParty)
		{
			this._ownerHero = owner;
			this._otherHero = other;
			this._otherParty = otherParty;
			this._maxGold = val;
			base.CurrentAmount = 0;
		}

		public override int GetUnitValueForFaction(IFaction faction)
		{
			Hero originalOwner = base.OriginalOwner;
			if (faction != ((originalOwner != null) ? originalOwner.Clan : null))
			{
				Hero originalOwner2 = base.OriginalOwner;
				if (faction != ((originalOwner2 != null) ? originalOwner2.MapFaction : null))
				{
					PartyBase originalParty = base.OriginalParty;
					if (faction != ((originalParty != null) ? originalParty.MapFaction : null))
					{
						Hero otherHero = this._otherHero;
						if (faction != ((otherHero != null) ? otherHero.Clan : null))
						{
							Hero otherHero2 = this._otherHero;
							if (faction != ((otherHero2 != null) ? otherHero2.MapFaction : null))
							{
								PartyBase otherParty = this._otherParty;
								if (faction != ((otherParty != null) ? otherParty.MapFaction : null))
								{
									return 0;
								}
							}
						}
						return 1;
					}
				}
			}
			return -1;
		}

		public override ImageIdentifier GetVisualIdentifier()
		{
			return null;
		}

		public override string GetEncyclopediaLink()
		{
			return "";
		}

		public override void Apply()
		{
			if (this._ownerHero != null && this._otherHero != null)
			{
				GiveGoldAction.ApplyBetweenCharacters(this._ownerHero, this._otherHero, base.CurrentAmount, false);
				return;
			}
			if (base.OriginalParty == PartyBase.MainParty)
			{
				GiveGoldAction.ApplyForCharacterToParty(Hero.MainHero, this._otherParty, base.CurrentAmount, false);
				return;
			}
			if (this._otherParty != PartyBase.MainParty)
			{
				GiveGoldAction.ApplyForPartyToParty(base.OriginalParty, this._otherParty, base.CurrentAmount, false);
				return;
			}
			GiveGoldAction.ApplyForPartyToCharacter(base.OriginalParty, Hero.MainHero, base.CurrentAmount, false);
		}

		internal static void AutoGeneratedStaticCollectObjectsGoldBarterable(object o, List<object> collectedObjects)
		{
			((GoldBarterable)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._ownerHero);
			collectedObjects.Add(this._otherHero);
			collectedObjects.Add(this._otherParty);
		}

		internal static object AutoGeneratedGetMemberValue_maxGold(object o)
		{
			return ((GoldBarterable)o)._maxGold;
		}

		internal static object AutoGeneratedGetMemberValue_ownerHero(object o)
		{
			return ((GoldBarterable)o)._ownerHero;
		}

		internal static object AutoGeneratedGetMemberValue_otherHero(object o)
		{
			return ((GoldBarterable)o)._otherHero;
		}

		internal static object AutoGeneratedGetMemberValue_otherParty(object o)
		{
			return ((GoldBarterable)o)._otherParty;
		}

		[SaveableField(200)]
		private readonly int _maxGold;

		[SaveableField(201)]
		private readonly Hero _ownerHero;

		[SaveableField(202)]
		private readonly Hero _otherHero;

		[SaveableField(203)]
		private readonly PartyBase _otherParty;
	}
}

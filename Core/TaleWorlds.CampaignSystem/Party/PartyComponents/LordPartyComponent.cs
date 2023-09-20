﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Party.PartyComponents
{
	public class LordPartyComponent : WarPartyComponent
	{
		internal static void AutoGeneratedStaticCollectObjectsLordPartyComponent(object o, List<object> collectedObjects)
		{
			((LordPartyComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._leader);
			collectedObjects.Add(this.Owner);
		}

		internal static object AutoGeneratedGetMemberValueOwner(object o)
		{
			return ((LordPartyComponent)o).Owner;
		}

		internal static object AutoGeneratedGetMemberValue_leader(object o)
		{
			return ((LordPartyComponent)o)._leader;
		}

		internal static object AutoGeneratedGetMemberValue_wagePaymentLimit(object o)
		{
			return ((LordPartyComponent)o)._wagePaymentLimit;
		}

		public override Hero PartyOwner
		{
			get
			{
				return this.Owner;
			}
		}

		public override TextObject Name
		{
			get
			{
				if (this._cachedName == null && this.Owner != null)
				{
					this._cachedName = this.GetPartyName();
				}
				return this._cachedName ?? new TextObject("{=!}unnamedMobileParty", null);
			}
		}

		public override Settlement HomeSettlement
		{
			get
			{
				return this.Owner.HomeSettlement;
			}
		}

		[SaveableProperty(20)]
		public Hero Owner { get; private set; }

		public override Hero Leader
		{
			get
			{
				return this._leader;
			}
		}

		public override int WagePaymentLimit
		{
			get
			{
				return this._wagePaymentLimit;
			}
		}

		public override void SetWagePaymentLimit(int newLimit)
		{
			this._wagePaymentLimit = newLimit;
		}

		public static MobileParty CreateLordParty(string stringId, Hero hero, Vec2 position, float spawnRadius, Settlement spawnSettlement, Hero partyLeader)
		{
			return MobileParty.CreateParty(hero.CharacterObject.StringId + "_party_1", new LordPartyComponent(hero, partyLeader), delegate(MobileParty mobileParty)
			{
				mobileParty.LordPartyComponent.InitializeLordPartyProperties(mobileParty, position, spawnRadius, spawnSettlement);
			});
		}

		protected internal LordPartyComponent(Hero owner, Hero leader)
		{
			this.Owner = owner;
			this._leader = leader;
		}

		internal void ChangePartyOwner(Hero owner)
		{
			this.ClearCachedName();
			this.Owner = owner;
		}

		public override void ChangePartyLeader(Hero newLeader)
		{
			this._leader = newLeader;
			this.ClearCachedName();
		}

		public override void ClearCachedName()
		{
			this._cachedName = null;
		}

		private TextObject GetPartyName()
		{
			TextObject textObject = GameTexts.FindText("str_lord_party_name", null);
			textObject.SetCharacterProperties("TROOP", this.Owner.CharacterObject, false);
			textObject.SetTextVariable("IS_LORDPARTY", 1);
			return textObject;
		}

		private void InitializeLordPartyProperties(MobileParty mobileParty, Vec2 position, float spawnRadius, Settlement spawnSettlement)
		{
			mobileParty.AddElementToMemberRoster(this.Owner.CharacterObject, 1, true);
			mobileParty.ActualClan = this.Owner.Clan;
			int num = ((this.Owner != Hero.MainHero && this.Owner.Clan != Clan.PlayerClan) ? ((int)MathF.Min(this.Owner.Clan.IsRebelClan ? 40f : 19f, (this.Owner.Clan.IsRebelClan ? 0.2f : 0.1f) * (float)mobileParty.Party.PartySizeLimit)) : 0);
			if (!Campaign.Current.GameStarted)
			{
				float randomFloat = MBRandom.RandomFloat;
				float num2 = MathF.Sqrt(MBRandom.RandomFloat);
				float num3 = 1f - randomFloat * num2;
				num = (int)((float)mobileParty.Party.PartySizeLimit * num3);
			}
			mobileParty.InitializeMobilePartyAroundPosition(this.Owner.Clan.DefaultPartyTemplate, position, spawnRadius, 0f, num);
			mobileParty.Party.Visuals.SetMapIconAsDirty();
			if (spawnSettlement != null)
			{
				mobileParty.Ai.SetMoveGoToSettlement(spawnSettlement);
			}
			mobileParty.Aggressiveness = 0.9f + 0.1f * (float)this.Owner.GetTraitLevel(DefaultTraits.Valor) - 0.05f * (float)this.Owner.GetTraitLevel(DefaultTraits.Mercy);
			mobileParty.ItemRoster.Add(new ItemRosterElement(DefaultItems.Grain, MBRandom.RandomInt(15, 30), null));
			this.Owner.PassedTimeAtHomeSettlement = (float)((int)(MBRandom.RandomFloat * 100f));
			if (spawnSettlement != null)
			{
				mobileParty.Ai.SetMoveGoToSettlement(spawnSettlement);
			}
		}

		[CachedData]
		private TextObject _cachedName;

		[SaveableField(30)]
		private Hero _leader;

		[SaveableField(40)]
		private int _wagePaymentLimit = Campaign.Current.Models.PartyWageModel.MaxWage;
	}
}

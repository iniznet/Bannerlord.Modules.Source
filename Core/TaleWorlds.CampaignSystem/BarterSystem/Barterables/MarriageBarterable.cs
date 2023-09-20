using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	public class MarriageBarterable : Barterable
	{
		public override string StringID
		{
			get
			{
				return "marriage_barterable";
			}
		}

		public MarriageBarterable(Hero owner, PartyBase ownerParty, Hero heroBeingProposedTo, Hero proposingHero)
			: base(owner, ownerParty)
		{
			this.HeroBeingProposedTo = heroBeingProposedTo;
			this.ProposingHero = proposingHero;
		}

		public override TextObject Name
		{
			get
			{
				StringHelpers.SetCharacterProperties("HERO_BEING_PROPOSED_TO", this.HeroBeingProposedTo.CharacterObject, null, false);
				StringHelpers.SetCharacterProperties("HERO_TO_MARRY", this.ProposingHero.CharacterObject, null, false);
				return new TextObject("{=rv6hk8X2}{HERO_BEING_PROPOSED_TO.NAME} to marry {HERO_TO_MARRY.NAME}", null);
			}
		}

		public override int GetUnitValueForFaction(IFaction faction)
		{
			if (faction == this.ProposingHero.Clan)
			{
				float num = -50000f;
				float num2 = (float)this.ProposingHero.RandomInt(10000);
				float num3 = (float)(this.ProposingHero.RandomInt(-25000, 25000) + this.HeroBeingProposedTo.RandomInt(-25000, 25000));
				if (this.ProposingHero == Hero.MainHero)
				{
					num3 = 0f;
					num2 = 0f;
				}
				float num4 = (float)(this.ProposingHero.GetRelation(this.HeroBeingProposedTo) * 1000);
				Campaign.Current.Models.DiplomacyModel.GetHeroCommandingStrengthForClan(this.ProposingHero);
				Campaign.Current.Models.DiplomacyModel.GetHeroCommandingStrengthForClan(this.HeroBeingProposedTo);
				float num5 = ((this.ProposingHero.Clan == null) ? 0f : ((float)this.ProposingHero.Clan.Tier + ((this.ProposingHero.Clan.Leader == this.ProposingHero.MapFaction.Leader) ? (MathF.Min(3f, (float)this.ProposingHero.MapFaction.Fiefs.Count / 10f) + 0.5f) : 0f)));
				float num6 = ((this.HeroBeingProposedTo.Clan == null) ? 0f : ((float)this.HeroBeingProposedTo.Clan.Tier + ((this.HeroBeingProposedTo.Clan.Leader == this.HeroBeingProposedTo.MapFaction.Leader) ? (MathF.Min(3f, (float)this.HeroBeingProposedTo.MapFaction.Fiefs.Count / 10f) + 0.5f) : 0f)));
				float num7 = ((faction == this.ProposingHero.Clan) ? ((num6 - num5) * MathF.Abs(num6 - num5) * 1000f) : ((num5 - num6) * MathF.Abs(num5 - num6) * 1000f));
				int relationBetweenClans = FactionManager.GetRelationBetweenClans(this.HeroBeingProposedTo.Clan, this.ProposingHero.Clan);
				int num8 = 1000 * relationBetweenClans;
				Clan clanAfterMarriage = Campaign.Current.Models.MarriageModel.GetClanAfterMarriage(this.HeroBeingProposedTo, this.ProposingHero);
				float num9 = 0f;
				float num10 = 0f;
				if (clanAfterMarriage != this.HeroBeingProposedTo.Clan)
				{
					if (faction == clanAfterMarriage)
					{
						num9 = Campaign.Current.Models.DiplomacyModel.GetValueOfHeroForFaction(this.HeroBeingProposedTo, clanAfterMarriage, true);
					}
					else if (faction == this.HeroBeingProposedTo.Clan)
					{
						num9 = Campaign.Current.Models.DiplomacyModel.GetValueOfHeroForFaction(this.HeroBeingProposedTo, this.HeroBeingProposedTo.Clan, true);
					}
					if (clanAfterMarriage.Kingdom != null && clanAfterMarriage.Kingdom != this.HeroBeingProposedTo.Clan.Kingdom)
					{
						num10 = Campaign.Current.Models.DiplomacyModel.GetValueOfHeroForFaction(this.HeroBeingProposedTo, clanAfterMarriage.Kingdom, true);
					}
				}
				float num11 = 2f * MathF.Min(0f, 20f - MathF.Max(this.HeroBeingProposedTo.Age - 18f, 0f)) * MathF.Min(0f, 20f - MathF.Max(this.HeroBeingProposedTo.Age - 18f, 0f)) * MathF.Min(0f, 20f - MathF.Max(this.HeroBeingProposedTo.Age - 18f, 0f));
				return (int)(num + num2 + num3 + num4 + num9 + (float)num8 + num10 + num7 + num11);
			}
			float num12 = -this.HeroBeingProposedTo.Clan.Renown;
			float num13 = -(2f * MathF.Min(0f, 20f - MathF.Max(this.HeroBeingProposedTo.Age - 18f, 0f)) * MathF.Min(0f, 20f - MathF.Max(this.HeroBeingProposedTo.Age - 18f, 0f)) * MathF.Min(0f, 20f - MathF.Max(this.HeroBeingProposedTo.Age - 18f, 0f)));
			return (int)(num12 + num13);
		}

		public override void CheckBarterLink(Barterable linkedBarterable)
		{
			if (linkedBarterable.GetType() == typeof(MarriageBarterable) && linkedBarterable.OriginalOwner == base.OriginalOwner && ((MarriageBarterable)linkedBarterable).HeroBeingProposedTo == this.HeroBeingProposedTo && ((MarriageBarterable)linkedBarterable).ProposingHero == this.ProposingHero)
			{
				base.AddBarterLink(linkedBarterable);
			}
		}

		public override bool IsCompatible(Barterable barterable)
		{
			MarriageBarterable marriageBarterable = barterable as MarriageBarterable;
			return marriageBarterable == null || (marriageBarterable.HeroBeingProposedTo != this.HeroBeingProposedTo && marriageBarterable.HeroBeingProposedTo != this.ProposingHero && marriageBarterable.ProposingHero != this.HeroBeingProposedTo && marriageBarterable.ProposingHero != this.ProposingHero);
		}

		public override ImageIdentifier GetVisualIdentifier()
		{
			return new ImageIdentifier(CharacterCode.CreateFrom(this.HeroBeingProposedTo.CharacterObject));
		}

		public override string GetEncyclopediaLink()
		{
			return this.HeroBeingProposedTo.EncyclopediaLink;
		}

		public override void Apply()
		{
			MarriageAction.Apply(this.HeroBeingProposedTo, this.ProposingHero, this.HeroBeingProposedTo.Clan == Clan.PlayerClan || this.ProposingHero.Clan == Clan.PlayerClan);
		}

		internal static void AutoGeneratedStaticCollectObjectsMarriageBarterable(object o, List<object> collectedObjects)
		{
			((MarriageBarterable)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.ProposingHero);
			collectedObjects.Add(this.HeroBeingProposedTo);
		}

		internal static object AutoGeneratedGetMemberValueProposingHero(object o)
		{
			return ((MarriageBarterable)o).ProposingHero;
		}

		internal static object AutoGeneratedGetMemberValueHeroBeingProposedTo(object o)
		{
			return ((MarriageBarterable)o).HeroBeingProposedTo;
		}

		[SaveableField(600)]
		public readonly Hero ProposingHero;

		[SaveableField(601)]
		public readonly Hero HeroBeingProposedTo;
	}
}

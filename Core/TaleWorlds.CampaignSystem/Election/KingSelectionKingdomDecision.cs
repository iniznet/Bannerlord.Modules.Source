﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
	public class KingSelectionKingdomDecision : KingdomDecision
	{
		internal static void AutoGeneratedStaticCollectObjectsKingSelectionKingdomDecision(object o, List<object> collectedObjects)
		{
			((KingSelectionKingdomDecision)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._clanToExclude);
		}

		internal static object AutoGeneratedGetMemberValue_clanToExclude(object o)
		{
			return ((KingSelectionKingdomDecision)o)._clanToExclude;
		}

		public KingSelectionKingdomDecision(Clan proposerClan, Clan clanToExclude = null)
			: base(proposerClan)
		{
			this._clanToExclude = clanToExclude;
		}

		public override bool IsAllowed()
		{
			return Campaign.Current.Models.KingdomDecisionPermissionModel.IsKingSelectionDecisionAllowed(base.Kingdom);
		}

		public override int GetProposalInfluenceCost()
		{
			return Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingWar(base.ProposerClan);
		}

		public override TextObject GetGeneralTitle()
		{
			TextObject textObject = new TextObject("{=ZYSGp5vO}King of {KINGDOM_NAME}", null);
			textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
			return textObject;
		}

		public override TextObject GetSupportTitle()
		{
			TextObject textObject = new TextObject("{=B0uKPW9S}Vote for the next ruler of {KINGDOM_NAME}", null);
			textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
			return textObject;
		}

		public override TextObject GetChooseTitle()
		{
			TextObject textObject = new TextObject("{=L0Oxzkfw}Choose the next ruler of {KINGDOM_NAME}", null);
			textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
			return textObject;
		}

		public override TextObject GetSupportDescription()
		{
			TextObject textObject = new TextObject("{=XGuDyJMZ}{KINGDOM_NAME} will decide who will bear the crown as the next ruler. You can pick your stance regarding this decision.", null);
			textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
			return textObject;
		}

		public override TextObject GetChooseDescription()
		{
			TextObject textObject = new TextObject("{=L0Oxzkfw}Choose the next ruler of {KINGDOM_NAME}", null);
			textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
			return textObject;
		}

		public override bool IsKingsVoteAllowed
		{
			get
			{
				return false;
			}
		}

		protected override bool CanProposerClanChangeOpinion()
		{
			return true;
		}

		public override float CalculateMeritOfOutcome(DecisionOutcome candidateOutcome)
		{
			float num = 1f;
			foreach (Clan clan in base.Kingdom.Clans)
			{
				if (clan.Leader != Hero.MainHero)
				{
					num += this.CalculateMeritOfOutcomeForClan(clan, candidateOutcome);
				}
			}
			return num;
		}

		public float CalculateMeritOfOutcomeForClan(Clan clan, DecisionOutcome candidateOutcome)
		{
			float num = 0f;
			Hero king = ((KingSelectionKingdomDecision.KingSelectionDecisionOutcome)candidateOutcome).King;
			if (king.Clan == base.Kingdom.RulingClan)
			{
				if (clan.Leader.GetTraitLevel(DefaultTraits.Authoritarian) > 0)
				{
					num += 3f;
				}
				else if (clan.Leader.GetTraitLevel(DefaultTraits.Oligarchic) > 0)
				{
					num += 2f;
				}
				else
				{
					num += 1f;
				}
			}
			List<float> list = (from t in base.Kingdom.Clans
				select Campaign.Current.Models.DiplomacyModel.GetClanStrength(t) into t
				orderby t descending
				select t).ToList<float>();
			int num2 = 6;
			float num3 = (float)num2 / (list[0] - list[list.Count - 1]);
			float num4 = (float)num2 / 2f - num3 * list[0];
			float num5 = Campaign.Current.Models.DiplomacyModel.GetClanStrength(king.Clan) * num3 + num4;
			num += num5;
			return MathF.Clamp(num, -3f, 8f);
		}

		public override IEnumerable<DecisionOutcome> DetermineInitialCandidates()
		{
			Dictionary<Clan, float> dictionary = new Dictionary<Clan, float>();
			foreach (Clan clan in base.Kingdom.Clans)
			{
				if (Campaign.Current.Models.DiplomacyModel.IsClanEligibleToBecomeRuler(clan) && clan != this._clanToExclude)
				{
					dictionary.Add(clan, Campaign.Current.Models.DiplomacyModel.GetClanStrength(clan));
				}
			}
			IEnumerable<KeyValuePair<Clan, float>> enumerable = dictionary.OrderByDescending((KeyValuePair<Clan, float> t) => t.Value).Take(3);
			foreach (KeyValuePair<Clan, float> keyValuePair in enumerable)
			{
				yield return new KingSelectionKingdomDecision.KingSelectionDecisionOutcome(keyValuePair.Key.Leader);
			}
			IEnumerator<KeyValuePair<Clan, float>> enumerator2 = null;
			yield break;
			yield break;
		}

		public override Clan DetermineChooser()
		{
			return base.Kingdom.RulingClan;
		}

		public override float DetermineSupport(Clan clan, DecisionOutcome possibleOutcome)
		{
			return this.CalculateMeritOfOutcomeForClan(clan, possibleOutcome) * 10f;
		}

		public override void DetermineSponsors(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			foreach (DecisionOutcome decisionOutcome in possibleOutcomes)
			{
				decisionOutcome.SetSponsor(((KingSelectionKingdomDecision.KingSelectionDecisionOutcome)decisionOutcome).King.Clan);
			}
		}

		public override void ApplyChosenOutcome(DecisionOutcome chosenOutcome)
		{
			Hero king = ((KingSelectionKingdomDecision.KingSelectionDecisionOutcome)chosenOutcome).King;
			if (king != king.Clan.Leader)
			{
				ChangeClanLeaderAction.ApplyWithSelectedNewLeader(king.Clan, king);
			}
			ChangeRulingClanAction.Apply(base.Kingdom, king.Clan);
		}

		public override TextObject GetSecondaryEffects()
		{
			return new TextObject("{=!}All supporters gains some relation with each other.", null);
		}

		public override void ApplySecondaryEffects(MBReadOnlyList<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome)
		{
		}

		public override TextObject GetChosenOutcomeText(DecisionOutcome chosenOutcome, KingdomDecision.SupportStatus supportStatus, bool isShortVersion = false)
		{
			TextObject textObject = new TextObject("{=JQligd8z}The council of the {KINGDOM} has chosen {KING.NAME} as the new ruler.", null);
			textObject.SetTextVariable("KINGDOM", base.Kingdom.Name);
			StringHelpers.SetCharacterProperties("KING", ((KingSelectionKingdomDecision.KingSelectionDecisionOutcome)chosenOutcome).King.CharacterObject, textObject, false);
			return textObject;
		}

		public override DecisionOutcome GetQueriedDecisionOutcome(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			return possibleOutcomes.OrderByDescending((DecisionOutcome k) => k.Merit).FirstOrDefault<DecisionOutcome>();
		}

		[SaveableField(1)]
		private Clan _clanToExclude;

		public class KingSelectionDecisionOutcome : DecisionOutcome
		{
			internal static void AutoGeneratedStaticCollectObjectsKingSelectionDecisionOutcome(object o, List<object> collectedObjects)
			{
				((KingSelectionKingdomDecision.KingSelectionDecisionOutcome)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this.King);
			}

			internal static object AutoGeneratedGetMemberValueKing(object o)
			{
				return ((KingSelectionKingdomDecision.KingSelectionDecisionOutcome)o).King;
			}

			public KingSelectionDecisionOutcome(Hero king)
			{
				this.King = king;
			}

			public override TextObject GetDecisionTitle()
			{
				TextObject textObject = new TextObject("{=4G3Aeqna}{KING.NAME}", null);
				StringHelpers.SetCharacterProperties("KING", this.King.CharacterObject, textObject, false);
				return textObject;
			}

			public override TextObject GetDecisionDescription()
			{
				TextObject textObject = new TextObject("{=FTjKWm8s}{KING.NAME} should rule us", null);
				StringHelpers.SetCharacterProperties("KING", this.King.CharacterObject, textObject, false);
				return textObject;
			}

			public override string GetDecisionLink()
			{
				return null;
			}

			public override ImageIdentifier GetDecisionImageIdentifier()
			{
				return null;
			}

			[SaveableField(100)]
			public readonly Hero King;
		}
	}
}

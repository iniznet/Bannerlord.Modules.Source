using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultTournamentModel : TournamentModel
	{
		public override TournamentGame CreateTournament(Town town)
		{
			return new FightTournamentGame(town);
		}

		public override float GetTournamentStartChance(Town town)
		{
			if (town.Settlement.SiegeEvent != null)
			{
				return 0f;
			}
			if (Math.Abs(town.StringId.GetHashCode() % 3) != CampaignTime.Now.GetWeekOfSeason)
			{
				return 0f;
			}
			return 0.1f * (float)(town.Settlement.Parties.Count((MobileParty x) => x.IsLordParty) + town.Settlement.HeroesWithoutParty.Count((Hero x) => this.SuitableForTournament(x))) - 0.2f;
		}

		public override int GetNumLeaderboardVictoriesAtGameStart()
		{
			return 500;
		}

		public override float GetTournamentEndChance(TournamentGame tournament)
		{
			float elapsedDaysUntilNow = tournament.CreationTime.ElapsedDaysUntilNow;
			return MathF.Max(0f, (elapsedDaysUntilNow - 10f) * 0.05f);
		}

		private bool SuitableForTournament(Hero hero)
		{
			return hero.Age >= 18f && MathF.Max(hero.GetSkillValue(DefaultSkills.OneHanded), hero.GetSkillValue(DefaultSkills.TwoHanded)) > 100;
		}

		public override float GetTournamentSimulationScore(CharacterObject character)
		{
			return (character.IsHero ? 1f : 0.4f) * (MathF.Max((float)character.GetSkillValue(DefaultSkills.OneHanded), (float)character.GetSkillValue(DefaultSkills.TwoHanded), (float)character.GetSkillValue(DefaultSkills.Polearm)) + (float)character.GetSkillValue(DefaultSkills.Athletics) + (float)character.GetSkillValue(DefaultSkills.Riding)) * 0.01f;
		}

		public override int GetRenownReward(Hero winner, Town town)
		{
			float num = 3f;
			if (winner.GetPerkValue(DefaultPerks.OneHanded.Duelist))
			{
				num *= DefaultPerks.OneHanded.Duelist.SecondaryBonus;
			}
			if (winner.GetPerkValue(DefaultPerks.Charm.SelfPromoter))
			{
				num += DefaultPerks.Charm.SelfPromoter.PrimaryBonus;
			}
			return MathF.Round(num);
		}

		public override int GetInfluenceReward(Hero winner, Town town)
		{
			return 0;
		}

		[return: TupleElementNames(new string[] { "skill", "xp" })]
		public override ValueTuple<SkillObject, int> GetSkillXpGainFromTournament(Town town)
		{
			float randomFloat = MBRandom.RandomFloat;
			SkillObject skillObject = ((randomFloat < 0.2f) ? DefaultSkills.OneHanded : ((randomFloat < 0.4f) ? DefaultSkills.TwoHanded : ((randomFloat < 0.6f) ? DefaultSkills.Polearm : ((randomFloat < 0.8f) ? DefaultSkills.Riding : DefaultSkills.Athletics))));
			int num = 500;
			return new ValueTuple<SkillObject, int>(skillObject, num);
		}

		public override Equipment GetParticipantArmor(CharacterObject participant)
		{
			if (CampaignMission.Current != null && CampaignMission.Current.Mode != MissionMode.Tournament && Settlement.CurrentSettlement != null)
			{
				return (Game.Current.ObjectManager.GetObject<CharacterObject>("gear_practice_dummy_" + Settlement.CurrentSettlement.MapFaction.Culture.StringId) ?? Game.Current.ObjectManager.GetObject<CharacterObject>("gear_practice_dummy_empire")).RandomBattleEquipment;
			}
			return participant.RandomBattleEquipment;
		}
	}
}

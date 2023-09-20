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
	// Token: 0x02000145 RID: 325
	public class DefaultTournamentModel : TournamentModel
	{
		// Token: 0x060017E7 RID: 6119 RVA: 0x00078613 File Offset: 0x00076813
		public override TournamentGame CreateTournament(Town town)
		{
			return new FightTournamentGame(town);
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x0007861C File Offset: 0x0007681C
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

		// Token: 0x060017E9 RID: 6121 RVA: 0x000786BC File Offset: 0x000768BC
		public override int GetNumLeaderboardVictoriesAtGameStart()
		{
			return 500;
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x000786C4 File Offset: 0x000768C4
		public override float GetTournamentEndChance(TournamentGame tournament)
		{
			float elapsedDaysUntilNow = tournament.CreationTime.ElapsedDaysUntilNow;
			return MathF.Max(0f, (elapsedDaysUntilNow - 10f) * 0.05f);
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x000786F7 File Offset: 0x000768F7
		private bool SuitableForTournament(Hero hero)
		{
			return hero.Age >= 18f && MathF.Max(hero.GetSkillValue(DefaultSkills.OneHanded), hero.GetSkillValue(DefaultSkills.TwoHanded)) > 100;
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x00078728 File Offset: 0x00076928
		public override float GetTournamentSimulationScore(CharacterObject character)
		{
			return (character.IsHero ? 1f : 0.4f) * (MathF.Max((float)character.GetSkillValue(DefaultSkills.OneHanded), (float)character.GetSkillValue(DefaultSkills.TwoHanded), (float)character.GetSkillValue(DefaultSkills.Polearm)) + (float)character.GetSkillValue(DefaultSkills.Athletics) + (float)character.GetSkillValue(DefaultSkills.Riding)) * 0.01f;
		}

		// Token: 0x060017ED RID: 6125 RVA: 0x00078794 File Offset: 0x00076994
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

		// Token: 0x060017EE RID: 6126 RVA: 0x000787E1 File Offset: 0x000769E1
		public override int GetInfluenceReward(Hero winner, Town town)
		{
			return 0;
		}

		// Token: 0x060017EF RID: 6127 RVA: 0x000787E4 File Offset: 0x000769E4
		[return: TupleElementNames(new string[] { "skill", "xp" })]
		public override ValueTuple<SkillObject, int> GetSkillXpGainFromTournament(Town town)
		{
			float randomFloat = MBRandom.RandomFloat;
			SkillObject skillObject = ((randomFloat < 0.2f) ? DefaultSkills.OneHanded : ((randomFloat < 0.4f) ? DefaultSkills.TwoHanded : ((randomFloat < 0.6f) ? DefaultSkills.Polearm : ((randomFloat < 0.8f) ? DefaultSkills.Riding : DefaultSkills.Athletics))));
			int num = 500;
			return new ValueTuple<SkillObject, int>(skillObject, num);
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x00078844 File Offset: 0x00076A44
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

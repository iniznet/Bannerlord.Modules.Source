using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200038F RID: 911
	public class EmissarySystemCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060035AD RID: 13741 RVA: 0x000EDBCD File Offset: 0x000EBDCD
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
		}

		// Token: 0x060035AE RID: 13742 RVA: 0x000EDBE6 File Offset: 0x000EBDE6
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060035AF RID: 13743 RVA: 0x000EDBE8 File Offset: 0x000EBDE8
		private void DailyTick()
		{
			EmissaryModel emissaryModel = Campaign.Current.Models.EmissaryModel;
			foreach (Hero hero in Clan.PlayerClan.Heroes)
			{
				if (emissaryModel.IsEmissary(hero))
				{
					float num = (0.1f + 0.75f * ((float)hero.GetSkillValue(DefaultSkills.Charm) / 300f)) / 10f;
					if (MBRandom.RandomFloat <= num)
					{
						bool flag = MBRandom.RandomFloat <= 0.5f;
						if (!flag)
						{
							goto IL_AF;
						}
						if (!hero.CurrentSettlement.HeroesWithoutParty.Any((Hero h) => h.Occupation == Occupation.Lord))
						{
							goto IL_AF;
						}
						bool flag2 = true;
						IL_FA:
						if (!flag2)
						{
							Hero randomElement = hero.CurrentSettlement.Notables.GetRandomElement<Hero>();
							if (randomElement != null)
							{
								ChangeRelationAction.ApplyEmissaryRelation(hero, randomElement, emissaryModel.EmissaryRelationBonusForMainClan, true);
								continue;
							}
							continue;
						}
						else
						{
							Hero randomElementWithPredicate = hero.CurrentSettlement.HeroesWithoutParty.GetRandomElementWithPredicate((Hero n) => !n.IsPrisoner && n.CharacterObject.Occupation == Occupation.Lord && n.Clan != Clan.PlayerClan);
							if (randomElementWithPredicate != null)
							{
								ChangeRelationAction.ApplyEmissaryRelation(hero, randomElementWithPredicate, emissaryModel.EmissaryRelationBonusForMainClan, true);
								continue;
							}
							continue;
						}
						IL_AF:
						if (!flag && hero.CurrentSettlement.Notables.Count == 0)
						{
							flag2 = hero.CurrentSettlement.HeroesWithoutParty.Any((Hero h) => h.Occupation == Occupation.Lord);
							goto IL_FA;
						}
						flag2 = false;
						goto IL_FA;
					}
				}
			}
		}
	}
}

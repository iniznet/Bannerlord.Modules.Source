using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200010A RID: 266
	public class DefaultEquipmentSelectionModel : EquipmentSelectionModel
	{
		// Token: 0x06001594 RID: 5524 RVA: 0x000660B8 File Offset: 0x000642B8
		public override MBList<MBEquipmentRoster> GetEquipmentRostersForHeroComeOfAge(Hero hero, bool isCivilian)
		{
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			bool flag = this.IsHeroCombatant(hero);
			foreach (MBEquipmentRoster mbequipmentRoster in MBEquipmentRosterExtensions.All)
			{
				if (this.IsRosterAppropriateForHeroAsTemplate(mbequipmentRoster, hero, EquipmentFlags.IsNobleTemplate, false))
				{
					if (flag)
					{
						if (isCivilian)
						{
							if (mbequipmentRoster.HasEquipmentFlags(EquipmentFlags.IsCombatantTemplate | EquipmentFlags.IsCivilianTemplate))
							{
								mblist.Add(mbequipmentRoster);
							}
						}
						else if (mbequipmentRoster.HasEquipmentFlags(EquipmentFlags.IsMediumTemplate))
						{
							mblist.Add(mbequipmentRoster);
						}
					}
					else if (isCivilian)
					{
						if (mbequipmentRoster.HasEquipmentFlags(EquipmentFlags.IsNoncombatantTemplate))
						{
							mblist.Add(mbequipmentRoster);
						}
					}
					else if (mbequipmentRoster.HasEquipmentFlags(EquipmentFlags.IsMediumTemplate))
					{
						mblist.Add(mbequipmentRoster);
					}
				}
			}
			return mblist;
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x00066178 File Offset: 0x00064378
		public override MBList<MBEquipmentRoster> GetEquipmentRostersForHeroReachesTeenAge(Hero hero)
		{
			EquipmentFlags equipmentFlags = EquipmentFlags.IsNobleTemplate | EquipmentFlags.IsTeenagerEquipmentTemplate;
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			this.AddEquipmentsToRoster(hero, equipmentFlags, ref mblist, true);
			return mblist;
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x000661A0 File Offset: 0x000643A0
		public override MBList<MBEquipmentRoster> GetEquipmentRostersForInitialChildrenGeneration(Hero hero)
		{
			bool flag = hero.Age < (float)Campaign.Current.Models.AgeModel.BecomeTeenagerAge;
			EquipmentFlags equipmentFlags = EquipmentFlags.IsNobleTemplate | (flag ? EquipmentFlags.IsChildEquipmentTemplate : EquipmentFlags.IsTeenagerEquipmentTemplate);
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			this.AddEquipmentsToRoster(hero, equipmentFlags, ref mblist, true);
			return mblist;
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x000661F0 File Offset: 0x000643F0
		public override MBList<MBEquipmentRoster> GetEquipmentRostersForDeliveredOffspring(Hero hero)
		{
			EquipmentFlags equipmentFlags = EquipmentFlags.IsNobleTemplate | EquipmentFlags.IsChildEquipmentTemplate;
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			this.AddEquipmentsToRoster(hero, equipmentFlags, ref mblist, true);
			return mblist;
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x00066218 File Offset: 0x00064418
		public override MBList<MBEquipmentRoster> GetEquipmentRostersForCompanion(Hero hero, bool isCivilian)
		{
			EquipmentFlags equipmentFlags = (isCivilian ? (EquipmentFlags.IsCivilianTemplate | EquipmentFlags.IsNobleTemplate) : (EquipmentFlags.IsNobleTemplate | EquipmentFlags.IsMediumTemplate));
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			this.AddEquipmentsToRoster(hero, equipmentFlags, ref mblist, isCivilian);
			return mblist;
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x00066244 File Offset: 0x00064444
		private bool IsRosterAppropriateForHeroAsTemplate(MBEquipmentRoster equipmentRoster, Hero hero, EquipmentFlags customFlags = EquipmentFlags.None, bool shouldMatchGender = false)
		{
			bool flag = false;
			if (equipmentRoster.IsEquipmentTemplate() && (!shouldMatchGender || equipmentRoster.HasEquipmentFlags(EquipmentFlags.IsFemaleTemplate) == hero.IsFemale) && equipmentRoster.EquipmentCulture == hero.Culture && (customFlags == EquipmentFlags.None || equipmentRoster.HasEquipmentFlags(customFlags)))
			{
				bool flag2 = equipmentRoster.HasEquipmentFlags(EquipmentFlags.IsNomadTemplate) || equipmentRoster.HasEquipmentFlags(EquipmentFlags.IsWoodlandTemplate);
				bool flag3 = !hero.IsChild && (equipmentRoster.HasEquipmentFlags(EquipmentFlags.IsChildEquipmentTemplate) || equipmentRoster.HasEquipmentFlags(EquipmentFlags.IsTeenagerEquipmentTemplate));
				if (!flag2 && !flag3)
				{
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x000662D8 File Offset: 0x000644D8
		private bool IsHeroCombatant(Hero hero)
		{
			return !hero.IsFemale || hero.Clan == Hero.MainHero.Clan || (hero.Mother != null && !hero.Mother.IsNoncombatant) || (hero.RandomIntWithSeed(17U, 0, 1) == 0 && hero.GetTraitLevel(DefaultTraits.Valor) == 1);
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x00066334 File Offset: 0x00064534
		private void AddEquipmentsToRoster(Hero hero, EquipmentFlags suitableFlags, ref MBList<MBEquipmentRoster> roster, bool shouldMatchGender = false)
		{
			foreach (MBEquipmentRoster mbequipmentRoster in MBEquipmentRosterExtensions.All)
			{
				if (this.IsRosterAppropriateForHeroAsTemplate(mbequipmentRoster, hero, suitableFlags, shouldMatchGender))
				{
					roster.Add(mbequipmentRoster);
				}
			}
		}
	}
}

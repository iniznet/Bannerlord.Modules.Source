using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultEquipmentSelectionModel : EquipmentSelectionModel
	{
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

		public override MBList<MBEquipmentRoster> GetEquipmentRostersForHeroReachesTeenAge(Hero hero)
		{
			EquipmentFlags equipmentFlags = EquipmentFlags.IsNobleTemplate | EquipmentFlags.IsTeenagerEquipmentTemplate;
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			this.AddEquipmentsToRoster(hero, equipmentFlags, ref mblist, true);
			return mblist;
		}

		public override MBList<MBEquipmentRoster> GetEquipmentRostersForInitialChildrenGeneration(Hero hero)
		{
			bool flag = hero.Age < (float)Campaign.Current.Models.AgeModel.BecomeTeenagerAge;
			EquipmentFlags equipmentFlags = EquipmentFlags.IsNobleTemplate | (flag ? EquipmentFlags.IsChildEquipmentTemplate : EquipmentFlags.IsTeenagerEquipmentTemplate);
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			this.AddEquipmentsToRoster(hero, equipmentFlags, ref mblist, true);
			return mblist;
		}

		public override MBList<MBEquipmentRoster> GetEquipmentRostersForDeliveredOffspring(Hero hero)
		{
			EquipmentFlags equipmentFlags = EquipmentFlags.IsNobleTemplate | EquipmentFlags.IsChildEquipmentTemplate;
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			this.AddEquipmentsToRoster(hero, equipmentFlags, ref mblist, true);
			return mblist;
		}

		public override MBList<MBEquipmentRoster> GetEquipmentRostersForCompanion(Hero hero, bool isCivilian)
		{
			EquipmentFlags equipmentFlags = (isCivilian ? (EquipmentFlags.IsCivilianTemplate | EquipmentFlags.IsNobleTemplate) : (EquipmentFlags.IsNobleTemplate | EquipmentFlags.IsMediumTemplate));
			MBList<MBEquipmentRoster> mblist = new MBList<MBEquipmentRoster>();
			this.AddEquipmentsToRoster(hero, equipmentFlags, ref mblist, isCivilian);
			return mblist;
		}

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

		private bool IsHeroCombatant(Hero hero)
		{
			return !hero.IsFemale || hero.Clan == Hero.MainHero.Clan || (hero.Mother != null && !hero.Mother.IsNoncombatant) || (hero.RandomIntWithSeed(17U, 0, 1) == 0 && hero.GetTraitLevel(DefaultTraits.Valor) == 1);
		}

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

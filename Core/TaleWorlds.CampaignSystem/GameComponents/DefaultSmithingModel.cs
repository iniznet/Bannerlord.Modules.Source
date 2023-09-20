using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultSmithingModel : SmithingModel
	{
		public override Crafting.OverrideData GetModifierChanges(int modifierTier, Hero hero, WeaponComponentData weapon)
		{
			int pointsToModify = this.GetPointsToModify(modifierTier);
			Crafting.OverrideData overrideData = ((pointsToModify != 0) ? this.ModifyWeaponDesign(pointsToModify) : new Crafting.OverrideData(0f, 0, 0, 0, 0));
			if (hero.GetPerkValue(DefaultPerks.Crafting.SharpenedEdge))
			{
				overrideData.SwingDamageOverriden += MathF.Round((float)weapon.SwingDamage * DefaultPerks.Crafting.SharpenedEdge.PrimaryBonus);
			}
			if (hero.GetPerkValue(DefaultPerks.Crafting.SharpenedTip))
			{
				overrideData.ThrustDamageOverriden += MathF.Round((float)weapon.ThrustDamage * DefaultPerks.Crafting.SharpenedTip.PrimaryBonus);
			}
			return overrideData;
		}

		private Crafting.OverrideData ModifyWeaponDesign(int numPoints)
		{
			Crafting.OverrideData overrideData = new Crafting.OverrideData(0f, 0, 0, 0, 0);
			int num = 0;
			int num2 = 0;
			while (num2 != numPoints && num < 500)
			{
				int num3 = ((numPoints > 0) ? 1 : (-1));
				if (MBRandom.RandomFloat < 0.1f)
				{
					num3 = -num3;
				}
				float randomFloat = MBRandom.RandomFloat;
				if (randomFloat < 0.2f)
				{
					overrideData.SwingSpeedOverriden += num3;
				}
				else if (randomFloat < 0.4f)
				{
					overrideData.SwingDamageOverriden += num3;
				}
				else if (randomFloat < 0.6f)
				{
					overrideData.ThrustSpeedOverriden += num3;
				}
				else if (randomFloat < 0.8f)
				{
					overrideData.ThrustDamageOverriden += num3;
				}
				else
				{
					overrideData.Handling += num3;
				}
				num++;
				num2 = overrideData.SwingSpeedOverriden + overrideData.SwingDamageOverriden + overrideData.ThrustSpeedOverriden + overrideData.ThrustDamageOverriden + overrideData.Handling;
			}
			return overrideData;
		}

		private int GetPointsToModify(int modifierTier)
		{
			if (modifierTier <= -4)
			{
				return -8;
			}
			if (modifierTier == -3)
			{
				return -6;
			}
			if (modifierTier == -2)
			{
				return -4;
			}
			if (modifierTier == -1)
			{
				return -2;
			}
			if (modifierTier == 0)
			{
				return 0;
			}
			if (modifierTier == 1)
			{
				return 2;
			}
			if (modifierTier == 2)
			{
				return 5;
			}
			if (modifierTier != 3)
			{
				return 10;
			}
			return 10;
		}

		public override int GetCraftingPartDifficulty(CraftingPiece craftingPiece)
		{
			if (!craftingPiece.IsEmptyPiece)
			{
				return craftingPiece.PieceTier * 50;
			}
			return 0;
		}

		public override int CalculateWeaponDesignDifficulty(WeaponDesign weaponDesign)
		{
			float num = 0f;
			float num2 = 0f;
			foreach (WeaponDesignElement weaponDesignElement in weaponDesign.UsedPieces)
			{
				if (weaponDesignElement.IsValid && !weaponDesignElement.CraftingPiece.IsEmptyPiece)
				{
					if (weaponDesignElement.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Blade)
					{
						num += 100f;
						num2 += (float)(this.GetCraftingPartDifficulty(weaponDesignElement.CraftingPiece) * 100);
					}
					else if (weaponDesignElement.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Guard)
					{
						num += 20f;
						num2 += (float)(this.GetCraftingPartDifficulty(weaponDesignElement.CraftingPiece) * 20);
					}
					else if (weaponDesignElement.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Handle)
					{
						num += 60f;
						num2 += (float)(this.GetCraftingPartDifficulty(weaponDesignElement.CraftingPiece) * 60);
					}
					else if (weaponDesignElement.CraftingPiece.PieceType == CraftingPiece.PieceTypes.Pommel)
					{
						num += 20f;
						num2 += (float)(this.GetCraftingPartDifficulty(weaponDesignElement.CraftingPiece) * 20);
					}
				}
			}
			return MathF.Round(num2 / num);
		}

		public override int GetModifierTierForSmithedWeapon(WeaponDesign weaponDesign, Hero hero)
		{
			int num = this.CalculateWeaponDesignDifficulty(weaponDesign);
			int num2 = hero.CharacterObject.GetSkillValue(DefaultSkills.Crafting) - num;
			if (num2 < 0)
			{
				return this.GetPenaltyForLowSkill(num2);
			}
			float randomFloat = MBRandom.RandomFloat;
			float num3 = (hero.GetPerkValue(DefaultPerks.Crafting.LegendarySmith) ? (DefaultPerks.Crafting.LegendarySmith.PrimaryBonus + Math.Max(0f, (float)(hero.GetSkillValue(DefaultSkills.Crafting) - 300)) * 0.01f) : 0f);
			float num4 = (hero.GetPerkValue(DefaultPerks.Crafting.MasterSmith) ? DefaultPerks.Crafting.MasterSmith.PrimaryBonus : 0f);
			float num5 = (hero.GetPerkValue(DefaultPerks.Crafting.ExperiencedSmith) ? DefaultPerks.Crafting.ExperiencedSmith.PrimaryBonus : 0f);
			if (randomFloat < num3)
			{
				return 3;
			}
			if (randomFloat < num3 + num4)
			{
				return 2;
			}
			if (randomFloat < num3 + num4 + num5)
			{
				return 1;
			}
			return 0;
		}

		private int GetPenaltyForLowSkill(int difference)
		{
			float num = MBRandom.RandomFloat;
			num += -0.01f * (float)difference;
			if (num < 0.4f)
			{
				return -1;
			}
			if (num < 0.7f)
			{
				return -2;
			}
			if (num < 0.9f)
			{
				return -3;
			}
			if (num >= 1f)
			{
				return -5;
			}
			return -4;
		}

		private float GetDifficultyForElement(WeaponDesignElement weaponDesignElement)
		{
			return (float)weaponDesignElement.CraftingPiece.PieceTier * (1f + 0.5f * weaponDesignElement.ScaleFactor);
		}

		public override IEnumerable<Crafting.RefiningFormula> GetRefiningFormulas(Hero weaponsmith)
		{
			if (weaponsmith.GetPerkValue(DefaultPerks.Crafting.CharcoalMaker))
			{
				yield return new Crafting.RefiningFormula(CraftingMaterials.Wood, 2, CraftingMaterials.Iron1, 0, CraftingMaterials.Charcoal, 3, CraftingMaterials.IronOre, 0);
			}
			else
			{
				yield return new Crafting.RefiningFormula(CraftingMaterials.Wood, 2, CraftingMaterials.Iron1, 0, CraftingMaterials.Charcoal, 1, CraftingMaterials.IronOre, 0);
			}
			yield return new Crafting.RefiningFormula(CraftingMaterials.IronOre, 1, CraftingMaterials.Charcoal, 1, CraftingMaterials.Iron1, weaponsmith.GetPerkValue(DefaultPerks.Crafting.IronMaker) ? 3 : 2, CraftingMaterials.IronOre, 0);
			yield return new Crafting.RefiningFormula(CraftingMaterials.Iron1, 1, CraftingMaterials.Charcoal, 1, CraftingMaterials.Iron2, 1, CraftingMaterials.IronOre, 0);
			yield return new Crafting.RefiningFormula(CraftingMaterials.Iron2, 2, CraftingMaterials.Charcoal, 1, CraftingMaterials.Iron3, 1, CraftingMaterials.Iron1, 1);
			if (weaponsmith.GetPerkValue(DefaultPerks.Crafting.SteelMaker))
			{
				yield return new Crafting.RefiningFormula(CraftingMaterials.Iron3, 2, CraftingMaterials.Charcoal, 1, CraftingMaterials.Iron4, 1, CraftingMaterials.Iron1, 1);
			}
			if (weaponsmith.GetPerkValue(DefaultPerks.Crafting.SteelMaker2))
			{
				yield return new Crafting.RefiningFormula(CraftingMaterials.Iron4, 2, CraftingMaterials.Charcoal, 1, CraftingMaterials.Iron5, 1, CraftingMaterials.Iron1, 1);
			}
			if (weaponsmith.GetPerkValue(DefaultPerks.Crafting.SteelMaker3))
			{
				yield return new Crafting.RefiningFormula(CraftingMaterials.Iron5, 2, CraftingMaterials.Charcoal, 1, CraftingMaterials.Iron6, 1, CraftingMaterials.Iron1, 1);
			}
			yield break;
		}

		public override int GetSkillXpForRefining(ref Crafting.RefiningFormula refineFormula)
		{
			return MathF.Round(0.3f * (float)(this.GetCraftingMaterialItem(refineFormula.Output).Value * refineFormula.OutputCount));
		}

		public override int GetSkillXpForSmelting(ItemObject item)
		{
			return MathF.Round(0.02f * (float)item.Value);
		}

		public override int GetSkillXpForSmithingInFreeBuildMode(ItemObject item)
		{
			return MathF.Round(0.02f * (float)item.Value);
		}

		public override int GetSkillXpForSmithingInCraftingOrderMode(ItemObject item)
		{
			return MathF.Round(0.1f * (float)item.Value);
		}

		public override int GetEnergyCostForRefining(ref Crafting.RefiningFormula refineFormula, Hero hero)
		{
			int num = 6;
			if (hero.GetPerkValue(DefaultPerks.Crafting.PracticalRefiner))
			{
				num = (num + 1) / 2;
			}
			return num;
		}

		public override int GetEnergyCostForSmithing(ItemObject item, Hero hero)
		{
			int num = (int)(10 + ItemObject.ItemTiers.Tier6 * item.Tier);
			if (hero.GetPerkValue(DefaultPerks.Crafting.PracticalSmith))
			{
				num = (num + 1) / 2;
			}
			return num;
		}

		public override int GetEnergyCostForSmelting(ItemObject item, Hero hero)
		{
			int num = 10;
			if (hero.GetPerkValue(DefaultPerks.Crafting.PracticalSmelter))
			{
				num = (num + 1) / 2;
			}
			return num;
		}

		public override ItemObject GetCraftingMaterialItem(CraftingMaterials craftingMaterial)
		{
			switch (craftingMaterial)
			{
			case CraftingMaterials.IronOre:
				return DefaultItems.IronOre;
			case CraftingMaterials.Iron1:
				return DefaultItems.IronIngot1;
			case CraftingMaterials.Iron2:
				return DefaultItems.IronIngot2;
			case CraftingMaterials.Iron3:
				return DefaultItems.IronIngot3;
			case CraftingMaterials.Iron4:
				return DefaultItems.IronIngot4;
			case CraftingMaterials.Iron5:
				return DefaultItems.IronIngot5;
			case CraftingMaterials.Iron6:
				return DefaultItems.IronIngot6;
			case CraftingMaterials.Wood:
				return DefaultItems.HardWood;
			case CraftingMaterials.Charcoal:
				return DefaultItems.Charcoal;
			default:
				return DefaultItems.IronIngot1;
			}
		}

		public override int[] GetSmeltingOutputForItem(ItemObject item)
		{
			int[] array = new int[9];
			if (item.WeaponDesign != null)
			{
				foreach (WeaponDesignElement weaponDesignElement in item.WeaponDesign.UsedPieces)
				{
					if (weaponDesignElement != null && weaponDesignElement.IsValid)
					{
						foreach (ValueTuple<CraftingMaterials, int> valueTuple in weaponDesignElement.CraftingPiece.MaterialsUsed)
						{
							array[(int)valueTuple.Item1] += valueTuple.Item2;
						}
					}
				}
				this.AddSmeltingReductions(array);
			}
			return array;
		}

		private void AddSmeltingReductions(int[] quantities)
		{
			if (quantities[6] > 0)
			{
				quantities[6]--;
				quantities[5]++;
			}
			else if (quantities[5] > 0)
			{
				quantities[5]--;
				quantities[4]++;
			}
			else if (quantities[4] > 0)
			{
				quantities[4]--;
				quantities[3]++;
			}
			else if (quantities[3] > 0)
			{
				quantities[3]--;
				quantities[2]++;
			}
			else if (quantities[2] > 0)
			{
				quantities[2]--;
				quantities[1]++;
			}
			quantities[8]--;
		}

		public override int[] GetSmithingCostsForWeaponDesign(WeaponDesign weaponDesign)
		{
			int[] array = new int[9];
			foreach (WeaponDesignElement weaponDesignElement in weaponDesign.UsedPieces)
			{
				if (weaponDesignElement != null && weaponDesignElement.IsValid)
				{
					foreach (ValueTuple<CraftingMaterials, int> valueTuple in weaponDesignElement.CraftingPiece.MaterialsUsed)
					{
						array[(int)valueTuple.Item1] -= valueTuple.Item2;
					}
				}
			}
			array[8]--;
			return array;
		}

		public override float ResearchPointsNeedForNewPart(int totalPartCount, int openedPartCount)
		{
			return MathF.Sqrt(100f / (float)totalPartCount) * ((float)openedPartCount * 9f + 10f);
		}

		public override int GetPartResearchGainForSmeltingItem(ItemObject item, Hero hero)
		{
			int num = 1 + MathF.Round(0.02f * (float)item.Value);
			if (hero.GetPerkValue(DefaultPerks.Crafting.CuriousSmelter))
			{
				num *= 2;
			}
			return num;
		}

		public override int GetPartResearchGainForSmithingItem(ItemObject item, Hero hero, bool isFreeBuild)
		{
			float num = 1f;
			if (hero.GetPerkValue(DefaultPerks.Crafting.CuriousSmith))
			{
				num *= 2f;
			}
			if (isFreeBuild)
			{
				num *= 0.1f;
			}
			return 1 + MathF.Floor(0.1f * (float)item.Value * num);
		}

		private const int BladeDifficultyCalculationWeight = 100;

		private const int GuardDifficultyCalculationWeight = 20;

		private const int HandleDifficultyCalculationWeight = 60;

		private const int PommelDifficultyCalculationWeight = 20;
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CraftingSystem
{
	public class CraftingOrder
	{
		internal static void AutoGeneratedStaticCollectObjectsCraftingOrder(object o, List<object> collectedObjects)
		{
			((CraftingOrder)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this.PreCraftedWeaponDesignItem);
			collectedObjects.Add(this.OrderOwner);
			collectedObjects.Add(this._weaponDesignTemplate);
			collectedObjects.Add(this._preCraftedWeaponDesignItemData);
		}

		internal static object AutoGeneratedGetMemberValueBaseGoldReward(object o)
		{
			return ((CraftingOrder)o).BaseGoldReward;
		}

		internal static object AutoGeneratedGetMemberValueOrderDifficulty(object o)
		{
			return ((CraftingOrder)o).OrderDifficulty;
		}

		internal static object AutoGeneratedGetMemberValuePreCraftedWeaponDesignItem(object o)
		{
			return ((CraftingOrder)o).PreCraftedWeaponDesignItem;
		}

		internal static object AutoGeneratedGetMemberValueOrderOwner(object o)
		{
			return ((CraftingOrder)o).OrderOwner;
		}

		internal static object AutoGeneratedGetMemberValueDifficultyLevel(object o)
		{
			return ((CraftingOrder)o).DifficultyLevel;
		}

		internal static object AutoGeneratedGetMemberValue_weaponDesignTemplate(object o)
		{
			return ((CraftingOrder)o)._weaponDesignTemplate;
		}

		internal static object AutoGeneratedGetMemberValue_preCraftedWeaponDesignItemData(object o)
		{
			return ((CraftingOrder)o)._preCraftedWeaponDesignItemData;
		}

		public bool IsLordOrder
		{
			get
			{
				return this.OrderOwner.IsLord;
			}
		}

		public CraftingOrder(Hero orderOwner, float orderDifficulty, WeaponDesign weaponDesignTemplate, CraftingTemplate template, int difficultyLevel = -1)
		{
			this.OrderOwner = orderOwner;
			this.OrderDifficulty = orderDifficulty;
			this.DifficultyLevel = difficultyLevel;
			this._weaponDesignTemplate = weaponDesignTemplate;
			Crafting.GenerateItem(weaponDesignTemplate, TextObject.Empty, orderOwner.Culture, template.ItemModifierGroup, ref this.PreCraftedWeaponDesignItem, null);
			if (this.PreCraftedWeaponDesignItem == null)
			{
				this.PreCraftedWeaponDesignItem = DefaultItems.Trash;
			}
			new Crafting.OverrideData(this.PreCraftedWeaponDesignItem.Weight + this.PreCraftedWeaponDesignItem.Weight * 0.2f, 0, 0, 0, 0);
			this._preCraftedWeaponDesignItemData = new CraftingCampaignBehavior.CraftedItemInitializationData(weaponDesignTemplate, this.PreCraftedWeaponDesignItem.Name, orderOwner.Culture);
			int theoreticalMaxItemMarketValue = Campaign.Current.Models.TradeItemPriceFactorModel.GetTheoreticalMaxItemMarketValue(this.PreCraftedWeaponDesignItem);
			this.BaseGoldReward = (int)((float)theoreticalMaxItemMarketValue + (float)theoreticalMaxItemMarketValue * MBRandom.RandomFloatRanged(-0.1f, 0.1f));
		}

		public void InitializeCraftingOrderOnLoad()
		{
			this.PreCraftedWeaponDesignItem = Crafting.InitializePreCraftedWeaponOnLoad(this.PreCraftedWeaponDesignItem, this._preCraftedWeaponDesignItemData.CraftedData, this._preCraftedWeaponDesignItemData.ItemName, this._preCraftedWeaponDesignItemData.Culture, null);
			if (this.PreCraftedWeaponDesignItem != null)
			{
				this.PreCraftedWeaponDesignItem.IsReady = true;
			}
		}

		public WeaponComponentData GetStatWeapon()
		{
			if (this.PreCraftedWeaponDesignItem.Weapons.Count > 1)
			{
				string stringId = this.PreCraftedWeaponDesignItem.WeaponDesign.Template.StringId;
				string text = "";
				if (stringId == "TwoHandedSword")
				{
					text = "TwoHandedSword";
				}
				else if (stringId == "Dagger")
				{
					text = "Dagger";
				}
				else if (stringId == "ThrowingKnife")
				{
					text = "ThrowingKnife";
				}
				else if (stringId == "TwoHandedAxe")
				{
					text = "TwoHandedAxe";
				}
				else if (stringId == "ThrowingAxe")
				{
					text = "ThrowingAxe";
				}
				else if (stringId == "TwoHandedPolearm")
				{
					text = "TwoHandedPolearm";
				}
				else if (stringId == "Javelin")
				{
					text = "Javelin";
				}
				if (!text.IsEmpty<char>())
				{
					foreach (WeaponComponentData weaponComponentData in this.PreCraftedWeaponDesignItem.Weapons)
					{
						if (weaponComponentData.WeaponDescriptionId == text || (text == "TwoHandedPolearm" && (weaponComponentData.WeaponDescriptionId == "TwoHandedPolearm_Couchable" || weaponComponentData.WeaponDescriptionId == "TwoHandedPolearm_Bracing")))
						{
							return weaponComponentData;
						}
					}
				}
			}
			return this.PreCraftedWeaponDesignItem.PrimaryWeapon;
		}

		public bool IsOrderAvailableForHero(Hero hero)
		{
			return (float)(hero.GetSkillValue(DefaultSkills.Crafting) + 50) >= this.OrderDifficulty;
		}

		public bool CanHeroCompleteOrder(Hero hero, ItemObject craftDesignItem)
		{
			return true;
		}

		public float GetOrderExperience(ItemObject craftedItem)
		{
			int num = (int)(this.PreCraftedWeaponDesignItem.Tier + 1);
			int num2 = (int)(craftedItem.Tier + 1);
			float num3 = MathF.Pow(3f, (float)(num2 - num));
			num3 = MathF.Clamp(num3, 0f, 1f);
			float num4 = (float)Campaign.Current.Models.TradeItemPriceFactorModel.GetTheoreticalMaxItemMarketValue(this.PreCraftedWeaponDesignItem) * 0.25f;
			float num5;
			float num6;
			bool flag;
			bool flag2;
			this.CheckForBonusesAndPenalties(craftedItem, out num5, out num6, out flag, out flag2);
			if (num5 < num6 || !flag || !flag2)
			{
				num4 = num4 * 0.5f * num3;
			}
			return num4;
		}

		public void CheckForBonusesAndPenalties(ItemObject craftedItem, out float craftedStatsSum, out float requiredStatsSum, out bool thrustDamageCheck, out bool swingDamageCheck)
		{
			WeaponComponentData weaponComponentData;
			List<CraftingStatData> statDataForItem = this.GetStatDataForItem(this.PreCraftedWeaponDesignItem, out weaponComponentData);
			WeaponComponentData weaponComponentData2;
			List<CraftingStatData> statDataForItem2 = this.GetStatDataForItem(craftedItem, out weaponComponentData2);
			swingDamageCheck = true;
			thrustDamageCheck = true;
			if (weaponComponentData.SwingDamageType != DamageTypes.Invalid && weaponComponentData.SwingDamageType != weaponComponentData2.SwingDamageType)
			{
				swingDamageCheck = false;
			}
			if (weaponComponentData.ThrustDamageType != DamageTypes.Invalid && weaponComponentData.ThrustDamageType != weaponComponentData2.ThrustDamageType)
			{
				thrustDamageCheck = false;
			}
			requiredStatsSum = 0f;
			craftedStatsSum = 0f;
			using (List<CraftingStatData>.Enumerator enumerator = statDataForItem.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CraftingStatData orderStat = enumerator.Current;
					if (orderStat.IsValid)
					{
						CraftingStatData craftingStatData = statDataForItem2.FirstOrDefault((CraftingStatData x) => x.Type == orderStat.Type);
						if (craftingStatData.CurValue != orderStat.CurValue)
						{
							requiredStatsSum += orderStat.CurValue;
							craftedStatsSum += craftingStatData.CurValue;
						}
					}
				}
			}
		}

		public List<CraftingStatData> GetStatDataForItem(ItemObject itemObject, out WeaponComponentData weapon)
		{
			List<CraftingStatData> list = new List<CraftingStatData>();
			weapon = null;
			WeaponComponentData statWeapon = this.GetStatWeapon();
			int num = -1;
			for (int i = 0; i < itemObject.Weapons.Count; i++)
			{
				if (statWeapon.WeaponDescriptionId == itemObject.Weapons[i].WeaponDescriptionId)
				{
					weapon = itemObject.Weapons[i];
					num = i;
					break;
				}
			}
			if (weapon == null && this.PreCraftedWeaponDesignItem.Weapons.Count > 1)
			{
				for (int j = 0; j < this.PreCraftedWeaponDesignItem.Weapons.Count; j++)
				{
					if (itemObject.PrimaryWeapon.WeaponDescriptionId == this.PreCraftedWeaponDesignItem.Weapons[j].WeaponDescriptionId)
					{
						weapon = itemObject.PrimaryWeapon;
						num = 1;
						break;
					}
				}
			}
			bool flag = weapon.ThrustDamageType != DamageTypes.Invalid;
			bool flag2 = weapon.SwingDamageType != DamageTypes.Invalid;
			foreach (KeyValuePair<CraftingTemplate.CraftingStatTypes, float> keyValuePair in itemObject.WeaponDesign.Template.GetStatDatas(num, weapon.ThrustDamageType, weapon.SwingDamageType))
			{
				TextObject textObject = GameTexts.FindText("str_crafting_stat", keyValuePair.Key.ToString());
				if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.ThrustSpeed && flag)
				{
					list.Add(new CraftingStatData(textObject, (float)weapon.ThrustSpeed, keyValuePair.Value, keyValuePair.Key, DamageTypes.Invalid));
				}
				else if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.SwingSpeed && flag2)
				{
					list.Add(new CraftingStatData(textObject, (float)weapon.SwingSpeed, keyValuePair.Value, keyValuePair.Key, DamageTypes.Invalid));
				}
				else if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.ThrustDamage && flag)
				{
					textObject.SetTextVariable("THRUST_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.ThrustDamageType).ToString()));
					list.Add(new CraftingStatData(textObject, (float)weapon.ThrustDamage, keyValuePair.Value, keyValuePair.Key, weapon.ThrustDamageType));
				}
				else if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.SwingDamage && flag2)
				{
					textObject.SetTextVariable("SWING_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.SwingDamageType).ToString()));
					list.Add(new CraftingStatData(textObject, (float)weapon.SwingDamage, keyValuePair.Value, keyValuePair.Key, weapon.SwingDamageType));
				}
				else if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.Handling)
				{
					list.Add(new CraftingStatData(textObject, (float)weapon.Handling, keyValuePair.Value, keyValuePair.Key, DamageTypes.Invalid));
				}
				else if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.MissileDamage)
				{
					if (weapon.ThrustDamageType != DamageTypes.Invalid)
					{
						textObject.SetTextVariable("THRUST_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.ThrustDamageType).ToString()));
					}
					else if (weapon.SwingDamageType != DamageTypes.Invalid)
					{
						textObject.SetTextVariable("SWING_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.SwingDamageType).ToString()));
					}
					else
					{
						Debug.FailedAssert("Missile damage type is missing.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CraftingSystem\\CraftingOrder.cs", "GetStatDataForItem", 301);
					}
					list.Add(new CraftingStatData(textObject, (float)weapon.MissileDamage, keyValuePair.Value, keyValuePair.Key, DamageTypes.Invalid));
				}
				else if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.MissileSpeed)
				{
					list.Add(new CraftingStatData(textObject, (float)weapon.MissileSpeed, keyValuePair.Value, keyValuePair.Key, DamageTypes.Invalid));
				}
				else if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.Accuracy)
				{
					list.Add(new CraftingStatData(textObject, (float)weapon.Accuracy, keyValuePair.Value, keyValuePair.Key, DamageTypes.Invalid));
				}
				else if (keyValuePair.Key == CraftingTemplate.CraftingStatTypes.WeaponReach)
				{
					list.Add(new CraftingStatData(textObject, (float)weapon.WeaponLength, keyValuePair.Value, keyValuePair.Key, DamageTypes.Invalid));
				}
			}
			return list;
		}

		private const string TwoHandedSwordCraftingTemplateId = "TwoHandedSword";

		private const string DaggerCraftingTemplateId = "Dagger";

		private const string ThrowingKnifeCraftingTemplateId = "ThrowingKnife";

		private const string TwoHandedAxeCraftingTemplateId = "TwoHandedAxe";

		private const string ThrowingAxeCraftingTemplateId = "ThrowingAxe";

		private const string TwoHandedPolearmCraftingTemplateId = "TwoHandedPolearm";

		private const string JavelinCraftingTemplateId = "Javelin";

		private const string TwoHandedPolearmUsageCouchableId = "TwoHandedPolearm_Couchable";

		private const string TwoHandedPolearmUsageBracingId = "TwoHandedPolearm_Bracing";

		private const int CraftingAttemptDifficultyGap = 50;

		[SaveableField(10)]
		public readonly int BaseGoldReward;

		[SaveableField(20)]
		public readonly float OrderDifficulty;

		[SaveableField(30)]
		private readonly WeaponDesign _weaponDesignTemplate;

		[SaveableField(40)]
		public ItemObject PreCraftedWeaponDesignItem;

		[SaveableField(50)]
		private readonly CraftingCampaignBehavior.CraftedItemInitializationData _preCraftedWeaponDesignItemData;

		[SaveableField(60)]
		public Hero OrderOwner;

		[SaveableField(70)]
		public readonly int DifficultyLevel;
	}
}
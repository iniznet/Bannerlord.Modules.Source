using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class Crafting
	{
		public Crafting(CraftingTemplate craftingTemplate, BasicCultureObject culture, TextObject name)
		{
			this.CraftedWeaponName = name;
			this.CurrentCraftingTemplate = craftingTemplate;
			this.CurrentCulture = culture;
		}

		public BasicCultureObject CurrentCulture { get; }

		public CraftingTemplate CurrentCraftingTemplate { get; }

		public WeaponDesign CurrentWeaponDesign { get; private set; }

		public ItemModifierGroup CurrentItemModifierGroup { get; private set; }

		public TextObject CraftedWeaponName { get; private set; }

		public void SetCraftedWeaponName(string name)
		{
			if (!name.Equals(this.CraftedWeaponName.ToString()))
			{
				this.CraftedWeaponName = new TextObject("{=!}" + name, null);
				this._craftedItemObject.WeaponDesign.SetWeaponName(this.CraftedWeaponName);
			}
		}

		public void Init()
		{
			this._history = new List<WeaponDesign>();
			this.UsablePiecesList = new List<WeaponDesignElement>[4];
			using (IEnumerator<CraftingPiece> enumerator = ((IEnumerable<CraftingPiece>)this.CurrentCraftingTemplate.Pieces).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CraftingPiece craftingPiece = enumerator.Current;
					if (!this.CurrentCraftingTemplate.BuildOrders.All((PieceData x) => x.PieceType != craftingPiece.PieceType))
					{
						int pieceType = (int)craftingPiece.PieceType;
						if (this.UsablePiecesList[pieceType] == null)
						{
							this.UsablePiecesList[pieceType] = new List<WeaponDesignElement>();
						}
						this.UsablePiecesList[pieceType].Add(WeaponDesignElement.CreateUsablePiece(craftingPiece, 100));
					}
				}
			}
			WeaponDesignElement[] array = new WeaponDesignElement[4];
			for (int i = 0; i < array.Length; i++)
			{
				if (this.UsablePiecesList[i] != null)
				{
					array[i] = this.UsablePiecesList[i].First((WeaponDesignElement p) => !p.CraftingPiece.IsHiddenOnDesigner);
				}
				else
				{
					array[i] = WeaponDesignElement.GetInvalidPieceForType((CraftingPiece.PieceTypes)i);
				}
			}
			this.CurrentWeaponDesign = new WeaponDesign(this.CurrentCraftingTemplate, null, array);
			this._history.Add(this.CurrentWeaponDesign);
		}

		public List<WeaponDesignElement>[] UsablePiecesList { get; private set; }

		public WeaponDesignElement[] SelectedPieces
		{
			get
			{
				return this.CurrentWeaponDesign.UsedPieces;
			}
		}

		public WeaponDesignElement GetRandomPieceOfType(CraftingPiece.PieceTypes pieceType, bool randomScale)
		{
			if (!this.CurrentCraftingTemplate.IsPieceTypeUsable(pieceType))
			{
				return WeaponDesignElement.GetInvalidPieceForType(pieceType);
			}
			WeaponDesignElement copy = this.UsablePiecesList[(int)pieceType][MBRandom.RandomInt(this.UsablePiecesList[(int)pieceType].Count)].GetCopy();
			if (randomScale)
			{
				copy.SetScale((int)(90f + MBRandom.RandomFloat * 20f));
			}
			return copy;
		}

		public void SwitchToCraftedItem(ItemObject item)
		{
			WeaponDesignElement[] usedPieces = item.WeaponDesign.UsedPieces;
			WeaponDesignElement[] array = new WeaponDesignElement[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = usedPieces[i].GetCopy();
			}
			this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, array);
			this.ReIndex(false);
		}

		public void Randomize()
		{
			WeaponDesignElement[] array = new WeaponDesignElement[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.GetRandomPieceOfType((CraftingPiece.PieceTypes)i, true);
			}
			this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, array);
			this.ReIndex(false);
		}

		public void SwitchToPiece(WeaponDesignElement piece)
		{
			CraftingPiece.PieceTypes pieceType = piece.CraftingPiece.PieceType;
			WeaponDesignElement[] array = new WeaponDesignElement[4];
			for (int i = 0; i < array.Length; i++)
			{
				if (pieceType == (CraftingPiece.PieceTypes)i)
				{
					array[i] = piece.GetCopy();
					array[i].SetScale(100);
				}
				else
				{
					array[i] = this.CurrentWeaponDesign.UsedPieces[i].GetCopy();
					if (array[i].IsValid)
					{
						array[i].SetScale(this.CurrentWeaponDesign.UsedPieces[i].ScalePercentage);
					}
				}
			}
			this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, array);
			this.ReIndex(false);
		}

		public void ScaleThePiece(CraftingPiece.PieceTypes scalingPieceType, int percentage)
		{
			WeaponDesignElement[] array = new WeaponDesignElement[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.SelectedPieces[i].GetCopy();
				if (this.SelectedPieces[i].IsPieceScaled)
				{
					array[i].SetScale(this.SelectedPieces[i].ScalePercentage);
				}
			}
			array[(int)scalingPieceType].SetScale(percentage);
			this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, array);
			this.ReIndex(false);
		}

		public void ReIndex(bool enforceReCreation = false)
		{
			if (!TextObject.IsNullOrEmpty(this.CurrentWeaponDesign.WeaponName) && !this.CurrentWeaponDesign.WeaponName.ToString().Equals(this.CraftedWeaponName.ToString()))
			{
				this.CraftedWeaponName = this.CurrentWeaponDesign.WeaponName.CopyTextObject();
			}
			if (enforceReCreation)
			{
				this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, this.CurrentWeaponDesign.UsedPieces.ToArray<WeaponDesignElement>());
			}
			this.SetItemObject(null);
		}

		public bool Undo()
		{
			if (this._currentHistoryIndex <= 0)
			{
				return false;
			}
			this._currentHistoryIndex--;
			this.CurrentWeaponDesign = this._history[this._currentHistoryIndex];
			this.ReIndex(false);
			return true;
		}

		public bool Redo()
		{
			if (this._currentHistoryIndex + 1 >= this._history.Count)
			{
				return false;
			}
			this._currentHistoryIndex++;
			this.CurrentWeaponDesign = this._history[this._currentHistoryIndex];
			this.ReIndex(false);
			return true;
		}

		public void UpdateHistory()
		{
			if (this._currentHistoryIndex < this._history.Count - 1)
			{
				this._history.RemoveRange(this._currentHistoryIndex + 1, this._history.Count - 1 - this._currentHistoryIndex);
			}
			WeaponDesignElement[] array = new WeaponDesignElement[this.CurrentWeaponDesign.UsedPieces.Length];
			for (int i = 0; i < this.CurrentWeaponDesign.UsedPieces.Length; i++)
			{
				array[i] = this.CurrentWeaponDesign.UsedPieces[i].GetCopy();
				if (array[i].IsValid)
				{
					array[i].SetScale(this.CurrentWeaponDesign.UsedPieces[i].ScalePercentage);
				}
			}
			this._history.Add(new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, array));
			this._currentHistoryIndex = this._history.Count - 1;
		}

		public TextObject GetRandomCraftName()
		{
			return new TextObject("{=!}RANDOM_NAME", null);
		}

		public static void GenerateItem(WeaponDesign weaponDesignTemplate, TextObject name, BasicCultureObject culture, ItemModifierGroup itemModifierGroup, ref ItemObject itemObject)
		{
			if (itemObject == null)
			{
				itemObject = new ItemObject();
			}
			WeaponDesignElement[] array = new WeaponDesignElement[weaponDesignTemplate.UsedPieces.Length];
			for (int i = 0; i < weaponDesignTemplate.UsedPieces.Length; i++)
			{
				WeaponDesignElement weaponDesignElement = weaponDesignTemplate.UsedPieces[i];
				array[i] = WeaponDesignElement.CreateUsablePiece(weaponDesignElement.CraftingPiece, weaponDesignElement.ScalePercentage);
			}
			WeaponDesign weaponDesign = new WeaponDesign(weaponDesignTemplate.Template, name, array);
			float num = MathF.Round(weaponDesign.UsedPieces.Sum((WeaponDesignElement selectedUsablePiece) => selectedUsablePiece.ScaledWeight), 2);
			float num2 = (weaponDesign.UsedPieces[3].IsValid ? weaponDesign.UsedPieces[3].CraftingPiece.Appearance : weaponDesign.UsedPieces[0].CraftingPiece.Appearance);
			itemObject.StringId = ((!string.IsNullOrEmpty(itemObject.StringId)) ? itemObject.StringId : weaponDesign.HashedCode);
			ItemObject.InitCraftedItemObject(ref itemObject, name, culture, Crafting.GetItemFlags(weaponDesign), num, num2, weaponDesign, weaponDesign.Template.ItemType);
			itemObject = Crafting.CraftedItemGenerationHelper.GenerateCraftedItem(itemObject, weaponDesign, itemModifierGroup);
			if (itemObject != null)
			{
				if (itemObject.IsCraftedByPlayer)
				{
					itemObject.IsReady = true;
				}
				itemObject.DetermineValue();
				itemObject.DetermineItemCategoryForItem();
			}
		}

		private static ItemFlags GetItemFlags(WeaponDesign weaponDesign)
		{
			return weaponDesign.UsedPieces[0].CraftingPiece.AdditionalItemFlags;
		}

		private void SetItemObject(ItemObject itemObject = null)
		{
			if (itemObject == null)
			{
				itemObject = new ItemObject();
			}
			Crafting.GenerateItem(this.CurrentWeaponDesign, this.CraftedWeaponName, this.CurrentCulture, this.CurrentItemModifierGroup, ref itemObject);
			this._craftedItemObject = itemObject;
		}

		public ItemObject GetCurrentCraftedItemObject(bool forceReCreate = false)
		{
			if (forceReCreate)
			{
				this.SetItemObject(null);
			}
			return this._craftedItemObject;
		}

		public static IEnumerable<CraftingStatData> GetStatDatasFromTemplate(int usageIndex, ItemObject craftedItemObject, CraftingTemplate template)
		{
			WeaponComponentData weapon = craftedItemObject.GetWeaponWithUsageIndex(usageIndex);
			DamageTypes statDamageType = DamageTypes.Invalid;
			foreach (KeyValuePair<CraftingTemplate.CraftingStatTypes, float> keyValuePair in template.GetStatDatas(usageIndex, weapon.ThrustDamageType, weapon.SwingDamageType))
			{
				TextObject textObject = GameTexts.FindText("str_crafting_stat", keyValuePair.Key.ToString());
				switch (keyValuePair.Key)
				{
				case CraftingTemplate.CraftingStatTypes.ThrustDamage:
					textObject.SetTextVariable("THRUST_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.ThrustDamageType).ToString()));
					statDamageType = weapon.ThrustDamageType;
					break;
				case CraftingTemplate.CraftingStatTypes.SwingDamage:
					textObject.SetTextVariable("SWING_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.SwingDamageType).ToString()));
					statDamageType = weapon.SwingDamageType;
					break;
				case CraftingTemplate.CraftingStatTypes.MissileDamage:
					if (weapon.ThrustDamageType != DamageTypes.Invalid)
					{
						textObject.SetTextVariable("THRUST_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.ThrustDamageType).ToString()));
						statDamageType = weapon.ThrustDamageType;
					}
					else if (weapon.SwingDamageType != DamageTypes.Invalid)
					{
						textObject.SetTextVariable("SWING_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.SwingDamageType).ToString()));
						statDamageType = weapon.SwingDamageType;
					}
					else
					{
						Debug.FailedAssert("Missile damage type is missing.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "GetStatDatasFromTemplate", 1169);
					}
					break;
				}
				float value = keyValuePair.Value;
				float num = Crafting.GetValueForCraftingStatForWeaponOfUsageIndex(keyValuePair.Key, craftedItemObject, weapon);
				num = MBMath.ClampFloat(num, 0f, value);
				yield return new CraftingStatData(textObject, num, value, keyValuePair.Key, statDamageType);
			}
			IEnumerator<KeyValuePair<CraftingTemplate.CraftingStatTypes, float>> enumerator = null;
			yield break;
			yield break;
		}

		private static float GetValueForCraftingStatForWeaponOfUsageIndex(CraftingTemplate.CraftingStatTypes craftingStatType, ItemObject item, WeaponComponentData weapon)
		{
			switch (craftingStatType)
			{
			case CraftingTemplate.CraftingStatTypes.Weight:
				return item.Weight;
			case CraftingTemplate.CraftingStatTypes.WeaponReach:
				return (float)weapon.WeaponLength;
			case CraftingTemplate.CraftingStatTypes.ThrustSpeed:
				return (float)weapon.ThrustSpeed;
			case CraftingTemplate.CraftingStatTypes.SwingSpeed:
				return (float)weapon.SwingSpeed;
			case CraftingTemplate.CraftingStatTypes.ThrustDamage:
				return (float)weapon.ThrustDamage;
			case CraftingTemplate.CraftingStatTypes.SwingDamage:
				return (float)weapon.SwingDamage;
			case CraftingTemplate.CraftingStatTypes.Handling:
				return (float)weapon.Handling;
			case CraftingTemplate.CraftingStatTypes.MissileDamage:
				return (float)weapon.MissileDamage;
			case CraftingTemplate.CraftingStatTypes.MissileSpeed:
				return (float)weapon.MissileSpeed;
			case CraftingTemplate.CraftingStatTypes.Accuracy:
				return (float)weapon.Accuracy;
			case CraftingTemplate.CraftingStatTypes.StackAmount:
				return (float)weapon.GetModifiedStackCount(null);
			default:
				throw new ArgumentOutOfRangeException("craftingStatType", craftingStatType, null);
			}
		}

		public IEnumerable<CraftingStatData> GetStatDatas(int usageIndex)
		{
			WeaponComponentData weapon = this._craftedItemObject.GetWeaponWithUsageIndex(usageIndex);
			foreach (KeyValuePair<CraftingTemplate.CraftingStatTypes, float> keyValuePair in this.CurrentCraftingTemplate.GetStatDatas(usageIndex, weapon.ThrustDamageType, weapon.SwingDamageType))
			{
				DamageTypes damageTypes = DamageTypes.Invalid;
				TextObject textObject = GameTexts.FindText("str_crafting_stat", keyValuePair.Key.ToString());
				switch (keyValuePair.Key)
				{
				case CraftingTemplate.CraftingStatTypes.ThrustDamage:
					textObject.SetTextVariable("THRUST_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.ThrustDamageType).ToString()));
					damageTypes = weapon.ThrustDamageType;
					break;
				case CraftingTemplate.CraftingStatTypes.SwingDamage:
					textObject.SetTextVariable("SWING_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.SwingDamageType).ToString()));
					damageTypes = weapon.SwingDamageType;
					break;
				case CraftingTemplate.CraftingStatTypes.MissileDamage:
					if (weapon.ThrustDamageType != DamageTypes.Invalid)
					{
						textObject.SetTextVariable("THRUST_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.ThrustDamageType).ToString()));
						damageTypes = weapon.ThrustDamageType;
					}
					else if (weapon.SwingDamageType != DamageTypes.Invalid)
					{
						textObject.SetTextVariable("SWING_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int)weapon.SwingDamageType).ToString()));
						damageTypes = weapon.SwingDamageType;
					}
					else
					{
						Debug.FailedAssert("Missile damage type is missing.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "GetStatDatas", 1254);
					}
					break;
				}
				float valueForCraftingStatForWeaponOfUsageIndex = Crafting.GetValueForCraftingStatForWeaponOfUsageIndex(keyValuePair.Key, this._craftedItemObject, weapon);
				float value = keyValuePair.Value;
				yield return new CraftingStatData(textObject, valueForCraftingStatForWeaponOfUsageIndex, value, keyValuePair.Key, damageTypes);
			}
			IEnumerator<KeyValuePair<CraftingTemplate.CraftingStatTypes, float>> enumerator = null;
			yield break;
			yield break;
		}

		public string GetXmlCodeForCurrentItem(ItemObject item)
		{
			string text = "";
			text = string.Concat(new object[]
			{
				text,
				"<CraftedItem id=\"",
				this.CurrentWeaponDesign.HashedCode,
				"\"\n\t\t\t\t\t\t\t name=\"",
				this.CraftedWeaponName,
				"\"\n\t\t\t\t\t\t\t crafting_template=\"",
				this.CurrentCraftingTemplate.StringId,
				"\">"
			});
			text += "\n";
			text += "<Pieces>";
			text += "\n";
			foreach (WeaponDesignElement weaponDesignElement in this.SelectedPieces)
			{
				if (weaponDesignElement.IsValid)
				{
					string text2 = "";
					if (weaponDesignElement.ScalePercentage != 100)
					{
						int scalePercentage = weaponDesignElement.ScalePercentage;
						text2 = "\n\t\t\t scale_factor=\"" + scalePercentage + "\"";
					}
					text = string.Concat(new object[]
					{
						text,
						"<Piece id=\"",
						weaponDesignElement.CraftingPiece.StringId,
						"\"\n\t\t\t Type=\"",
						weaponDesignElement.CraftingPiece.PieceType,
						"\"",
						text2,
						"/>"
					});
					text += "\n";
				}
			}
			text += "</Pieces>";
			text += "\n";
			text += "<!-- ";
			text = text + "Length: " + item.PrimaryWeapon.WeaponLength;
			text = text + " Weight: " + MathF.Round(item.Weight, 2);
			text += " -->";
			text += "\n";
			return text + "</CraftedItem>";
		}

		public bool TryGetWeaponPropertiesFromXmlCode(string xmlCode, out CraftingTemplate craftingTemplate, out ValueTuple<CraftingPiece, int>[] pieces)
		{
			bool flag;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xmlCode);
				pieces = new ValueTuple<CraftingPiece, int>[4];
				XmlNode xmlNode = xmlDocument.SelectSingleNode("CraftedItem");
				string value = xmlNode.Attributes["crafting_template"].Value;
				craftingTemplate = CraftingTemplate.GetTemplateFromId(value);
				foreach (object obj in xmlNode.SelectSingleNode("Pieces").ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					CraftingPiece.PieceTypes pieceTypes = CraftingPiece.PieceTypes.Invalid;
					string pieceId = null;
					int num = 100;
					foreach (object obj2 in xmlNode2.Attributes)
					{
						XmlAttribute xmlAttribute = (XmlAttribute)obj2;
						if (xmlAttribute.Name == "Type")
						{
							pieceTypes = (CraftingPiece.PieceTypes)Enum.Parse(typeof(CraftingPiece.PieceTypes), xmlAttribute.Value);
						}
						else if (xmlAttribute.Name == "id")
						{
							pieceId = xmlAttribute.Value;
						}
						else if (xmlAttribute.Name == "scale_factor")
						{
							num = int.Parse(xmlAttribute.Value);
						}
					}
					if (pieceTypes != CraftingPiece.PieceTypes.Invalid && !string.IsNullOrEmpty(pieceId) && craftingTemplate.IsPieceTypeUsable(pieceTypes))
					{
						pieces[(int)pieceTypes] = new ValueTuple<CraftingPiece, int>(CraftingPiece.All.FirstOrDefault((CraftingPiece p) => p.StringId == pieceId), num);
					}
				}
				flag = true;
			}
			catch (Exception)
			{
				craftingTemplate = null;
				pieces = null;
				flag = false;
			}
			return flag;
		}

		public static ItemObject CreatePreCraftedWeapon(ItemObject itemObject, WeaponDesignElement[] usedPieces, string templateId, TextObject weaponName, ItemModifierGroup itemModifierGroup)
		{
			for (int i = 0; i < usedPieces.Length; i++)
			{
				if (usedPieces[i] == null)
				{
					usedPieces[i] = WeaponDesignElement.GetInvalidPieceForType((CraftingPiece.PieceTypes)i);
				}
			}
			TextObject textObject = ((!TextObject.IsNullOrEmpty(weaponName)) ? weaponName : new TextObject("{=Uz1HHeKg}Crafted Random Weapon", null));
			WeaponDesign weaponDesign = new WeaponDesign(CraftingTemplate.GetTemplateFromId(templateId), textObject, usedPieces);
			Crafting crafting = new Crafting(CraftingTemplate.GetTemplateFromId(templateId), null, textObject);
			crafting.CurrentWeaponDesign = weaponDesign;
			crafting.CurrentItemModifierGroup = itemModifierGroup;
			crafting._history = new List<WeaponDesign> { weaponDesign };
			crafting.SetItemObject(itemObject);
			return crafting._craftedItemObject;
		}

		public static ItemObject InitializePreCraftedWeaponOnLoad(ItemObject itemObject, WeaponDesign craftedData, TextObject itemName, BasicCultureObject culture)
		{
			Crafting crafting = new Crafting(craftedData.Template, culture, itemName);
			crafting.CurrentWeaponDesign = craftedData;
			crafting._history = new List<WeaponDesign> { craftedData };
			crafting.SetItemObject(itemObject);
			return crafting._craftedItemObject;
		}

		public static ItemObject CreateRandomCraftedItem(BasicCultureObject culture)
		{
			CraftingTemplate randomElement = CraftingTemplate.All.GetRandomElement<CraftingTemplate>();
			TextObject textObject = new TextObject("{=uZhHh7pm}Crafted {CURR_TEMPLATE_NAME}", null);
			textObject.SetTextVariable("CURR_TEMPLATE_NAME", randomElement.TemplateName);
			Crafting crafting = new Crafting(randomElement, culture, textObject);
			crafting.Init();
			crafting.Randomize();
			string hashedCode = crafting._craftedItemObject.WeaponDesign.HashedCode;
			crafting._craftedItemObject.StringId = hashedCode;
			ItemObject itemObject = MBObjectManager.Instance.GetObject<ItemObject>(hashedCode);
			if (itemObject == null)
			{
				itemObject = MBObjectManager.Instance.RegisterObject<ItemObject>(crafting._craftedItemObject);
			}
			return itemObject;
		}

		public const int WeightOfCrudeIron = 1;

		public const int WeightOfIron = 2;

		public const int WeightOfCompositeIron = 3;

		public const int WeightOfSteel = 4;

		public const int WeightOfRefinedSteel = 5;

		public const int WeightOfCalradianSteel = 6;

		private List<WeaponDesign> _history;

		private int _currentHistoryIndex;

		private ItemObject _craftedItemObject;

		public class RefiningFormula
		{
			public RefiningFormula(CraftingMaterials input1, int input1Count, CraftingMaterials input2, int input2Count, CraftingMaterials output, int outputCount = 1, CraftingMaterials output2 = CraftingMaterials.IronOre, int output2Count = 0)
			{
				this.Output = output;
				this.OutputCount = outputCount;
				this.Output2 = output2;
				this.Output2Count = output2Count;
				this.Input1 = input1;
				this.Input1Count = input1Count;
				this.Input2 = input2;
				this.Input2Count = input2Count;
			}

			public CraftingMaterials Output { get; }

			public int OutputCount { get; }

			public CraftingMaterials Output2 { get; }

			public int Output2Count { get; }

			public CraftingMaterials Input1 { get; }

			public int Input1Count { get; }

			public CraftingMaterials Input2 { get; }

			public int Input2Count { get; }
		}

		private static class CraftedItemGenerationHelper
		{
			public static ItemObject GenerateCraftedItem(ItemObject item, WeaponDesign weaponDesign, ItemModifierGroup itemModifierGroup)
			{
				foreach (WeaponDesignElement weaponDesignElement in weaponDesign.UsedPieces)
				{
					if ((weaponDesignElement.IsValid && !weaponDesign.Template.Pieces.Contains(weaponDesignElement.CraftingPiece)) || (weaponDesignElement.CraftingPiece.IsInitialized && !weaponDesignElement.IsValid))
					{
						Debug.Print(weaponDesignElement.CraftingPiece.StringId + " is not a valid valid anymore.", 0, Debug.DebugColor.White, 17592186044416UL);
						return null;
					}
				}
				bool flag = false;
				foreach (WeaponDescription weaponDescription in weaponDesign.Template.WeaponDescriptions)
				{
					int num = 4;
					for (int j = 0; j < weaponDesign.UsedPieces.Length; j++)
					{
						if (!weaponDesign.UsedPieces[j].IsValid)
						{
							num--;
						}
					}
					foreach (CraftingPiece craftingPiece in weaponDescription.AvailablePieces)
					{
						int pieceType = (int)craftingPiece.PieceType;
						if (weaponDesign.UsedPieces[pieceType].CraftingPiece == craftingPiece)
						{
							num--;
						}
						if (num == 0)
						{
							break;
						}
					}
					if (num <= 0)
					{
						WeaponFlags weaponFlags = weaponDescription.WeaponFlags | weaponDesign.WeaponFlags;
						WeaponComponentData weaponComponentData;
						Crafting.CraftedItemGenerationHelper.CraftingStats.FillWeapon(item, weaponDescription, weaponFlags, flag, out weaponComponentData);
						item.AddWeapon(weaponComponentData, itemModifierGroup);
						flag = true;
					}
				}
				return item;
			}

			private struct CraftingStats
			{
				public static void FillWeapon(ItemObject item, WeaponDescription weaponDescription, WeaponFlags weaponFlags, bool isAlternative, out WeaponComponentData filledWeapon)
				{
					filledWeapon = new WeaponComponentData(item, weaponDescription.WeaponClass, weaponFlags);
					Crafting.CraftedItemGenerationHelper.CraftingStats craftingStats = new Crafting.CraftedItemGenerationHelper.CraftingStats
					{
						_craftedData = item.WeaponDesign,
						_weaponDescription = weaponDescription
					};
					craftingStats.CalculateStats();
					craftingStats.SetWeaponData(filledWeapon, isAlternative);
				}

				private void CalculateStats()
				{
					WeaponDesign craftedData = this._craftedData;
					this._stoppingTorque = 10f;
					this._armInertia = 2.9f;
					if (this._weaponDescription.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand))
					{
						this._stoppingTorque *= 1.5f;
						this._armInertia *= 1.4f;
					}
					if (this._weaponDescription.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip))
					{
						this._stoppingTorque *= 1.5f;
						this._armInertia *= 1.4f;
					}
					this._currentWeaponWeight = 0f;
					this._currentWeaponReach = 0f;
					this._currentWeaponCenterOfMass = 0f;
					this._currentWeaponInertia = 0f;
					this._currentWeaponInertiaAroundShoulder = 0f;
					this._currentWeaponInertiaAroundGrip = 0f;
					this._currentWeaponSwingSpeed = 1f;
					this._currentWeaponThrustSpeed = 1f;
					this._currentWeaponSwingDamage = 0f;
					this._currentWeaponThrustDamage = 0f;
					this._currentWeaponHandling = 1f;
					this._currentWeaponTier = WeaponComponentData.WeaponTiers.Tier1;
					this._currentWeaponWeight = MathF.Round(craftedData.UsedPieces.Sum((WeaponDesignElement selectedUsablePiece) => selectedUsablePiece.ScaledWeight), 2);
					this._currentWeaponReach = MathF.Round(this._craftedData.CraftedWeaponLength, 2);
					this._currentWeaponCenterOfMass = this.CalculateCenterOfMass();
					this._currentWeaponInertia = this.CalculateWeaponInertia();
					this._currentWeaponInertiaAroundShoulder = Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis(this._currentWeaponInertia, this._currentWeaponWeight, 0.5f + this._currentWeaponCenterOfMass);
					this._currentWeaponInertiaAroundGrip = Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis(this._currentWeaponInertia, this._currentWeaponWeight, this._currentWeaponCenterOfMass);
					this._currentWeaponSwingSpeed = this.CalculateSwingSpeed();
					this._currentWeaponThrustSpeed = this.CalculateThrustSpeed();
					this._currentWeaponHandling = (float)this.CalculateAgility();
					this._currentWeaponTier = this.CalculateWeaponTier();
					this._swingDamageFactor = this._craftedData.UsedPieces[0].CraftingPiece.BladeData.SwingDamageFactor;
					this._thrustDamageFactor = this._craftedData.UsedPieces[0].CraftingPiece.BladeData.ThrustDamageFactor;
					if (this._weaponDescription.WeaponClass == WeaponClass.ThrowingAxe || this._weaponDescription.WeaponClass == WeaponClass.ThrowingKnife || this._weaponDescription.WeaponClass == WeaponClass.Javelin)
					{
						this._currentWeaponSwingDamage = 0f;
						this.CalculateMissileDamage(out this._currentWeaponThrustDamage);
					}
					else
					{
						this.CalculateSwingBaseDamage(out this._currentWeaponSwingDamage);
						this.CalculateThrustBaseDamage(out this._currentWeaponThrustDamage, false);
					}
					this._currentWeaponSweetSpot = this.CalculateSweetSpot();
				}

				private void SetWeaponData(WeaponComponentData weapon, bool isAlternative)
				{
					BladeData bladeData = this._craftedData.UsedPieces[0].CraftingPiece.BladeData;
					short num = 0;
					string text = "";
					int num2 = 0;
					int num3 = 0;
					MatrixFrame identity = MatrixFrame.Identity;
					short num4 = 1;
					if (this._weaponDescription.WeaponClass == WeaponClass.Javelin || this._weaponDescription.WeaponClass == WeaponClass.ThrowingAxe || this._weaponDescription.WeaponClass == WeaponClass.ThrowingKnife)
					{
						short num5 = (isAlternative ? 1 : bladeData.StackAmount);
						switch (this._weaponDescription.WeaponClass)
						{
						case WeaponClass.ThrowingAxe:
							num = num5;
							num2 = 93;
							text = "event:/mission/combat/throwing/passby";
							break;
						case WeaponClass.ThrowingKnife:
							num = num5;
							num2 = 95;
							text = "event:/mission/combat/throwing/passby";
							break;
						case WeaponClass.Javelin:
							num = num5;
							num2 = 92;
							text = "event:/mission/combat/missile/passby";
							break;
						}
						num3 = MathF.Floor(this.CalculateMissileSpeed());
						Mat3 identity2 = Mat3.Identity;
						switch (this._weaponDescription.WeaponClass)
						{
						case WeaponClass.ThrowingAxe:
						{
							float bladeWidth = this._craftedData.UsedPieces[0].CraftingPiece.BladeData.BladeWidth;
							float num6 = this._craftedData.PiecePivotDistances[0];
							float scaledDistanceToNextPiece = this._craftedData.UsedPieces[0].ScaledDistanceToNextPiece;
							identity2.RotateAboutUp(1.5707964f);
							identity2.RotateAboutSide(-(15f + scaledDistanceToNextPiece * 3f / num6 * 25f) * 0.017453292f);
							identity = new MatrixFrame(identity2, -identity2.u * (num6 + scaledDistanceToNextPiece * 0.6f) - identity2.f * bladeWidth * 0.8f);
							break;
						}
						case WeaponClass.ThrowingKnife:
							identity2.RotateAboutForward(-1.5707964f);
							identity = new MatrixFrame(identity2, Vec3.Side * this._currentWeaponReach);
							break;
						case WeaponClass.Javelin:
							identity2.RotateAboutSide(1.5707964f);
							identity = new MatrixFrame(identity2, -Vec3.Up * this._currentWeaponReach);
							break;
						}
					}
					if (this._weaponDescription.WeaponClass == WeaponClass.Arrow || this._weaponDescription.WeaponClass == WeaponClass.Bolt)
					{
						identity.rotation.RotateAboutSide(1.5707964f);
					}
					Vec3 zero = Vec3.Zero;
					if (this._weaponDescription.WeaponClass == WeaponClass.ThrowingAxe)
					{
						zero = new Vec3(0f, 18f, 0f, -1f);
					}
					else if (this._weaponDescription.WeaponClass == WeaponClass.ThrowingKnife)
					{
						zero = new Vec3(0f, 24f, 0f, -1f);
					}
					weapon.Init(this._weaponDescription.StringId, bladeData.PhysicsMaterial, this.GetItemUsage(), bladeData.ThrustDamageType, bladeData.SwingDamageType, this.GetWeaponHandArmorBonus(), (int)(this._currentWeaponReach * 100f), MathF.Round(this.GetWeaponBalance(), 2), this._currentWeaponInertia, this._currentWeaponCenterOfMass, MathF.Floor(this._currentWeaponHandling), MathF.Round(this._swingDamageFactor, 2), MathF.Round(this._thrustDamageFactor, 2), num, text, num2, num3, identity, this.GetAmmoClass(), this._currentWeaponSweetSpot, MathF.Floor(this._currentWeaponSwingSpeed * 4.5454545f), MathF.Round(this._currentWeaponSwingDamage), MathF.Floor(this._currentWeaponThrustSpeed * 11.764706f), MathF.Round(this._currentWeaponThrustDamage), zero, this._currentWeaponTier, num4);
					Mat3 identity3 = Mat3.Identity;
					Vec3 vec = Vec3.Zero;
					if (this._weaponDescription.RotatedInHand)
					{
						identity3.RotateAboutSide(3.1415927f);
					}
					if (this._weaponDescription.UseCenterOfMassAsHandBase)
					{
						vec = -Vec3.Up * this._currentWeaponCenterOfMass;
					}
					weapon.SetFrame(new MatrixFrame(identity3, identity3.TransformToParent(vec)));
				}

				private float CalculateSweetSpot()
				{
					float num = -1f;
					float num2 = -1f;
					for (int i = 0; i < 100; i++)
					{
						float num3 = 0.01f * (float)i;
						float num4 = CombatStatCalculator.CalculateStrikeMagnitudeForSwing(this._currentWeaponSwingSpeed, num3, this._currentWeaponWeight, this._currentWeaponReach, this._currentWeaponInertia, this._currentWeaponCenterOfMass, 0f);
						if (num < num4)
						{
							num = num4;
							num2 = num3;
						}
					}
					return num2;
				}

				private float CalculateCenterOfMass()
				{
					float num = 0f;
					float num2 = 0f;
					float num3 = 0f;
					foreach (PieceData pieceData in this._craftedData.Template.BuildOrders)
					{
						CraftingPiece.PieceTypes pieceType = pieceData.PieceType;
						WeaponDesignElement weaponDesignElement = this._craftedData.UsedPieces[(int)pieceType];
						if (weaponDesignElement.IsValid)
						{
							float scaledWeight = weaponDesignElement.ScaledWeight;
							float num4 = 0f;
							if (pieceData.Order < 0)
							{
								num4 -= (num3 + (weaponDesignElement.ScaledLength - weaponDesignElement.ScaledCenterOfMass)) * scaledWeight;
								num3 += weaponDesignElement.ScaledLength;
							}
							else
							{
								num4 += (num2 + weaponDesignElement.ScaledCenterOfMass) * scaledWeight;
								num2 += weaponDesignElement.ScaledLength;
							}
							num += num4;
						}
					}
					return num / this._currentWeaponWeight - (this._craftedData.UsedPieces[2].ScaledDistanceToPreviousPiece - this._craftedData.UsedPieces[2].ScaledPieceOffset);
				}

				private float CalculateWeaponInertia()
				{
					float num = -this._currentWeaponCenterOfMass;
					float num2 = 0f;
					foreach (PieceData pieceData in this._craftedData.Template.BuildOrders)
					{
						WeaponDesignElement weaponDesignElement = this._craftedData.UsedPieces[(int)pieceData.PieceType];
						if (weaponDesignElement.IsValid)
						{
							float num3 = 1f;
							num2 += Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis(weaponDesignElement, num, num3);
							num += weaponDesignElement.ScaledLength;
						}
					}
					return num2;
				}

				private float CalculateSwingSpeed()
				{
					double num = 1.0 * (double)this._currentWeaponInertiaAroundShoulder + 0.9;
					double num2 = 170.0;
					double num3 = 90.0;
					double num4 = 27.0;
					double num5 = 15.0;
					double num6 = 7.0;
					if (this._weaponDescription.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand))
					{
						if (this._weaponDescription.WeaponFlags.HasAnyFlag(WeaponFlags.WideGrip))
						{
							num += 1.5;
							num6 *= 4.0;
							num5 *= 1.7;
							num3 *= 1.3;
							num2 *= 1.15;
						}
						else
						{
							num += 1.0;
							num6 *= 2.4;
							num5 *= 1.3;
							num3 *= 1.35;
							num2 *= 1.15;
						}
					}
					num4 = MathF.Max(1.0, num4 - num);
					num5 = MathF.Max(1.0, num5 - num);
					num6 = MathF.Max(1.0, num6 - num);
					double num7;
					double num8;
					this.SimulateSwingLayer(1.5, 200.0, num4, 2.0 + num, out num7, out num8);
					double num9;
					double num10;
					this.SimulateSwingLayer(1.5, num2, num5, 1.0 + num, out num9, out num10);
					double num11;
					double num12;
					this.SimulateSwingLayer(1.5, num3, num6, 0.5 + num, out num11, out num12);
					double num13 = 0.33 * (num8 + num10 + num12);
					return (float)(20.8 / num13);
				}

				private float CalculateThrustSpeed()
				{
					double num = 1.8 + (double)this._currentWeaponWeight + (double)this._currentWeaponInertiaAroundGrip * 0.2;
					double num2 = 170.0;
					double num3 = 90.0;
					double num4 = 24.0;
					double num5 = 15.0;
					if (this._weaponDescription.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand) && !this._weaponDescription.WeaponFlags.HasAnyFlag(WeaponFlags.WideGrip))
					{
						num += 0.6;
						num5 *= 1.9;
						num4 *= 1.1;
						num3 *= 1.2;
						num2 *= 1.05;
					}
					else if (this._weaponDescription.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.WideGrip))
					{
						num += 0.9;
						num5 *= 2.1;
						num4 *= 1.2;
						num3 *= 1.2;
						num2 *= 1.05;
					}
					double num6;
					double num7;
					this.SimulateThrustLayer(0.6, 250.0, 48.0, 4.0 + num, out num6, out num7);
					double num8;
					double num9;
					this.SimulateThrustLayer(0.6, num2, num4, 2.0 + num, out num8, out num9);
					double num10;
					double num11;
					this.SimulateThrustLayer(0.6, num3, num5, 0.5 + num, out num10, out num11);
					double num12 = 0.33 * (num7 + num9 + num11);
					return (float)(3.8500000000000005 / num12);
				}

				private void CalculateSwingBaseDamage(out float damage)
				{
					float num = 0f;
					for (float num2 = 0.93f; num2 > 0.5f; num2 -= 0.05f)
					{
						float num3 = CombatStatCalculator.CalculateBaseBlowMagnitudeForSwing(this._currentWeaponSwingSpeed, this._currentWeaponReach, this._currentWeaponWeight, this._currentWeaponInertia, this._currentWeaponCenterOfMass, num2, 0f);
						if (num3 > num)
						{
							num = num3;
						}
					}
					damage = num * this._swingDamageFactor;
				}

				private void CalculateThrustBaseDamage(out float damage, bool isThrown = false)
				{
					float num = CombatStatCalculator.CalculateStrikeMagnitudeForThrust(this._currentWeaponThrustSpeed, this._currentWeaponWeight, 0f, isThrown);
					damage = num * this._thrustDamageFactor;
				}

				private void CalculateMissileDamage(out float damage)
				{
					switch (this._weaponDescription.WeaponClass)
					{
					case WeaponClass.ThrowingAxe:
						this.CalculateSwingBaseDamage(out damage);
						damage *= 2f;
						return;
					case WeaponClass.ThrowingKnife:
						this.CalculateThrustBaseDamage(out damage, true);
						damage *= 3.3f;
						return;
					case WeaponClass.Javelin:
						this.CalculateThrustBaseDamage(out damage, true);
						damage *= 9f;
						return;
					default:
						damage = 0f;
						Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "CalculateMissileDamage", 508);
						return;
					}
				}

				private WeaponComponentData.WeaponTiers CalculateWeaponTier()
				{
					int num = 0;
					int num2 = 0;
					foreach (WeaponDesignElement weaponDesignElement in this._craftedData.UsedPieces.Where((WeaponDesignElement ucp) => ucp.IsValid))
					{
						num += weaponDesignElement.CraftingPiece.PieceTier;
						num2++;
					}
					WeaponComponentData.WeaponTiers weaponTiers;
					if (Enum.TryParse<WeaponComponentData.WeaponTiers>(((int)((float)num / (float)num2)).ToString(), out weaponTiers))
					{
						return weaponTiers;
					}
					Debug.FailedAssert("Couldn't calculate weapon tier", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "CalculateWeaponTier", 529);
					return WeaponComponentData.WeaponTiers.Tier1;
				}

				private string GetItemUsage()
				{
					List<string> list = this._weaponDescription.ItemUsageFeatures.Split(new char[] { ':' }).ToList<string>();
					foreach (WeaponDesignElement weaponDesignElement in this._craftedData.UsedPieces.Where((WeaponDesignElement ucp) => ucp.IsValid))
					{
						foreach (string text in weaponDesignElement.CraftingPiece.ItemUsageFeaturesToExclude.Split(new char[] { ':' }))
						{
							if (!string.IsNullOrEmpty(text))
							{
								list.Remove(text);
							}
						}
					}
					string text2 = "";
					for (int j = 0; j < list.Count; j++)
					{
						text2 += list[j];
						if (j < list.Count - 1)
						{
							text2 += "_";
						}
					}
					return text2;
				}

				private float CalculateMissileSpeed()
				{
					if (this._weaponDescription.WeaponClass == WeaponClass.ThrowingAxe)
					{
						return this._currentWeaponThrustSpeed * 3.2f;
					}
					if (this._weaponDescription.WeaponClass == WeaponClass.ThrowingKnife)
					{
						return this._currentWeaponThrustSpeed * 3.9f;
					}
					if (this._weaponDescription.WeaponClass == WeaponClass.Javelin)
					{
						return this._currentWeaponThrustSpeed * 3.6f;
					}
					Debug.FailedAssert("Weapon is not a missile.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "CalculateMissileSpeed", 580);
					return 10f;
				}

				private int CalculateAgility()
				{
					float num = this._currentWeaponInertiaAroundGrip;
					if (this._weaponDescription.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand))
					{
						num *= 0.5f;
						num += 0.9f;
					}
					else if (this._weaponDescription.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip))
					{
						num *= 0.4f;
						num += 1f;
					}
					else
					{
						num += 0.7f;
					}
					float num2 = MathF.Pow(1f / num, 0.55f);
					num2 *= 1f;
					return MathF.Round(100f * num2);
				}

				private float GetWeaponBalance()
				{
					return MBMath.ClampFloat((this._currentWeaponSwingSpeed * 4.5454545f - 70f) / 30f, 0f, 1f);
				}

				private int GetWeaponHandArmorBonus()
				{
					WeaponDesignElement weaponDesignElement = this._craftedData.UsedPieces[1];
					if (weaponDesignElement == null)
					{
						return 0;
					}
					return weaponDesignElement.CraftingPiece.ArmorBonus;
				}

				private WeaponClass GetAmmoClass()
				{
					if (this._weaponDescription.WeaponClass != WeaponClass.ThrowingKnife && this._weaponDescription.WeaponClass != WeaponClass.ThrowingAxe && this._weaponDescription.WeaponClass != WeaponClass.Javelin)
					{
						return WeaponClass.Undefined;
					}
					return this._weaponDescription.WeaponClass;
				}

				private static float ParallelAxis(WeaponDesignElement selectedPiece, float offset, float weightMultiplier)
				{
					float inertia = selectedPiece.CraftingPiece.Inertia;
					float num = offset + selectedPiece.CraftingPiece.CenterOfMass;
					float num2 = selectedPiece.ScaledWeight * weightMultiplier;
					return Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis(inertia, num2, num);
				}

				private static float ParallelAxis(float inertiaAroundCm, float mass, float offsetFromCm)
				{
					return inertiaAroundCm + mass * offsetFromCm * offsetFromCm;
				}

				private void SimulateSwingLayer(double angleSpan, double usablePower, double maxUsableTorque, double inertia, out double finalSpeed, out double finalTime)
				{
					double num = 0.0;
					double num2 = 0.01;
					double num3 = 0.0;
					double num4 = 3.9 * (double)this._currentWeaponReach * (this._weaponDescription.WeaponFlags.HasAllFlags(WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip) ? 1.0 : 0.3);
					while (num < angleSpan)
					{
						double num5 = usablePower / num2;
						if (num5 > maxUsableTorque)
						{
							num5 = maxUsableTorque;
						}
						num5 -= num2 * num4;
						double num6 = 0.009999999776482582 * num5 / inertia;
						num2 += num6;
						num += num2 * 0.009999999776482582;
						num3 += 0.009999999776482582;
					}
					finalSpeed = num2;
					finalTime = num3;
				}

				private void SimulateThrustLayer(double distance, double usablePower, double maxUsableForce, double mass, out double finalSpeed, out double finalTime)
				{
					double num = 0.0;
					double num2 = 0.01;
					double num3 = 0.0;
					while (num < distance)
					{
						double num4 = usablePower / num2;
						if (num4 > maxUsableForce)
						{
							num4 = maxUsableForce;
						}
						double num5 = 0.01 * num4 / mass;
						num2 += num5;
						num += num2 * 0.01;
						num3 += 0.01;
					}
					finalSpeed = num2;
					finalTime = num3;
				}

				private WeaponDesign _craftedData;

				private WeaponDescription _weaponDescription;

				private float _stoppingTorque;

				private float _armInertia;

				private float _swingDamageFactor;

				private float _thrustDamageFactor;

				private float _currentWeaponWeight;

				private float _currentWeaponReach;

				private float _currentWeaponSweetSpot;

				private float _currentWeaponCenterOfMass;

				private float _currentWeaponInertia;

				private float _currentWeaponInertiaAroundShoulder;

				private float _currentWeaponInertiaAroundGrip;

				private float _currentWeaponSwingSpeed;

				private float _currentWeaponThrustSpeed;

				private float _currentWeaponHandling;

				private float _currentWeaponSwingDamage;

				private float _currentWeaponThrustDamage;

				private WeaponComponentData.WeaponTiers _currentWeaponTier;
			}
		}
	}
}

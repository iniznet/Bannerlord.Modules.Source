using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	// Token: 0x0200003C RID: 60
	public class Crafting
	{
		// Token: 0x0600048C RID: 1164 RVA: 0x00010F05 File Offset: 0x0000F105
		public Crafting(CraftingTemplate craftingTemplate, BasicCultureObject culture, TextObject name)
		{
			this.CraftedWeaponName = name;
			this.CurrentCraftingTemplate = craftingTemplate;
			this.CurrentCulture = culture;
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x00010F22 File Offset: 0x0000F122
		public BasicCultureObject CurrentCulture { get; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x00010F2A File Offset: 0x0000F12A
		public CraftingTemplate CurrentCraftingTemplate { get; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00010F32 File Offset: 0x0000F132
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x00010F3A File Offset: 0x0000F13A
		public WeaponDesign CurrentWeaponDesign { get; private set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x00010F43 File Offset: 0x0000F143
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x00010F4B File Offset: 0x0000F14B
		public ItemModifierGroup CurrentItemModifierGroup { get; private set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x00010F54 File Offset: 0x0000F154
		// (set) Token: 0x06000494 RID: 1172 RVA: 0x00010F5C File Offset: 0x0000F15C
		public TextObject CraftedWeaponName { get; private set; }

		// Token: 0x06000495 RID: 1173 RVA: 0x00010F68 File Offset: 0x0000F168
		public void SetCraftedWeaponName(string name)
		{
			if (!name.Equals(this.CraftedWeaponName.ToString()))
			{
				this.CraftedWeaponName = new TextObject("{=!}" + name, null);
				this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CraftedWeaponName, this.CurrentWeaponDesign.UsedPieces);
			}
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x00010FC8 File Offset: 0x0000F1C8
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

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x00011118 File Offset: 0x0000F318
		// (set) Token: 0x06000498 RID: 1176 RVA: 0x00011120 File Offset: 0x0000F320
		public List<WeaponDesignElement>[] UsablePiecesList { get; private set; }

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x00011129 File Offset: 0x0000F329
		public WeaponDesignElement[] SelectedPieces
		{
			get
			{
				return this.CurrentWeaponDesign.UsedPieces;
			}
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00011138 File Offset: 0x0000F338
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

		// Token: 0x0600049B RID: 1179 RVA: 0x0001119C File Offset: 0x0000F39C
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

		// Token: 0x0600049C RID: 1180 RVA: 0x00011200 File Offset: 0x0000F400
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

		// Token: 0x0600049D RID: 1181 RVA: 0x00011258 File Offset: 0x0000F458
		public void SwitchToPiece(WeaponDesignElement piece)
		{
			this.SelectedPieces[(int)piece.CraftingPiece.PieceType].SetScale(100);
			CraftingPiece.PieceTypes pieceType = piece.CraftingPiece.PieceType;
			WeaponDesignElement[] array = new WeaponDesignElement[4];
			for (int i = 0; i < array.Length; i++)
			{
				if (pieceType == (CraftingPiece.PieceTypes)i)
				{
					array[i] = piece.GetCopy();
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

		// Token: 0x0600049E RID: 1182 RVA: 0x00011310 File Offset: 0x0000F510
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

		// Token: 0x0600049F RID: 1183 RVA: 0x00011398 File Offset: 0x0000F598
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
			this.SetItemObject(null, null);
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001142B File Offset: 0x0000F62B
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

		// Token: 0x060004A1 RID: 1185 RVA: 0x00011468 File Offset: 0x0000F668
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

		// Token: 0x060004A2 RID: 1186 RVA: 0x000114BC File Offset: 0x0000F6BC
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

		// Token: 0x060004A3 RID: 1187 RVA: 0x000115A3 File Offset: 0x0000F7A3
		public TextObject GetRandomCraftName()
		{
			return new TextObject("{=!}RANDOM_NAME", null);
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x000115B0 File Offset: 0x0000F7B0
		public static void GenerateItem(WeaponDesign weaponDesignTemplate, TextObject name, BasicCultureObject culture, ItemModifierGroup itemModifierGroup, ref ItemObject itemObject, Crafting.OverrideData overridenData)
		{
			if (itemObject == null)
			{
				itemObject = new ItemObject();
			}
			if (overridenData == null)
			{
				overridenData = new Crafting.OverrideData(0f, 0, 0, 0, 0);
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
			overridenData.WeightOverriden += num;
			itemObject.StringId = ((!string.IsNullOrEmpty(itemObject.StringId)) ? itemObject.StringId : weaponDesign.HashedCode);
			ItemObject.InitCraftedItemObject(ref itemObject, name, culture, Crafting.GetItemFlags(weaponDesign), overridenData.WeightOverriden, num2, weaponDesign, weaponDesign.Template.ItemType);
			itemObject = Crafting.CraftedItemGenerationHelper.GenerateCraftedItem(itemObject, weaponDesign, itemModifierGroup, overridenData);
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

		// Token: 0x060004A5 RID: 1189 RVA: 0x0001172A File Offset: 0x0000F92A
		private static ItemFlags GetItemFlags(WeaponDesign weaponDesign)
		{
			return weaponDesign.UsedPieces[0].CraftingPiece.AdditionalItemFlags;
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0001173E File Offset: 0x0000F93E
		private void SetItemObject(Crafting.OverrideData overridenData, ItemObject itemObject = null)
		{
			if (itemObject == null)
			{
				itemObject = new ItemObject();
			}
			Crafting.GenerateItem(this.CurrentWeaponDesign, this.CraftedWeaponName, this.CurrentCulture, this.CurrentItemModifierGroup, ref itemObject, overridenData);
			this._craftedItemObject = itemObject;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00011771 File Offset: 0x0000F971
		public ItemObject GetCurrentCraftedItemObject(bool forceReCreate = false, Crafting.OverrideData overrideData = null)
		{
			if (forceReCreate)
			{
				this.SetItemObject(overrideData, null);
			}
			return this._craftedItemObject;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00011784 File Offset: 0x0000F984
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
						Debug.FailedAssert("Missile damage type is missing.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "GetStatDatasFromTemplate", 1216);
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

		// Token: 0x060004A9 RID: 1193 RVA: 0x000117A4 File Offset: 0x0000F9A4
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

		// Token: 0x060004AA RID: 1194 RVA: 0x0001184E File Offset: 0x0000FA4E
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
						Debug.FailedAssert("Missile damage type is missing.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "GetStatDatas", 1301);
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

		// Token: 0x060004AB RID: 1195 RVA: 0x00011868 File Offset: 0x0000FA68
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

		// Token: 0x060004AC RID: 1196 RVA: 0x00011A34 File Offset: 0x0000FC34
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

		// Token: 0x060004AD RID: 1197 RVA: 0x00011C2C File Offset: 0x0000FE2C
		public static ItemObject CreatePreCraftedWeapon(ItemObject itemObject, WeaponDesignElement[] usedPieces, string templateId, TextObject weaponName, Crafting.OverrideData overridenData, ItemModifierGroup itemModifierGroup)
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
			crafting.SetItemObject(overridenData, itemObject);
			return crafting._craftedItemObject;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00011CB6 File Offset: 0x0000FEB6
		public static ItemObject InitializePreCraftedWeaponOnLoad(ItemObject itemObject, WeaponDesign craftedData, TextObject itemName, BasicCultureObject culture, Crafting.OverrideData overrideData = null)
		{
			Crafting crafting = new Crafting(craftedData.Template, culture, itemName);
			crafting.CurrentWeaponDesign = craftedData;
			crafting._history = new List<WeaponDesign> { craftedData };
			crafting.SetItemObject(overrideData, itemObject);
			return crafting._craftedItemObject;
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00011CEC File Offset: 0x0000FEEC
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

		// Token: 0x0400023F RID: 575
		public const int WeightOfCrudeIron = 1;

		// Token: 0x04000240 RID: 576
		public const int WeightOfIron = 2;

		// Token: 0x04000241 RID: 577
		public const int WeightOfCompositeIron = 3;

		// Token: 0x04000242 RID: 578
		public const int WeightOfSteel = 4;

		// Token: 0x04000243 RID: 579
		public const int WeightOfRefinedSteel = 5;

		// Token: 0x04000244 RID: 580
		public const int WeightOfCalradianSteel = 6;

		// Token: 0x0400024B RID: 587
		private List<WeaponDesign> _history;

		// Token: 0x0400024C RID: 588
		private int _currentHistoryIndex;

		// Token: 0x0400024D RID: 589
		private ItemObject _craftedItemObject;

		// Token: 0x020000EC RID: 236
		public class OverrideData
		{
			// Token: 0x060009ED RID: 2541 RVA: 0x000205AE File Offset: 0x0001E7AE
			internal static void AutoGeneratedStaticCollectObjectsOverrideData(object o, List<object> collectedObjects)
			{
				((Crafting.OverrideData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x060009EE RID: 2542 RVA: 0x000205BC File Offset: 0x0001E7BC
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
			}

			// Token: 0x060009EF RID: 2543 RVA: 0x000205BE File Offset: 0x0001E7BE
			internal static object AutoGeneratedGetMemberValueWeightOverriden(object o)
			{
				return ((Crafting.OverrideData)o).WeightOverriden;
			}

			// Token: 0x060009F0 RID: 2544 RVA: 0x000205D0 File Offset: 0x0001E7D0
			internal static object AutoGeneratedGetMemberValueSwingSpeedOverriden(object o)
			{
				return ((Crafting.OverrideData)o).SwingSpeedOverriden;
			}

			// Token: 0x060009F1 RID: 2545 RVA: 0x000205E2 File Offset: 0x0001E7E2
			internal static object AutoGeneratedGetMemberValueThrustSpeedOverriden(object o)
			{
				return ((Crafting.OverrideData)o).ThrustSpeedOverriden;
			}

			// Token: 0x060009F2 RID: 2546 RVA: 0x000205F4 File Offset: 0x0001E7F4
			internal static object AutoGeneratedGetMemberValueSwingDamageOverriden(object o)
			{
				return ((Crafting.OverrideData)o).SwingDamageOverriden;
			}

			// Token: 0x060009F3 RID: 2547 RVA: 0x00020606 File Offset: 0x0001E806
			internal static object AutoGeneratedGetMemberValueThrustDamageOverriden(object o)
			{
				return ((Crafting.OverrideData)o).ThrustDamageOverriden;
			}

			// Token: 0x060009F4 RID: 2548 RVA: 0x00020618 File Offset: 0x0001E818
			internal static object AutoGeneratedGetMemberValueHandling(object o)
			{
				return ((Crafting.OverrideData)o).Handling;
			}

			// Token: 0x17000343 RID: 835
			// (get) Token: 0x060009F5 RID: 2549 RVA: 0x0002062A File Offset: 0x0001E82A
			public static Crafting.OverrideData Invalid
			{
				get
				{
					return new Crafting.OverrideData(0f, 0, 0, 0, 0);
				}
			}

			// Token: 0x060009F6 RID: 2550 RVA: 0x0002063A File Offset: 0x0001E83A
			public OverrideData(float weightOverriden = 0f, int swingSpeedOverriden = 0, int thrustSpeedOverriden = 0, int swingDamageOverriden = 0, int thrustDamageOverriden = 0)
			{
				this.WeightOverriden = weightOverriden;
				this.SwingSpeedOverriden = swingSpeedOverriden;
				this.ThrustSpeedOverriden = thrustSpeedOverriden;
				this.SwingDamageOverriden = swingDamageOverriden;
				this.ThrustDamageOverriden = thrustDamageOverriden;
			}

			// Token: 0x04000682 RID: 1666
			[SaveableField(10)]
			public float WeightOverriden;

			// Token: 0x04000683 RID: 1667
			[SaveableField(20)]
			public int SwingSpeedOverriden;

			// Token: 0x04000684 RID: 1668
			[SaveableField(30)]
			public int ThrustSpeedOverriden;

			// Token: 0x04000685 RID: 1669
			[SaveableField(40)]
			public int SwingDamageOverriden;

			// Token: 0x04000686 RID: 1670
			[SaveableField(50)]
			public int ThrustDamageOverriden;

			// Token: 0x04000687 RID: 1671
			[SaveableField(60)]
			public int Handling;
		}

		// Token: 0x020000ED RID: 237
		public class RefiningFormula
		{
			// Token: 0x060009F7 RID: 2551 RVA: 0x00020668 File Offset: 0x0001E868
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

			// Token: 0x17000344 RID: 836
			// (get) Token: 0x060009F8 RID: 2552 RVA: 0x000206B8 File Offset: 0x0001E8B8
			public CraftingMaterials Output { get; }

			// Token: 0x17000345 RID: 837
			// (get) Token: 0x060009F9 RID: 2553 RVA: 0x000206C0 File Offset: 0x0001E8C0
			public int OutputCount { get; }

			// Token: 0x17000346 RID: 838
			// (get) Token: 0x060009FA RID: 2554 RVA: 0x000206C8 File Offset: 0x0001E8C8
			public CraftingMaterials Output2 { get; }

			// Token: 0x17000347 RID: 839
			// (get) Token: 0x060009FB RID: 2555 RVA: 0x000206D0 File Offset: 0x0001E8D0
			public int Output2Count { get; }

			// Token: 0x17000348 RID: 840
			// (get) Token: 0x060009FC RID: 2556 RVA: 0x000206D8 File Offset: 0x0001E8D8
			public CraftingMaterials Input1 { get; }

			// Token: 0x17000349 RID: 841
			// (get) Token: 0x060009FD RID: 2557 RVA: 0x000206E0 File Offset: 0x0001E8E0
			public int Input1Count { get; }

			// Token: 0x1700034A RID: 842
			// (get) Token: 0x060009FE RID: 2558 RVA: 0x000206E8 File Offset: 0x0001E8E8
			public CraftingMaterials Input2 { get; }

			// Token: 0x1700034B RID: 843
			// (get) Token: 0x060009FF RID: 2559 RVA: 0x000206F0 File Offset: 0x0001E8F0
			public int Input2Count { get; }
		}

		// Token: 0x020000EE RID: 238
		private static class CraftedItemGenerationHelper
		{
			// Token: 0x06000A00 RID: 2560 RVA: 0x000206F8 File Offset: 0x0001E8F8
			public static ItemObject GenerateCraftedItem(ItemObject item, WeaponDesign craftedData, ItemModifierGroup itemModifierGroup, Crafting.OverrideData overridenData)
			{
				foreach (WeaponDesignElement weaponDesignElement in craftedData.UsedPieces)
				{
					if ((weaponDesignElement.IsValid && !craftedData.Template.Pieces.Contains(weaponDesignElement.CraftingPiece)) || (weaponDesignElement.CraftingPiece.IsInitialized && !weaponDesignElement.IsValid))
					{
						Debug.Print(weaponDesignElement.CraftingPiece.StringId + " is not a valid valid anymore.", 0, Debug.DebugColor.White, 17592186044416UL);
						return null;
					}
				}
				bool flag = false;
				foreach (WeaponDescription weaponDescription in craftedData.Template.WeaponDescriptions)
				{
					int num = 4;
					for (int j = 0; j < craftedData.UsedPieces.Length; j++)
					{
						if (!craftedData.UsedPieces[j].IsValid)
						{
							num--;
						}
					}
					foreach (CraftingPiece craftingPiece in weaponDescription.AvailablePieces)
					{
						int pieceType = (int)craftingPiece.PieceType;
						if (craftedData.UsedPieces[pieceType].CraftingPiece == craftingPiece)
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
						WeaponComponentData weaponComponentData = new WeaponComponentData(item, weaponDescription.WeaponClass, weaponDescription.WeaponFlags | craftedData.WeaponFlags);
						Crafting.CraftedItemGenerationHelper.CraftingStats.FillWeapon(item, weaponComponentData, weaponDescription, flag, overridenData);
						item.AddWeapon(weaponComponentData, itemModifierGroup);
						flag = true;
					}
				}
				return item;
			}

			// Token: 0x0200011A RID: 282
			private struct CraftingStats
			{
				// Token: 0x06000A77 RID: 2679 RVA: 0x00021C14 File Offset: 0x0001FE14
				public static void FillWeapon(ItemObject item, WeaponComponentData weapon, WeaponDescription weaponDescription, bool isAlternative, Crafting.OverrideData overridenData)
				{
					Crafting.CraftedItemGenerationHelper.CraftingStats craftingStats = new Crafting.CraftedItemGenerationHelper.CraftingStats
					{
						_craftedData = item.WeaponDesign,
						_weaponDescription = weaponDescription
					};
					craftingStats.CalculateStats(overridenData);
					craftingStats.SetWeaponData(weapon, isAlternative);
				}

				// Token: 0x06000A78 RID: 2680 RVA: 0x00021C54 File Offset: 0x0001FE54
				private void CalculateStats(Crafting.OverrideData overridenData)
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
					this._currentWeaponSwingSpeed += (float)overridenData.SwingSpeedOverriden / 4.5454545f;
					this._currentWeaponThrustSpeed += (float)overridenData.ThrustSpeedOverriden / 11.764706f;
					this._currentWeaponSwingDamage += (float)overridenData.SwingDamageOverriden;
					this._currentWeaponThrustDamage += (float)overridenData.ThrustDamageOverriden;
					this._currentWeaponHandling += (float)overridenData.Handling;
					this._currentWeaponSweetSpot = this.CalculateSweetSpot();
					this._currentWeaponWeight += overridenData.WeightOverriden;
				}

				// Token: 0x06000A79 RID: 2681 RVA: 0x00021F78 File Offset: 0x00020178
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

				// Token: 0x06000A7A RID: 2682 RVA: 0x00022340 File Offset: 0x00020540
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

				// Token: 0x06000A7B RID: 2683 RVA: 0x000223A8 File Offset: 0x000205A8
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

				// Token: 0x06000A7C RID: 2684 RVA: 0x000224AC File Offset: 0x000206AC
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

				// Token: 0x06000A7D RID: 2685 RVA: 0x0002252C File Offset: 0x0002072C
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

				// Token: 0x06000A7E RID: 2686 RVA: 0x00022700 File Offset: 0x00020900
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

				// Token: 0x06000A7F RID: 2687 RVA: 0x000228AC File Offset: 0x00020AAC
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

				// Token: 0x06000A80 RID: 2688 RVA: 0x00022914 File Offset: 0x00020B14
				private void CalculateThrustBaseDamage(out float damage, bool isThrown = false)
				{
					float num = CombatStatCalculator.CalculateStrikeMagnitudeForThrust(this._currentWeaponThrustSpeed, this._currentWeaponWeight, 0f, isThrown);
					damage = num * this._thrustDamageFactor;
				}

				// Token: 0x06000A81 RID: 2689 RVA: 0x00022944 File Offset: 0x00020B44
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
						Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "CalculateMissileDamage", 550);
						return;
					}
				}

				// Token: 0x06000A82 RID: 2690 RVA: 0x000229CC File Offset: 0x00020BCC
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
					Debug.FailedAssert("Couldn't calculate weapon tier", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "CalculateWeaponTier", 571);
					return WeaponComponentData.WeaponTiers.Tier1;
				}

				// Token: 0x06000A83 RID: 2691 RVA: 0x00022A8C File Offset: 0x00020C8C
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

				// Token: 0x06000A84 RID: 2692 RVA: 0x00022BA4 File Offset: 0x00020DA4
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
					Debug.FailedAssert("Weapon is not a missile.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Crafting.cs", "CalculateMissileSpeed", 622);
					return 10f;
				}

				// Token: 0x06000A85 RID: 2693 RVA: 0x00022C24 File Offset: 0x00020E24
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

				// Token: 0x06000A86 RID: 2694 RVA: 0x00022CB4 File Offset: 0x00020EB4
				private float GetWeaponBalance()
				{
					return MBMath.ClampFloat((this._currentWeaponSwingSpeed * 4.5454545f - 70f) / 30f, 0f, 1f);
				}

				// Token: 0x06000A87 RID: 2695 RVA: 0x00022CDD File Offset: 0x00020EDD
				private int GetWeaponHandArmorBonus()
				{
					WeaponDesignElement weaponDesignElement = this._craftedData.UsedPieces[1];
					if (weaponDesignElement == null)
					{
						return 0;
					}
					return weaponDesignElement.CraftingPiece.ArmorBonus;
				}

				// Token: 0x06000A88 RID: 2696 RVA: 0x00022CFC File Offset: 0x00020EFC
				private WeaponClass GetAmmoClass()
				{
					if (this._weaponDescription.WeaponClass != WeaponClass.ThrowingKnife && this._weaponDescription.WeaponClass != WeaponClass.ThrowingAxe && this._weaponDescription.WeaponClass != WeaponClass.Javelin)
					{
						return WeaponClass.Undefined;
					}
					return this._weaponDescription.WeaponClass;
				}

				// Token: 0x06000A89 RID: 2697 RVA: 0x00022D38 File Offset: 0x00020F38
				private static float ParallelAxis(WeaponDesignElement selectedPiece, float offset, float weightMultiplier)
				{
					float inertia = selectedPiece.CraftingPiece.Inertia;
					float num = offset + selectedPiece.CraftingPiece.CenterOfMass;
					float num2 = selectedPiece.ScaledWeight * weightMultiplier;
					return Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis(inertia, num2, num);
				}

				// Token: 0x06000A8A RID: 2698 RVA: 0x00022D6E File Offset: 0x00020F6E
				private static float ParallelAxis(float inertiaAroundCm, float mass, float offsetFromCm)
				{
					return inertiaAroundCm + mass * offsetFromCm * offsetFromCm;
				}

				// Token: 0x06000A8B RID: 2699 RVA: 0x00022D78 File Offset: 0x00020F78
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

				// Token: 0x06000A8C RID: 2700 RVA: 0x00022E34 File Offset: 0x00021034
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

				// Token: 0x04000727 RID: 1831
				private WeaponDesign _craftedData;

				// Token: 0x04000728 RID: 1832
				private WeaponDescription _weaponDescription;

				// Token: 0x04000729 RID: 1833
				private float _stoppingTorque;

				// Token: 0x0400072A RID: 1834
				private float _armInertia;

				// Token: 0x0400072B RID: 1835
				private float _swingDamageFactor;

				// Token: 0x0400072C RID: 1836
				private float _thrustDamageFactor;

				// Token: 0x0400072D RID: 1837
				private float _currentWeaponWeight;

				// Token: 0x0400072E RID: 1838
				private float _currentWeaponReach;

				// Token: 0x0400072F RID: 1839
				private float _currentWeaponSweetSpot;

				// Token: 0x04000730 RID: 1840
				private float _currentWeaponCenterOfMass;

				// Token: 0x04000731 RID: 1841
				private float _currentWeaponInertia;

				// Token: 0x04000732 RID: 1842
				private float _currentWeaponInertiaAroundShoulder;

				// Token: 0x04000733 RID: 1843
				private float _currentWeaponInertiaAroundGrip;

				// Token: 0x04000734 RID: 1844
				private float _currentWeaponSwingSpeed;

				// Token: 0x04000735 RID: 1845
				private float _currentWeaponThrustSpeed;

				// Token: 0x04000736 RID: 1846
				private float _currentWeaponHandling;

				// Token: 0x04000737 RID: 1847
				private float _currentWeaponSwingDamage;

				// Token: 0x04000738 RID: 1848
				private float _currentWeaponThrustDamage;

				// Token: 0x04000739 RID: 1849
				private WeaponComponentData.WeaponTiers _currentWeaponTier;
			}
		}
	}
}

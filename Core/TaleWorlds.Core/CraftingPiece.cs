﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	public sealed class CraftingPiece : MBObjectBase
	{
		internal static void AutoGeneratedStaticCollectObjectsCraftingPiece(object o, List<object> collectedObjects)
		{
			((CraftingPiece)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		public CraftingPiece()
		{
			this.InitializeLists();
		}

		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData)
		{
			this.InitializeLists();
		}

		private void InitializeLists()
		{
			this._materialCosts = new MBList<int>(9);
			for (int i = 0; i < 9; i++)
			{
				this._materialCosts.Add(0);
			}
			this._materialsUsed = new MBList<ValueTuple<CraftingMaterials, int>>(0);
		}

		public static CraftingPiece GetInvalidCraftingPiece(CraftingPiece.PieceTypes pieceType)
		{
			if (CraftingPiece._invalidCraftingPiece == null)
			{
				CraftingPiece._invalidCraftingPiece = new CraftingPiece[4];
			}
			if (CraftingPiece._invalidCraftingPiece[(int)pieceType] == null)
			{
				CraftingPiece._invalidCraftingPiece[(int)pieceType] = new CraftingPiece
				{
					PieceType = pieceType,
					Name = new TextObject("{=!}Invalid", null),
					IsValid = false
				};
			}
			return CraftingPiece._invalidCraftingPiece[(int)pieceType];
		}

		public bool IsValid { get; private set; }

		public TextObject Name { get; private set; }

		public CraftingPiece.PieceTypes PieceType { get; private set; }

		public string MeshName { get; private set; }

		public BasicCultureObject Culture { get; private set; }

		public float Length { get; private set; }

		public float DistanceToNextPiece { get; private set; }

		public float DistanceToPreviousPiece { get; private set; }

		public float PieceOffset { get; private set; }

		public float PreviousPieceOffset { get; private set; }

		public float NextPieceOffset { get; private set; }

		public float Weight { get; private set; }

		public float Inertia { get; private set; }

		public float CenterOfMass { get; private set; }

		public int ArmorBonus { get; private set; }

		public int SwingDamageBonus { get; private set; }

		public int SwingSpeedBonus { get; private set; }

		public int ThrustDamageBonus { get; private set; }

		public int ThrustSpeedBonus { get; private set; }

		public int HandlingBonus { get; private set; }

		public int AccuracyBonus { get; private set; }

		public int PieceTier { get; private set; }

		public bool FullScale { get; private set; }

		public Vec3 ItemHolsterPosShift { get; private set; }

		public float Appearance { get; private set; }

		public bool IsGivenByDefault { get; private set; }

		public bool IsHiddenOnDesigner { get; private set; }

		public bool IsUnique { get; private set; }

		public string ItemUsageFeaturesToExclude { get; private set; }

		public MBReadOnlyList<ValueTuple<CraftingMaterials, int>> MaterialsUsed
		{
			get
			{
				return this._materialsUsed;
			}
		}

		public bool IsEmptyPiece
		{
			get
			{
				return this._materialCosts.All((int cost) => cost == 0);
			}
		}

		public int CraftingCost { get; private set; }

		public int RequiredSkillValue { get; private set; }

		public BladeData BladeData { get; private set; }

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.IsValid = true;
			this.Name = new TextObject(node.Attributes["name"].InnerText, null);
			this.PieceType = (CraftingPiece.PieceTypes)Enum.Parse(typeof(CraftingPiece.PieceTypes), node.Attributes["piece_type"].InnerText, true);
			this.MeshName = node.Attributes["mesh"].InnerText;
			this.Culture = ((node.Attributes["mesh"] != null) ? ((BasicCultureObject)objectManager.ReadObjectReferenceFromXml("culture", typeof(BasicCultureObject), node)) : null);
			this.Appearance = ((node.Attributes["appearance"] != null) ? float.Parse(node.Attributes["appearance"].Value) : 0.5f);
			this.CraftingCost = ((node.Attributes["CraftingCost"] != null) ? int.Parse(node.Attributes["CraftingCost"].Value) : 0);
			XmlAttribute xmlAttribute = node.Attributes["weight"];
			this.Weight = ((xmlAttribute != null) ? float.Parse(xmlAttribute.Value) : 0f);
			XmlAttribute xmlAttribute2 = node.Attributes["length"];
			if (xmlAttribute2 != null)
			{
				this.Length = 0.01f * float.Parse(xmlAttribute2.Value);
				this.DistanceToNextPiece = this.Length / 2f;
				this.DistanceToPreviousPiece = this.Length / 2f;
			}
			else
			{
				XmlAttribute xmlAttribute3 = node.Attributes["distance_to_next_piece"];
				XmlAttribute xmlAttribute4 = node.Attributes["distance_to_previous_piece"];
				this.DistanceToNextPiece = 0.01f * float.Parse(xmlAttribute3.Value);
				this.DistanceToPreviousPiece = 0.01f * float.Parse(xmlAttribute4.Value);
				this.Length = this.DistanceToNextPiece + this.DistanceToPreviousPiece;
			}
			this.Inertia = 0.083333336f * this.Weight * this.Length * this.Length;
			XmlAttribute xmlAttribute5 = node.Attributes["center_of_mass"];
			float num = ((xmlAttribute5 != null) ? float.Parse(xmlAttribute5.Value) : 0.5f);
			this.CenterOfMass = this.Length * num;
			XmlAttribute xmlAttribute6 = node.Attributes["item_holster_pos_shift"];
			Vec3 vec = default(Vec3);
			if (xmlAttribute6 != null)
			{
				string[] array = xmlAttribute6.Value.Split(new char[] { ',' });
				if (array.Length == 3)
				{
					float.TryParse(array[0], out vec.x);
					float.TryParse(array[1], out vec.y);
					float.TryParse(array[2], out vec.z);
				}
			}
			this.ItemHolsterPosShift = vec;
			XmlAttribute xmlAttribute7 = node.Attributes["tier"];
			this.PieceTier = ((xmlAttribute7 != null) ? int.Parse(xmlAttribute7.Value) : 1);
			this.IsUnique = XmlHelper.ReadBool(node, "is_unique");
			this.IsGivenByDefault = XmlHelper.ReadBool(node, "is_default");
			this.IsHiddenOnDesigner = XmlHelper.ReadBool(node, "is_hidden");
			XmlAttribute xmlAttribute8 = node.Attributes["full_scale"];
			this.FullScale = ((xmlAttribute8 != null) ? (xmlAttribute8.InnerText == "true") : (this.PieceType == CraftingPiece.PieceTypes.Guard || this.PieceType == CraftingPiece.PieceTypes.Pommel));
			XmlAttribute xmlAttribute9 = node.Attributes["excluded_item_usage_features"];
			this.ItemUsageFeaturesToExclude = ((xmlAttribute9 != null) ? xmlAttribute9.InnerText : "");
			XmlAttribute xmlAttribute10 = node.Attributes["required_skill_value"];
			this.RequiredSkillValue = ((xmlAttribute10 != null) ? int.Parse(xmlAttribute10.Value) : 0);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Attributes != null)
				{
					string name = xmlNode.Name;
					if (!(name == "StatContributions"))
					{
						if (!(name == "BladeData"))
						{
							if (!(name == "BuildData"))
							{
								if (!(name == "Materials"))
								{
									if (name == "Flags")
									{
										this.AdditionalItemFlags = (ItemFlags)0U;
										this.AdditionalWeaponFlags = (WeaponFlags)0UL;
										foreach (object obj2 in xmlNode.ChildNodes)
										{
											XmlNode xmlNode2 = (XmlNode)obj2;
											XmlAttribute xmlAttribute11 = xmlNode2.Attributes["name"];
											XmlAttribute xmlAttribute12 = xmlNode2.Attributes["type"];
											if (xmlAttribute12 == null || xmlAttribute12.Value == "WeaponFlags")
											{
												WeaponFlags weaponFlags = (WeaponFlags)Enum.Parse(typeof(WeaponFlags), xmlAttribute11.Value, true);
												this.AdditionalWeaponFlags |= weaponFlags;
											}
											else
											{
												ItemFlags itemFlags = (ItemFlags)Enum.Parse(typeof(ItemFlags), xmlAttribute11.Value, true);
												this.AdditionalItemFlags |= itemFlags;
											}
										}
									}
								}
								else
								{
									this._materialsUsed = new MBList<ValueTuple<CraftingMaterials, int>>();
									foreach (object obj3 in xmlNode.ChildNodes)
									{
										XmlNode xmlNode3 = (XmlNode)obj3;
										string value = xmlNode3.Attributes["id"].Value;
										string value2 = xmlNode3.Attributes["count"].Value;
										CraftingMaterials craftingMaterials;
										Enum.TryParse<CraftingMaterials>(value, out craftingMaterials);
										int num2;
										if (int.TryParse(value2, out num2) && num2 > 0)
										{
											this._materialsUsed.Add(new ValueTuple<CraftingMaterials, int>(craftingMaterials, num2));
										}
										this._materialCosts[(int)craftingMaterials] = num2;
									}
									this._materialsUsed.Capacity = this._materialsUsed.Count;
								}
							}
							else
							{
								XmlAttribute xmlAttribute13 = xmlNode.Attributes["piece_offset"];
								XmlAttribute xmlAttribute14 = xmlNode.Attributes["previous_piece_offset"];
								XmlAttribute xmlAttribute15 = xmlNode.Attributes["next_piece_offset"];
								this.PieceOffset = ((xmlAttribute13 != null) ? (0.01f * float.Parse(xmlAttribute13.Value)) : 0f);
								this.PreviousPieceOffset = ((xmlAttribute14 != null) ? (0.01f * float.Parse(xmlAttribute14.Value)) : 0f);
								this.NextPieceOffset = ((xmlAttribute15 != null) ? (0.01f * float.Parse(xmlAttribute15.Value)) : 0f);
							}
						}
						else
						{
							this.BladeData = new BladeData(this.PieceType, this.Length);
							this.BladeData.Deserialize(objectManager, xmlNode);
						}
					}
					else
					{
						XmlAttribute xmlAttribute16 = xmlNode.Attributes["armor_bonus"];
						this.ArmorBonus = ((xmlAttribute16 != null) ? int.Parse(xmlAttribute16.Value) : 0);
						XmlAttribute xmlAttribute17 = xmlNode.Attributes["handling_bonus"];
						this.HandlingBonus = ((xmlAttribute17 != null) ? int.Parse(xmlAttribute17.Value) : 0);
						XmlAttribute xmlAttribute18 = xmlNode.Attributes["swing_damage_bonus"];
						this.SwingDamageBonus = ((xmlAttribute18 != null) ? int.Parse(xmlAttribute18.Value) : 0);
						XmlAttribute xmlAttribute19 = xmlNode.Attributes["swing_speed_bonus"];
						this.SwingSpeedBonus = ((xmlAttribute19 != null) ? int.Parse(xmlAttribute19.Value) : 0);
						XmlAttribute xmlAttribute20 = xmlNode.Attributes["thrust_damage_bonus"];
						this.ThrustDamageBonus = ((xmlAttribute20 != null) ? int.Parse(xmlAttribute20.Value) : 0);
						XmlAttribute xmlAttribute21 = xmlNode.Attributes["thrust_speed_bonus"];
						this.ThrustSpeedBonus = ((xmlAttribute21 != null) ? int.Parse(xmlAttribute21.Value) : 0);
						XmlAttribute xmlAttribute22 = xmlNode.Attributes["accuracy_bonus"];
						this.AccuracyBonus = ((xmlAttribute22 != null) ? int.Parse(xmlAttribute22.Value) : 0);
					}
				}
			}
			CraftingPiece.PieceTypes pieceType = this.PieceType;
		}

		public static MBReadOnlyList<CraftingPiece> All
		{
			get
			{
				return Game.Current.ObjectManager.GetObjectTypeList<CraftingPiece>();
			}
		}

		private static CraftingPiece[] _invalidCraftingPiece;

		public WeaponFlags AdditionalWeaponFlags;

		public ItemFlags AdditionalItemFlags;

		private MBList<int> _materialCosts;

		private MBList<ValueTuple<CraftingMaterials, int>> _materialsUsed;

		public enum PieceTypes
		{
			Invalid = -1,
			Blade,
			Guard,
			Handle,
			Pommel,
			NumberOfPieceTypes
		}
	}
}
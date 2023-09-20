using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class BasicCharacterObject : MBObjectBase
	{
		public virtual TextObject Name
		{
			get
			{
				return this._basicName;
			}
		}

		private void SetName(TextObject name)
		{
			this._basicName = name;
		}

		public override TextObject GetName()
		{
			return this.Name;
		}

		public override string ToString()
		{
			return this.Name.ToString();
		}

		public virtual MBBodyProperty BodyPropertyRange { get; protected set; }

		public int DefaultFormationGroup { get; set; }

		public FormationClass DefaultFormationClass { get; protected set; }

		public FormationPositionPreference FormationPositionPreference { get; protected set; }

		public bool IsInfantry
		{
			get
			{
				return !this.IsRanged && !this.IsMounted;
			}
		}

		public virtual bool IsMounted
		{
			get
			{
				return this._isMounted;
			}
		}

		public virtual bool IsRanged
		{
			get
			{
				return this._isRanged;
			}
		}

		public int Race { get; set; }

		public virtual bool IsFemale { get; set; }

		public bool FaceMeshCache { get; private set; }

		public virtual MBReadOnlyList<Equipment> AllEquipments
		{
			get
			{
				if (this._equipmentRoster == null)
				{
					return new MBList<Equipment> { MBEquipmentRoster.EmptyEquipment };
				}
				return this._equipmentRoster.AllEquipments;
			}
		}

		public virtual Equipment Equipment
		{
			get
			{
				if (this._equipmentRoster == null)
				{
					return MBEquipmentRoster.EmptyEquipment;
				}
				return this._equipmentRoster.DefaultEquipment;
			}
		}

		public bool IsObsolete { get; private set; }

		private bool HasCivilianEquipment()
		{
			return this.AllEquipments.Any((Equipment eq) => eq.IsCivilian);
		}

		public void InitializeEquipmentsOnLoad(BasicCharacterObject character)
		{
			this._equipmentRoster = character._equipmentRoster;
		}

		public Equipment GetFirstEquipment(bool civilianSet)
		{
			if (!civilianSet)
			{
				return this.Equipment;
			}
			if (!this.HasCivilianEquipment())
			{
				return this.Equipment;
			}
			return this.AllEquipments.FirstOrDefault((Equipment eq) => eq.IsCivilian);
		}

		public virtual int Level { get; set; }

		public BasicCultureObject Culture
		{
			get
			{
				return this._culture;
			}
			set
			{
				this._culture = value;
			}
		}

		public virtual bool IsPlayerCharacter
		{
			get
			{
				return false;
			}
		}

		public virtual float Age
		{
			get
			{
				return this._age;
			}
			set
			{
				this._age = value;
			}
		}

		public virtual int HitPoints
		{
			get
			{
				return this.MaxHitPoints();
			}
		}

		public virtual BodyProperties GetBodyPropertiesMin(bool returnBaseValue = false)
		{
			return this.BodyPropertyRange.BodyPropertyMin;
		}

		protected void FillFrom(BasicCharacterObject character)
		{
			this._culture = character._culture;
			this.DefaultFormationClass = character.DefaultFormationClass;
			this.DefaultFormationGroup = character.DefaultFormationGroup;
			this.BodyPropertyRange = character.BodyPropertyRange;
			this.FormationPositionPreference = character.FormationPositionPreference;
			this.IsFemale = character.IsFemale;
			this.Race = character.Race;
			this.Level = character.Level;
			this._basicName = character._basicName;
			this._age = character._age;
			this.DefaultCharacterSkills = character.DefaultCharacterSkills;
			this.HairTags = character.HairTags;
			this.BeardTags = character.BeardTags;
			this.InitializeEquipmentsOnLoad(character);
		}

		public virtual BodyProperties GetBodyPropertiesMax()
		{
			return this.BodyPropertyRange.BodyPropertyMax;
		}

		public virtual BodyProperties GetBodyProperties(Equipment equipment, int seed = -1)
		{
			BodyProperties bodyPropertiesMin = this.GetBodyPropertiesMin(false);
			BodyProperties bodyPropertiesMax = this.GetBodyPropertiesMax();
			return FaceGen.GetRandomBodyProperties(this.Race, this.IsFemale, bodyPropertiesMin, bodyPropertiesMax, (int)((equipment != null) ? equipment.HairCoverType : ArmorComponent.HairCoverTypes.None), seed, this.HairTags, this.BeardTags, this.TattooTags);
		}

		public virtual void UpdatePlayerCharacterBodyProperties(BodyProperties properties, int race, bool isFemale)
		{
			this.BodyPropertyRange.Init(properties, properties);
			this.Race = race;
			this.IsFemale = isFemale;
		}

		public float FaceDirtAmount { get; set; }

		public virtual string HairTags { get; set; } = "";

		public virtual string BeardTags { get; set; } = "";

		public virtual string TattooTags { get; set; } = "";

		public virtual bool IsHero
		{
			get
			{
				return this._isBasicHero;
			}
		}

		public bool IsSoldier { get; private set; }

		public BasicCharacterObject()
		{
			this.DefaultFormationClass = FormationClass.Infantry;
		}

		public int GetDefaultFaceSeed(int rank)
		{
			int num = base.StringId.GetDeterministicHashCode() * 6791 + rank * 197;
			return ((num >= 0) ? num : (-num)) % 2000;
		}

		public float GetStepSize()
		{
			return Math.Min(0.8f + 0.2f * (float)this.GetSkillValue(DefaultSkills.Athletics) * 0.00333333f, 1f);
		}

		public bool HasMount()
		{
			return this.Equipment[10].Item != null;
		}

		public virtual int MaxHitPoints()
		{
			return FaceGen.GetBaseMonsterFromRace(this.Race).HitPoints;
		}

		public virtual float GetPower()
		{
			int num = this.Level + 10;
			return 0.2f + (float)(num * num) * 0.0025f;
		}

		public virtual float GetBattlePower()
		{
			return 1f;
		}

		public virtual float GetMoraleResistance()
		{
			return 1f;
		}

		public virtual int GetMountKeySeed()
		{
			return MBRandom.RandomInt();
		}

		public virtual int GetBattleTier()
		{
			if (this.IsHero)
			{
				return 7;
			}
			return MathF.Min(MathF.Max(MathF.Ceiling(((float)this.Level - 5f) / 5f), 0), 7);
		}

		public virtual int GetSkillValue(SkillObject skill)
		{
			return this.DefaultCharacterSkills.Skills.GetPropertyValue(skill);
		}

		protected void InitializeHeroBasicCharacterOnAfterLoad(BasicCharacterObject originCharacter)
		{
			this.IsSoldier = originCharacter.IsSoldier;
			this._isBasicHero = originCharacter._isBasicHero;
			this.DefaultCharacterSkills = originCharacter.DefaultCharacterSkills;
			this.HairTags = originCharacter.HairTags;
			this.BeardTags = originCharacter.BeardTags;
			this.TattooTags = originCharacter.TattooTags;
			this.BodyPropertyRange = originCharacter.BodyPropertyRange;
			this.IsFemale = originCharacter.IsFemale;
			this.Race = originCharacter.Race;
			this.Culture = originCharacter.Culture;
			this.DefaultFormationGroup = originCharacter.DefaultFormationGroup;
			this.DefaultFormationClass = originCharacter.DefaultFormationClass;
			this.FormationPositionPreference = originCharacter.FormationPositionPreference;
			this._equipmentRoster = originCharacter._equipmentRoster;
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			XmlAttribute xmlAttribute = node.Attributes["name"];
			if (xmlAttribute != null)
			{
				this.SetName(new TextObject(xmlAttribute.Value, null));
			}
			this.HairTags = "";
			this.BeardTags = "";
			this.TattooTags = "";
			this.Race = 0;
			XmlAttribute xmlAttribute2 = node.Attributes["race"];
			if (xmlAttribute2 != null)
			{
				this.Race = FaceGen.GetRaceOrDefault(xmlAttribute2.Value);
			}
			XmlNode xmlNode = node.Attributes["occupation"];
			if (xmlNode != null)
			{
				this.IsSoldier = xmlNode.InnerText.IndexOf("soldier", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			this._isBasicHero = XmlHelper.ReadBool(node, "is_hero");
			this.FaceMeshCache = XmlHelper.ReadBool(node, "face_mesh_cache");
			this.IsObsolete = XmlHelper.ReadBool(node, "is_obsolete");
			MBCharacterSkills mbcharacterSkills = objectManager.ReadObjectReferenceFromXml("skill_template", typeof(MBCharacterSkills), node) as MBCharacterSkills;
			if (mbcharacterSkills != null)
			{
				this.DefaultCharacterSkills = mbcharacterSkills;
			}
			else
			{
				this.DefaultCharacterSkills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(base.StringId);
			}
			BodyProperties bodyProperties = default(BodyProperties);
			BodyProperties bodyProperties2 = default(BodyProperties);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				if (xmlNode2.Name == "Skills" || xmlNode2.Name == "skills")
				{
					if (mbcharacterSkills == null)
					{
						this.DefaultCharacterSkills.Init(objectManager, xmlNode2);
					}
				}
				else if (xmlNode2.Name == "Equipments" || xmlNode2.Name == "equipments")
				{
					List<XmlNode> list = new List<XmlNode>();
					foreach (object obj2 in xmlNode2.ChildNodes)
					{
						XmlNode xmlNode3 = (XmlNode)obj2;
						if (xmlNode3.Name == "equipment")
						{
							list.Add(xmlNode3);
						}
					}
					foreach (object obj3 in xmlNode2.ChildNodes)
					{
						XmlNode xmlNode4 = (XmlNode)obj3;
						if (xmlNode4.Name == "EquipmentRoster" || xmlNode4.Name == "equipmentRoster")
						{
							if (this._equipmentRoster == null)
							{
								this._equipmentRoster = MBObjectManager.Instance.CreateObject<MBEquipmentRoster>(base.StringId);
							}
							this._equipmentRoster.Init(objectManager, xmlNode4);
						}
						else if (xmlNode4.Name == "EquipmentSet" || xmlNode4.Name == "equipmentSet")
						{
							string innerText = xmlNode4.Attributes["id"].InnerText;
							bool flag = xmlNode4.Attributes["civilian"] != null && bool.Parse(xmlNode4.Attributes["civilian"].InnerText);
							if (this._equipmentRoster == null)
							{
								this._equipmentRoster = MBObjectManager.Instance.CreateObject<MBEquipmentRoster>(base.StringId);
							}
							this._equipmentRoster.AddEquipmentRoster(MBObjectManager.Instance.GetObject<MBEquipmentRoster>(innerText), flag);
						}
					}
					if (list.Count > 0)
					{
						this._equipmentRoster.AddOverridenEquipments(objectManager, list);
					}
				}
				else if (xmlNode2.Name == "face")
				{
					foreach (object obj4 in xmlNode2.ChildNodes)
					{
						XmlNode xmlNode5 = (XmlNode)obj4;
						if (xmlNode5.Name == "hair_tags")
						{
							using (IEnumerator enumerator3 = xmlNode5.ChildNodes.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									object obj5 = enumerator3.Current;
									XmlNode xmlNode6 = (XmlNode)obj5;
									this.HairTags = this.HairTags + xmlNode6.Attributes["name"].Value + ",";
								}
								continue;
							}
						}
						if (xmlNode5.Name == "beard_tags")
						{
							using (IEnumerator enumerator3 = xmlNode5.ChildNodes.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									object obj6 = enumerator3.Current;
									XmlNode xmlNode7 = (XmlNode)obj6;
									this.BeardTags = this.BeardTags + xmlNode7.Attributes["name"].Value + ",";
								}
								continue;
							}
						}
						if (xmlNode5.Name == "tattoo_tags")
						{
							using (IEnumerator enumerator3 = xmlNode5.ChildNodes.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									object obj7 = enumerator3.Current;
									XmlNode xmlNode8 = (XmlNode)obj7;
									this.TattooTags = this.TattooTags + xmlNode8.Attributes["name"].Value + ",";
								}
								continue;
							}
						}
						if (xmlNode5.Name == "BodyProperties")
						{
							if (!BodyProperties.FromXmlNode(xmlNode5, out bodyProperties))
							{
								Debug.FailedAssert("cannot read body properties", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\BasicCharacterObject.cs", "Deserialize", 413);
							}
						}
						else if (xmlNode5.Name == "BodyPropertiesMax")
						{
							if (!BodyProperties.FromXmlNode(xmlNode5, out bodyProperties2))
							{
								bodyProperties = bodyProperties2;
								Debug.FailedAssert("cannot read max body properties", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\BasicCharacterObject.cs", "Deserialize", 422);
							}
						}
						else if (xmlNode5.Name == "face_key_template")
						{
							MBBodyProperty mbbodyProperty = objectManager.ReadObjectReferenceFromXml<MBBodyProperty>("value", xmlNode5);
							this.BodyPropertyRange = mbbodyProperty;
						}
					}
				}
			}
			if (this.BodyPropertyRange == null)
			{
				this.BodyPropertyRange = MBObjectManager.Instance.RegisterPresumedObject<MBBodyProperty>(new MBBodyProperty(base.StringId));
				this.BodyPropertyRange.Init(bodyProperties, bodyProperties2);
			}
			this.IsFemale = false;
			this.DefaultFormationGroup = 0;
			XmlNode xmlNode9 = node.Attributes["is_female"];
			if (xmlNode9 != null)
			{
				this.IsFemale = Convert.ToBoolean(xmlNode9.InnerText);
			}
			this.Culture = objectManager.ReadObjectReferenceFromXml<BasicCultureObject>("culture", node);
			XmlNode xmlNode10 = node.Attributes["age"];
			this.Age = ((xmlNode10 == null) ? MathF.Max(20f, this.BodyPropertyRange.BodyPropertyMax.Age) : ((float)Convert.ToInt32(xmlNode10.InnerText)));
			XmlNode xmlNode11 = node.Attributes["level"];
			this.Level = ((xmlNode11 != null) ? Convert.ToInt32(xmlNode11.InnerText) : 1);
			XmlNode xmlNode12 = node.Attributes["default_group"];
			if (xmlNode12 != null)
			{
				this.DefaultFormationGroup = this.FetchDefaultFormationGroup(xmlNode12.InnerText);
			}
			this.DefaultFormationClass = (FormationClass)this.DefaultFormationGroup;
			this._isRanged = this.DefaultFormationClass.IsRanged();
			this._isMounted = this.DefaultFormationClass.IsMounted();
			XmlNode xmlNode13 = node.Attributes["formation_position_preference"];
			this.FormationPositionPreference = ((xmlNode13 != null) ? ((FormationPositionPreference)Enum.Parse(typeof(FormationPositionPreference), xmlNode13.InnerText)) : FormationPositionPreference.Middle);
			XmlNode xmlNode14 = node.Attributes["default_equipment_set"];
			if (xmlNode14 != null)
			{
				this._equipmentRoster.InitializeDefaultEquipment(xmlNode14.Value);
			}
			MBEquipmentRoster equipmentRoster = this._equipmentRoster;
			if (equipmentRoster == null)
			{
				return;
			}
			equipmentRoster.OrderEquipments();
		}

		protected int FetchDefaultFormationGroup(string innerText)
		{
			FormationClass formationClass;
			if (Enum.TryParse<FormationClass>(innerText, true, out formationClass))
			{
				return (int)formationClass;
			}
			return -1;
		}

		public virtual FormationClass GetFormationClass()
		{
			return this.DefaultFormationClass;
		}

		internal static void AutoGeneratedStaticCollectObjectsBasicCharacterObject(object o, List<object> collectedObjects)
		{
			((BasicCharacterObject)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected TextObject _basicName;

		private bool _isMounted;

		private bool _isRanged;

		private MBEquipmentRoster _equipmentRoster;

		private BasicCultureObject _culture;

		[CachedData]
		private float _age;

		[CachedData]
		private bool _isBasicHero;

		protected MBCharacterSkills DefaultCharacterSkills;
	}
}

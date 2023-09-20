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
	// Token: 0x02000017 RID: 23
	public class BasicCharacterObject : MBObjectBase
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000100 RID: 256 RVA: 0x0000509A File Offset: 0x0000329A
		public virtual TextObject Name
		{
			get
			{
				return this._basicName;
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000050A2 File Offset: 0x000032A2
		private void SetName(TextObject name)
		{
			this._basicName = name;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000050AB File Offset: 0x000032AB
		public override TextObject GetName()
		{
			return this.Name;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000050B3 File Offset: 0x000032B3
		public override string ToString()
		{
			return this.Name.ToString();
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000104 RID: 260 RVA: 0x000050C0 File Offset: 0x000032C0
		// (set) Token: 0x06000105 RID: 261 RVA: 0x000050C8 File Offset: 0x000032C8
		public virtual MBBodyProperty BodyPropertyRange { get; protected set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000106 RID: 262 RVA: 0x000050D1 File Offset: 0x000032D1
		// (set) Token: 0x06000107 RID: 263 RVA: 0x000050D9 File Offset: 0x000032D9
		public int DefaultFormationGroup { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000108 RID: 264 RVA: 0x000050E2 File Offset: 0x000032E2
		// (set) Token: 0x06000109 RID: 265 RVA: 0x000050EA File Offset: 0x000032EA
		public FormationClass DefaultFormationClass { get; protected set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600010A RID: 266 RVA: 0x000050F3 File Offset: 0x000032F3
		// (set) Token: 0x0600010B RID: 267 RVA: 0x000050FB File Offset: 0x000032FB
		public FormationPositionPreference FormationPositionPreference { get; protected set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00005104 File Offset: 0x00003304
		public bool IsInfantry
		{
			get
			{
				return !this.IsRanged && !this.IsMounted;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00005119 File Offset: 0x00003319
		public virtual bool IsMounted
		{
			get
			{
				return this._isMounted;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00005121 File Offset: 0x00003321
		public virtual bool IsRanged
		{
			get
			{
				return this._isRanged;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00005129 File Offset: 0x00003329
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00005131 File Offset: 0x00003331
		public int Race { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000111 RID: 273 RVA: 0x0000513A File Offset: 0x0000333A
		// (set) Token: 0x06000112 RID: 274 RVA: 0x00005142 File Offset: 0x00003342
		public virtual bool IsFemale { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000113 RID: 275 RVA: 0x0000514B File Offset: 0x0000334B
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00005153 File Offset: 0x00003353
		public bool FaceMeshCache { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000115 RID: 277 RVA: 0x0000515C File Offset: 0x0000335C
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

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00005182 File Offset: 0x00003382
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

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000117 RID: 279 RVA: 0x0000519D File Offset: 0x0000339D
		// (set) Token: 0x06000118 RID: 280 RVA: 0x000051A5 File Offset: 0x000033A5
		public bool IsObsolete { get; private set; }

		// Token: 0x06000119 RID: 281 RVA: 0x000051AE File Offset: 0x000033AE
		private bool HasCivilianEquipment()
		{
			return this.AllEquipments.Any((Equipment eq) => eq.IsCivilian);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000051DA File Offset: 0x000033DA
		public void InitializeEquipmentsOnLoad(BasicCharacterObject character)
		{
			this._equipmentRoster = character._equipmentRoster;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000051E8 File Offset: 0x000033E8
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

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00005238 File Offset: 0x00003438
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00005240 File Offset: 0x00003440
		public virtual int Level { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00005249 File Offset: 0x00003449
		// (set) Token: 0x0600011F RID: 287 RVA: 0x00005251 File Offset: 0x00003451
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

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000525A File Offset: 0x0000345A
		public virtual bool IsPlayerCharacter
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000121 RID: 289 RVA: 0x0000525D File Offset: 0x0000345D
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00005265 File Offset: 0x00003465
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

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000123 RID: 291 RVA: 0x0000526E File Offset: 0x0000346E
		public virtual int HitPoints
		{
			get
			{
				return this.MaxHitPoints();
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00005276 File Offset: 0x00003476
		public virtual BodyProperties GetBodyPropertiesMin(bool returnBaseValue = false)
		{
			return this.BodyPropertyRange.BodyPropertyMin;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00005283 File Offset: 0x00003483
		public virtual BodyProperties GetBodyPropertiesMax()
		{
			return this.BodyPropertyRange.BodyPropertyMax;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00005290 File Offset: 0x00003490
		public virtual BodyProperties GetBodyProperties(Equipment equipment, int seed = -1)
		{
			BodyProperties bodyPropertiesMin = this.GetBodyPropertiesMin(false);
			BodyProperties bodyPropertiesMax = this.GetBodyPropertiesMax();
			return FaceGen.GetRandomBodyProperties(this.Race, this.IsFemale, bodyPropertiesMin, bodyPropertiesMax, (int)((equipment != null) ? equipment.HairCoverType : ArmorComponent.HairCoverTypes.None), seed, this.HairTags, this.BeardTags, this.TattooTags);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x000052DE File Offset: 0x000034DE
		public virtual void UpdatePlayerCharacterBodyProperties(BodyProperties properties, int race, bool isFemale)
		{
			this.BodyPropertyRange.Init(properties, properties);
			this.Race = race;
			this.IsFemale = isFemale;
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000128 RID: 296 RVA: 0x000052FB File Offset: 0x000034FB
		// (set) Token: 0x06000129 RID: 297 RVA: 0x00005303 File Offset: 0x00003503
		public float FaceDirtAmount { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600012A RID: 298 RVA: 0x0000530C File Offset: 0x0000350C
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00005314 File Offset: 0x00003514
		public virtual string HairTags { get; set; } = "";

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600012C RID: 300 RVA: 0x0000531D File Offset: 0x0000351D
		// (set) Token: 0x0600012D RID: 301 RVA: 0x00005325 File Offset: 0x00003525
		public virtual string BeardTags { get; set; } = "";

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600012E RID: 302 RVA: 0x0000532E File Offset: 0x0000352E
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00005336 File Offset: 0x00003536
		public virtual string TattooTags { get; set; } = "";

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000130 RID: 304 RVA: 0x0000533F File Offset: 0x0000353F
		public virtual bool IsHero
		{
			get
			{
				return this._isBasicHero;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00005347 File Offset: 0x00003547
		// (set) Token: 0x06000132 RID: 306 RVA: 0x0000534F File Offset: 0x0000354F
		public bool IsSoldier { get; private set; }

		// Token: 0x06000133 RID: 307 RVA: 0x00005358 File Offset: 0x00003558
		public BasicCharacterObject()
		{
			this.DefaultFormationClass = FormationClass.Infantry;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00005388 File Offset: 0x00003588
		public int GetDefaultFaceSeed(int rank)
		{
			int num = base.StringId.GetDeterministicHashCode() * 6791 + rank * 197;
			return ((num >= 0) ? num : (-num)) % 2000;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000053BE File Offset: 0x000035BE
		public float GetStepSize()
		{
			return Math.Min(0.8f + 0.2f * (float)this.GetSkillValue(DefaultSkills.Athletics) * 0.00333333f, 1f);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x000053E8 File Offset: 0x000035E8
		public bool HasMount()
		{
			return this.Equipment[10].Item != null;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000540D File Offset: 0x0000360D
		public virtual int MaxHitPoints()
		{
			return FaceGen.GetBaseMonsterFromRace(this.Race).HitPoints;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00005420 File Offset: 0x00003620
		public virtual float GetPower()
		{
			int num = this.Level + 10;
			return 0.2f + (float)(num * num) * 0.0025f;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00005447 File Offset: 0x00003647
		public virtual float GetBattlePower()
		{
			return 1f;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000544E File Offset: 0x0000364E
		public virtual float GetMoraleResistance()
		{
			return 1f;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00005455 File Offset: 0x00003655
		public virtual int GetMountKeySeed()
		{
			return MBRandom.RandomInt();
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000545C File Offset: 0x0000365C
		public virtual int GetBattleTier()
		{
			if (this.IsHero)
			{
				return 7;
			}
			return MathF.Min(MathF.Max(MathF.Ceiling(((float)this.Level - 5f) / 5f), 0), 7);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000548C File Offset: 0x0000368C
		public virtual int GetSkillValue(SkillObject skill)
		{
			return this.CharacterSkills.Skills.GetPropertyValue(skill);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000054A0 File Offset: 0x000036A0
		protected void InitializeHeroBasicCharacterOnAfterLoad(BasicCharacterObject originCharacter)
		{
			this.IsSoldier = originCharacter.IsSoldier;
			this._isBasicHero = originCharacter._isBasicHero;
			this.CharacterSkills = originCharacter.CharacterSkills;
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

		// Token: 0x0600013F RID: 319 RVA: 0x00005558 File Offset: 0x00003758
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
				this.CharacterSkills = mbcharacterSkills;
			}
			else
			{
				this.CharacterSkills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(base.StringId);
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
						this.CharacterSkills.Init(objectManager, xmlNode2);
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
								Debug.FailedAssert("cannot read body properties", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\BasicCharacterObject.cs", "Deserialize", 385);
							}
						}
						else if (xmlNode5.Name == "BodyPropertiesMax")
						{
							if (!BodyProperties.FromXmlNode(xmlNode5, out bodyProperties2))
							{
								bodyProperties = bodyProperties2;
								Debug.FailedAssert("cannot read max body properties", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\BasicCharacterObject.cs", "Deserialize", 394);
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
			XmlNode xmlNode13 = node.Attributes["formation_position_preference"];
			this.FormationPositionPreference = ((xmlNode13 != null) ? ((FormationPositionPreference)Enum.Parse(typeof(FormationPositionPreference), xmlNode13.InnerText)) : FormationPositionPreference.Middle);
			XmlNode xmlNode14 = node.Attributes["default_equipment_set"];
			if (xmlNode14 != null)
			{
				this._equipmentRoster.InitializeDefaultEquipment(xmlNode14.Value);
			}
			MBEquipmentRoster equipmentRoster = this._equipmentRoster;
			if (equipmentRoster != null)
			{
				equipmentRoster.OrderEquipments();
			}
			this._isRanged = this.DefaultFormationClass == FormationClass.HorseArcher || this.DefaultFormationClass == FormationClass.Ranged;
			this._isMounted = this.DefaultFormationClass == FormationClass.Cavalry || this.DefaultFormationClass == FormationClass.HorseArcher;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00005DFC File Offset: 0x00003FFC
		protected int FetchDefaultFormationGroup(string innerText)
		{
			FormationClass formationClass;
			if (Enum.TryParse<FormationClass>(innerText, true, out formationClass))
			{
				return (int)formationClass;
			}
			return -1;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00005E17 File Offset: 0x00004017
		public virtual FormationClass GetFormationClass()
		{
			return this.DefaultFormationClass;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00005E1F File Offset: 0x0000401F
		internal static void AutoGeneratedStaticCollectObjectsBasicCharacterObject(object o, List<object> collectedObjects)
		{
			((BasicCharacterObject)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00005E2D File Offset: 0x0000402D
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0400010E RID: 270
		protected TextObject _basicName;

		// Token: 0x04000112 RID: 274
		private bool _isMounted;

		// Token: 0x04000113 RID: 275
		private bool _isRanged;

		// Token: 0x04000118 RID: 280
		private MBEquipmentRoster _equipmentRoster;

		// Token: 0x0400011B RID: 283
		private BasicCultureObject _culture;

		// Token: 0x0400011C RID: 284
		[CachedData]
		private float _age;

		// Token: 0x04000121 RID: 289
		[CachedData]
		private bool _isBasicHero;

		// Token: 0x04000123 RID: 291
		protected MBCharacterSkills CharacterSkills;
	}
}

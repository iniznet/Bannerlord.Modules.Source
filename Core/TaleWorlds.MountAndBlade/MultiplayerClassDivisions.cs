using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000308 RID: 776
	public class MultiplayerClassDivisions
	{
		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06002A07 RID: 10759 RVA: 0x000A2AEF File Offset: 0x000A0CEF
		// (set) Token: 0x06002A08 RID: 10760 RVA: 0x000A2AF6 File Offset: 0x000A0CF6
		public static List<MultiplayerClassDivisions.MPHeroClassGroup> MultiplayerHeroClassGroups { get; private set; }

		// Token: 0x06002A09 RID: 10761 RVA: 0x000A2B00 File Offset: 0x000A0D00
		public static IEnumerable<MultiplayerClassDivisions.MPHeroClass> GetMPHeroClasses(BasicCultureObject culture)
		{
			return from x in MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>()
				where x.Culture == culture
				select x;
		}

		// Token: 0x06002A0A RID: 10762 RVA: 0x000A2B35 File Offset: 0x000A0D35
		public static MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> GetMPHeroClasses()
		{
			return MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>();
		}

		// Token: 0x06002A0B RID: 10763 RVA: 0x000A2B44 File Offset: 0x000A0D44
		public static MultiplayerClassDivisions.MPHeroClass GetMPHeroClassForCharacter(BasicCharacterObject character)
		{
			return MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>().FirstOrDefault((MultiplayerClassDivisions.MPHeroClass x) => x.HeroCharacter == character || x.TroopCharacter == character);
		}

		// Token: 0x06002A0C RID: 10764 RVA: 0x000A2B7C File Offset: 0x000A0D7C
		public static List<List<IReadOnlyPerkObject>> GetAllPerksForHeroClass(MultiplayerClassDivisions.MPHeroClass heroClass, string forcedForGameMode = null)
		{
			List<List<IReadOnlyPerkObject>> list = new List<List<IReadOnlyPerkObject>>();
			for (int i = 0; i < 3; i++)
			{
				list.Add(heroClass.GetAllAvailablePerksForListIndex(i, forcedForGameMode).ToList<IReadOnlyPerkObject>());
			}
			return list;
		}

		// Token: 0x06002A0D RID: 10765 RVA: 0x000A2BB0 File Offset: 0x000A0DB0
		public static MultiplayerClassDivisions.MPHeroClass GetMPHeroClassForPeer(MissionPeer peer, bool skipTeamCheck = false)
		{
			Team team = peer.Team;
			if ((!skipTeamCheck && (team == null || team.Side == BattleSideEnum.None)) || (peer.SelectedTroopIndex < 0 && peer.ControlledAgent == null))
			{
				return null;
			}
			if (peer.ControlledAgent != null)
			{
				return MultiplayerClassDivisions.GetMPHeroClassForCharacter(peer.ControlledAgent.Character);
			}
			if (peer.SelectedTroopIndex >= 0)
			{
				return MultiplayerClassDivisions.GetMPHeroClasses(peer.Culture).ToList<MultiplayerClassDivisions.MPHeroClass>()[peer.SelectedTroopIndex];
			}
			Debug.FailedAssert("This should not be seen.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\MultiplayerClassDivisions.cs", "GetMPHeroClassForPeer", 255);
			return null;
		}

		// Token: 0x06002A0E RID: 10766 RVA: 0x000A2C40 File Offset: 0x000A0E40
		public static TargetIconType GetMPHeroClassForFormation(Formation formation)
		{
			switch (formation.PrimaryClass)
			{
			case FormationClass.Infantry:
				return TargetIconType.Infantry_Light;
			case FormationClass.Ranged:
				return TargetIconType.Archer_Light;
			case FormationClass.Cavalry:
				return TargetIconType.Cavalry_Light;
			default:
				return TargetIconType.HorseArcher_Light;
			}
		}

		// Token: 0x06002A0F RID: 10767 RVA: 0x000A2C70 File Offset: 0x000A0E70
		public static List<List<IReadOnlyPerkObject>> GetAvailablePerksForPeer(MissionPeer missionPeer)
		{
			if (((missionPeer != null) ? missionPeer.Team : null) != null)
			{
				return MultiplayerClassDivisions.GetAllPerksForHeroClass(MultiplayerClassDivisions.GetMPHeroClassForPeer(missionPeer, false), null);
			}
			return new List<List<IReadOnlyPerkObject>>();
		}

		// Token: 0x06002A10 RID: 10768 RVA: 0x000A2C94 File Offset: 0x000A0E94
		public static void Initialize()
		{
			MultiplayerClassDivisions.MultiplayerHeroClassGroups = new List<MultiplayerClassDivisions.MPHeroClassGroup>
			{
				new MultiplayerClassDivisions.MPHeroClassGroup("Infantry"),
				new MultiplayerClassDivisions.MPHeroClassGroup("Ranged"),
				new MultiplayerClassDivisions.MPHeroClassGroup("Cavalry"),
				new MultiplayerClassDivisions.MPHeroClassGroup("HorseArcher")
			};
			MultiplayerClassDivisions.AvailableCultures = from x in MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>().ToArray()
				where x.IsMainCulture
				select x;
		}

		// Token: 0x06002A11 RID: 10769 RVA: 0x000A2D23 File Offset: 0x000A0F23
		public static void Release()
		{
			MultiplayerClassDivisions.MultiplayerHeroClassGroups.Clear();
			MultiplayerClassDivisions.AvailableCultures = null;
		}

		// Token: 0x06002A12 RID: 10770 RVA: 0x000A2D35 File Offset: 0x000A0F35
		private static BasicCharacterObject GetMPCharacter(string stringId)
		{
			return MBObjectManager.Instance.GetObject<BasicCharacterObject>(stringId);
		}

		// Token: 0x06002A13 RID: 10771 RVA: 0x000A2D44 File Offset: 0x000A0F44
		public static int GetMinimumTroopCost(BasicCultureObject culture = null)
		{
			MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> mpheroClasses = MultiplayerClassDivisions.GetMPHeroClasses();
			if (culture != null)
			{
				return mpheroClasses.Where((MultiplayerClassDivisions.MPHeroClass c) => c.Culture == culture).Min((MultiplayerClassDivisions.MPHeroClass troop) => troop.TroopCost);
			}
			return mpheroClasses.Min((MultiplayerClassDivisions.MPHeroClass troop) => troop.TroopCost);
		}

		// Token: 0x0400100F RID: 4111
		public static IEnumerable<BasicCultureObject> AvailableCultures;

		// Token: 0x0200061B RID: 1563
		public class MPHeroClass : MBObjectBase
		{
			// Token: 0x170009C3 RID: 2499
			// (get) Token: 0x06003D60 RID: 15712 RVA: 0x000F2BE3 File Offset: 0x000F0DE3
			// (set) Token: 0x06003D61 RID: 15713 RVA: 0x000F2BEB File Offset: 0x000F0DEB
			public BasicCharacterObject HeroCharacter { get; private set; }

			// Token: 0x170009C4 RID: 2500
			// (get) Token: 0x06003D62 RID: 15714 RVA: 0x000F2BF4 File Offset: 0x000F0DF4
			// (set) Token: 0x06003D63 RID: 15715 RVA: 0x000F2BFC File Offset: 0x000F0DFC
			public BasicCharacterObject TroopCharacter { get; private set; }

			// Token: 0x170009C5 RID: 2501
			// (get) Token: 0x06003D64 RID: 15716 RVA: 0x000F2C05 File Offset: 0x000F0E05
			// (set) Token: 0x06003D65 RID: 15717 RVA: 0x000F2C0D File Offset: 0x000F0E0D
			public BasicCharacterObject BannerBearerCharacter { get; private set; }

			// Token: 0x170009C6 RID: 2502
			// (get) Token: 0x06003D66 RID: 15718 RVA: 0x000F2C16 File Offset: 0x000F0E16
			// (set) Token: 0x06003D67 RID: 15719 RVA: 0x000F2C1E File Offset: 0x000F0E1E
			public BasicCultureObject Culture { get; private set; }

			// Token: 0x170009C7 RID: 2503
			// (get) Token: 0x06003D68 RID: 15720 RVA: 0x000F2C27 File Offset: 0x000F0E27
			// (set) Token: 0x06003D69 RID: 15721 RVA: 0x000F2C2F File Offset: 0x000F0E2F
			public MultiplayerClassDivisions.MPHeroClassGroup ClassGroup { get; private set; }

			// Token: 0x170009C8 RID: 2504
			// (get) Token: 0x06003D6A RID: 15722 RVA: 0x000F2C38 File Offset: 0x000F0E38
			// (set) Token: 0x06003D6B RID: 15723 RVA: 0x000F2C40 File Offset: 0x000F0E40
			public string HeroIdleAnim { get; private set; }

			// Token: 0x170009C9 RID: 2505
			// (get) Token: 0x06003D6C RID: 15724 RVA: 0x000F2C49 File Offset: 0x000F0E49
			// (set) Token: 0x06003D6D RID: 15725 RVA: 0x000F2C51 File Offset: 0x000F0E51
			public string HeroMountIdleAnim { get; private set; }

			// Token: 0x170009CA RID: 2506
			// (get) Token: 0x06003D6E RID: 15726 RVA: 0x000F2C5A File Offset: 0x000F0E5A
			// (set) Token: 0x06003D6F RID: 15727 RVA: 0x000F2C62 File Offset: 0x000F0E62
			public string TroopIdleAnim { get; private set; }

			// Token: 0x170009CB RID: 2507
			// (get) Token: 0x06003D70 RID: 15728 RVA: 0x000F2C6B File Offset: 0x000F0E6B
			// (set) Token: 0x06003D71 RID: 15729 RVA: 0x000F2C73 File Offset: 0x000F0E73
			public string TroopMountIdleAnim { get; private set; }

			// Token: 0x170009CC RID: 2508
			// (get) Token: 0x06003D72 RID: 15730 RVA: 0x000F2C7C File Offset: 0x000F0E7C
			// (set) Token: 0x06003D73 RID: 15731 RVA: 0x000F2C84 File Offset: 0x000F0E84
			public int ArmorValue { get; private set; }

			// Token: 0x170009CD RID: 2509
			// (get) Token: 0x06003D74 RID: 15732 RVA: 0x000F2C8D File Offset: 0x000F0E8D
			// (set) Token: 0x06003D75 RID: 15733 RVA: 0x000F2C95 File Offset: 0x000F0E95
			public int Health { get; private set; }

			// Token: 0x170009CE RID: 2510
			// (get) Token: 0x06003D76 RID: 15734 RVA: 0x000F2C9E File Offset: 0x000F0E9E
			// (set) Token: 0x06003D77 RID: 15735 RVA: 0x000F2CA6 File Offset: 0x000F0EA6
			public float HeroMovementSpeedMultiplier { get; private set; }

			// Token: 0x170009CF RID: 2511
			// (get) Token: 0x06003D78 RID: 15736 RVA: 0x000F2CAF File Offset: 0x000F0EAF
			// (set) Token: 0x06003D79 RID: 15737 RVA: 0x000F2CB7 File Offset: 0x000F0EB7
			public float HeroCombatMovementSpeedMultiplier { get; private set; }

			// Token: 0x170009D0 RID: 2512
			// (get) Token: 0x06003D7A RID: 15738 RVA: 0x000F2CC0 File Offset: 0x000F0EC0
			// (set) Token: 0x06003D7B RID: 15739 RVA: 0x000F2CC8 File Offset: 0x000F0EC8
			public float HeroTopSpeedReachDuration { get; private set; }

			// Token: 0x170009D1 RID: 2513
			// (get) Token: 0x06003D7C RID: 15740 RVA: 0x000F2CD1 File Offset: 0x000F0ED1
			// (set) Token: 0x06003D7D RID: 15741 RVA: 0x000F2CD9 File Offset: 0x000F0ED9
			public float TroopMovementSpeedMultiplier { get; private set; }

			// Token: 0x170009D2 RID: 2514
			// (get) Token: 0x06003D7E RID: 15742 RVA: 0x000F2CE2 File Offset: 0x000F0EE2
			// (set) Token: 0x06003D7F RID: 15743 RVA: 0x000F2CEA File Offset: 0x000F0EEA
			public float TroopCombatMovementSpeedMultiplier { get; private set; }

			// Token: 0x170009D3 RID: 2515
			// (get) Token: 0x06003D80 RID: 15744 RVA: 0x000F2CF3 File Offset: 0x000F0EF3
			// (set) Token: 0x06003D81 RID: 15745 RVA: 0x000F2CFB File Offset: 0x000F0EFB
			public float TroopTopSpeedReachDuration { get; private set; }

			// Token: 0x170009D4 RID: 2516
			// (get) Token: 0x06003D82 RID: 15746 RVA: 0x000F2D04 File Offset: 0x000F0F04
			// (set) Token: 0x06003D83 RID: 15747 RVA: 0x000F2D0C File Offset: 0x000F0F0C
			public float TroopMultiplier { get; private set; }

			// Token: 0x170009D5 RID: 2517
			// (get) Token: 0x06003D84 RID: 15748 RVA: 0x000F2D15 File Offset: 0x000F0F15
			// (set) Token: 0x06003D85 RID: 15749 RVA: 0x000F2D1D File Offset: 0x000F0F1D
			public int TroopCost { get; private set; }

			// Token: 0x170009D6 RID: 2518
			// (get) Token: 0x06003D86 RID: 15750 RVA: 0x000F2D26 File Offset: 0x000F0F26
			// (set) Token: 0x06003D87 RID: 15751 RVA: 0x000F2D2E File Offset: 0x000F0F2E
			public int TroopCasualCost { get; private set; }

			// Token: 0x170009D7 RID: 2519
			// (get) Token: 0x06003D88 RID: 15752 RVA: 0x000F2D37 File Offset: 0x000F0F37
			// (set) Token: 0x06003D89 RID: 15753 RVA: 0x000F2D3F File Offset: 0x000F0F3F
			public int TroopBattleCost { get; private set; }

			// Token: 0x170009D8 RID: 2520
			// (get) Token: 0x06003D8A RID: 15754 RVA: 0x000F2D48 File Offset: 0x000F0F48
			// (set) Token: 0x06003D8B RID: 15755 RVA: 0x000F2D50 File Offset: 0x000F0F50
			public int MeleeAI { get; private set; }

			// Token: 0x170009D9 RID: 2521
			// (get) Token: 0x06003D8C RID: 15756 RVA: 0x000F2D59 File Offset: 0x000F0F59
			// (set) Token: 0x06003D8D RID: 15757 RVA: 0x000F2D61 File Offset: 0x000F0F61
			public int RangedAI { get; private set; }

			// Token: 0x170009DA RID: 2522
			// (get) Token: 0x06003D8E RID: 15758 RVA: 0x000F2D6A File Offset: 0x000F0F6A
			// (set) Token: 0x06003D8F RID: 15759 RVA: 0x000F2D72 File Offset: 0x000F0F72
			public TextObject HeroInformation { get; private set; }

			// Token: 0x170009DB RID: 2523
			// (get) Token: 0x06003D90 RID: 15760 RVA: 0x000F2D7B File Offset: 0x000F0F7B
			// (set) Token: 0x06003D91 RID: 15761 RVA: 0x000F2D83 File Offset: 0x000F0F83
			public TextObject TroopInformation { get; private set; }

			// Token: 0x170009DC RID: 2524
			// (get) Token: 0x06003D92 RID: 15762 RVA: 0x000F2D8C File Offset: 0x000F0F8C
			// (set) Token: 0x06003D93 RID: 15763 RVA: 0x000F2D94 File Offset: 0x000F0F94
			public TargetIconType IconType { get; private set; }

			// Token: 0x170009DD RID: 2525
			// (get) Token: 0x06003D94 RID: 15764 RVA: 0x000F2D9D File Offset: 0x000F0F9D
			public TextObject HeroName
			{
				get
				{
					return this.HeroCharacter.Name;
				}
			}

			// Token: 0x170009DE RID: 2526
			// (get) Token: 0x06003D95 RID: 15765 RVA: 0x000F2DAA File Offset: 0x000F0FAA
			public TextObject TroopName
			{
				get
				{
					return this.TroopCharacter.Name;
				}
			}

			// Token: 0x06003D96 RID: 15766 RVA: 0x000F2DB7 File Offset: 0x000F0FB7
			public override bool Equals(object obj)
			{
				return obj is MultiplayerClassDivisions.MPHeroClass && ((MultiplayerClassDivisions.MPHeroClass)obj).StringId.Equals(base.StringId);
			}

			// Token: 0x06003D97 RID: 15767 RVA: 0x000F2DD9 File Offset: 0x000F0FD9
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06003D98 RID: 15768 RVA: 0x000F2DE4 File Offset: 0x000F0FE4
			public List<IReadOnlyPerkObject> GetAllAvailablePerksForListIndex(int index, string forcedForGameMode = null)
			{
				string text = forcedForGameMode ?? MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				List<IReadOnlyPerkObject> list = new List<IReadOnlyPerkObject>();
				foreach (IReadOnlyPerkObject readOnlyPerkObject in this._perks)
				{
					foreach (string text2 in readOnlyPerkObject.GameModes)
					{
						if ((text2.Equals(text, StringComparison.InvariantCultureIgnoreCase) || text2.Equals("all", StringComparison.InvariantCultureIgnoreCase)) && readOnlyPerkObject.PerkListIndex == index)
						{
							list.Add(readOnlyPerkObject);
							break;
						}
					}
				}
				return list;
			}

			// Token: 0x06003D99 RID: 15769 RVA: 0x000F2EB0 File Offset: 0x000F10B0
			public override void Deserialize(MBObjectManager objectManager, XmlNode node)
			{
				base.Deserialize(objectManager, node);
				this.HeroCharacter = MultiplayerClassDivisions.GetMPCharacter(node.Attributes["hero"].Value);
				this.TroopCharacter = MultiplayerClassDivisions.GetMPCharacter(node.Attributes["troop"].Value);
				XmlAttribute xmlAttribute = node.Attributes["banner_bearer"];
				string text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				if (text != null)
				{
					this.BannerBearerCharacter = MultiplayerClassDivisions.GetMPCharacter(text);
				}
				XmlAttribute xmlAttribute2 = node.Attributes["hero_idle_anim"];
				this.HeroIdleAnim = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				XmlAttribute xmlAttribute3 = node.Attributes["hero_mount_idle_anim"];
				this.HeroMountIdleAnim = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				XmlAttribute xmlAttribute4 = node.Attributes["troop_idle_anim"];
				this.TroopIdleAnim = ((xmlAttribute4 != null) ? xmlAttribute4.Value : null);
				XmlAttribute xmlAttribute5 = node.Attributes["troop_mount_idle_anim"];
				this.TroopMountIdleAnim = ((xmlAttribute5 != null) ? xmlAttribute5.Value : null);
				this.Culture = this.HeroCharacter.Culture;
				this.ClassGroup = new MultiplayerClassDivisions.MPHeroClassGroup(this.HeroCharacter.DefaultFormationClass.GetName());
				this.TroopMultiplier = (float)Convert.ToDouble(node.Attributes["multiplier"].Value);
				this.TroopCost = Convert.ToInt32(node.Attributes["cost"].Value);
				this.ArmorValue = Convert.ToInt32(node.Attributes["armor"].Value);
				XmlAttribute xmlAttribute6 = node.Attributes["casual_cost"];
				XmlAttribute xmlAttribute7 = node.Attributes["battle_cost"];
				this.TroopCasualCost = ((xmlAttribute6 != null) ? Convert.ToInt32(node.Attributes["casual_cost"].Value) : this.TroopCost);
				this.TroopBattleCost = ((xmlAttribute7 != null) ? Convert.ToInt32(node.Attributes["battle_cost"].Value) : this.TroopCost);
				this.Health = 100;
				this.MeleeAI = 50;
				this.RangedAI = 50;
				XmlNode xmlNode = node.Attributes["hitpoints"];
				if (xmlNode != null)
				{
					this.Health = Convert.ToInt32(xmlNode.Value);
				}
				this.HeroMovementSpeedMultiplier = (float)Convert.ToDouble(node.Attributes["movement_speed"].Value);
				this.HeroCombatMovementSpeedMultiplier = (float)Convert.ToDouble(node.Attributes["combat_movement_speed"].Value);
				this.HeroTopSpeedReachDuration = (float)Convert.ToDouble(node.Attributes["acceleration"].Value);
				XmlAttribute xmlAttribute8 = node.Attributes["troop_movement_speed"];
				XmlAttribute xmlAttribute9 = node.Attributes["troop_combat_movement_speed"];
				XmlAttribute xmlAttribute10 = node.Attributes["troop_acceleration"];
				this.TroopMovementSpeedMultiplier = ((xmlAttribute8 != null) ? ((float)Convert.ToDouble(xmlAttribute8.Value)) : this.HeroMovementSpeedMultiplier);
				this.TroopCombatMovementSpeedMultiplier = ((xmlAttribute9 != null) ? ((float)Convert.ToDouble(xmlAttribute9.Value)) : this.HeroCombatMovementSpeedMultiplier);
				this.TroopTopSpeedReachDuration = ((xmlAttribute10 != null) ? ((float)Convert.ToDouble(xmlAttribute10.Value)) : this.HeroTopSpeedReachDuration);
				this.MeleeAI = Convert.ToInt32(node.Attributes["melee_ai"].Value);
				this.RangedAI = Convert.ToInt32(node.Attributes["ranged_ai"].Value);
				TargetIconType targetIconType;
				if (Enum.TryParse<TargetIconType>(node.Attributes["icon"].Value, true, out targetIconType))
				{
					this.IconType = targetIconType;
				}
				foreach (object obj in node.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					if (xmlNode2.NodeType != XmlNodeType.Comment && xmlNode2.Name == "Perks")
					{
						this._perks = new List<IReadOnlyPerkObject>();
						foreach (object obj2 in xmlNode2.ChildNodes)
						{
							XmlNode xmlNode3 = (XmlNode)obj2;
							if (xmlNode3.NodeType != XmlNodeType.Comment)
							{
								this._perks.Add(MPPerkObject.Deserialize(xmlNode3));
							}
						}
					}
				}
			}

			// Token: 0x06003D9A RID: 15770 RVA: 0x000F333C File Offset: 0x000F153C
			public bool IsTroopCharacter(BasicCharacterObject character)
			{
				return this.TroopCharacter == character;
			}

			// Token: 0x04001FB9 RID: 8121
			private List<IReadOnlyPerkObject> _perks;
		}

		// Token: 0x0200061C RID: 1564
		public class MPHeroClassGroup
		{
			// Token: 0x06003D9C RID: 15772 RVA: 0x000F334F File Offset: 0x000F154F
			public MPHeroClassGroup(string stringId)
			{
				this.StringId = stringId;
				this.Name = GameTexts.FindText("str_troop_type_name", this.StringId);
			}

			// Token: 0x06003D9D RID: 15773 RVA: 0x000F3374 File Offset: 0x000F1574
			public override bool Equals(object obj)
			{
				return obj is MultiplayerClassDivisions.MPHeroClassGroup && ((MultiplayerClassDivisions.MPHeroClassGroup)obj).StringId.Equals(this.StringId);
			}

			// Token: 0x06003D9E RID: 15774 RVA: 0x000F3396 File Offset: 0x000F1596
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x04001FBA RID: 8122
			public readonly string StringId;

			// Token: 0x04001FBB RID: 8123
			public readonly TextObject Name;
		}
	}
}

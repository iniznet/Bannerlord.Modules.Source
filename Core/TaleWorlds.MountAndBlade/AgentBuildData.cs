using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class AgentBuildData
	{
		public AgentData AgentData { get; private set; }

		public BasicCharacterObject AgentCharacter
		{
			get
			{
				return this.AgentData.AgentCharacter;
			}
		}

		public Monster AgentMonster
		{
			get
			{
				return this.AgentData.AgentMonster;
			}
		}

		public Equipment AgentOverridenSpawnEquipment
		{
			get
			{
				return this.AgentData.AgentOverridenEquipment;
			}
		}

		public MissionEquipment AgentOverridenSpawnMissionEquipment { get; private set; }

		public int AgentEquipmentSeed
		{
			get
			{
				return this.AgentData.AgentEquipmentSeed;
			}
		}

		public bool AgentNoHorses
		{
			get
			{
				return this.AgentData.AgentNoHorses;
			}
		}

		public string AgentMountKey
		{
			get
			{
				return this.AgentData.AgentMountKey;
			}
		}

		public bool AgentNoWeapons
		{
			get
			{
				return this.AgentData.AgentNoWeapons;
			}
		}

		public bool AgentNoArmor
		{
			get
			{
				return this.AgentData.AgentNoArmor;
			}
		}

		public bool AgentFixedEquipment
		{
			get
			{
				return this.AgentData.AgentFixedEquipment;
			}
		}

		public bool AgentCivilianEquipment
		{
			get
			{
				return this.AgentData.AgentCivilianEquipment;
			}
		}

		public uint AgentClothingColor1
		{
			get
			{
				return this.AgentData.AgentClothingColor1;
			}
		}

		public uint AgentClothingColor2
		{
			get
			{
				return this.AgentData.AgentClothingColor2;
			}
		}

		public bool BodyPropertiesOverriden
		{
			get
			{
				return this.AgentData.BodyPropertiesOverriden;
			}
		}

		public BodyProperties AgentBodyProperties
		{
			get
			{
				return this.AgentData.AgentBodyProperties;
			}
		}

		public bool AgeOverriden
		{
			get
			{
				return this.AgentData.AgeOverriden;
			}
		}

		public int AgentAge
		{
			get
			{
				return this.AgentData.AgentAge;
			}
		}

		public bool GenderOverriden
		{
			get
			{
				return this.AgentData.GenderOverriden;
			}
		}

		public bool AgentIsFemale
		{
			get
			{
				return this.AgentData.AgentIsFemale;
			}
		}

		public int AgentRace
		{
			get
			{
				return this.AgentData.AgentRace;
			}
		}

		public IAgentOriginBase AgentOrigin
		{
			get
			{
				return this.AgentData.AgentOrigin;
			}
		}

		public Agent.ControllerType AgentController { get; private set; }

		public Team AgentTeam { get; private set; }

		public bool AgentIsReinforcement { get; private set; }

		public bool AgentSpawnsIntoOwnFormation { get; private set; }

		public bool AgentSpawnsUsingOwnTroopClass { get; private set; }

		public float MakeUnitStandOutDistance { get; private set; }

		public Vec3? AgentInitialPosition { get; private set; }

		public Vec2? AgentInitialDirection { get; private set; }

		public Formation AgentFormation { get; private set; }

		public int AgentFormationTroopSpawnCount { get; private set; }

		public int AgentFormationTroopSpawnIndex { get; private set; }

		public MissionPeer AgentMissionPeer { get; private set; }

		public MissionPeer OwningAgentMissionPeer { get; private set; }

		public bool AgentIndexOverriden { get; private set; }

		public int AgentIndex { get; private set; }

		public bool AgentMountIndexOverriden { get; private set; }

		public int AgentMountIndex { get; private set; }

		public int AgentVisualsIndex { get; private set; }

		public Banner AgentBanner { get; private set; }

		public ItemObject AgentBannerItem { get; private set; }

		public ItemObject AgentBannerReplacementWeaponItem { get; private set; }

		public bool AgentCanSpawnOutsideOfMissionBoundary { get; private set; }

		public bool RandomizeColors
		{
			get
			{
				return this.AgentCharacter != null && !this.AgentCharacter.IsHero && this.AgentMissionPeer == null;
			}
		}

		private AgentBuildData()
		{
			this.AgentController = Agent.ControllerType.AI;
			this.AgentTeam = TaleWorlds.MountAndBlade.Team.Invalid;
			this.AgentFormation = null;
			this.AgentMissionPeer = null;
			this.AgentFormationTroopSpawnIndex = -1;
		}

		public AgentBuildData(AgentData agentData)
			: this()
		{
			this.AgentData = agentData;
		}

		public AgentBuildData(IAgentOriginBase agentOrigin)
			: this()
		{
			this.AgentData = new AgentData(agentOrigin);
		}

		public AgentBuildData(BasicCharacterObject characterObject)
			: this()
		{
			this.AgentData = new AgentData(characterObject);
		}

		public AgentBuildData Character(BasicCharacterObject characterObject)
		{
			this.AgentData.Character(characterObject);
			return this;
		}

		public AgentBuildData Controller(Agent.ControllerType controller)
		{
			this.AgentController = controller;
			return this;
		}

		public AgentBuildData Team(Team team)
		{
			this.AgentTeam = team;
			return this;
		}

		public AgentBuildData IsReinforcement(bool isReinforcement)
		{
			this.AgentIsReinforcement = isReinforcement;
			return this;
		}

		public AgentBuildData SpawnsIntoOwnFormation(bool spawnIntoOwnFormation)
		{
			this.AgentSpawnsIntoOwnFormation = spawnIntoOwnFormation;
			return this;
		}

		public AgentBuildData SpawnsUsingOwnTroopClass(bool spawnUsingOwnTroopClass)
		{
			this.AgentSpawnsUsingOwnTroopClass = spawnUsingOwnTroopClass;
			return this;
		}

		public AgentBuildData MakeUnitStandOutOfFormationDistance(float makeUnitStandOutDistance)
		{
			this.MakeUnitStandOutDistance = makeUnitStandOutDistance;
			return this;
		}

		public AgentBuildData InitialPosition(in Vec3 position)
		{
			this.AgentInitialPosition = new Vec3?(position);
			return this;
		}

		public AgentBuildData InitialDirection(in Vec2 direction)
		{
			this.AgentInitialDirection = new Vec2?(direction);
			return this;
		}

		public AgentBuildData InitialFrameFromSpawnPointEntity(GameEntity entity)
		{
			MatrixFrame globalFrame = entity.GetGlobalFrame();
			this.AgentInitialPosition = new Vec3?(globalFrame.origin);
			this.AgentInitialDirection = new Vec2?(globalFrame.rotation.f.AsVec2.Normalized());
			return this;
		}

		public AgentBuildData Formation(Formation formation)
		{
			this.AgentFormation = formation;
			return this;
		}

		public AgentBuildData Monster(Monster monster)
		{
			this.AgentData.Monster(monster);
			return this;
		}

		public AgentBuildData VisualsIndex(int index)
		{
			this.AgentVisualsIndex = index;
			return this;
		}

		public AgentBuildData Equipment(Equipment equipment)
		{
			this.AgentData.Equipment(equipment);
			return this;
		}

		public AgentBuildData MissionEquipment(MissionEquipment missionEquipment)
		{
			this.AgentOverridenSpawnMissionEquipment = missionEquipment;
			return this;
		}

		public AgentBuildData EquipmentSeed(int seed)
		{
			this.AgentData.EquipmentSeed(seed);
			return this;
		}

		public AgentBuildData NoHorses(bool noHorses)
		{
			this.AgentData.NoHorses(noHorses);
			return this;
		}

		public AgentBuildData NoWeapons(bool noWeapons)
		{
			this.AgentData.NoWeapons(noWeapons);
			return this;
		}

		public AgentBuildData NoArmor(bool noArmor)
		{
			this.AgentData.NoArmor(noArmor);
			return this;
		}

		public AgentBuildData FixedEquipment(bool fixedEquipment)
		{
			this.AgentData.FixedEquipment(fixedEquipment);
			return this;
		}

		public AgentBuildData CivilianEquipment(bool civilianEquipment)
		{
			this.AgentData.CivilianEquipment(civilianEquipment);
			return this;
		}

		public AgentBuildData ClothingColor1(uint color)
		{
			this.AgentData.ClothingColor1(color);
			return this;
		}

		public AgentBuildData ClothingColor2(uint color)
		{
			this.AgentData.ClothingColor2(color);
			return this;
		}

		public AgentBuildData MissionPeer(MissionPeer missionPeer)
		{
			this.AgentMissionPeer = missionPeer;
			return this;
		}

		public AgentBuildData OwningMissionPeer(MissionPeer missionPeer)
		{
			this.OwningAgentMissionPeer = missionPeer;
			return this;
		}

		public AgentBuildData BodyProperties(BodyProperties bodyProperties)
		{
			this.AgentData.BodyProperties(bodyProperties);
			return this;
		}

		public AgentBuildData Age(int age)
		{
			this.AgentData.Age(age);
			return this;
		}

		public AgentBuildData TroopOrigin(IAgentOriginBase troopOrigin)
		{
			this.AgentData.TroopOrigin(troopOrigin);
			return this;
		}

		public AgentBuildData IsFemale(bool isFemale)
		{
			this.AgentData.IsFemale(isFemale);
			return this;
		}

		public AgentBuildData Race(int race)
		{
			this.AgentData.Race(race);
			return this;
		}

		public AgentBuildData MountKey(string mountKey)
		{
			this.AgentData.MountKey(mountKey);
			return this;
		}

		public AgentBuildData Index(int index)
		{
			this.AgentIndex = index;
			this.AgentIndexOverriden = true;
			return this;
		}

		public AgentBuildData MountIndex(int mountIndex)
		{
			this.AgentMountIndex = mountIndex;
			this.AgentMountIndexOverriden = true;
			return this;
		}

		public AgentBuildData Banner(Banner banner)
		{
			this.AgentBanner = banner;
			return this;
		}

		public AgentBuildData BannerItem(ItemObject bannerItem)
		{
			this.AgentBannerItem = bannerItem;
			return this;
		}

		public AgentBuildData BannerReplacementWeaponItem(ItemObject weaponItem)
		{
			this.AgentBannerReplacementWeaponItem = weaponItem;
			return this;
		}

		public AgentBuildData FormationTroopSpawnCount(int formationTroopCount)
		{
			this.AgentFormationTroopSpawnCount = formationTroopCount;
			return this;
		}

		public AgentBuildData FormationTroopSpawnIndex(int formationTroopIndex)
		{
			this.AgentFormationTroopSpawnIndex = formationTroopIndex;
			return this;
		}

		public AgentBuildData CanSpawnOutsideOfMissionBoundary(bool canSpawn)
		{
			this.AgentCanSpawnOutsideOfMissionBoundary = canSpawn;
			return this;
		}
	}
}

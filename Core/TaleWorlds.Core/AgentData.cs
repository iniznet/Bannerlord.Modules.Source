using System;

namespace TaleWorlds.Core
{
	public class AgentData
	{
		public BasicCharacterObject AgentCharacter { get; private set; }

		public Monster AgentMonster { get; private set; }

		public IBattleCombatant AgentOwnerParty { get; private set; }

		public Equipment AgentOverridenEquipment { get; private set; }

		public int AgentEquipmentSeed { get; private set; }

		public bool AgentNoHorses { get; private set; }

		public string AgentMountKey { get; private set; }

		public bool AgentNoWeapons { get; private set; }

		public bool AgentNoArmor { get; private set; }

		public bool AgentFixedEquipment { get; private set; }

		public bool AgentCivilianEquipment { get; private set; }

		public uint AgentClothingColor1 { get; private set; }

		public uint AgentClothingColor2 { get; private set; }

		public bool BodyPropertiesOverriden { get; private set; }

		public BodyProperties AgentBodyProperties { get; private set; }

		public bool AgeOverriden { get; private set; }

		public int AgentAge { get; private set; }

		public bool GenderOverriden { get; private set; }

		public bool AgentIsFemale { get; private set; }

		public int AgentRace { get; private set; }

		public IAgentOriginBase AgentOrigin { get; private set; }

		public AgentData(IAgentOriginBase agentOrigin)
			: this(agentOrigin.Troop)
		{
			this.AgentOrigin = agentOrigin;
			this.AgentCharacter = agentOrigin.Troop;
			this.AgentEquipmentSeed = agentOrigin.Seed;
		}

		public AgentData(BasicCharacterObject characterObject)
		{
			this.AgentCharacter = characterObject;
			this.AgentRace = characterObject.Race;
			this.AgentMonster = FaceGen.GetBaseMonsterFromRace(this.AgentRace);
			this.AgentOwnerParty = null;
			this.AgentOverridenEquipment = null;
			this.AgentEquipmentSeed = 0;
			this.AgentNoHorses = false;
			this.AgentNoWeapons = false;
			this.AgentNoArmor = false;
			this.AgentFixedEquipment = false;
			this.AgentCivilianEquipment = false;
			this.AgentClothingColor1 = uint.MaxValue;
			this.AgentClothingColor2 = uint.MaxValue;
			this.BodyPropertiesOverriden = false;
			this.GenderOverriden = false;
		}

		public AgentData Character(BasicCharacterObject characterObject)
		{
			this.AgentCharacter = characterObject;
			return this;
		}

		public AgentData Monster(Monster monster)
		{
			this.AgentMonster = monster;
			return this;
		}

		public AgentData OwnerParty(IBattleCombatant owner)
		{
			this.AgentOwnerParty = owner;
			return this;
		}

		public AgentData Equipment(Equipment equipment)
		{
			this.AgentOverridenEquipment = equipment;
			return this;
		}

		public AgentData EquipmentSeed(int seed)
		{
			this.AgentEquipmentSeed = seed;
			return this;
		}

		public AgentData NoHorses(bool noHorses)
		{
			this.AgentNoHorses = noHorses;
			return this;
		}

		public AgentData NoWeapons(bool noWeapons)
		{
			this.AgentNoWeapons = noWeapons;
			return this;
		}

		public AgentData NoArmor(bool noArmor)
		{
			this.AgentNoArmor = noArmor;
			return this;
		}

		public AgentData FixedEquipment(bool fixedEquipment)
		{
			this.AgentFixedEquipment = fixedEquipment;
			return this;
		}

		public AgentData CivilianEquipment(bool civilianEquipment)
		{
			this.AgentCivilianEquipment = civilianEquipment;
			return this;
		}

		public AgentData ClothingColor1(uint color)
		{
			this.AgentClothingColor1 = color;
			return this;
		}

		public AgentData ClothingColor2(uint color)
		{
			this.AgentClothingColor2 = color;
			return this;
		}

		public AgentData BodyProperties(BodyProperties bodyProperties)
		{
			this.AgentBodyProperties = bodyProperties;
			this.BodyPropertiesOverriden = true;
			return this;
		}

		public AgentData Age(int age)
		{
			this.AgentAge = age;
			this.AgeOverriden = true;
			return this;
		}

		public AgentData TroopOrigin(IAgentOriginBase troopOrigin)
		{
			this.AgentOrigin = troopOrigin;
			if (((troopOrigin != null) ? troopOrigin.Troop : null) != null && !troopOrigin.Troop.IsHero)
			{
				this.EquipmentSeed(troopOrigin.Seed);
			}
			return this;
		}

		public AgentData IsFemale(bool isFemale)
		{
			this.AgentIsFemale = isFemale;
			this.GenderOverriden = true;
			return this;
		}

		public AgentData Race(int race)
		{
			this.AgentRace = race;
			this.GenderOverriden = true;
			return this;
		}

		public AgentData MountKey(string mountKey)
		{
			this.AgentMountKey = mountKey;
			return this;
		}
	}
}

using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000006 RID: 6
	public class AgentData
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021F0 File Offset: 0x000003F0
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000021F8 File Offset: 0x000003F8
		public BasicCharacterObject AgentCharacter { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002201 File Offset: 0x00000401
		// (set) Token: 0x0600000D RID: 13 RVA: 0x00002209 File Offset: 0x00000409
		public Monster AgentMonster { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002212 File Offset: 0x00000412
		// (set) Token: 0x0600000F RID: 15 RVA: 0x0000221A File Offset: 0x0000041A
		public IBattleCombatant AgentOwnerParty { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002223 File Offset: 0x00000423
		// (set) Token: 0x06000011 RID: 17 RVA: 0x0000222B File Offset: 0x0000042B
		public Equipment AgentOverridenEquipment { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002234 File Offset: 0x00000434
		// (set) Token: 0x06000013 RID: 19 RVA: 0x0000223C File Offset: 0x0000043C
		public int AgentEquipmentSeed { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002245 File Offset: 0x00000445
		// (set) Token: 0x06000015 RID: 21 RVA: 0x0000224D File Offset: 0x0000044D
		public bool AgentNoHorses { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002256 File Offset: 0x00000456
		// (set) Token: 0x06000017 RID: 23 RVA: 0x0000225E File Offset: 0x0000045E
		public string AgentMountKey { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00002267 File Offset: 0x00000467
		// (set) Token: 0x06000019 RID: 25 RVA: 0x0000226F File Offset: 0x0000046F
		public bool AgentNoWeapons { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002278 File Offset: 0x00000478
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002280 File Offset: 0x00000480
		public bool AgentNoArmor { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002289 File Offset: 0x00000489
		// (set) Token: 0x0600001D RID: 29 RVA: 0x00002291 File Offset: 0x00000491
		public bool AgentFixedEquipment { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001E RID: 30 RVA: 0x0000229A File Offset: 0x0000049A
		// (set) Token: 0x0600001F RID: 31 RVA: 0x000022A2 File Offset: 0x000004A2
		public bool AgentCivilianEquipment { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000020 RID: 32 RVA: 0x000022AB File Offset: 0x000004AB
		// (set) Token: 0x06000021 RID: 33 RVA: 0x000022B3 File Offset: 0x000004B3
		public uint AgentClothingColor1 { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000022BC File Offset: 0x000004BC
		// (set) Token: 0x06000023 RID: 35 RVA: 0x000022C4 File Offset: 0x000004C4
		public uint AgentClothingColor2 { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000022CD File Offset: 0x000004CD
		// (set) Token: 0x06000025 RID: 37 RVA: 0x000022D5 File Offset: 0x000004D5
		public bool BodyPropertiesOverriden { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000022DE File Offset: 0x000004DE
		// (set) Token: 0x06000027 RID: 39 RVA: 0x000022E6 File Offset: 0x000004E6
		public BodyProperties AgentBodyProperties { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000022EF File Offset: 0x000004EF
		// (set) Token: 0x06000029 RID: 41 RVA: 0x000022F7 File Offset: 0x000004F7
		public bool AgeOverriden { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002300 File Offset: 0x00000500
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002308 File Offset: 0x00000508
		public int AgentAge { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002311 File Offset: 0x00000511
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002319 File Offset: 0x00000519
		public bool GenderOverriden { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002322 File Offset: 0x00000522
		// (set) Token: 0x0600002F RID: 47 RVA: 0x0000232A File Offset: 0x0000052A
		public bool AgentIsFemale { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002333 File Offset: 0x00000533
		// (set) Token: 0x06000031 RID: 49 RVA: 0x0000233B File Offset: 0x0000053B
		public int AgentRace { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002344 File Offset: 0x00000544
		// (set) Token: 0x06000033 RID: 51 RVA: 0x0000234C File Offset: 0x0000054C
		public IAgentOriginBase AgentOrigin { get; private set; }

		// Token: 0x06000034 RID: 52 RVA: 0x00002355 File Offset: 0x00000555
		public AgentData(IAgentOriginBase agentOrigin)
			: this(agentOrigin.Troop)
		{
			this.AgentOrigin = agentOrigin;
			this.AgentCharacter = agentOrigin.Troop;
			this.AgentEquipmentSeed = agentOrigin.Seed;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002384 File Offset: 0x00000584
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

		// Token: 0x06000036 RID: 54 RVA: 0x0000240F File Offset: 0x0000060F
		public AgentData Character(BasicCharacterObject characterObject)
		{
			this.AgentCharacter = characterObject;
			return this;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002419 File Offset: 0x00000619
		public AgentData Monster(Monster monster)
		{
			this.AgentMonster = monster;
			return this;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002423 File Offset: 0x00000623
		public AgentData OwnerParty(IBattleCombatant owner)
		{
			this.AgentOwnerParty = owner;
			return this;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000242D File Offset: 0x0000062D
		public AgentData Equipment(Equipment equipment)
		{
			this.AgentOverridenEquipment = equipment;
			return this;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002437 File Offset: 0x00000637
		public AgentData EquipmentSeed(int seed)
		{
			this.AgentEquipmentSeed = seed;
			return this;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002441 File Offset: 0x00000641
		public AgentData NoHorses(bool noHorses)
		{
			this.AgentNoHorses = noHorses;
			return this;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000244B File Offset: 0x0000064B
		public AgentData NoWeapons(bool noWeapons)
		{
			this.AgentNoWeapons = noWeapons;
			return this;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002455 File Offset: 0x00000655
		public AgentData NoArmor(bool noArmor)
		{
			this.AgentNoArmor = noArmor;
			return this;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000245F File Offset: 0x0000065F
		public AgentData FixedEquipment(bool fixedEquipment)
		{
			this.AgentFixedEquipment = fixedEquipment;
			return this;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002469 File Offset: 0x00000669
		public AgentData CivilianEquipment(bool civilianEquipment)
		{
			this.AgentCivilianEquipment = civilianEquipment;
			return this;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002473 File Offset: 0x00000673
		public AgentData ClothingColor1(uint color)
		{
			this.AgentClothingColor1 = color;
			return this;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000247D File Offset: 0x0000067D
		public AgentData ClothingColor2(uint color)
		{
			this.AgentClothingColor2 = color;
			return this;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002487 File Offset: 0x00000687
		public AgentData BodyProperties(BodyProperties bodyProperties)
		{
			this.AgentBodyProperties = bodyProperties;
			this.BodyPropertiesOverriden = true;
			return this;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002498 File Offset: 0x00000698
		public AgentData Age(int age)
		{
			this.AgentAge = age;
			this.AgeOverriden = true;
			return this;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000024A9 File Offset: 0x000006A9
		public AgentData TroopOrigin(IAgentOriginBase troopOrigin)
		{
			this.AgentOrigin = troopOrigin;
			if (((troopOrigin != null) ? troopOrigin.Troop : null) != null && !troopOrigin.Troop.IsHero)
			{
				this.EquipmentSeed(troopOrigin.Seed);
			}
			return this;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000024DB File Offset: 0x000006DB
		public AgentData IsFemale(bool isFemale)
		{
			this.AgentIsFemale = isFemale;
			this.GenderOverriden = true;
			return this;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000024EC File Offset: 0x000006EC
		public AgentData Race(int race)
		{
			this.AgentRace = race;
			this.GenderOverriden = true;
			return this;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000024FD File Offset: 0x000006FD
		public AgentData MountKey(string mountKey)
		{
			this.AgentMountKey = mountKey;
			return this;
		}
	}
}

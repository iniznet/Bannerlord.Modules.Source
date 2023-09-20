using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Settlements.Locations
{
	// Token: 0x0200036F RID: 879
	public class LocationCharacter
	{
		// Token: 0x17000C81 RID: 3201
		// (get) Token: 0x0600334F RID: 13135 RVA: 0x000D3DC5 File Offset: 0x000D1FC5
		public CharacterObject Character
		{
			get
			{
				return (CharacterObject)this.AgentData.AgentCharacter;
			}
		}

		// Token: 0x17000C82 RID: 3202
		// (get) Token: 0x06003350 RID: 13136 RVA: 0x000D3DD7 File Offset: 0x000D1FD7
		public IAgentOriginBase AgentOrigin
		{
			get
			{
				return this.AgentData.AgentOrigin;
			}
		}

		// Token: 0x17000C83 RID: 3203
		// (get) Token: 0x06003351 RID: 13137 RVA: 0x000D3DE4 File Offset: 0x000D1FE4
		public AgentData AgentData { get; }

		// Token: 0x17000C84 RID: 3204
		// (get) Token: 0x06003352 RID: 13138 RVA: 0x000D3DEC File Offset: 0x000D1FEC
		public bool UseCivilianEquipment { get; }

		// Token: 0x17000C85 RID: 3205
		// (get) Token: 0x06003353 RID: 13139 RVA: 0x000D3DF4 File Offset: 0x000D1FF4
		public string ActionSetCode { get; }

		// Token: 0x17000C86 RID: 3206
		// (get) Token: 0x06003354 RID: 13140 RVA: 0x000D3DFC File Offset: 0x000D1FFC
		public string AlarmedActionSetCode { get; }

		// Token: 0x17000C87 RID: 3207
		// (get) Token: 0x06003355 RID: 13141 RVA: 0x000D3E04 File Offset: 0x000D2004
		// (set) Token: 0x06003356 RID: 13142 RVA: 0x000D3E0C File Offset: 0x000D200C
		public string SpecialTargetTag { get; set; }

		// Token: 0x17000C88 RID: 3208
		// (get) Token: 0x06003357 RID: 13143 RVA: 0x000D3E15 File Offset: 0x000D2015
		public LocationCharacter.AddBehaviorsDelegate AddBehaviors { get; }

		// Token: 0x17000C89 RID: 3209
		// (get) Token: 0x06003358 RID: 13144 RVA: 0x000D3E1D File Offset: 0x000D201D
		public bool FixedLocation { get; }

		// Token: 0x17000C8A RID: 3210
		// (get) Token: 0x06003359 RID: 13145 RVA: 0x000D3E25 File Offset: 0x000D2025
		// (set) Token: 0x0600335A RID: 13146 RVA: 0x000D3E2D File Offset: 0x000D202D
		public Alley MemberOfAlley { get; private set; }

		// Token: 0x17000C8B RID: 3211
		// (get) Token: 0x0600335B RID: 13147 RVA: 0x000D3E36 File Offset: 0x000D2036
		public ItemObject SpecialItem { get; }

		// Token: 0x17000C8C RID: 3212
		// (get) Token: 0x0600335C RID: 13148 RVA: 0x000D3E3E File Offset: 0x000D203E
		// (set) Token: 0x0600335D RID: 13149 RVA: 0x000D3E46 File Offset: 0x000D2046
		public bool IsHidden { get; set; }

		// Token: 0x0600335E RID: 13150 RVA: 0x000D3E50 File Offset: 0x000D2050
		public LocationCharacter(AgentData agentData, LocationCharacter.AddBehaviorsDelegate addBehaviorsDelegate, string spawnTag, bool fixedLocation, LocationCharacter.CharacterRelations characterRelation, string actionSetCode, bool useCivilianEquipment, bool isFixedCharacter = false, ItemObject specialItem = null, bool isHidden = false, bool isVisualTracked = false, bool overrideBodyProperties = true)
		{
			this.AgentData = agentData;
			if (Campaign.Current.GameMode == CampaignGameMode.Campaign)
			{
				int num = -2;
				if (overrideBodyProperties)
				{
					num = (isFixedCharacter ? (Settlement.CurrentSettlement.StringId.GetDeterministicHashCode() + this.Character.StringId.GetDeterministicHashCode()) : agentData.AgentEquipmentSeed);
				}
				this.AgentData.BodyProperties(this.Character.GetBodyProperties(this.Character.Equipment, num));
			}
			this.AddBehaviors = addBehaviorsDelegate;
			this.SpecialTargetTag = spawnTag;
			this.FixedLocation = fixedLocation;
			this.ActionSetCode = actionSetCode ?? TaleWorlds.Core.ActionSetCode.GenerateActionSetNameWithSuffix(this.AgentData.AgentMonster, this.AgentData.AgentCharacter.IsFemale, "_villager");
			this.AlarmedActionSetCode = TaleWorlds.Core.ActionSetCode.GenerateActionSetNameWithSuffix(this.AgentData.AgentMonster, this.AgentData.AgentIsFemale, "_villager");
			this.PrefabNamesForBones = new Dictionary<sbyte, string>();
			this.CharacterRelation = characterRelation;
			this.SpecialItem = specialItem;
			this.UseCivilianEquipment = useCivilianEquipment;
			this.IsHidden = isHidden;
			this.IsVisualTracked = isVisualTracked;
		}

		// Token: 0x0600335F RID: 13151 RVA: 0x000D3F6E File Offset: 0x000D216E
		public void SetAlleyOfCharacter(Alley alley)
		{
			this.MemberOfAlley = alley;
		}

		// Token: 0x06003360 RID: 13152 RVA: 0x000D3F78 File Offset: 0x000D2178
		public static LocationCharacter CreateBodyguardHero(Hero hero, MobileParty party, LocationCharacter.AddBehaviorsDelegate addBehaviorsDelegate)
		{
			UniqueTroopDescriptor uniqueTroopDescriptor = new UniqueTroopDescriptor(FlattenedTroopRoster.GenerateUniqueNoFromParty(party, 0));
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(hero.CharacterObject.Race, "_settlement");
			return new LocationCharacter(new AgentData(new PartyAgentOrigin(PartyBase.MainParty, hero.CharacterObject, -1, uniqueTroopDescriptor, false)).Monster(monsterWithSuffix).NoHorses(true), addBehaviorsDelegate, null, false, LocationCharacter.CharacterRelations.Friendly, null, !PlayerEncounter.LocationEncounter.Settlement.IsVillage, false, null, false, false, true);
		}

		// Token: 0x040010A7 RID: 4263
		public bool IsVisualTracked;

		// Token: 0x040010AE RID: 4270
		public Dictionary<sbyte, string> PrefabNamesForBones;

		// Token: 0x040010B1 RID: 4273
		public LocationCharacter.CharacterRelations CharacterRelation;

		// Token: 0x020006A7 RID: 1703
		// (Invoke) Token: 0x060053D3 RID: 21459
		public delegate void AddBehaviorsDelegate(IAgent agent);

		// Token: 0x020006A8 RID: 1704
		public enum CharacterRelations
		{
			// Token: 0x04001B8C RID: 7052
			Neutral,
			// Token: 0x04001B8D RID: 7053
			Friendly,
			// Token: 0x04001B8E RID: 7054
			Enemy
		}
	}
}

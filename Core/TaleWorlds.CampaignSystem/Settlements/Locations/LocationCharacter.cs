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
	public class LocationCharacter
	{
		public CharacterObject Character
		{
			get
			{
				return (CharacterObject)this.AgentData.AgentCharacter;
			}
		}

		public IAgentOriginBase AgentOrigin
		{
			get
			{
				return this.AgentData.AgentOrigin;
			}
		}

		public AgentData AgentData { get; }

		public bool UseCivilianEquipment { get; }

		public string ActionSetCode { get; }

		public string AlarmedActionSetCode { get; }

		public string SpecialTargetTag { get; set; }

		public LocationCharacter.AddBehaviorsDelegate AddBehaviors { get; }

		public bool FixedLocation { get; }

		public Alley MemberOfAlley { get; private set; }

		public ItemObject SpecialItem { get; }

		public bool IsHidden { get; set; }

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

		public void SetAlleyOfCharacter(Alley alley)
		{
			this.MemberOfAlley = alley;
		}

		public static LocationCharacter CreateBodyguardHero(Hero hero, MobileParty party, LocationCharacter.AddBehaviorsDelegate addBehaviorsDelegate)
		{
			UniqueTroopDescriptor uniqueTroopDescriptor = new UniqueTroopDescriptor(FlattenedTroopRoster.GenerateUniqueNoFromParty(party, 0));
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(hero.CharacterObject.Race, "_settlement");
			return new LocationCharacter(new AgentData(new PartyAgentOrigin(PartyBase.MainParty, hero.CharacterObject, -1, uniqueTroopDescriptor, false)).Monster(monsterWithSuffix).NoHorses(true), addBehaviorsDelegate, null, false, LocationCharacter.CharacterRelations.Friendly, null, !PlayerEncounter.LocationEncounter.Settlement.IsVillage, false, null, false, false, true);
		}

		public bool IsVisualTracked;

		public Dictionary<sbyte, string> PrefabNamesForBones;

		public LocationCharacter.CharacterRelations CharacterRelation;

		public delegate void AddBehaviorsDelegate(IAgent agent);

		public enum CharacterRelations
		{
			Neutral,
			Friendly,
			Enemy
		}
	}
}

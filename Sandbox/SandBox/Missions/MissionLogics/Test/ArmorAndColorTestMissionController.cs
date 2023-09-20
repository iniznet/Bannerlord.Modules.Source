using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.Missions.MissionLogics.Test
{
	public class ArmorAndColorTestMissionController : MissionLogic
	{
		public ArmorAndColorTestMissionController(bool spawnSoldiers, bool spawnLords, bool spawnBandits, bool spawnLadies, string oneTypeSoldierStringId = null, int testTypeId = 2, bool spawnHelmetTestCharacters = false)
		{
			this._spawnLords = spawnLords;
			this._spawnSoldiers = spawnSoldiers;
			this._spawnBandits = spawnBandits;
			this._spawnLadies = spawnLadies;
			this._oneTypeSoldierStringId = oneTypeSoldierStringId;
			this._testTypeId = testTypeId;
			this._spawnHelmetTestCharacters = spawnHelmetTestCharacters;
		}

		public override void AfterStart()
		{
			this.InitializeColors();
			this.InitializeTeams();
			this.SpawnPlayer();
			this.SpawnCharacters();
		}

		private void InitializeColors()
		{
			this._colorDictionary = new Dictionary<IFaction, uint[]>();
			foreach (string text in this._factionIdList)
			{
				Kingdom kingdom = Campaign.Current.CampaignObjectManager.Find<Kingdom>(text);
				this._colorDictionary.Add(kingdom, new uint[] { kingdom.Color, kingdom.Color2 });
			}
		}

		private void InitializeTeams()
		{
			base.Mission.PlayerTeam = base.Mission.Teams.Add(0, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2, null, true, false, true);
		}

		private void SpawnPlayer()
		{
			CharacterObject characterObject = CharacterObject.PlayerCharacter ?? Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero");
			MatrixFrame nextSpawnFrame = this.GetNextSpawnFrame();
			nextSpawnFrame.rotation.RotateAboutUp(3.1415927f);
			Mission mission = base.Mission;
			AgentBuildData agentBuildData = new AgentBuildData(characterObject).Team(base.Mission.PlayerTeam).TroopOrigin(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).InitialPosition(ref nextSpawnFrame.origin);
			Vec2 vec = nextSpawnFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			mission.SpawnAgent(agentBuildData.InitialDirection(ref vec).ClothingColor1(base.Mission.PlayerTeam.Color).ClothingColor2(base.Mission.PlayerTeam.Color2)
				.Controller(2), false);
			this.SetCoordniateNewHorizontalLine();
		}

		private void SpawnCharacters()
		{
			if (this._spawnLords && this._spawnLadies)
			{
				this.SpawnLordAndLadies();
				return;
			}
			if (this._spawnLords)
			{
				this.SpawnLords();
			}
			if (this._spawnSoldiers)
			{
				this.SpawnSoldiers();
			}
			if (this._spawnBandits)
			{
				this.SpawnBandits();
			}
			if (this._oneTypeSoldierStringId != null)
			{
				this.SpawnOneTypeSoldier();
			}
			if (this._spawnHelmetTestCharacters)
			{
				this.SpawnHelmetTestCharacters();
			}
		}

		private void SpawnHelmetTestCharacters()
		{
			List<CharacterObject> list = (from character in Game.Current.ObjectManager.GetObjectTypeList<CharacterObject>()
				where character.StringId.Contains("helmet_test")
				select character).ToList<CharacterObject>();
			List<ItemObject> list2 = (from item in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>()
				where item.ItemType == 12
				select item).ToList<ItemObject>();
			foreach (CharacterObject characterObject in list)
			{
				foreach (ItemObject itemObject in list2)
				{
					characterObject.Equipment[5] = new EquipmentElement(itemObject, null, null, false);
					this.SpawnCharacter(characterObject, new uint[] { uint.MaxValue, uint.MaxValue }, false);
				}
			}
		}

		private void SpawnLordAndLadies()
		{
			foreach (CharacterObject characterObject in (from character in Game.Current.ObjectManager.GetObjectTypeList<CharacterObject>()
				where character.IsHero && !character.HeroObject.IsTemplate && character.HeroObject.IsLord
				select character).ToList<CharacterObject>())
			{
				if (this.CheckIfEquipmentSetCountExceedsMapHorizontalEnd(characterObject.AllEquipments.Count))
				{
					this.SetCoordniateNewHorizontalLine();
					this.SetCoordniateNewHorizontalLine();
				}
				IFaction mapFaction = characterObject.HeroObject.MapFaction;
				uint[] array2;
				if (!this._colorDictionary.ContainsKey(mapFaction))
				{
					uint[] array = new uint[2];
					array[0] = mapFaction.Color;
					array2 = array;
					array[1] = mapFaction.Color2;
				}
				else
				{
					array2 = this._colorDictionary[mapFaction];
				}
				uint[] array3 = array2;
				this.SpawnCharacter(characterObject, array3, true);
				this._coordinate.x = this._coordinate.x + 1.5f;
			}
		}

		private void SpawnLords()
		{
			foreach (CharacterObject characterObject in (from character in Game.Current.ObjectManager.GetObjectTypeList<CharacterObject>()
				where character.IsHero && !character.HeroObject.IsTemplate
				select character).ToList<CharacterObject>())
			{
				if (this.CheckIfEquipmentSetCountExceedsMapHorizontalEnd(characterObject.AllEquipments.Count))
				{
					this.SetCoordniateNewHorizontalLine();
					this.SetCoordniateNewHorizontalLine();
				}
				IFaction mapFaction = characterObject.HeroObject.MapFaction;
				uint[] array2;
				if (!this._colorDictionary.ContainsKey(mapFaction))
				{
					uint[] array = new uint[2];
					array[0] = mapFaction.Color;
					array2 = array;
					array[1] = mapFaction.Color2;
				}
				else
				{
					array2 = this._colorDictionary[mapFaction];
				}
				uint[] array3 = array2;
				this.SpawnCharacter(characterObject, array3, true);
				this._coordinate.x = this._coordinate.x + 1.5f;
			}
		}

		private void SpawnBandits()
		{
			foreach (CharacterObject characterObject in (from character in Game.Current.ObjectManager.GetObjectTypeList<CharacterObject>()
				where character.Occupation == 15
				select character).ToList<CharacterObject>())
			{
				if (this.CheckIfEquipmentSetCountExceedsMapHorizontalEnd(characterObject.AllEquipments.Count))
				{
					this.SetCoordniateNewHorizontalLine();
					this.SetCoordniateNewHorizontalLine();
				}
				List<Equipment> list = characterObject.AllEquipments.ToList<Equipment>();
				for (int i = 0; i < list.Count; i++)
				{
					MatrixFrame nextSpawnFrame = this.GetNextSpawnFrame();
					Mission mission = base.Mission;
					AgentBuildData agentBuildData = new AgentBuildData(characterObject).TroopOrigin(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).Equipment(list[i]).Team(base.Mission.PlayerTeam)
						.InitialPosition(ref nextSpawnFrame.origin);
					Vec2 vec = nextSpawnFrame.rotation.f.AsVec2;
					vec = vec.Normalized();
					mission.SpawnAgent(agentBuildData.InitialDirection(ref vec).ClothingColor1(4290822336U).ClothingColor2(4285098345U), false);
				}
				this._coordinate.x = this._coordinate.x + 1.5f;
			}
		}

		private void SpawnSoldiers()
		{
			List<CharacterObject> list = new List<CharacterObject>();
			foreach (CharacterObject characterObject in (from character in Game.Current.ObjectManager.GetObjectTypeList<CharacterObject>()
				where character.Occupation == 7 && character.IsBasicTroop
				select character).ToList<CharacterObject>())
			{
				List<CharacterObject> list2 = new List<CharacterObject>();
				this.GetUpgradeTargets(characterObject, ref list2);
				list.AddRange(list2);
			}
			List<CharacterObject> list3 = new List<CharacterObject>();
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = i + 1; j < list.Count; j++)
				{
					if (list[j] == list[i])
					{
						list3.Add(list[j]);
					}
				}
			}
			for (int k = 0; k < list3.Count; k++)
			{
				while (list.Contains(list3[k]))
				{
					list.Remove(list3[k]);
				}
			}
			using (Dictionary<IFaction, uint[]>.Enumerator enumerator2 = this._colorDictionary.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<IFaction, uint[]> kvp = enumerator2.Current;
					List<CharacterObject> list4 = list.FindAll((CharacterObject x) => x.Culture == kvp.Key.Culture);
					uint[] value = kvp.Value;
					foreach (CharacterObject characterObject2 in list4)
					{
						if (this.CheckIfEquipmentSetCountExceedsMapHorizontalEnd(characterObject2.AllEquipments.Count))
						{
							this.SetCoordniateNewHorizontalLine();
						}
						this.SpawnCharacter(characterObject2, value, true);
						this._coordinate.x = this._coordinate.x + 1.5f;
					}
				}
			}
		}

		private void SpawnOneTypeSoldier()
		{
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>(this._oneTypeSoldierStringId);
			for (int i = 0; i < this.OneTypeSoldierSpawnCount; i++)
			{
				MatrixFrame nextSpawnFrame = this.GetNextSpawnFrame();
				Kingdom kingdom = Campaign.Current.CampaignObjectManager.Find<Kingdom>(@object.Culture.StringId);
				uint[] array2;
				if (!this._colorDictionary.ContainsKey(kingdom))
				{
					uint[] array = new uint[2];
					array[0] = uint.MaxValue;
					array2 = array;
					array[1] = uint.MaxValue;
				}
				else
				{
					array2 = this._colorDictionary[kingdom];
				}
				uint[] array3 = array2;
				Mission mission = base.Mission;
				AgentBuildData agentBuildData = new AgentBuildData(@object).Team(base.Mission.PlayerTeam).TroopOrigin(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).InitialPosition(ref nextSpawnFrame.origin);
				Vec2 vec = nextSpawnFrame.rotation.f.AsVec2;
				vec = vec.Normalized();
				mission.SpawnAgent(agentBuildData.InitialDirection(ref vec).ClothingColor1(array3[0]).ClothingColor2(array3[1]), false);
			}
		}

		private void SpawnCharacter(CharacterObject character, uint[] colors, bool spawnAllEquipmentSets = true)
		{
			List<Equipment> list = character.AllEquipments.ToList<Equipment>();
			for (int i = 0; i < list.Count; i++)
			{
				if (this._testTypeId == 0 && list[i].IsCivilian)
				{
					MatrixFrame nextSpawnFrame = this.GetNextSpawnFrame();
					Mission mission = base.Mission;
					AgentBuildData agentBuildData = new AgentBuildData(character).TroopOrigin(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Equipment(list[i]).Team(base.Mission.PlayerTeam)
						.InitialPosition(ref nextSpawnFrame.origin);
					Vec2 vec = nextSpawnFrame.rotation.f.AsVec2;
					vec = vec.Normalized();
					mission.SpawnAgent(agentBuildData.InitialDirection(ref vec).ClothingColor1(colors[0]).ClothingColor2(colors[1])
						.NoHorses(true), false);
					if (!spawnAllEquipmentSets)
					{
						return;
					}
				}
				else if (this._testTypeId == 1 && !list[i].IsCivilian)
				{
					if (!list[i].IsCivilian)
					{
						MatrixFrame nextSpawnFrame2 = this.GetNextSpawnFrame();
						Mission mission2 = base.Mission;
						AgentBuildData agentBuildData2 = new AgentBuildData(character).Equipment(list[i]).TroopOrigin(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam)
							.InitialPosition(ref nextSpawnFrame2.origin);
						Vec2 vec = nextSpawnFrame2.rotation.f.AsVec2;
						vec = vec.Normalized();
						mission2.SpawnAgent(agentBuildData2.InitialDirection(ref vec).ClothingColor1(colors[0]).ClothingColor2(colors[1])
							.NoHorses(true), false);
						if (!spawnAllEquipmentSets)
						{
							return;
						}
					}
				}
				else if (this._testTypeId == 2)
				{
					MatrixFrame nextSpawnFrame3 = this.GetNextSpawnFrame();
					Mission mission3 = base.Mission;
					AgentBuildData agentBuildData3 = new AgentBuildData(character).Equipment(list[i]).Team(base.Mission.PlayerTeam).InitialPosition(ref nextSpawnFrame3.origin);
					Vec2 vec = nextSpawnFrame3.rotation.f.AsVec2;
					vec = vec.Normalized();
					mission3.SpawnAgent(agentBuildData3.InitialDirection(ref vec).ClothingColor1(colors[0]).ClothingColor2(colors[1])
						.TroopOrigin(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))), false);
					if (!spawnAllEquipmentSets)
					{
						return;
					}
				}
			}
		}

		private MatrixFrame GetNextSpawnFrame()
		{
			this._coordinate += new Vec3(1.5f, 0f, 0f, -1f);
			if (this._coordinate.x > (float)this._mapHorizontalEndCoordinate)
			{
				this.SetCoordniateNewHorizontalLine();
			}
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = this._coordinate;
			identity.rotation.RotateAboutUp(3.1415927f);
			return identity;
		}

		private void SetCoordniateNewHorizontalLine()
		{
			this._coordinate.x = 3f;
			this._coordinate.y = this._coordinate.y + 3f;
		}

		private bool CheckIfEquipmentSetCountExceedsMapHorizontalEnd(int equipmentSetCount)
		{
			return this._coordinate.x + 1.5f * (float)equipmentSetCount > (float)this._mapHorizontalEndCoordinate;
		}

		private void GetUpgradeTargets(CharacterObject troop, ref List<CharacterObject> list)
		{
			list.Add(troop);
			if (troop.UpgradeTargets != null)
			{
				foreach (CharacterObject characterObject in troop.UpgradeTargets)
				{
					this.GetUpgradeTargets(characterObject, ref list);
				}
			}
		}

		private const float HorizontalGap = 1.5f;

		private const float VerticalGap = 3f;

		private readonly string[] _factionIdList = new string[] { "vlandia", "empire", "empire_w", "empire_s", "sturgia", "aserai", "battania", "khuzait" };

		private readonly bool _spawnLords;

		private readonly bool _spawnSoldiers;

		private readonly bool _spawnBandits;

		private readonly bool _spawnLadies;

		private readonly bool _spawnHelmetTestCharacters;

		private string _oneTypeSoldierStringId;

		private int _testTypeId;

		private Vec3 _coordinate = new Vec3(3f, 3f, 0f, -1f);

		private int _mapHorizontalEndCoordinate = 50;

		private int OneTypeSoldierSpawnCount = 100;

		private Dictionary<IFaction, uint[]> _colorDictionary;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	public static class CustomBattleHelper
	{
		public static void StartGame(CustomBattleData data)
		{
			Game.Current.PlayerTroop = data.PlayerCharacter;
			if (data.GameType == CustomBattleGameType.Siege)
			{
				BannerlordMissions.OpenSiegeMissionWithDeployment(data.SceneId, data.PlayerCharacter, data.PlayerParty, data.EnemyParty, data.IsPlayerGeneral, data.WallHitpointPercentages, data.HasAnySiegeTower, data.AttackerMachines, data.DefenderMachines, data.IsPlayerAttacker, data.SceneUpgradeLevel, data.SeasonId, data.IsSallyOut, data.IsReliefAttack, data.TimeOfDay);
				return;
			}
			BannerlordMissions.OpenCustomBattleMission(data.SceneId, data.PlayerCharacter, data.PlayerParty, data.EnemyParty, data.IsPlayerGeneral, data.PlayerSideGeneralCharacter, data.SceneLevel, data.SeasonId, data.TimeOfDay);
		}

		public static int[] GetTroopCounts(int armySize, CustomBattleCompositionData compositionData)
		{
			int[] array = new int[4];
			armySize--;
			array[1] = MathF.Round(compositionData.RangedPercentage * (float)armySize);
			array[2] = MathF.Round(compositionData.CavalryPercentage * (float)armySize);
			array[3] = MathF.Round(compositionData.RangedCavalryPercentage * (float)armySize);
			array[0] = armySize - array.Sum();
			return array;
		}

		public static float[] GetWallHitpointPercentages(int breachedWallCount)
		{
			float[] array = new float[2];
			if (breachedWallCount == 1)
			{
				int num = MBRandom.RandomInt(2);
				array[num] = 0f;
				array[1 - num] = 1f;
			}
			else if (breachedWallCount == 0)
			{
				array[0] = 1f;
				array[1] = 1f;
			}
			else
			{
				array[0] = 0f;
				array[1] = 0f;
			}
			return array;
		}

		public static SiegeEngineType GetSiegeWeaponType(SiegeEngineType siegeWeaponType)
		{
			if (siegeWeaponType == DefaultSiegeEngineTypes.Ladder)
			{
				return DefaultSiegeEngineTypes.Ladder;
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.Ballista)
			{
				return DefaultSiegeEngineTypes.Ballista;
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.FireBallista)
			{
				return DefaultSiegeEngineTypes.FireBallista;
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.Ram || siegeWeaponType == DefaultSiegeEngineTypes.ImprovedRam)
			{
				return DefaultSiegeEngineTypes.Ram;
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.SiegeTower)
			{
				return DefaultSiegeEngineTypes.SiegeTower;
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.Onager || siegeWeaponType == DefaultSiegeEngineTypes.Catapult)
			{
				return DefaultSiegeEngineTypes.Onager;
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.FireOnager || siegeWeaponType == DefaultSiegeEngineTypes.FireCatapult)
			{
				return DefaultSiegeEngineTypes.FireOnager;
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.Trebuchet || siegeWeaponType == DefaultSiegeEngineTypes.Bricole)
			{
				return DefaultSiegeEngineTypes.Trebuchet;
			}
			return siegeWeaponType;
		}

		public static CustomBattleData PrepareBattleData(BasicCharacterObject playerCharacter, BasicCharacterObject playerSideGeneralCharacter, CustomBattleCombatant playerParty, CustomBattleCombatant enemyParty, CustomBattlePlayerSide playerSide, CustomBattlePlayerType battlePlayerType, CustomBattleGameType gameType, string scene, string season, float timeOfDay, List<MissionSiegeWeapon> attackerMachines, List<MissionSiegeWeapon> defenderMachines, float[] wallHitPointsPercentages, int sceneLevel, bool isSallyOut)
		{
			bool flag = playerSide == CustomBattlePlayerSide.Attacker;
			bool flag2 = battlePlayerType == CustomBattlePlayerType.Commander;
			CustomBattleData customBattleData = new CustomBattleData
			{
				GameType = gameType,
				SceneId = scene,
				PlayerCharacter = playerCharacter,
				PlayerParty = playerParty,
				EnemyParty = enemyParty,
				IsPlayerGeneral = flag2,
				PlayerSideGeneralCharacter = playerSideGeneralCharacter,
				SeasonId = season,
				SceneLevel = "",
				TimeOfDay = timeOfDay
			};
			if (customBattleData.GameType == CustomBattleGameType.Siege)
			{
				customBattleData.AttackerMachines = attackerMachines;
				customBattleData.DefenderMachines = defenderMachines;
				customBattleData.WallHitpointPercentages = wallHitPointsPercentages;
				customBattleData.HasAnySiegeTower = attackerMachines.Exists((MissionSiegeWeapon mm) => mm.Type == DefaultSiegeEngineTypes.SiegeTower);
				customBattleData.IsPlayerAttacker = flag;
				customBattleData.SceneUpgradeLevel = sceneLevel;
				customBattleData.IsSallyOut = isSallyOut;
				customBattleData.IsReliefAttack = false;
			}
			return customBattleData;
		}

		public static CustomBattleCombatant[] GetCustomBattleParties(BasicCharacterObject playerCharacter, BasicCharacterObject playerSideGeneralCharacter, BasicCharacterObject enemyCharacter, BasicCultureObject playerFaction, int[] playerNumbers, List<BasicCharacterObject>[] playerTroopSelections, BasicCultureObject enemyFaction, int[] enemyNumbers, List<BasicCharacterObject>[] enemyTroopSelections, bool isPlayerAttacker)
		{
			Banner banner = new Banner(playerFaction.BannerKey, playerFaction.Color, playerFaction.Color2);
			Banner banner2 = new Banner(enemyFaction.BannerKey, enemyFaction.Color, enemyFaction.Color2);
			if (playerFaction.StringId == enemyFaction.StringId)
			{
				uint primaryColor = banner2.GetPrimaryColor();
				banner2.ChangePrimaryColor(banner2.GetFirstIconColor());
				banner2.ChangeIconColors(primaryColor);
			}
			CustomBattleCombatant[] array = new CustomBattleCombatant[]
			{
				new CustomBattleCombatant(new TextObject("{=sSJSTe5p}Player Party", null), playerFaction, banner),
				new CustomBattleCombatant(new TextObject("{=0xC75dN6}Enemy Party", null), enemyFaction, banner2)
			};
			array[0].Side = (isPlayerAttacker ? 1 : 0);
			array[0].AddCharacter(playerCharacter, 1);
			if (playerSideGeneralCharacter != null)
			{
				array[0].AddCharacter(playerSideGeneralCharacter, 1);
				array[0].SetGeneral(playerSideGeneralCharacter);
			}
			else
			{
				array[0].SetGeneral(playerCharacter);
			}
			array[1].Side = Extensions.GetOppositeSide(array[0].Side);
			array[1].AddCharacter(enemyCharacter, 1);
			for (int i = 0; i < array.Length; i++)
			{
				CustomBattleHelper.PopulateListsWithDefaults(ref array[i], (i == 0) ? playerNumbers : enemyNumbers, (i == 0) ? playerTroopSelections : enemyTroopSelections);
			}
			return array;
		}

		private static void PopulateListsWithDefaults(ref CustomBattleCombatant customBattleParties, int[] numbers, List<BasicCharacterObject>[] troopList)
		{
			BasicCultureObject basicCulture = customBattleParties.BasicCulture;
			if (troopList == null)
			{
				troopList = new List<BasicCharacterObject>[]
				{
					new List<BasicCharacterObject>(),
					new List<BasicCharacterObject>(),
					new List<BasicCharacterObject>(),
					new List<BasicCharacterObject>()
				};
			}
			if (troopList[0].Count == 0)
			{
				troopList[0] = new List<BasicCharacterObject> { CustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCulture, 0) };
			}
			if (troopList[1].Count == 0)
			{
				troopList[1] = new List<BasicCharacterObject> { CustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCulture, 1) };
			}
			if (troopList[2].Count == 0)
			{
				troopList[2] = new List<BasicCharacterObject> { CustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCulture, 2) };
			}
			if (troopList[3].Count == 0)
			{
				troopList[3] = new List<BasicCharacterObject> { CustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCulture, 3) };
			}
			if (troopList[3].Count != 0)
			{
				if (!troopList[3].All((BasicCharacterObject troop) => troop == null))
				{
					goto IL_12C;
				}
			}
			numbers[2] += numbers[3] / 3;
			numbers[1] += numbers[3] / 3;
			numbers[0] += numbers[3] / 3;
			numbers[0] += numbers[3] - numbers[3] / 3 * 3;
			numbers[3] = 0;
			IL_12C:
			for (int i = 0; i < 4; i++)
			{
				int count = troopList[i].Count;
				int num = numbers[i];
				if (num > 0)
				{
					float num2 = (float)num / (float)count;
					float num3 = 0f;
					for (int j = 0; j < count; j++)
					{
						float num4 = num2 + num3;
						int num5 = MathF.Floor(num4);
						num3 = num4 - (float)num5;
						customBattleParties.AddCharacter(troopList[i][j], num5);
						numbers[i] -= num5;
						if (j == count - 1 && numbers[i] > 0)
						{
							customBattleParties.AddCharacter(troopList[i][j], numbers[i]);
							numbers[i] = 0;
						}
					}
				}
			}
		}

		public static void AssertMissingTroopsForDebug()
		{
			foreach (BasicCultureObject basicCultureObject in MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>())
			{
				for (int i = 0; i < 4; i++)
				{
					CustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCultureObject, i);
				}
			}
		}

		public static BasicCharacterObject GetDefaultTroopOfFormationForFaction(BasicCultureObject culture, FormationClass formation)
		{
			if (culture.StringId.ToLower() == "empire")
			{
				switch (formation)
				{
				case 0:
					return CustomBattleHelper.GetTroopFromId("imperial_veteran_infantryman");
				case 1:
					return CustomBattleHelper.GetTroopFromId("imperial_archer");
				case 2:
					return CustomBattleHelper.GetTroopFromId("imperial_heavy_horseman");
				case 3:
					return CustomBattleHelper.GetTroopFromId("bucellarii");
				}
			}
			else if (culture.StringId.ToLower() == "sturgia")
			{
				switch (formation)
				{
				case 0:
					return CustomBattleHelper.GetTroopFromId("sturgian_spearman");
				case 1:
					return CustomBattleHelper.GetTroopFromId("sturgian_archer");
				case 2:
					return CustomBattleHelper.GetTroopFromId("sturgian_hardened_brigand");
				}
			}
			else if (culture.StringId.ToLower() == "aserai")
			{
				switch (formation)
				{
				case 0:
					return CustomBattleHelper.GetTroopFromId("aserai_infantry");
				case 1:
					return CustomBattleHelper.GetTroopFromId("aserai_archer");
				case 2:
					return CustomBattleHelper.GetTroopFromId("aserai_mameluke_cavalry");
				case 3:
					return CustomBattleHelper.GetTroopFromId("aserai_faris");
				}
			}
			else if (culture.StringId.ToLower() == "vlandia")
			{
				switch (formation)
				{
				case 0:
					return CustomBattleHelper.GetTroopFromId("vlandian_swordsman");
				case 1:
					return CustomBattleHelper.GetTroopFromId("vlandian_hardened_crossbowman");
				case 2:
					return CustomBattleHelper.GetTroopFromId("vlandian_knight");
				}
			}
			else if (culture.StringId.ToLower() == "battania")
			{
				switch (formation)
				{
				case 0:
					return CustomBattleHelper.GetTroopFromId("battanian_picked_warrior");
				case 1:
					return CustomBattleHelper.GetTroopFromId("battanian_hero");
				case 2:
					return CustomBattleHelper.GetTroopFromId("battanian_scout");
				}
			}
			else if (culture.StringId.ToLower() == "khuzait")
			{
				switch (formation)
				{
				case 0:
					return CustomBattleHelper.GetTroopFromId("khuzait_spear_infantry");
				case 1:
					return CustomBattleHelper.GetTroopFromId("khuzait_archer");
				case 2:
					return CustomBattleHelper.GetTroopFromId("khuzait_lancer");
				case 3:
					return CustomBattleHelper.GetTroopFromId("khuzait_horse_archer");
				}
			}
			return null;
		}

		public static BasicCharacterObject GetBannermanTroopOfFormationForFaction(BasicCultureObject culture, FormationClass formation)
		{
			if (culture.StringId.ToLower() == "empire")
			{
				if (formation == null)
				{
					return CustomBattleHelper.GetTroopFromId("imperial_infantry_banner_bearer");
				}
				if (formation == 2)
				{
					return CustomBattleHelper.GetTroopFromId("imperial_cavalry_banner_bearer");
				}
			}
			else if (culture.StringId.ToLower() == "sturgia")
			{
				if (formation == null)
				{
					return CustomBattleHelper.GetTroopFromId("sturgian_infantry_banner_bearer");
				}
				if (formation == 2)
				{
					return CustomBattleHelper.GetTroopFromId("sturgian_cavalry_banner_bearer");
				}
			}
			else if (culture.StringId.ToLower() == "aserai")
			{
				if (formation == null)
				{
					return CustomBattleHelper.GetTroopFromId("aserai_infantry_banner_bearer");
				}
				if (formation == 2)
				{
					return CustomBattleHelper.GetTroopFromId("aserai_cavalry_banner_bearer");
				}
			}
			else if (culture.StringId.ToLower() == "vlandia")
			{
				if (formation == null)
				{
					return CustomBattleHelper.GetTroopFromId("vlandian_infantry_banner_bearer");
				}
				if (formation == 2)
				{
					return CustomBattleHelper.GetTroopFromId("vlandian_cavalry_banner_bearer");
				}
			}
			else if (culture.StringId.ToLower() == "battania")
			{
				if (formation == null)
				{
					return CustomBattleHelper.GetTroopFromId("battanian_woodrunner");
				}
				if (formation == 2)
				{
					return CustomBattleHelper.GetTroopFromId("battanian_cavalry_banner_bearer");
				}
			}
			else if (culture.StringId.ToLower() == "khuzait")
			{
				if (formation == null)
				{
					return CustomBattleHelper.GetTroopFromId("khuzait_infantry_banner_bearer");
				}
				if (formation == 2)
				{
					return CustomBattleHelper.GetTroopFromId("khuzait_cavalry_banner_bearer");
				}
			}
			return null;
		}

		private static BasicCharacterObject GetTroopFromId(string troopId)
		{
			return MBObjectManager.Instance.GetObject<BasicCharacterObject>(troopId);
		}

		private const string EmpireInfantryTroop = "imperial_veteran_infantryman";

		private const string EmpireRangedTroop = "imperial_archer";

		private const string EmpireCavalryTroop = "imperial_heavy_horseman";

		private const string EmpireHorseArcherTroop = "bucellarii";

		private const string EmpireInfantryBannermanTroop = "imperial_infantry_banner_bearer";

		private const string EmpireCavalryBannermanTroop = "imperial_cavalry_banner_bearer";

		private const string SturgiaInfantryTroop = "sturgian_spearman";

		private const string SturgiaRangedTroop = "sturgian_archer";

		private const string SturgiaCavalryTroop = "sturgian_hardened_brigand";

		private const string SturgiaInfantryBannermanTroop = "sturgian_infantry_banner_bearer";

		private const string SturgiaCavalryBannermanTroop = "sturgian_cavalry_banner_bearer";

		private const string AseraiInfantryTroop = "aserai_infantry";

		private const string AseraiRangedTroop = "aserai_archer";

		private const string AseraiCavalryTroop = "aserai_mameluke_cavalry";

		private const string AseraiHorseArcherTroop = "aserai_faris";

		private const string AseraiInfantryBannermanTroop = "aserai_infantry_banner_bearer";

		private const string AseraiCavalryBannermanTroop = "aserai_cavalry_banner_bearer";

		private const string VlandiaInfantryTroop = "vlandian_swordsman";

		private const string VlandiaRangedTroop = "vlandian_hardened_crossbowman";

		private const string VlandiaCavalryTroop = "vlandian_knight";

		private const string VlandiaInfantryBannermanTroop = "vlandian_infantry_banner_bearer";

		private const string VlandiaCavalryBannermanTroop = "vlandian_cavalry_banner_bearer";

		private const string BattaniaInfantryTroop = "battanian_picked_warrior";

		private const string BattaniaRangedTroop = "battanian_hero";

		private const string BattaniaCavalryTroop = "battanian_scout";

		private const string BattaniaInfantryBannermanTroop = "battanian_woodrunner";

		private const string BattaniaCavalryBannermanTroop = "battanian_cavalry_banner_bearer";

		private const string KhuzaitInfantryTroop = "khuzait_spear_infantry";

		private const string KhuzaitRangedTroop = "khuzait_archer";

		private const string KhuzaitCavalryTroop = "khuzait_lancer";

		private const string KhuzaitHorseArcherTroop = "khuzait_horse_archer";

		private const string KhuzaitInfantryBannermanTroop = "khuzait_infantry_banner_bearer";

		private const string KhuzaitCavalryBannermanTroop = "khuzait_cavalry_banner_bearer";
	}
}

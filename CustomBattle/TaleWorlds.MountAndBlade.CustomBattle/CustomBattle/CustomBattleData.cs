using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	public struct CustomBattleData
	{
		public static IEnumerable<SiegeEngineType> GetAllAttackerMeleeMachines()
		{
			yield return DefaultSiegeEngineTypes.Ram;
			yield return DefaultSiegeEngineTypes.SiegeTower;
			yield break;
		}

		public static IEnumerable<SiegeEngineType> GetAllDefenderRangedMachines()
		{
			yield return DefaultSiegeEngineTypes.Ballista;
			yield return DefaultSiegeEngineTypes.FireBallista;
			yield return DefaultSiegeEngineTypes.Catapult;
			yield return DefaultSiegeEngineTypes.FireCatapult;
			yield break;
		}

		public static IEnumerable<SiegeEngineType> GetAllAttackerRangedMachines()
		{
			yield return DefaultSiegeEngineTypes.Ballista;
			yield return DefaultSiegeEngineTypes.FireBallista;
			yield return DefaultSiegeEngineTypes.Onager;
			yield return DefaultSiegeEngineTypes.FireOnager;
			yield return DefaultSiegeEngineTypes.Trebuchet;
			yield break;
		}

		public static IEnumerable<Tuple<string, CustomBattleGameType>> GameTypes
		{
			get
			{
				yield return new Tuple<string, CustomBattleGameType>(GameTexts.FindText("str_battle", null).ToString(), CustomBattleGameType.Battle);
				if (!Module.CurrentModule.IsOnlyCoreContentEnabled)
				{
					yield return new Tuple<string, CustomBattleGameType>(new TextObject("{=Ua6CNLBZ}Village", null).ToString(), CustomBattleGameType.Village);
					yield return new Tuple<string, CustomBattleGameType>(GameTexts.FindText("str_siege", null).ToString(), CustomBattleGameType.Siege);
				}
				yield break;
			}
		}

		public static IEnumerable<Tuple<string, CustomBattlePlayerType>> PlayerTypes
		{
			get
			{
				yield return new Tuple<string, CustomBattlePlayerType>(GameTexts.FindText("str_team_commander", null).ToString(), CustomBattlePlayerType.Commander);
				if (!Module.CurrentModule.IsOnlyCoreContentEnabled)
				{
					yield return new Tuple<string, CustomBattlePlayerType>(new TextObject("{=g9VIbA9s}Sergeant", null).ToString(), CustomBattlePlayerType.Sergeant);
				}
				yield break;
			}
		}

		public static IEnumerable<Tuple<string, CustomBattlePlayerSide>> PlayerSides
		{
			get
			{
				yield return new Tuple<string, CustomBattlePlayerSide>(new TextObject("{=XEVFUaFj}Defender", null).ToString(), CustomBattlePlayerSide.Defender);
				yield return new Tuple<string, CustomBattlePlayerSide>(new TextObject("{=KASD0tnO}Attacker", null).ToString(), CustomBattlePlayerSide.Attacker);
				yield break;
			}
		}

		public static IEnumerable<BasicCharacterObject> Characters
		{
			get
			{
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_1");
				if (!Module.CurrentModule.IsOnlyCoreContentEnabled)
				{
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_2");
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_3");
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_4");
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_5");
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_6");
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_7");
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_8");
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_9");
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_10");
				}
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_11");
				if (!Module.CurrentModule.IsOnlyCoreContentEnabled)
				{
					yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_12");
				}
				yield break;
			}
		}

		public static IEnumerable<BasicCultureObject> Factions
		{
			get
			{
				yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("empire");
				if (!Module.CurrentModule.IsOnlyCoreContentEnabled)
				{
					yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("sturgia");
					yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("aserai");
					yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("vlandia");
					yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("battania");
					yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("khuzait");
				}
				yield break;
			}
		}

		public static IEnumerable<Tuple<string, CustomBattleTimeOfDay>> TimesOfDay
		{
			get
			{
				yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=X3gcUz7C}Morning", null).ToString(), CustomBattleTimeOfDay.Morning);
				yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=CTtjSwRb}Noon", null).ToString(), CustomBattleTimeOfDay.Noon);
				yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=J2gvnexb}Afternoon", null).ToString(), CustomBattleTimeOfDay.Afternoon);
				yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=gENb9SSW}Evening", null).ToString(), CustomBattleTimeOfDay.Evening);
				yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=fAxjyMt5}Night", null).ToString(), CustomBattleTimeOfDay.Night);
				yield break;
			}
		}

		public static IEnumerable<Tuple<string, string>> Seasons
		{
			get
			{
				yield return new Tuple<string, string>(new TextObject("{=f7vOVQb7}Summer", null).ToString(), "summer");
				yield return new Tuple<string, string>(new TextObject("{=cZzfNlxd}Fall", null).ToString(), "fall");
				yield return new Tuple<string, string>(new TextObject("{=nwqUFaU8}Winter", null).ToString(), "winter");
				yield return new Tuple<string, string>(new TextObject("{=nWbp3o3H}Spring", null).ToString(), "spring");
				yield break;
			}
		}

		public static IEnumerable<Tuple<string, int>> WallHitpoints
		{
			get
			{
				yield return new Tuple<string, int>(new TextObject("{=dsMeB3vi}Solid", null).ToString(), 0);
				yield return new Tuple<string, int>(new TextObject("{=Kvxo2jzJ}Single Breached", null).ToString(), 1);
				yield return new Tuple<string, int>(new TextObject("{=AiNXIt5N}Dual Breached", null).ToString(), 2);
				yield break;
			}
		}

		public static IEnumerable<int> SceneLevels
		{
			get
			{
				yield return 1;
				yield return 2;
				yield return 3;
				yield break;
			}
		}

		public const int NumberOfAttackerMeleeMachines = 3;

		public const int NumberOfAttackerRangedMachines = 4;

		public const int NumberOfDefenderRangedMachines = 4;

		public const string CoreContentDefaultSceneName = "battle_terrain_029";

		public CustomBattleGameType GameType;

		public string SceneId;

		public string SeasonId;

		public BasicCharacterObject PlayerCharacter;

		public BasicCharacterObject PlayerSideGeneralCharacter;

		public CustomBattleCombatant PlayerParty;

		public CustomBattleCombatant EnemyParty;

		public float TimeOfDay;

		public bool IsPlayerGeneral;

		public string SceneLevel;

		public List<MissionSiegeWeapon> AttackerMachines;

		public List<MissionSiegeWeapon> DefenderMachines;

		public float[] WallHitpointPercentages;

		public bool HasAnySiegeTower;

		public bool IsPlayerAttacker;

		public bool IsReliefAttack;

		public bool IsSallyOut;

		public int SceneUpgradeLevel;
	}
}

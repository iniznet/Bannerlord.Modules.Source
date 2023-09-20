using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	// Token: 0x02000017 RID: 23
	public struct CustomBattleData
	{
		// Token: 0x060000FD RID: 253 RVA: 0x00008347 File Offset: 0x00006547
		public static IEnumerable<SiegeEngineType> GetAllAttackerMeleeMachines()
		{
			yield return DefaultSiegeEngineTypes.Ram;
			yield return DefaultSiegeEngineTypes.SiegeTower;
			yield break;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00008350 File Offset: 0x00006550
		public static IEnumerable<SiegeEngineType> GetAllDefenderRangedMachines()
		{
			yield return DefaultSiegeEngineTypes.Ballista;
			yield return DefaultSiegeEngineTypes.FireBallista;
			yield return DefaultSiegeEngineTypes.Catapult;
			yield return DefaultSiegeEngineTypes.FireCatapult;
			yield break;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00008359 File Offset: 0x00006559
		public static IEnumerable<SiegeEngineType> GetAllAttackerRangedMachines()
		{
			yield return DefaultSiegeEngineTypes.Ballista;
			yield return DefaultSiegeEngineTypes.FireBallista;
			yield return DefaultSiegeEngineTypes.Onager;
			yield return DefaultSiegeEngineTypes.FireOnager;
			yield return DefaultSiegeEngineTypes.Trebuchet;
			yield break;
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00008362 File Offset: 0x00006562
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000101 RID: 257 RVA: 0x0000836B File Offset: 0x0000656B
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

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00008374 File Offset: 0x00006574
		public static IEnumerable<Tuple<string, CustomBattlePlayerSide>> PlayerSides
		{
			get
			{
				yield return new Tuple<string, CustomBattlePlayerSide>(new TextObject("{=XEVFUaFj}Defender", null).ToString(), CustomBattlePlayerSide.Defender);
				yield return new Tuple<string, CustomBattlePlayerSide>(new TextObject("{=KASD0tnO}Attacker", null).ToString(), CustomBattlePlayerSide.Attacker);
				yield break;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000103 RID: 259 RVA: 0x0000837D File Offset: 0x0000657D
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00008386 File Offset: 0x00006586
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

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000105 RID: 261 RVA: 0x0000838F File Offset: 0x0000658F
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000106 RID: 262 RVA: 0x00008398 File Offset: 0x00006598
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

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000107 RID: 263 RVA: 0x000083A1 File Offset: 0x000065A1
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

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000108 RID: 264 RVA: 0x000083AA File Offset: 0x000065AA
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

		// Token: 0x04000099 RID: 153
		public const int NumberOfAttackerMeleeMachines = 3;

		// Token: 0x0400009A RID: 154
		public const int NumberOfAttackerRangedMachines = 4;

		// Token: 0x0400009B RID: 155
		public const int NumberOfDefenderRangedMachines = 4;

		// Token: 0x0400009C RID: 156
		public const string CoreContentDefaultSceneName = "battle_terrain_029";

		// Token: 0x0400009D RID: 157
		public CustomBattleGameType GameType;

		// Token: 0x0400009E RID: 158
		public string SceneId;

		// Token: 0x0400009F RID: 159
		public string SeasonId;

		// Token: 0x040000A0 RID: 160
		public BasicCharacterObject PlayerCharacter;

		// Token: 0x040000A1 RID: 161
		public BasicCharacterObject PlayerSideGeneralCharacter;

		// Token: 0x040000A2 RID: 162
		public CustomBattleCombatant PlayerParty;

		// Token: 0x040000A3 RID: 163
		public CustomBattleCombatant EnemyParty;

		// Token: 0x040000A4 RID: 164
		public float TimeOfDay;

		// Token: 0x040000A5 RID: 165
		public bool IsPlayerGeneral;

		// Token: 0x040000A6 RID: 166
		public string SceneLevel;

		// Token: 0x040000A7 RID: 167
		public List<MissionSiegeWeapon> AttackerMachines;

		// Token: 0x040000A8 RID: 168
		public List<MissionSiegeWeapon> DefenderMachines;

		// Token: 0x040000A9 RID: 169
		public float[] WallHitpointPercentages;

		// Token: 0x040000AA RID: 170
		public bool HasAnySiegeTower;

		// Token: 0x040000AB RID: 171
		public bool IsPlayerAttacker;

		// Token: 0x040000AC RID: 172
		public bool IsReliefAttack;

		// Token: 0x040000AD RID: 173
		public bool IsSallyOut;

		// Token: 0x040000AE RID: 174
		public int SceneUpgradeLevel;
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200030C RID: 780
	public class MultiplayerOptions
	{
		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06002A34 RID: 10804 RVA: 0x000A34D5 File Offset: 0x000A16D5
		public static MultiplayerOptions Instance
		{
			get
			{
				MultiplayerOptions multiplayerOptions;
				if ((multiplayerOptions = MultiplayerOptions._instance) == null)
				{
					multiplayerOptions = (MultiplayerOptions._instance = new MultiplayerOptions());
				}
				return multiplayerOptions;
			}
		}

		// Token: 0x06002A35 RID: 10805 RVA: 0x000A34EC File Offset: 0x000A16EC
		public MultiplayerOptions()
		{
			this._current = new MultiplayerOptions.MultiplayerOptionsContainer();
			this._next = new MultiplayerOptions.MultiplayerOptionsContainer();
			this._ui = new MultiplayerOptions.MultiplayerOptionsContainer();
			MultiplayerOptions.MultiplayerOptionsContainer container = this.GetContainer(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				container.CreateOption(optionType);
			}
			List<MultiplayerGameTypeInfo> multiplayerGameTypes = Module.CurrentModule.GetMultiplayerGameTypes();
			if (multiplayerGameTypes.Count > 0)
			{
				MultiplayerGameTypeInfo multiplayerGameTypeInfo = multiplayerGameTypes[0];
				container.UpdateOptionValue(MultiplayerOptions.OptionType.GameType, multiplayerGameTypeInfo.GameType);
				container.UpdateOptionValue(MultiplayerOptions.OptionType.PremadeMatchGameMode, multiplayerGameTypes.First((MultiplayerGameTypeInfo info) => info.GameType == "Skirmish").GameType);
				container.UpdateOptionValue(MultiplayerOptions.OptionType.Map, multiplayerGameTypeInfo.Scenes.FirstOrDefault<string>());
			}
			container.UpdateOptionValue(MultiplayerOptions.OptionType.CultureTeam1, MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>()[0].StringId);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.CultureTeam2, MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>()[2].StringId);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.MaxNumberOfPlayers, 120);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart, 1);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.WarmupTimeLimit, 5);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.MapTimeLimit, 30);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.RoundTimeLimit, 120);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.RoundPreparationTimeLimit, 10);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.RoundTotal, 1);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.RespawnPeriodTeam1, 3);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.RespawnPeriodTeam2, 3);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.MinScoreToWinMatch, 120000);
			container.UpdateOptionValue(MultiplayerOptions.OptionType.AutoTeamBalanceThreshold, 0);
			this._current.CopyAllValuesTo(this._next);
			this._current.CopyAllValuesTo(this._ui);
		}

		// Token: 0x06002A36 RID: 10806 RVA: 0x000A3672 File Offset: 0x000A1872
		internal static void Release()
		{
			MultiplayerOptions._instance = null;
		}

		// Token: 0x06002A37 RID: 10807 RVA: 0x000A367A File Offset: 0x000A187A
		public MultiplayerOptions.MultiplayerOption GetOptionFromOptionType(MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			return this.GetContainer(mode).GetOptionFromOptionType(optionType);
		}

		// Token: 0x06002A38 RID: 10808 RVA: 0x000A368C File Offset: 0x000A188C
		public void OnGameTypeChanged(MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			string text = "";
			if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
			{
				text = MultiplayerOptions.OptionType.GameType.GetStrValue(mode);
			}
			else if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.PremadeMatch)
			{
				text = MultiplayerOptions.OptionType.PremadeMatchGameMode.GetStrValue(mode);
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2173941516U)
			{
				if (num != 650645549U)
				{
					if (num != 1201370427U)
					{
						if (num == 2173941516U)
						{
							if (text == "Siege")
							{
								this.InitializeForSiege(mode);
							}
						}
					}
					else if (text == "Duel")
					{
						this.InitializeForDuel(mode);
					}
				}
				else if (text == "Skirmish")
				{
					this.InitializeForSkirmish(mode);
				}
			}
			else if (num <= 2508183895U)
			{
				if (num != 2298111883U)
				{
					if (num == 2508183895U)
					{
						if (text == "Battle")
						{
							this.InitializeForBattle(mode);
						}
					}
				}
				else if (text == "Captain")
				{
					this.InitializeForCaptain(mode);
				}
			}
			else if (num != 2569833005U)
			{
				if (num == 4010700071U)
				{
					if (text == "TeamDeathmatch")
					{
						this.InitializeForTeamDeathmatch(mode);
					}
				}
			}
			else if (text == "FreeForAll")
			{
				this.InitializeForFreeForAll(mode);
			}
			MBList<string> mapList = this.GetMapList();
			if (mapList.Count > 0)
			{
				MultiplayerOptions.OptionType.Map.SetValue(mapList[0], MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
		}

		// Token: 0x06002A39 RID: 10809 RVA: 0x000A37F0 File Offset: 0x000A19F0
		private void InitializeForFreeForAll(MultiplayerOptions.MultiplayerOptionsAccessMode mode)
		{
			string text = "FreeForAll";
			MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(this.GetNumberOfPlayersForGameMode(text), mode);
			MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.SpectatorCamera.SetValue(0, mode);
			MultiplayerOptions.OptionType.MapTimeLimit.SetValue(this.GetRoundTimeLimitInMinutesForGameMode(text), mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
			MultiplayerOptions.OptionType.MinScoreToWinMatch.SetValue(120000, mode);
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(0, mode);
		}

		// Token: 0x06002A3A RID: 10810 RVA: 0x000A3894 File Offset: 0x000A1A94
		private void InitializeForTeamDeathmatch(MultiplayerOptions.MultiplayerOptionsAccessMode mode)
		{
			string text = "TeamDeathmatch";
			MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(this.GetNumberOfPlayersForGameMode(text), mode);
			MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.SpectatorCamera.SetValue(0, mode);
			MultiplayerOptions.OptionType.MapTimeLimit.SetValue(this.GetRoundTimeLimitInMinutesForGameMode(text), mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
			MultiplayerOptions.OptionType.MinScoreToWinMatch.SetValue(120000, mode);
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
		}

		// Token: 0x06002A3B RID: 10811 RVA: 0x000A3938 File Offset: 0x000A1B38
		private void InitializeForDuel(MultiplayerOptions.MultiplayerOptionsAccessMode mode)
		{
			string text = "Duel";
			MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(this.GetNumberOfPlayersForGameMode(text), mode);
			MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.SpectatorCamera.SetValue(0, mode);
			MultiplayerOptions.OptionType.MapTimeLimit.SetValue(MultiplayerOptions.OptionType.MapTimeLimit.GetMaximumValue(), mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(0, mode);
			MultiplayerOptions.OptionType.MinScoreToWinDuel.SetValue(3, mode);
		}

		// Token: 0x06002A3C RID: 10812 RVA: 0x000A39D8 File Offset: 0x000A1BD8
		private void InitializeForSiege(MultiplayerOptions.MultiplayerOptionsAccessMode mode)
		{
			string text = "Siege";
			MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(this.GetNumberOfPlayersForGameMode(text), mode);
			MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(50, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(50, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.SpectatorCamera.SetValue(0, mode);
			MultiplayerOptions.OptionType.WarmupTimeLimit.SetValue(3, mode);
			MultiplayerOptions.OptionType.MapTimeLimit.SetValue(this.GetRoundTimeLimitInMinutesForGameMode(text), mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(12, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(30, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
		}

		// Token: 0x06002A3D RID: 10813 RVA: 0x000A3A7C File Offset: 0x000A1C7C
		private void InitializeForCaptain(MultiplayerOptions.MultiplayerOptionsAccessMode mode)
		{
			string text = "Captain";
			MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(this.GetNumberOfPlayersForGameMode(text), mode);
			MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(25, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(50, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(50, mode);
			MultiplayerOptions.OptionType.SpectatorCamera.SetValue(6, mode);
			MultiplayerOptions.OptionType.WarmupTimeLimit.SetValue(5, mode);
			MultiplayerOptions.OptionType.MapTimeLimit.SetValue(5, mode);
			MultiplayerOptions.OptionType.RoundTimeLimit.SetValue(this.GetRoundTimeLimitInMinutesForGameMode(text) * 60, mode);
			MultiplayerOptions.OptionType.RoundPreparationTimeLimit.SetValue(20, mode);
			MultiplayerOptions.OptionType.RoundTotal.SetValue(this.GetRoundCountForGameMode(text), mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
			MultiplayerOptions.OptionType.AllowPollsToKickPlayers.SetValue(true, mode);
			MultiplayerOptions.OptionType.SingleSpawn.SetValue(true, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

		// Token: 0x06002A3E RID: 10814 RVA: 0x000A3B54 File Offset: 0x000A1D54
		private void InitializeForSkirmish(MultiplayerOptions.MultiplayerOptionsAccessMode mode)
		{
			string text = "Skirmish";
			MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(this.GetNumberOfPlayersForGameMode(text), mode);
			MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(50, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(50, mode);
			MultiplayerOptions.OptionType.SpectatorCamera.SetValue(6, mode);
			MultiplayerOptions.OptionType.WarmupTimeLimit.SetValue(5, mode);
			MultiplayerOptions.OptionType.MapTimeLimit.SetValue(5, mode);
			MultiplayerOptions.OptionType.RoundTimeLimit.SetValue(this.GetRoundTimeLimitInMinutesForGameMode(text) * 60, mode);
			MultiplayerOptions.OptionType.RoundPreparationTimeLimit.SetValue(20, mode);
			MultiplayerOptions.OptionType.RoundTotal.SetValue(this.GetRoundCountForGameMode(text), mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
			MultiplayerOptions.OptionType.AllowPollsToKickPlayers.SetValue(true, mode);
		}

		// Token: 0x06002A3F RID: 10815 RVA: 0x000A3C20 File Offset: 0x000A1E20
		private void InitializeForBattle(MultiplayerOptions.MultiplayerOptionsAccessMode mode)
		{
			string text = "Battle";
			MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(this.GetNumberOfPlayersForGameMode(text), mode);
			MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(25, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(50, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(25, mode);
			MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(50, mode);
			MultiplayerOptions.OptionType.SpectatorCamera.SetValue(6, mode);
			MultiplayerOptions.OptionType.WarmupTimeLimit.SetValue(5, mode);
			MultiplayerOptions.OptionType.MapTimeLimit.SetValue(90, mode);
			MultiplayerOptions.OptionType.RoundTimeLimit.SetValue(this.GetRoundTimeLimitInMinutesForGameMode(text) * 60, mode);
			MultiplayerOptions.OptionType.RoundPreparationTimeLimit.SetValue(20, mode);
			MultiplayerOptions.OptionType.RoundTotal.SetValue(this.GetRoundCountForGameMode(text), mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
			MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
			MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
			MultiplayerOptions.OptionType.SingleSpawn.SetValue(true, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

		// Token: 0x06002A40 RID: 10816 RVA: 0x000A3CF0 File Offset: 0x000A1EF0
		public int GetNumberOfPlayersForGameMode(string gameModeID)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(gameModeID);
			if (num <= 2173941516U)
			{
				if (num != 650645549U)
				{
					if (num != 1201370427U)
					{
						if (num != 2173941516U)
						{
							return 0;
						}
						if (!(gameModeID == "Siege"))
						{
							return 0;
						}
					}
					else
					{
						if (!(gameModeID == "Duel"))
						{
							return 0;
						}
						return 32;
					}
				}
				else
				{
					if (!(gameModeID == "Skirmish"))
					{
						return 0;
					}
					return 12;
				}
			}
			else if (num <= 2508183895U)
			{
				if (num != 2298111883U)
				{
					if (num != 2508183895U)
					{
						return 0;
					}
					if (!(gameModeID == "Battle"))
					{
						return 0;
					}
				}
				else
				{
					if (!(gameModeID == "Captain"))
					{
						return 0;
					}
					return 12;
				}
			}
			else if (num != 2569833005U)
			{
				if (num != 4010700071U)
				{
					return 0;
				}
				if (!(gameModeID == "TeamDeathmatch"))
				{
					return 0;
				}
			}
			else if (!(gameModeID == "FreeForAll"))
			{
				return 0;
			}
			return 120;
		}

		// Token: 0x06002A41 RID: 10817 RVA: 0x000A3DD4 File Offset: 0x000A1FD4
		public int GetRoundCountForGameMode(string gameModeID)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(gameModeID);
			if (num <= 2173941516U)
			{
				if (num != 650645549U)
				{
					if (num != 1201370427U)
					{
						if (num != 2173941516U)
						{
							return 0;
						}
						if (!(gameModeID == "Siege"))
						{
							return 0;
						}
					}
					else if (!(gameModeID == "Duel"))
					{
						return 0;
					}
				}
				else
				{
					if (!(gameModeID == "Skirmish"))
					{
						return 0;
					}
					return 5;
				}
			}
			else if (num <= 2508183895U)
			{
				if (num != 2298111883U)
				{
					if (num != 2508183895U)
					{
						return 0;
					}
					if (!(gameModeID == "Battle"))
					{
						return 0;
					}
					return 9;
				}
				else
				{
					if (!(gameModeID == "Captain"))
					{
						return 0;
					}
					return 5;
				}
			}
			else if (num != 2569833005U)
			{
				if (num != 4010700071U)
				{
					return 0;
				}
				if (!(gameModeID == "TeamDeathmatch"))
				{
					return 0;
				}
			}
			else if (!(gameModeID == "FreeForAll"))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06002A42 RID: 10818 RVA: 0x000A3EB0 File Offset: 0x000A20B0
		public int GetRoundTimeLimitInMinutesForGameMode(string gameModeID)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(gameModeID);
			if (num <= 2173941516U)
			{
				if (num != 650645549U)
				{
					if (num != 1201370427U)
					{
						if (num != 2173941516U)
						{
							return 0;
						}
						if (!(gameModeID == "Siege"))
						{
							return 0;
						}
					}
					else if (!(gameModeID == "Duel"))
					{
						return 0;
					}
				}
				else
				{
					if (!(gameModeID == "Skirmish"))
					{
						return 0;
					}
					return 7;
				}
			}
			else if (num <= 2508183895U)
			{
				if (num != 2298111883U)
				{
					if (num != 2508183895U)
					{
						return 0;
					}
					if (!(gameModeID == "Battle"))
					{
						return 0;
					}
					return 20;
				}
				else
				{
					if (!(gameModeID == "Captain"))
					{
						return 0;
					}
					return 10;
				}
			}
			else if (num != 2569833005U)
			{
				if (num != 4010700071U)
				{
					return 0;
				}
				if (!(gameModeID == "TeamDeathmatch"))
				{
					return 0;
				}
			}
			else if (!(gameModeID == "FreeForAll"))
			{
				return 0;
			}
			return 30;
		}

		// Token: 0x06002A43 RID: 10819 RVA: 0x000A3F90 File Offset: 0x000A2190
		public static void InitializeFromConfigFile(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				string[] array = File.ReadAllLines(ModuleHelper.GetModuleFullPath("Native") + fileName);
				for (int i = 0; i < array.Length; i++)
				{
					GameNetwork.HandleConsoleCommand(array[i]);
				}
			}
		}

		// Token: 0x06002A44 RID: 10820 RVA: 0x000A3FD4 File Offset: 0x000A21D4
		public List<string> GetMultiplayerOptionsTextList(MultiplayerOptions.OptionType optionType)
		{
			List<string> list = new List<string>();
			string text = new TextObject("{=vBkrw5VV}Random", null).ToString();
			string text2 = "-- " + text + " --";
			switch (optionType)
			{
			case MultiplayerOptions.OptionType.PremadeMatchGameMode:
				return (from q in Module.CurrentModule.GetMultiplayerGameTypes()
					where q.GameType == "Skirmish" || q.GameType == "Captain"
					select GameTexts.FindText("str_multiplayer_official_game_type_name", q.GameType).ToString()).ToList<string>();
			case MultiplayerOptions.OptionType.GameType:
				return (from q in Module.CurrentModule.GetMultiplayerGameTypes()
					select GameTexts.FindText("str_multiplayer_official_game_type_name", q.GameType).ToString()).ToList<string>();
			case MultiplayerOptions.OptionType.PremadeGameType:
				break;
			case MultiplayerOptions.OptionType.Map:
			{
				List<string> list2 = new List<string>();
				if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
				{
					list2 = MultiplayerGameTypes.GetGameTypeInfo(MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Scenes.ToList<string>();
				}
				else if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.PremadeMatch)
				{
					list2 = this.GetAvailableClanMatchScenes();
					list.Insert(0, text2);
				}
				using (List<string>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text3 = enumerator.Current;
						list.Add(GameTexts.FindText("str_multiplayer_scene_name", text3).ToString());
					}
					return list;
				}
				break;
			}
			case MultiplayerOptions.OptionType.CultureTeam1:
			case MultiplayerOptions.OptionType.CultureTeam2:
				list = (from c in MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>()
					where c.IsMainCulture
					select c into x
					select MultiplayerOptions.GetLocalizedCultureNameFromStringID(x.StringId)).ToList<string>();
				if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.PremadeMatch)
				{
					list.Insert(0, text2);
					return list;
				}
				return list;
			default:
			{
				if (optionType == MultiplayerOptions.OptionType.SpectatorCamera)
				{
					return new List<string>
					{
						GameTexts.FindText("str_multiplayer_spectator_camera_type", SpectatorCameraTypes.LockToAnyAgent.ToString()).ToString(),
						GameTexts.FindText("str_multiplayer_spectator_camera_type", SpectatorCameraTypes.LockToAnyPlayer.ToString()).ToString(),
						GameTexts.FindText("str_multiplayer_spectator_camera_type", SpectatorCameraTypes.LockToTeamMembers.ToString()).ToString(),
						GameTexts.FindText("str_multiplayer_spectator_camera_type", SpectatorCameraTypes.LockToTeamMembersView.ToString()).ToString()
					};
				}
				if (optionType != MultiplayerOptions.OptionType.AutoTeamBalanceThreshold)
				{
					return this.GetMultiplayerOptionsList(optionType);
				}
				for (int i = 0; i < 6; i++)
				{
					AutoTeamBalanceLimits autoTeamBalanceLimits = (AutoTeamBalanceLimits)i;
					string text4 = autoTeamBalanceLimits.ToString();
					if (text4.ToLower().Contains("limitto"))
					{
						int num = -1;
						int.TryParse(text4.Substring(7), out num);
						list.Add(GameTexts.FindText("str_multiplayer_auto_team_balance_limit", "Value").SetTextVariable("LIMIT_VALUE", num).ToString());
					}
					else
					{
						list.Add(GameTexts.FindText("str_multiplayer_auto_team_balance_limit", text4).ToString());
					}
				}
				return list;
			}
			}
			list = new List<string>
			{
				new TextObject("{=H5tiRTya}Practice", null).ToString(),
				new TextObject("{=YNkPy4ta}Clan Match", null).ToString()
			};
			return list;
		}

		// Token: 0x06002A45 RID: 10821 RVA: 0x000A4348 File Offset: 0x000A2548
		public List<string> GetMultiplayerOptionsList(MultiplayerOptions.OptionType optionType)
		{
			List<string> list = new List<string>();
			switch (optionType)
			{
			case MultiplayerOptions.OptionType.PremadeMatchGameMode:
				list = (from q in Module.CurrentModule.GetMultiplayerGameTypes()
					select q.GameType).ToList<string>();
				list.Remove("FreeForAll");
				list.Remove("TeamDeathmatch");
				list.Remove("Duel");
				list.Remove("Siege");
				break;
			case MultiplayerOptions.OptionType.GameType:
				list = (from q in Module.CurrentModule.GetMultiplayerGameTypes()
					select q.GameType).ToList<string>();
				break;
			case MultiplayerOptions.OptionType.PremadeGameType:
				list = new List<string>
				{
					PremadeGameType.Practice.ToString(),
					PremadeGameType.Clan.ToString()
				};
				break;
			case MultiplayerOptions.OptionType.Map:
				if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
				{
					list = MultiplayerGameTypes.GetGameTypeInfo(MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).Scenes.ToList<string>();
				}
				else if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.PremadeMatch)
				{
					MultiplayerOptions.OptionType.PremadeMatchGameMode.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
					list = this.GetAvailableClanMatchScenes();
					list.Insert(0, "RandomSelection");
				}
				break;
			case MultiplayerOptions.OptionType.CultureTeam1:
			case MultiplayerOptions.OptionType.CultureTeam2:
				list = (from c in MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>()
					where c.IsMainCulture
					select c into x
					select x.StringId).ToList<string>();
				if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.PremadeMatch)
				{
					list.Insert(0, Parameters.RandomSelectionString);
				}
				break;
			default:
				if (optionType != MultiplayerOptions.OptionType.SpectatorCamera)
				{
					if (optionType == MultiplayerOptions.OptionType.AutoTeamBalanceThreshold)
					{
						List<string> list2 = new List<string>();
						for (int i = 0; i < 6; i++)
						{
							List<string> list3 = list2;
							AutoTeamBalanceLimits autoTeamBalanceLimits = (AutoTeamBalanceLimits)i;
							list3.Add(autoTeamBalanceLimits.ToString());
						}
						list = list2;
					}
				}
				else
				{
					list = new List<string>
					{
						SpectatorCameraTypes.LockToAnyAgent.ToString(),
						SpectatorCameraTypes.LockToAnyPlayer.ToString(),
						SpectatorCameraTypes.LockToTeamMembers.ToString(),
						SpectatorCameraTypes.LockToTeamMembersView.ToString()
					};
				}
				break;
			}
			return list;
		}

		// Token: 0x06002A46 RID: 10822 RVA: 0x000A45B8 File Offset: 0x000A27B8
		private List<string> GetAvailableClanMatchScenes()
		{
			string[] array = new string[0];
			string[] array2;
			if (NetworkMain.GameClient.AvailableScenes.ScenesByGameTypes.TryGetValue(MultiplayerOptions.OptionType.PremadeMatchGameMode.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions), out array2))
			{
				array = array2;
			}
			return array.ToList<string>();
		}

		// Token: 0x06002A47 RID: 10823 RVA: 0x000A45F4 File Offset: 0x000A27F4
		private MultiplayerOptions.MultiplayerOptionsContainer GetContainer(MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			switch (mode)
			{
			case MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions:
				return this._current;
			case MultiplayerOptions.MultiplayerOptionsAccessMode.NextMapOptions:
				return this._next;
			case MultiplayerOptions.MultiplayerOptionsAccessMode.MissionUIOptions:
				return this._ui;
			default:
				return null;
			}
		}

		// Token: 0x06002A48 RID: 10824 RVA: 0x000A4620 File Offset: 0x000A2820
		public void InitializeAllOptionsFromCurrent()
		{
			this._current.CopyAllValuesTo(this._ui);
			this._current.CopyAllValuesTo(this._next);
		}

		// Token: 0x06002A49 RID: 10825 RVA: 0x000A4644 File Offset: 0x000A2844
		public void InitializeAllOptionsFromNext()
		{
			this._next.CopyAllValuesTo(this._ui);
			this._next.CopyAllValuesTo(this._current);
			this.UpdateMbMultiplayerData();
		}

		// Token: 0x06002A4A RID: 10826 RVA: 0x000A4670 File Offset: 0x000A2870
		public void InitializeOptionsFromUi()
		{
			this._ui.CopyAllValuesTo(this._next);
			if (Mission.Current == null)
			{
				this._ui.CopyAllValuesTo(this._current);
			}
			else
			{
				this._ui.CopyImmediateEffectValuesTo(this._current);
			}
			this.UpdateMbMultiplayerData();
		}

		// Token: 0x06002A4B RID: 10827 RVA: 0x000A46C0 File Offset: 0x000A28C0
		private void UpdateMbMultiplayerData()
		{
			MultiplayerOptions.MultiplayerOptionsContainer container = this.GetContainer(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			container.GetOptionFromOptionType(MultiplayerOptions.OptionType.ServerName).GetValue(out MBMultiplayerData.ServerName);
			if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
			{
				container.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType).GetValue(out MBMultiplayerData.GameType);
			}
			else if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.PremadeMatch)
			{
				container.GetOptionFromOptionType(MultiplayerOptions.OptionType.PremadeMatchGameMode).GetValue(out MBMultiplayerData.GameType);
			}
			container.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map).GetValue(out MBMultiplayerData.Map);
			container.GetOptionFromOptionType(MultiplayerOptions.OptionType.MaxNumberOfPlayers).GetValue(out MBMultiplayerData.PlayerCountLimit);
		}

		// Token: 0x06002A4C RID: 10828 RVA: 0x000A4744 File Offset: 0x000A2944
		public MBList<string> GetMapList()
		{
			MultiplayerGameTypeInfo multiplayerGameTypeInfo = null;
			if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
			{
				multiplayerGameTypeInfo = MultiplayerGameTypes.GetGameTypeInfo(MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			}
			else if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.PremadeMatch)
			{
				multiplayerGameTypeInfo = MultiplayerGameTypes.GetGameTypeInfo(MultiplayerOptions.OptionType.PremadeMatchGameMode.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			}
			MBList<string> mblist = new MBList<string>();
			if (multiplayerGameTypeInfo.Scenes.Count > 0)
			{
				mblist.Add(multiplayerGameTypeInfo.Scenes[0]);
				MultiplayerOptions.OptionType.Map.SetValue(mblist[0], MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
			return mblist;
		}

		// Token: 0x06002A4D RID: 10829 RVA: 0x000A47B8 File Offset: 0x000A29B8
		public string GetValueTextForOptionWithMultipleSelection(MultiplayerOptions.OptionType optionType)
		{
			MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
			MultiplayerOptions.OptionValueType optionValueType = optionProperty.OptionValueType;
			if (optionValueType == MultiplayerOptions.OptionValueType.Enum)
			{
				return Enum.ToObject(optionProperty.EnumType, optionType.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).ToString();
			}
			if (optionValueType != MultiplayerOptions.OptionValueType.String)
			{
				return null;
			}
			return optionType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

		// Token: 0x06002A4E RID: 10830 RVA: 0x000A4800 File Offset: 0x000A2A00
		public void SetValueForOptionWithMultipleSelectionFromText(MultiplayerOptions.OptionType optionType, string value)
		{
			MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
			MultiplayerOptions.OptionValueType optionValueType = optionProperty.OptionValueType;
			if (optionValueType != MultiplayerOptions.OptionValueType.Enum)
			{
				if (optionValueType == MultiplayerOptions.OptionValueType.String)
				{
					optionType.SetValue(value, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
			else
			{
				optionType.SetValue((int)Enum.Parse(optionProperty.EnumType, value), MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
			if (optionType == MultiplayerOptions.OptionType.GameType || optionType == MultiplayerOptions.OptionType.PremadeMatchGameMode)
			{
				this.OnGameTypeChanged(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
		}

		// Token: 0x06002A4F RID: 10831 RVA: 0x000A4858 File Offset: 0x000A2A58
		private static string GetLocalizedCultureNameFromStringID(string cultureID)
		{
			if (cultureID == "sturgia")
			{
				return new TextObject("{=PjO7oY16}Sturgia", null).ToString();
			}
			if (cultureID == "vlandia")
			{
				return new TextObject("{=FjwRsf1C}Vlandia", null).ToString();
			}
			if (cultureID == "battania")
			{
				return new TextObject("{=0B27RrYJ}Battania", null).ToString();
			}
			if (cultureID == "empire")
			{
				return new TextObject("{=empirefaction}Empire", null).ToString();
			}
			if (cultureID == "khuzait")
			{
				return new TextObject("{=sZLd6VHi}Khuzait", null).ToString();
			}
			if (!(cultureID == "aserai"))
			{
				Debug.FailedAssert("Unidentified culture id: " + cultureID, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\MultiplayerOptions.cs", "GetLocalizedCultureNameFromStringID", 1039);
				return "";
			}
			return new TextObject("{=aseraifaction}Aserai", null).ToString();
		}

		// Token: 0x0400101E RID: 4126
		private const int PlayerCountLimitMin = 1;

		// Token: 0x0400101F RID: 4127
		private const int PlayerCountLimitMax = 1023;

		// Token: 0x04001020 RID: 4128
		private const int PlayerCountLimitForMatchStartMin = 0;

		// Token: 0x04001021 RID: 4129
		private const int PlayerCountLimitForMatchStartMax = 20;

		// Token: 0x04001022 RID: 4130
		private const int MapTimeLimitMin = 1;

		// Token: 0x04001023 RID: 4131
		private const int MapTimeLimitMax = 60;

		// Token: 0x04001024 RID: 4132
		private const int RoundLimitMin = 1;

		// Token: 0x04001025 RID: 4133
		private const int RoundLimitMax = 20;

		// Token: 0x04001026 RID: 4134
		private const int RoundTimeLimitMin = 60;

		// Token: 0x04001027 RID: 4135
		private const int RoundTimeLimitMax = 3600;

		// Token: 0x04001028 RID: 4136
		private const int RoundPreparationTimeLimitMin = 2;

		// Token: 0x04001029 RID: 4137
		private const int RoundPreparationTimeLimitMax = 60;

		// Token: 0x0400102A RID: 4138
		private const int RespawnPeriodMin = 1;

		// Token: 0x0400102B RID: 4139
		private const int RespawnPeriodMax = 60;

		// Token: 0x0400102C RID: 4140
		private const int GoldGainChangePercentageMin = -100;

		// Token: 0x0400102D RID: 4141
		private const int GoldGainChangePercentageMax = 100;

		// Token: 0x0400102E RID: 4142
		private const int PollAcceptThresholdMin = 0;

		// Token: 0x0400102F RID: 4143
		private const int PollAcceptThresholdMax = 10;

		// Token: 0x04001030 RID: 4144
		private const int BotsPerTeamLimitMin = 0;

		// Token: 0x04001031 RID: 4145
		private const int BotsPerTeamLimitMax = 510;

		// Token: 0x04001032 RID: 4146
		private const int BotsPerFormationLimitMin = 0;

		// Token: 0x04001033 RID: 4147
		private const int BotsPerFormationLimitMax = 100;

		// Token: 0x04001034 RID: 4148
		private const int FriendlyFireDamagePercentMin = 0;

		// Token: 0x04001035 RID: 4149
		private const int FriendlyFireDamagePercentMax = 100;

		// Token: 0x04001036 RID: 4150
		private const int GameDefinitionIdMin = -2147483648;

		// Token: 0x04001037 RID: 4151
		private const int GameDefinitionIdMax = 2147483647;

		// Token: 0x04001038 RID: 4152
		private const int MaxScoreToEndDuel = 7;

		// Token: 0x04001039 RID: 4153
		private static MultiplayerOptions _instance;

		// Token: 0x0400103A RID: 4154
		private readonly MultiplayerOptions.MultiplayerOptionsContainer _current;

		// Token: 0x0400103B RID: 4155
		private readonly MultiplayerOptions.MultiplayerOptionsContainer _next;

		// Token: 0x0400103C RID: 4156
		private readonly MultiplayerOptions.MultiplayerOptionsContainer _ui;

		// Token: 0x0400103D RID: 4157
		public MultiplayerOptions.OptionsCategory CurrentOptionsCategory;

		// Token: 0x02000626 RID: 1574
		public enum MultiplayerOptionsAccessMode
		{
			// Token: 0x04001FC7 RID: 8135
			CurrentMapOptions,
			// Token: 0x04001FC8 RID: 8136
			NextMapOptions,
			// Token: 0x04001FC9 RID: 8137
			MissionUIOptions
		}

		// Token: 0x02000627 RID: 1575
		public enum OptionValueType
		{
			// Token: 0x04001FCB RID: 8139
			Bool,
			// Token: 0x04001FCC RID: 8140
			Integer,
			// Token: 0x04001FCD RID: 8141
			Enum,
			// Token: 0x04001FCE RID: 8142
			String
		}

		// Token: 0x02000628 RID: 1576
		public enum OptionType
		{
			// Token: 0x04001FD0 RID: 8144
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the name of the server in the server list", 0, 0, null, false, null)]
			ServerName,
			// Token: 0x04001FD1 RID: 8145
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Welcome messages which is shown to all players when they enter the server.", 0, 0, null, false, null)]
			WelcomeMessage,
			// Token: 0x04001FD2 RID: 8146
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets a password that clients have to enter before connecting to the server.", 0, 0, null, false, null)]
			GamePassword,
			// Token: 0x04001FD3 RID: 8147
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets a password that allows players access to admin tools during the game.", 0, 0, null, false, null)]
			AdminPassword,
			// Token: 0x04001FD4 RID: 8148
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets ID of the private game definition.", -2147483648, 2147483647, null, false, null)]
			GameDefinitionId,
			// Token: 0x04001FD5 RID: 8149
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to kick other players.", 0, 0, null, false, null)]
			AllowPollsToKickPlayers,
			// Token: 0x04001FD6 RID: 8150
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to ban other players.", 0, 0, null, false, null)]
			AllowPollsToBanPlayers,
			// Token: 0x04001FD7 RID: 8151
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to change the current map.", 0, 0, null, false, null)]
			AllowPollsToChangeMaps,
			// Token: 0x04001FD8 RID: 8152
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to use their custom banner.", 0, 0, null, false, null)]
			AllowIndividualBanners,
			// Token: 0x04001FD9 RID: 8153
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Use animation progress dependent blocking.", 0, 0, null, false, null)]
			UseRealisticBlocking,
			// Token: 0x04001FDA RID: 8154
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the game type.", 0, 0, null, true, null)]
			PremadeMatchGameMode,
			// Token: 0x04001FDB RID: 8155
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the game type.", 0, 0, null, true, null)]
			GameType,
			// Token: 0x04001FDC RID: 8156
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Enum, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Type of the premade game.", 0, 1, null, true, typeof(PremadeGameType))]
			PremadeGameType,
			// Token: 0x04001FDD RID: 8157
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Map of the game.", 0, 0, null, true, null)]
			Map,
			// Token: 0x04001FDE RID: 8158
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Sets culture for team 1", 0, 0, null, true, null)]
			CultureTeam1,
			// Token: 0x04001FDF RID: 8159
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Sets culture for team 2", 0, 0, null, true, null)]
			CultureTeam2,
			// Token: 0x04001FE0 RID: 8160
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Set the maximum amount of player allowed on the server.", 1, 1023, null, false, null)]
			MaxNumberOfPlayers,
			// Token: 0x04001FE1 RID: 8161
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Set the amount of players that are needed to start the first round. If not met, players will just wait.", 0, 20, null, false, null)]
			MinNumberOfPlayersForMatchStart,
			// Token: 0x04001FE2 RID: 8162
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots on team 1", 0, 510, null, false, null)]
			NumberOfBotsTeam1,
			// Token: 0x04001FE3 RID: 8163
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots on team 2", 0, 510, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			NumberOfBotsTeam2,
			// Token: 0x04001FE4 RID: 8164
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots per formation", 0, 100, new string[] { "Captain" }, false, null)]
			NumberOfBotsPerFormation,
			// Token: 0x04001FE5 RID: 8165
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much melee damage inflicted upon a friend is dealt back to the inflictor.", 0, 100, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			FriendlyFireDamageMeleeSelfPercent,
			// Token: 0x04001FE6 RID: 8166
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much melee damage inflicted upon a friend is actually dealt.", 0, 100, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			FriendlyFireDamageMeleeFriendPercent,
			// Token: 0x04001FE7 RID: 8167
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much ranged damage inflicted upon a friend is dealt back to the inflictor.", 0, 100, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			FriendlyFireDamageRangedSelfPercent,
			// Token: 0x04001FE8 RID: 8168
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much ranged damage inflicted upon a friend is actually dealt.", 0, 100, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			FriendlyFireDamageRangedFriendPercent,
			// Token: 0x04001FE9 RID: 8169
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Enum, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Who can spectators look at, and how.", 0, 7, null, true, typeof(SpectatorCameraTypes))]
			SpectatorCamera,
			// Token: 0x04001FEA RID: 8170
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Maximum duration for the warmup. In minutes.", 1, 60, null, false, null)]
			WarmupTimeLimit,
			// Token: 0x04001FEB RID: 8171
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum duration for the map. In minutes.", 1, 60, null, false, null)]
			MapTimeLimit,
			// Token: 0x04001FEC RID: 8172
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum duration for each round. In seconds.", 60, 3600, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege" }, false, null)]
			RoundTimeLimit,
			// Token: 0x04001FED RID: 8173
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Time available to select class/equipment. In seconds.", 2, 60, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege" }, false, null)]
			RoundPreparationTimeLimit,
			// Token: 0x04001FEE RID: 8174
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum amount of rounds before the game ends.", 1, 20, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege" }, false, null)]
			RoundTotal,
			// Token: 0x04001FEF RID: 8175
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Wait time after death, before respawning again. In seconds.", 1, 60, new string[] { "Siege" }, false, null)]
			RespawnPeriodTeam1,
			// Token: 0x04001FF0 RID: 8176
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Wait time after death, before respawning again. In seconds.", 1, 60, new string[] { "Siege" }, false, null)]
			RespawnPeriodTeam2,
			// Token: 0x04001FF1 RID: 8177
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Unlimited gold option.", 0, 0, new string[] { "Battle", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			UnlimitedGold,
			// Token: 0x04001FF2 RID: 8178
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Gold gain multiplier from agent deaths.", -100, 100, new string[] { "Siege", "TeamDeathmatch" }, false, null)]
			GoldGainChangePercentageTeam1,
			// Token: 0x04001FF3 RID: 8179
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Gold gain multiplier from agent deaths.", -100, 100, new string[] { "Siege", "TeamDeathmatch" }, false, null)]
			GoldGainChangePercentageTeam2,
			// Token: 0x04001FF4 RID: 8180
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Min score to win match.", 0, 1023000, new string[] { "TeamDeathmatch" }, false, null)]
			MinScoreToWinMatch,
			// Token: 0x04001FF5 RID: 8181
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Min score to win duel.", 0, 7, new string[] { "Duel" }, false, null)]
			MinScoreToWinDuel,
			// Token: 0x04001FF6 RID: 8182
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Minimum needed difference in poll results before it is accepted.", 0, 10, null, false, null)]
			PollAcceptThreshold,
			// Token: 0x04001FF7 RID: 8183
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Enum, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum player imbalance between team 1 and team 2.", 0, 5, null, true, typeof(AutoTeamBalanceLimits))]
			AutoTeamBalanceThreshold,
			// Token: 0x04001FF8 RID: 8184
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Enables mission recording.", 0, 0, null, false, null)]
			EnableMissionRecording,
			// Token: 0x04001FF9 RID: 8185
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Sets if the game mode uses single spawning.", 0, 0, null, false, null)]
			SingleSpawn,
			// Token: 0x04001FFA RID: 8186
			NumOfSlots
		}

		// Token: 0x02000629 RID: 1577
		public enum OptionsCategory
		{
			// Token: 0x04001FFC RID: 8188
			Default,
			// Token: 0x04001FFD RID: 8189
			PremadeMatch
		}

		// Token: 0x0200062A RID: 1578
		public class MultiplayerOption
		{
			// Token: 0x06003DBE RID: 15806 RVA: 0x000F347E File Offset: 0x000F167E
			public static MultiplayerOptions.MultiplayerOption CreateMultiplayerOption(MultiplayerOptions.OptionType optionType)
			{
				return new MultiplayerOptions.MultiplayerOption(optionType);
			}

			// Token: 0x06003DBF RID: 15807 RVA: 0x000F3488 File Offset: 0x000F1688
			private MultiplayerOption(MultiplayerOptions.OptionType optionType)
			{
				this.OptionType = optionType;
				if (optionType.GetOptionProperty().OptionValueType == MultiplayerOptions.OptionValueType.String)
				{
					this._intValue = MultiplayerOptions.MultiplayerOption.IntegerValue.Invalid;
					this._stringValue = MultiplayerOptions.MultiplayerOption.StringValue.Create();
					return;
				}
				this._intValue = MultiplayerOptions.MultiplayerOption.IntegerValue.Create();
				this._stringValue = MultiplayerOptions.MultiplayerOption.StringValue.Invalid;
			}

			// Token: 0x06003DC0 RID: 15808 RVA: 0x000F34DD File Offset: 0x000F16DD
			public MultiplayerOptions.MultiplayerOption UpdateValue(bool value)
			{
				this.UpdateValue(value ? 1 : 0);
				return this;
			}

			// Token: 0x06003DC1 RID: 15809 RVA: 0x000F34EE File Offset: 0x000F16EE
			public MultiplayerOptions.MultiplayerOption UpdateValue(int value)
			{
				this._intValue.UpdateValue(value);
				return this;
			}

			// Token: 0x06003DC2 RID: 15810 RVA: 0x000F34FD File Offset: 0x000F16FD
			public MultiplayerOptions.MultiplayerOption UpdateValue(string value)
			{
				this._stringValue.UpdateValue(value);
				return this;
			}

			// Token: 0x06003DC3 RID: 15811 RVA: 0x000F350C File Offset: 0x000F170C
			public void GetValue(out bool value)
			{
				value = this._intValue.Value == 1;
			}

			// Token: 0x06003DC4 RID: 15812 RVA: 0x000F351E File Offset: 0x000F171E
			public void GetValue(out int value)
			{
				value = this._intValue.Value;
			}

			// Token: 0x06003DC5 RID: 15813 RVA: 0x000F352D File Offset: 0x000F172D
			public void GetValue(out string value)
			{
				value = this._stringValue.Value;
			}

			// Token: 0x04001FFE RID: 8190
			public readonly MultiplayerOptions.OptionType OptionType;

			// Token: 0x04001FFF RID: 8191
			private MultiplayerOptions.MultiplayerOption.IntegerValue _intValue;

			// Token: 0x04002000 RID: 8192
			private MultiplayerOptions.MultiplayerOption.StringValue _stringValue;

			// Token: 0x02000703 RID: 1795
			private struct IntegerValue
			{
				// Token: 0x17000A26 RID: 2598
				// (get) Token: 0x06004084 RID: 16516 RVA: 0x000FA104 File Offset: 0x000F8304
				public static MultiplayerOptions.MultiplayerOption.IntegerValue Invalid
				{
					get
					{
						return default(MultiplayerOptions.MultiplayerOption.IntegerValue);
					}
				}

				// Token: 0x17000A27 RID: 2599
				// (get) Token: 0x06004085 RID: 16517 RVA: 0x000FA11A File Offset: 0x000F831A
				// (set) Token: 0x06004086 RID: 16518 RVA: 0x000FA122 File Offset: 0x000F8322
				public bool IsValid { get; private set; }

				// Token: 0x17000A28 RID: 2600
				// (get) Token: 0x06004087 RID: 16519 RVA: 0x000FA12B File Offset: 0x000F832B
				// (set) Token: 0x06004088 RID: 16520 RVA: 0x000FA133 File Offset: 0x000F8333
				public int Value { get; private set; }

				// Token: 0x06004089 RID: 16521 RVA: 0x000FA13C File Offset: 0x000F833C
				public static MultiplayerOptions.MultiplayerOption.IntegerValue Create()
				{
					return new MultiplayerOptions.MultiplayerOption.IntegerValue
					{
						IsValid = true
					};
				}

				// Token: 0x0600408A RID: 16522 RVA: 0x000FA15A File Offset: 0x000F835A
				public void UpdateValue(int value)
				{
					this.Value = value;
				}
			}

			// Token: 0x02000704 RID: 1796
			private struct StringValue
			{
				// Token: 0x17000A29 RID: 2601
				// (get) Token: 0x0600408B RID: 16523 RVA: 0x000FA164 File Offset: 0x000F8364
				public static MultiplayerOptions.MultiplayerOption.StringValue Invalid
				{
					get
					{
						return default(MultiplayerOptions.MultiplayerOption.StringValue);
					}
				}

				// Token: 0x17000A2A RID: 2602
				// (get) Token: 0x0600408C RID: 16524 RVA: 0x000FA17A File Offset: 0x000F837A
				// (set) Token: 0x0600408D RID: 16525 RVA: 0x000FA182 File Offset: 0x000F8382
				public bool IsValid { get; private set; }

				// Token: 0x17000A2B RID: 2603
				// (get) Token: 0x0600408E RID: 16526 RVA: 0x000FA18B File Offset: 0x000F838B
				// (set) Token: 0x0600408F RID: 16527 RVA: 0x000FA193 File Offset: 0x000F8393
				public string Value { get; private set; }

				// Token: 0x06004090 RID: 16528 RVA: 0x000FA19C File Offset: 0x000F839C
				public static MultiplayerOptions.MultiplayerOption.StringValue Create()
				{
					return new MultiplayerOptions.MultiplayerOption.StringValue
					{
						IsValid = true
					};
				}

				// Token: 0x06004091 RID: 16529 RVA: 0x000FA1BA File Offset: 0x000F83BA
				public void UpdateValue(string value)
				{
					this.Value = value;
				}
			}
		}

		// Token: 0x0200062B RID: 1579
		private class MultiplayerOptionsContainer
		{
			// Token: 0x06003DC6 RID: 15814 RVA: 0x000F353C File Offset: 0x000F173C
			public MultiplayerOptionsContainer()
			{
				this._multiplayerOptions = new MultiplayerOptions.MultiplayerOption[42];
			}

			// Token: 0x06003DC7 RID: 15815 RVA: 0x000F3551 File Offset: 0x000F1751
			public MultiplayerOptions.MultiplayerOption GetOptionFromOptionType(MultiplayerOptions.OptionType optionType)
			{
				return this._multiplayerOptions[(int)optionType];
			}

			// Token: 0x06003DC8 RID: 15816 RVA: 0x000F355B File Offset: 0x000F175B
			private void CopyOptionFromOther(MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOption option)
			{
				this._multiplayerOptions[(int)optionType] = option;
			}

			// Token: 0x06003DC9 RID: 15817 RVA: 0x000F3566 File Offset: 0x000F1766
			public void CreateOption(MultiplayerOptions.OptionType optionType)
			{
				this._multiplayerOptions[(int)optionType] = MultiplayerOptions.MultiplayerOption.CreateMultiplayerOption(optionType);
			}

			// Token: 0x06003DCA RID: 15818 RVA: 0x000F3576 File Offset: 0x000F1776
			public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, int value)
			{
				this._multiplayerOptions[(int)optionType].UpdateValue(value);
			}

			// Token: 0x06003DCB RID: 15819 RVA: 0x000F3587 File Offset: 0x000F1787
			public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, string value)
			{
				this._multiplayerOptions[(int)optionType].UpdateValue(value);
			}

			// Token: 0x06003DCC RID: 15820 RVA: 0x000F3598 File Offset: 0x000F1798
			public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, bool value)
			{
				this._multiplayerOptions[(int)optionType].UpdateValue(value ? 1 : 0);
			}

			// Token: 0x06003DCD RID: 15821 RVA: 0x000F35AF File Offset: 0x000F17AF
			public void CopyAllValuesTo(MultiplayerOptions.MultiplayerOptionsContainer other)
			{
				this.CopyImmediateEffectValuesTo(other);
				this.CopyNewRoundValuesTo(other);
				this.CopyNewMapValuesTo(other);
			}

			// Token: 0x06003DCE RID: 15822 RVA: 0x000F35C8 File Offset: 0x000F17C8
			public void CopyImmediateEffectValuesTo(MultiplayerOptions.MultiplayerOptionsContainer other)
			{
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.AllowPollsToKickPlayers, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AllowPollsToKickPlayers));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.AllowPollsToBanPlayers, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AllowPollsToBanPlayers));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.AllowPollsToChangeMaps, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AllowPollsToChangeMaps));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.AllowIndividualBanners, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AllowIndividualBanners));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.UseRealisticBlocking, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.UseRealisticBlocking));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.MaxNumberOfPlayers, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.MaxNumberOfPlayers));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.WarmupTimeLimit, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.WarmupTimeLimit));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.MapTimeLimit, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.MapTimeLimit));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.RoundTotal, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundTotal));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.RoundTimeLimit, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundTimeLimit));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.RoundPreparationTimeLimit, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundPreparationTimeLimit));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.RespawnPeriodTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RespawnPeriodTeam1));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.RespawnPeriodTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RespawnPeriodTeam2));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.UnlimitedGold, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.UnlimitedGold));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.MinScoreToWinMatch, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.MinScoreToWinMatch));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.PollAcceptThreshold, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.PollAcceptThreshold));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.SpectatorCamera, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.SpectatorCamera));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.NumberOfBotsTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsTeam1));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.NumberOfBotsTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsTeam2));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.AutoTeamBalanceThreshold, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AutoTeamBalanceThreshold));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.WelcomeMessage, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.WelcomeMessage));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.GamePassword, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GamePassword));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.AdminPassword, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AdminPassword));
			}

			// Token: 0x06003DCF RID: 15823 RVA: 0x000F37A7 File Offset: 0x000F19A7
			public void CopyNewRoundValuesTo(MultiplayerOptions.MultiplayerOptionsContainer other)
			{
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.NumberOfBotsPerFormation, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsPerFormation));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.CultureTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.CultureTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam2));
			}

			// Token: 0x06003DD0 RID: 15824 RVA: 0x000F37D9 File Offset: 0x000F19D9
			public void CopyNewMapValuesTo(MultiplayerOptions.MultiplayerOptionsContainer other)
			{
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.Map, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.GameType, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.PremadeMatchGameMode, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.PremadeMatchGameMode));
			}

			// Token: 0x04002001 RID: 8193
			private readonly MultiplayerOptions.MultiplayerOption[] _multiplayerOptions;
		}
	}
}

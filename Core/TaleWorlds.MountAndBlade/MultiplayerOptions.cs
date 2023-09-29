using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerOptions
	{
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

		public static void Release()
		{
			MultiplayerOptions._instance = null;
		}

		public MultiplayerOptions.MultiplayerOption GetOptionFromOptionType(MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
		{
			return this.GetContainer(mode).GetOptionFromOptionType(optionType);
		}

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
			MultiplayerOptions.OptionType.DisableInactivityKick.SetValue(false, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
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
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(2, mode);
		}

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
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(2, mode);
		}

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
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(2, mode);
			MultiplayerOptions.OptionType.AllowPollsToKickPlayers.SetValue(true, mode);
			MultiplayerOptions.OptionType.SingleSpawn.SetValue(true, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

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
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(2, mode);
			MultiplayerOptions.OptionType.AllowPollsToKickPlayers.SetValue(true, mode);
		}

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
			MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(2, mode);
			MultiplayerOptions.OptionType.SingleSpawn.SetValue(true, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

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

		public static void InitializeFromCommandList(List<string> arguments)
		{
			foreach (string text in arguments)
			{
				GameNetwork.HandleConsoleCommand(text);
			}
		}

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
						TextObject textObject;
						string text4;
						if (GameTexts.TryGetText("str_multiplayer_scene_name", out textObject, text3))
						{
							text4 = textObject.ToString();
						}
						else
						{
							text4 = text3;
						}
						list.Add(text4);
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
				if (optionType != MultiplayerOptions.OptionType.SpectatorCamera)
				{
					return this.GetMultiplayerOptionsList(optionType);
				}
				return new List<string>
				{
					GameTexts.FindText("str_multiplayer_spectator_camera_type", SpectatorCameraTypes.LockToAnyAgent.ToString()).ToString(),
					GameTexts.FindText("str_multiplayer_spectator_camera_type", SpectatorCameraTypes.LockToAnyPlayer.ToString()).ToString(),
					GameTexts.FindText("str_multiplayer_spectator_camera_type", SpectatorCameraTypes.LockToTeamMembers.ToString()).ToString(),
					GameTexts.FindText("str_multiplayer_spectator_camera_type", SpectatorCameraTypes.LockToTeamMembersView.ToString()).ToString()
				};
			}
			list = new List<string>
			{
				new TextObject("{=H5tiRTya}Practice", null).ToString(),
				new TextObject("{=YNkPy4ta}Clan Match", null).ToString()
			};
			return list;
		}

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
				if (optionType == MultiplayerOptions.OptionType.SpectatorCamera)
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

		public void InitializeAllOptionsFromCurrent()
		{
			this._current.CopyAllValuesTo(this._ui);
			this._current.CopyAllValuesTo(this._next);
		}

		public void InitializeAllOptionsFromNext()
		{
			this._next.CopyAllValuesTo(this._ui);
			this._next.CopyAllValuesTo(this._current);
			this.UpdateMbMultiplayerData();
		}

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
				Debug.FailedAssert("Unidentified culture id: " + cultureID, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\MultiplayerOptions.cs", "GetLocalizedCultureNameFromStringID", 1047);
				return "";
			}
			return new TextObject("{=aseraifaction}Aserai", null).ToString();
		}

		public static bool TryGetOptionTypeFromString(string optionTypeString, out MultiplayerOptions.OptionType optionType, out MultiplayerOptionsProperty optionAttribute)
		{
			optionAttribute = null;
			for (optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
				if (optionProperty != null && optionType.ToString().Equals(optionTypeString))
				{
					optionAttribute = optionProperty;
					return true;
				}
			}
			return false;
		}

		private const int PlayerCountLimitMin = 1;

		private const int PlayerCountLimitMax = 1023;

		private const int PlayerCountLimitForMatchStartMin = 0;

		private const int PlayerCountLimitForMatchStartMax = 20;

		private const int MapTimeLimitMin = 1;

		private const int MapTimeLimitMax = 60;

		private const int RoundLimitMin = 1;

		private const int RoundLimitMax = 99;

		private const int RoundTimeLimitMin = 60;

		private const int RoundTimeLimitMax = 3600;

		private const int RoundPreparationTimeLimitMin = 2;

		private const int RoundPreparationTimeLimitMax = 60;

		private const int RespawnPeriodMin = 1;

		private const int RespawnPeriodMax = 60;

		private const int GoldGainChangePercentageMin = -100;

		private const int GoldGainChangePercentageMax = 100;

		private const int PollAcceptThresholdMin = 0;

		private const int PollAcceptThresholdMax = 10;

		private const int BotsPerTeamLimitMin = 0;

		private const int BotsPerTeamLimitMax = 510;

		private const int BotsPerFormationLimitMin = 0;

		private const int BotsPerFormationLimitMax = 100;

		private const int FriendlyFireDamagePercentMin = 0;

		private const int FriendlyFireDamagePercentMax = 100;

		private const int GameDefinitionIdMin = -2147483648;

		private const int GameDefinitionIdMax = 2147483647;

		private const int MaxScoreToEndDuel = 7;

		private static MultiplayerOptions _instance;

		private readonly MultiplayerOptions.MultiplayerOptionsContainer _current;

		private readonly MultiplayerOptions.MultiplayerOptionsContainer _next;

		private readonly MultiplayerOptions.MultiplayerOptionsContainer _ui;

		public MultiplayerOptions.OptionsCategory CurrentOptionsCategory;

		public enum MultiplayerOptionsAccessMode
		{
			CurrentMapOptions,
			NextMapOptions,
			MissionUIOptions
		}

		public enum OptionValueType
		{
			Bool,
			Integer,
			Enum,
			String
		}

		public enum OptionType
		{
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the name of the server in the server list", 0, 0, null, false, null)]
			ServerName,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Welcome messages which is shown to all players when they enter the server.", 0, 0, null, false, null)]
			WelcomeMessage,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets a password that clients have to enter before connecting to the server.", 0, 0, null, false, null)]
			GamePassword,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets a password that allows players access to admin tools during the game.", 0, 0, null, false, null)]
			AdminPassword,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets ID of the private game definition.", -2147483648, 2147483647, null, false, null)]
			GameDefinitionId,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to kick other players.", 0, 0, null, false, null)]
			AllowPollsToKickPlayers,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to ban other players.", 0, 0, null, false, null)]
			AllowPollsToBanPlayers,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to change the current map.", 0, 0, null, false, null)]
			AllowPollsToChangeMaps,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to use their custom banner.", 0, 0, null, false, null)]
			AllowIndividualBanners,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Use animation progress dependent blocking.", 0, 0, null, false, null)]
			UseRealisticBlocking,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the game type.", 0, 0, null, true, null)]
			PremadeMatchGameMode,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the game type.", 0, 0, null, true, null)]
			GameType,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Enum, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Type of the premade game.", 0, 1, null, true, typeof(PremadeGameType))]
			PremadeGameType,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Map of the game.", 0, 0, null, true, null)]
			Map,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Sets culture for team 1", 0, 0, null, true, null)]
			CultureTeam1,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Sets culture for team 2", 0, 0, null, true, null)]
			CultureTeam2,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Set the maximum amount of player allowed on the server.", 1, 1023, null, false, null)]
			MaxNumberOfPlayers,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Set the amount of players that are needed to start the first round. If not met, players will just wait.", 0, 20, null, false, null)]
			MinNumberOfPlayersForMatchStart,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots on team 1", 0, 510, null, false, null)]
			NumberOfBotsTeam1,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots on team 2", 0, 510, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			NumberOfBotsTeam2,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots per formation", 0, 100, new string[] { "Captain" }, false, null)]
			NumberOfBotsPerFormation,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much melee damage inflicted upon a friend is dealt back to the inflictor.", 0, 100, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			FriendlyFireDamageMeleeSelfPercent,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much melee damage inflicted upon a friend is actually dealt.", 0, 100, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			FriendlyFireDamageMeleeFriendPercent,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much ranged damage inflicted upon a friend is dealt back to the inflictor.", 0, 100, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			FriendlyFireDamageRangedSelfPercent,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much ranged damage inflicted upon a friend is actually dealt.", 0, 100, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			FriendlyFireDamageRangedFriendPercent,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Enum, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Who can spectators look at, and how.", 0, 7, null, true, typeof(SpectatorCameraTypes))]
			SpectatorCamera,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Maximum duration for the warmup. In minutes.", 1, 60, null, false, null)]
			WarmupTimeLimit,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum duration for the map. In minutes.", 1, 60, null, false, null)]
			MapTimeLimit,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum duration for each round. In seconds.", 60, 3600, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege" }, false, null)]
			RoundTimeLimit,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Time available to select class/equipment. In seconds.", 2, 60, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege" }, false, null)]
			RoundPreparationTimeLimit,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum amount of rounds before the game ends.", 1, 99, new string[] { "Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege" }, false, null)]
			RoundTotal,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Wait time after death, before respawning again. In seconds.", 1, 60, new string[] { "Siege" }, false, null)]
			RespawnPeriodTeam1,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Wait time after death, before respawning again. In seconds.", 1, 60, new string[] { "Siege" }, false, null)]
			RespawnPeriodTeam2,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Unlimited gold option.", 0, 0, new string[] { "Battle", "Skirmish", "Siege", "TeamDeathmatch" }, false, null)]
			UnlimitedGold,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Gold gain multiplier from agent deaths.", -100, 100, new string[] { "Siege", "TeamDeathmatch" }, false, null)]
			GoldGainChangePercentageTeam1,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Gold gain multiplier from agent deaths.", -100, 100, new string[] { "Siege", "TeamDeathmatch" }, false, null)]
			GoldGainChangePercentageTeam2,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Min score to win match.", 0, 1023000, new string[] { "TeamDeathmatch" }, false, null)]
			MinScoreToWinMatch,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Min score to win duel.", 0, 7, new string[] { "Duel" }, false, null)]
			MinScoreToWinDuel,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Minimum needed difference in poll results before it is accepted.", 0, 10, null, false, null)]
			PollAcceptThreshold,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum player imbalance between team 1 and team 2. Selecting 0 will disable auto team balancing.", 0, 30, null, false, null)]
			AutoTeamBalanceThreshold,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Enables mission recording.", 0, 0, null, false, null)]
			EnableMissionRecording,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Sets if the game mode uses single spawning.", 0, 0, null, false, null)]
			SingleSpawn,
			[MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Disables the inactivity kick timer.", 0, 0, null, false, null)]
			DisableInactivityKick,
			NumOfSlots
		}

		public enum OptionsCategory
		{
			Default,
			PremadeMatch
		}

		public class MultiplayerOption
		{
			public static MultiplayerOptions.MultiplayerOption CreateMultiplayerOption(MultiplayerOptions.OptionType optionType)
			{
				return new MultiplayerOptions.MultiplayerOption(optionType);
			}

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

			public MultiplayerOptions.MultiplayerOption UpdateValue(bool value)
			{
				this.UpdateValue(value ? 1 : 0);
				return this;
			}

			public MultiplayerOptions.MultiplayerOption UpdateValue(int value)
			{
				this._intValue.UpdateValue(value);
				return this;
			}

			public MultiplayerOptions.MultiplayerOption UpdateValue(string value)
			{
				this._stringValue.UpdateValue(value);
				return this;
			}

			public void GetValue(out bool value)
			{
				value = this._intValue.Value == 1;
			}

			public void GetValue(out int value)
			{
				value = this._intValue.Value;
			}

			public void GetValue(out string value)
			{
				value = this._stringValue.Value;
			}

			public readonly MultiplayerOptions.OptionType OptionType;

			private MultiplayerOptions.MultiplayerOption.IntegerValue _intValue;

			private MultiplayerOptions.MultiplayerOption.StringValue _stringValue;

			private struct IntegerValue
			{
				public static MultiplayerOptions.MultiplayerOption.IntegerValue Invalid
				{
					get
					{
						return default(MultiplayerOptions.MultiplayerOption.IntegerValue);
					}
				}

				public bool IsValid { get; private set; }

				public int Value { get; private set; }

				public static MultiplayerOptions.MultiplayerOption.IntegerValue Create()
				{
					return new MultiplayerOptions.MultiplayerOption.IntegerValue
					{
						IsValid = true
					};
				}

				public void UpdateValue(int value)
				{
					this.Value = value;
				}
			}

			private struct StringValue
			{
				public static MultiplayerOptions.MultiplayerOption.StringValue Invalid
				{
					get
					{
						return default(MultiplayerOptions.MultiplayerOption.StringValue);
					}
				}

				public bool IsValid { get; private set; }

				public string Value { get; private set; }

				public static MultiplayerOptions.MultiplayerOption.StringValue Create()
				{
					return new MultiplayerOptions.MultiplayerOption.StringValue
					{
						IsValid = true
					};
				}

				public void UpdateValue(string value)
				{
					this.Value = value;
				}
			}
		}

		private class MultiplayerOptionsContainer
		{
			public MultiplayerOptionsContainer()
			{
				this._multiplayerOptions = new MultiplayerOptions.MultiplayerOption[43];
			}

			public MultiplayerOptions.MultiplayerOption GetOptionFromOptionType(MultiplayerOptions.OptionType optionType)
			{
				return this._multiplayerOptions[(int)optionType];
			}

			private void CopyOptionFromOther(MultiplayerOptions.OptionType optionType, MultiplayerOptions.MultiplayerOption option)
			{
				this._multiplayerOptions[(int)optionType] = option;
			}

			public void CreateOption(MultiplayerOptions.OptionType optionType)
			{
				this._multiplayerOptions[(int)optionType] = MultiplayerOptions.MultiplayerOption.CreateMultiplayerOption(optionType);
			}

			public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, int value)
			{
				this._multiplayerOptions[(int)optionType].UpdateValue(value);
			}

			public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, string value)
			{
				this._multiplayerOptions[(int)optionType].UpdateValue(value);
			}

			public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, bool value)
			{
				this._multiplayerOptions[(int)optionType].UpdateValue(value ? 1 : 0);
			}

			public void CopyAllValuesTo(MultiplayerOptions.MultiplayerOptionsContainer other)
			{
				this.CopyImmediateEffectValuesTo(other);
				this.CopyNewRoundValuesTo(other);
				this.CopyNewMapValuesTo(other);
			}

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

			public void CopyNewRoundValuesTo(MultiplayerOptions.MultiplayerOptionsContainer other)
			{
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.NumberOfBotsPerFormation, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsPerFormation));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.CultureTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.CultureTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam2));
			}

			public void CopyNewMapValuesTo(MultiplayerOptions.MultiplayerOptionsContainer other)
			{
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.Map, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.GameType, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType));
				other.CopyOptionFromOther(MultiplayerOptions.OptionType.PremadeMatchGameMode, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.PremadeMatchGameMode));
			}

			private readonly MultiplayerOptions.MultiplayerOption[] _multiplayerOptions;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network;

namespace TaleWorlds.MountAndBlade
{
	public static class DedicatedServerConsoleCommandManager
	{
		static DedicatedServerConsoleCommandManager()
		{
			DedicatedServerConsoleCommandManager.AddType(typeof(DedicatedServerConsoleCommandManager));
		}

		public static void AddType(Type type)
		{
			DedicatedServerConsoleCommandManager._commandHandlerTypes.Add(type);
		}

		internal static void HandleConsoleCommand(string command)
		{
			int num = command.IndexOf(' ');
			string text = "";
			string text2;
			if (num > 0)
			{
				text2 = command.Substring(0, num);
				text = command.Substring(num + 1);
			}
			else
			{
				text2 = command;
			}
			bool flag = false;
			MultiplayerOptionsProperty multiplayerOptionsProperty = null;
			bool flag2 = false;
			MultiplayerOptions.OptionType optionType;
			for (optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				multiplayerOptionsProperty = optionType.GetOptionProperty();
				if (multiplayerOptionsProperty != null && optionType.ToString().Equals(text2))
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				if (text == "?")
				{
					Debug.Print(string.Concat(new object[] { "--", optionType, ": ", multiplayerOptionsProperty.Description }), 0, Debug.DebugColor.White, 17179869184UL);
					Debug.Print("--" + (multiplayerOptionsProperty.HasBounds ? string.Concat(new object[] { "Min: ", multiplayerOptionsProperty.BoundsMin, ", Max: ", multiplayerOptionsProperty.BoundsMax, ". " }) : "") + "Current value: " + optionType.GetValueText(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions), 0, Debug.DebugColor.White, 17179869184UL);
				}
				else if (text != "")
				{
					if (multiplayerOptionsProperty.OptionValueType == MultiplayerOptions.OptionValueType.String)
					{
						optionType.SetValue(text, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
					}
					else if (multiplayerOptionsProperty.OptionValueType == MultiplayerOptions.OptionValueType.Integer)
					{
						int num2;
						if (int.TryParse(text, out num2))
						{
							optionType.SetValue(num2, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						}
					}
					else if (multiplayerOptionsProperty.OptionValueType == MultiplayerOptions.OptionValueType.Enum)
					{
						int num3;
						if (int.TryParse(text, out num3))
						{
							optionType.SetValue(num3, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						}
					}
					else if (multiplayerOptionsProperty.OptionValueType == MultiplayerOptions.OptionValueType.Bool)
					{
						bool flag3;
						if (bool.TryParse(text, out flag3))
						{
							optionType.SetValue(flag3, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						}
					}
					else
					{
						Debug.FailedAssert("No valid type found for multiplayer option.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\DedicatedServerConsoleCommandManager.cs", "HandleConsoleCommand", 96);
					}
					Debug.Print(string.Concat(new object[]
					{
						"--Changed: ",
						optionType,
						", to: ",
						optionType.GetValueText(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
					}), 0, Debug.DebugColor.White, 17179869184UL);
				}
				else
				{
					Debug.Print(string.Concat(new object[]
					{
						"--Value of: ",
						optionType,
						", is: ",
						optionType.GetValueText(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
					}), 0, Debug.DebugColor.White, 17179869184UL);
				}
				flag = true;
			}
			if (!flag)
			{
				foreach (Type type in DedicatedServerConsoleCommandManager._commandHandlerTypes)
				{
					foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
					{
						object[] customAttributes = methodInfo.GetCustomAttributes(false);
						for (int j = 0; j < customAttributes.Length; j++)
						{
							ConsoleCommandMethod consoleCommandMethod = customAttributes[j] as ConsoleCommandMethod;
							if (consoleCommandMethod != null && consoleCommandMethod.CommandName.Equals(text2))
							{
								if (text == "?")
								{
									Debug.Print("--" + consoleCommandMethod.CommandName + ": " + consoleCommandMethod.Description, 0, Debug.DebugColor.White, 17179869184UL);
								}
								else
								{
									List<object> list;
									if (!string.IsNullOrEmpty(text))
									{
										(list = new List<object>()).Add(text);
									}
									else
									{
										list = null;
									}
									List<object> list2 = list;
									methodInfo.Invoke(null, (list2 != null) ? list2.ToArray() : null);
								}
								flag = true;
							}
						}
					}
				}
			}
			if (!flag)
			{
				bool flag4;
				string text3 = CommandLineFunctionality.CallFunction(text2, text, out flag4);
				if (flag4)
				{
					Debug.Print(text3, 0, Debug.DebugColor.White, 17179869184UL);
					flag = true;
				}
			}
			if (!flag)
			{
				Debug.Print("--Invalid command is given.", 0, Debug.DebugColor.White, 17179869184UL);
			}
		}

		[UsedImplicitly]
		[ConsoleCommandMethod("list", "Displays a list of all multiplayer options, their values and other possible commands")]
		private static void ListAllCommands()
		{
			Debug.Print("--List of all multiplayer command and their current values:", 0, Debug.DebugColor.White, 17179869184UL);
			for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				Debug.Print(string.Concat(new object[]
				{
					"----",
					optionType,
					": ",
					optionType.GetValueText(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
				}), 0, Debug.DebugColor.White, 17179869184UL);
			}
			Debug.Print("--List of additional commands:", 0, Debug.DebugColor.White, 17179869184UL);
			foreach (Type type in DedicatedServerConsoleCommandManager._commandHandlerTypes)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
				for (int i = 0; i < methods.Length; i++)
				{
					object[] customAttributes = methods[i].GetCustomAttributes(false);
					for (int j = 0; j < customAttributes.Length; j++)
					{
						ConsoleCommandMethod consoleCommandMethod = customAttributes[j] as ConsoleCommandMethod;
						if (consoleCommandMethod != null)
						{
							Debug.Print("----" + consoleCommandMethod.CommandName, 0, Debug.DebugColor.White, 17179869184UL);
						}
					}
				}
			}
			Debug.Print("--Add '?' after a command to get a more detailed description.", 0, Debug.DebugColor.White, 17179869184UL);
		}

		[UsedImplicitly]
		[ConsoleCommandMethod("set_winner_team", "Sets the winner team of flag domination based multiplayer missions.")]
		private static void SetWinnerTeam(string winnerTeamAsString)
		{
			MissionMultiplayerFlagDomination.SetWinnerTeam(int.Parse(winnerTeamAsString));
		}

		[UsedImplicitly]
		[ConsoleCommandMethod("stats", "Displays some game statistics, like FPS and players on the server.")]
		private static void ShowStats()
		{
			Debug.Print("--Current FPS: " + Utilities.GetFps(), 0, Debug.DebugColor.White, 17179869184UL);
			Debug.Print("--Active Players: " + GameNetwork.NetworkPeers.Count((NetworkCommunicator x) => x.IsSynchronized), 0, Debug.DebugColor.White, 17179869184UL);
		}

		[UsedImplicitly]
		[ConsoleCommandMethod("open_monitor", "Opens up the monitor window with continuous data-representations on server performance and network usage.")]
		private static void OpenMonitor()
		{
			DebugNetworkEventStatistics.ControlActivate();
			DebugNetworkEventStatistics.OpenExternalMonitor();
		}

		[UsedImplicitly]
		[ConsoleCommandMethod("crash_game", "Crashes the game process.")]
		private static void CrashGame()
		{
			Debug.Print("Crashing the process...", 0, Debug.DebugColor.White, 17179869184UL);
			throw new Exception("Game crashed by user command");
		}

		private static readonly List<Type> _commandHandlerTypes = new List<Type>();
	}
}

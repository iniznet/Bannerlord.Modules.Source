﻿using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	public class MPLobbyProfileGameModeSelectorItemVM : SelectorItemVM
	{
		public string GameModeCode { get; private set; }

		public MPLobbyProfileGameModeSelectorItemVM(string gameModeCode, TextObject gameModeName)
			: base(gameModeName)
		{
			this.GameModeCode = gameModeCode;
		}
	}
}

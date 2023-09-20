using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade
{
	public class ProfileSelectionState : GameState
	{
		public bool IsDirectPlayPossible { get; private set; } = true;

		public event ProfileSelectionState.OnProfileSelectionEvent OnProfileSelection;

		public void OnProfileSelected()
		{
			NativeOptions.ReadRGLConfigFiles();
			BannerlordConfig.Initialize();
			ProfileSelectionState.OnProfileSelectionEvent onProfileSelection = this.OnProfileSelection;
			if (onProfileSelection != null)
			{
				onProfileSelection();
			}
			this.StartGame();
		}

		public void StartGame()
		{
			Module.CurrentModule.SetInitialModuleScreenAsRootScreen();
		}

		public delegate void OnProfileSelectionEvent();
	}
}

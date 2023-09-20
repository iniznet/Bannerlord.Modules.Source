using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class InitialState : GameState
	{
		public override bool IsMusicMenuState
		{
			get
			{
				return true;
			}
		}

		public event OnInitialMenuOptionInvokedDelegate OnInitialMenuOptionInvoked;

		public event OnGameContentUpdatedDelegate OnGameContentUpdated;

		protected override void OnActivate()
		{
			base.OnActivate();
			if (Utilities.CommandLineArgumentExists("+connect_lobby"))
			{
				MBGameManager.StartNewGame(new MultiplayerGameManager());
			}
			MBMusicManager mbmusicManager = MBMusicManager.Current;
			if (mbmusicManager == null)
			{
				return;
			}
			mbmusicManager.UnpauseMusicManagerSystem();
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
		}

		public void OnExecutedInitialStateOption(InitialStateOption target)
		{
			OnInitialMenuOptionInvokedDelegate onInitialMenuOptionInvoked = this.OnInitialMenuOptionInvoked;
			if (onInitialMenuOptionInvoked == null)
			{
				return;
			}
			onInitialMenuOptionInvoked(target);
		}

		public void RefreshContentState()
		{
			OnGameContentUpdatedDelegate onGameContentUpdated = this.OnGameContentUpdated;
			if (onGameContentUpdated == null)
			{
				return;
			}
			onGameContentUpdated();
		}
	}
}

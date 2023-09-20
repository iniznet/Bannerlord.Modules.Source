using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000227 RID: 551
	public class InitialState : GameState
	{
		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x06001E27 RID: 7719 RVA: 0x0006CB96 File Offset: 0x0006AD96
		public override bool IsMusicMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06001E28 RID: 7720 RVA: 0x0006CB9C File Offset: 0x0006AD9C
		// (remove) Token: 0x06001E29 RID: 7721 RVA: 0x0006CBD4 File Offset: 0x0006ADD4
		public event OnInitialMenuOptionInvokedDelegate OnInitialMenuOptionInvoked;

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x06001E2A RID: 7722 RVA: 0x0006CC0C File Offset: 0x0006AE0C
		// (remove) Token: 0x06001E2B RID: 7723 RVA: 0x0006CC44 File Offset: 0x0006AE44
		public event OnGameContentUpdatedDelegate OnGameContentUpdated;

		// Token: 0x06001E2C RID: 7724 RVA: 0x0006CC79 File Offset: 0x0006AE79
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

		// Token: 0x06001E2D RID: 7725 RVA: 0x0006CCA6 File Offset: 0x0006AEA6
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
		}

		// Token: 0x06001E2E RID: 7726 RVA: 0x0006CCAF File Offset: 0x0006AEAF
		public void OnExecutedInitialStateOption(InitialStateOption target)
		{
			OnInitialMenuOptionInvokedDelegate onInitialMenuOptionInvoked = this.OnInitialMenuOptionInvoked;
			if (onInitialMenuOptionInvoked == null)
			{
				return;
			}
			onInitialMenuOptionInvoked(target);
		}

		// Token: 0x06001E2F RID: 7727 RVA: 0x0006CCC2 File Offset: 0x0006AEC2
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

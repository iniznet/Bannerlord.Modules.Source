using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C9 RID: 457
	public abstract class MBSubModuleBase
	{
		// Token: 0x060019FB RID: 6651 RVA: 0x0005C400 File Offset: 0x0005A600
		protected internal virtual void OnSubModuleLoad()
		{
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x0005C402 File Offset: 0x0005A602
		protected internal virtual void OnSubModuleUnloaded()
		{
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x0005C404 File Offset: 0x0005A604
		protected internal virtual void OnBeforeInitialModuleScreenSetAsRoot()
		{
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x0005C406 File Offset: 0x0005A606
		public virtual void OnConfigChanged()
		{
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x0005C408 File Offset: 0x0005A608
		protected internal virtual void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x0005C40A File Offset: 0x0005A60A
		protected internal virtual void OnApplicationTick(float dt)
		{
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x0005C40C File Offset: 0x0005A60C
		protected internal virtual void AfterAsyncTickTick(float dt)
		{
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x0005C40E File Offset: 0x0005A60E
		protected internal virtual void InitializeGameStarter(Game game, IGameStarter starterObject)
		{
		}

		// Token: 0x06001A03 RID: 6659 RVA: 0x0005C410 File Offset: 0x0005A610
		public virtual void OnGameLoaded(Game game, object initializerObject)
		{
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x0005C412 File Offset: 0x0005A612
		public virtual void OnNewGameCreated(Game game, object initializerObject)
		{
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x0005C414 File Offset: 0x0005A614
		public virtual void BeginGameStart(Game game)
		{
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x0005C416 File Offset: 0x0005A616
		public virtual void OnCampaignStart(Game game, object starterObject)
		{
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x0005C418 File Offset: 0x0005A618
		public virtual void RegisterSubModuleObjects(bool isSavedCampaign)
		{
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x0005C41A File Offset: 0x0005A61A
		public virtual void AfterRegisterSubModuleObjects(bool isSavedCampaign)
		{
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x0005C41C File Offset: 0x0005A61C
		public virtual void OnMultiplayerGameStart(Game game, object starterObject)
		{
		}

		// Token: 0x06001A0A RID: 6666 RVA: 0x0005C41E File Offset: 0x0005A61E
		public virtual void OnGameInitializationFinished(Game game)
		{
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x0005C420 File Offset: 0x0005A620
		public virtual void OnAfterGameInitializationFinished(Game game, object starterObject)
		{
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x0005C422 File Offset: 0x0005A622
		public virtual bool DoLoading(Game game)
		{
			return true;
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x0005C425 File Offset: 0x0005A625
		public virtual void OnGameEnd(Game game)
		{
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x0005C427 File Offset: 0x0005A627
		public virtual void OnMissionBehaviorInitialize(Mission mission)
		{
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x0005C429 File Offset: 0x0005A629
		public virtual void OnBeforeMissionBehaviorInitialize(Mission mission)
		{
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x0005C42B File Offset: 0x0005A62B
		public virtual void OnInitialState()
		{
		}
	}
}

using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.PlatformService;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200038E RID: 910
	public class MBProfileSelectionScreenBase : ScreenBase, IGameStateListener
	{
		// Token: 0x060031DE RID: 12766 RVA: 0x000CF4CE File Offset: 0x000CD6CE
		public MBProfileSelectionScreenBase(ProfileSelectionState state)
		{
			this._state = state;
		}

		// Token: 0x060031DF RID: 12767 RVA: 0x000CF4DD File Offset: 0x000CD6DD
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
		}

		// Token: 0x060031E0 RID: 12768 RVA: 0x000CF4E6 File Offset: 0x000CD6E6
		protected void OnActivateProfileSelection()
		{
			PlatformServices.Instance.LoginUser();
		}

		// Token: 0x060031E1 RID: 12769 RVA: 0x000CF4F2 File Offset: 0x000CD6F2
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x060031E2 RID: 12770 RVA: 0x000CF4F4 File Offset: 0x000CD6F4
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x060031E3 RID: 12771 RVA: 0x000CF4F6 File Offset: 0x000CD6F6
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x060031E4 RID: 12772 RVA: 0x000CF4F8 File Offset: 0x000CD6F8
		void IGameStateListener.OnInitialize()
		{
			Utilities.DisableGlobalLoadingWindow();
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		// Token: 0x040014ED RID: 5357
		private ProfileSelectionState _state;
	}
}

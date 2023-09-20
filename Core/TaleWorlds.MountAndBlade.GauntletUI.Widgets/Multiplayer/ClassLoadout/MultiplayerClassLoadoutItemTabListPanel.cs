using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	// Token: 0x020000BA RID: 186
	public class MultiplayerClassLoadoutItemTabListPanel : ListPanel
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000997 RID: 2455 RVA: 0x0001B830 File Offset: 0x00019A30
		// (remove) Token: 0x06000998 RID: 2456 RVA: 0x0001B868 File Offset: 0x00019A68
		public event Action OnInitialized;

		// Token: 0x06000999 RID: 2457 RVA: 0x0001B89D File Offset: 0x00019A9D
		public MultiplayerClassLoadoutItemTabListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x0001B8A6 File Offset: 0x00019AA6
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				this._isInitialized = true;
				Action onInitialized = this.OnInitialized;
				if (onInitialized == null)
				{
					return;
				}
				onInitialized();
			}
		}

		// Token: 0x04000467 RID: 1127
		private bool _isInitialized;
	}
}

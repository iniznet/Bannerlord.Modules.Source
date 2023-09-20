using System;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000346 RID: 838
	public class TutorialState : GameState
	{
		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x06002EC3 RID: 11971 RVA: 0x000C0559 File Offset: 0x000BE759
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002EC5 RID: 11973 RVA: 0x000C057F File Offset: 0x000BE77F
		protected override void OnActivate()
		{
			base.OnActivate();
			this.MenuContext.Refresh();
		}

		// Token: 0x06002EC6 RID: 11974 RVA: 0x000C0592 File Offset: 0x000BE792
		protected override void OnFinalize()
		{
			this.MenuContext.Destroy();
			this._objectManager.UnregisterObject(this.MenuContext);
			this.MenuContext = null;
			base.OnFinalize();
		}

		// Token: 0x06002EC7 RID: 11975 RVA: 0x000C05BD File Offset: 0x000BE7BD
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			this.MenuContext.OnTick(dt);
		}

		// Token: 0x04000DFF RID: 3583
		private MBObjectManager _objectManager = MBObjectManager.Instance;

		// Token: 0x04000E00 RID: 3584
		public MenuContext MenuContext = MBObjectManager.Instance.CreateObject<MenuContext>();
	}
}

using System;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class TutorialState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this.MenuContext.Refresh();
		}

		protected override void OnFinalize()
		{
			this.MenuContext.Destroy();
			this._objectManager.UnregisterObject(this.MenuContext);
			this.MenuContext = null;
			base.OnFinalize();
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			this.MenuContext.OnTick(dt);
		}

		private MBObjectManager _objectManager = MBObjectManager.Instance;

		public MenuContext MenuContext = MBObjectManager.Instance.CreateObject<MenuContext>();
	}
}

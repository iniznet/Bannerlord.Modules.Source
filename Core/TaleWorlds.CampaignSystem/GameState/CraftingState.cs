using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000333 RID: 819
	public class CraftingState : GameState
	{
		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x06002E2A RID: 11818 RVA: 0x000BFD8C File Offset: 0x000BDF8C
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x06002E2B RID: 11819 RVA: 0x000BFD8F File Offset: 0x000BDF8F
		// (set) Token: 0x06002E2C RID: 11820 RVA: 0x000BFD97 File Offset: 0x000BDF97
		public Crafting CraftingLogic { get; private set; }

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x06002E2D RID: 11821 RVA: 0x000BFDA0 File Offset: 0x000BDFA0
		// (set) Token: 0x06002E2E RID: 11822 RVA: 0x000BFDA8 File Offset: 0x000BDFA8
		public ICraftingStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		// Token: 0x06002E2F RID: 11823 RVA: 0x000BFDB1 File Offset: 0x000BDFB1
		public void InitializeLogic(Crafting newCraftingLogic, bool isReplacingWeaponClass = false)
		{
			this.CraftingLogic = newCraftingLogic;
			if (this._handler != null)
			{
				if (isReplacingWeaponClass)
				{
					this._handler.OnCraftingLogicRefreshed();
					return;
				}
				this._handler.OnCraftingLogicInitialized();
			}
		}

		// Token: 0x04000DE7 RID: 3559
		private ICraftingStateHandler _handler;
	}
}

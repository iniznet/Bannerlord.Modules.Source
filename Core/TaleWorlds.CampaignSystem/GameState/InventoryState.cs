using System;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x0200033A RID: 826
	public class InventoryState : PlayerGameState
	{
		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x06002E4E RID: 11854 RVA: 0x000BFEE4 File Offset: 0x000BE0E4
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x06002E4F RID: 11855 RVA: 0x000BFEE7 File Offset: 0x000BE0E7
		// (set) Token: 0x06002E50 RID: 11856 RVA: 0x000BFEEF File Offset: 0x000BE0EF
		public InventoryLogic InventoryLogic { get; private set; }

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x06002E51 RID: 11857 RVA: 0x000BFEF8 File Offset: 0x000BE0F8
		// (set) Token: 0x06002E52 RID: 11858 RVA: 0x000BFF00 File Offset: 0x000BE100
		public IInventoryStateHandler Handler
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

		// Token: 0x06002E53 RID: 11859 RVA: 0x000BFF09 File Offset: 0x000BE109
		public void InitializeLogic(InventoryLogic inventoryLogic)
		{
			this.InventoryLogic = inventoryLogic;
		}

		// Token: 0x04000DED RID: 3565
		private IInventoryStateHandler _handler;
	}
}

using System;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class InventoryState : PlayerGameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public InventoryLogic InventoryLogic { get; private set; }

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

		public void InitializeLogic(InventoryLogic inventoryLogic)
		{
			this.InventoryLogic = inventoryLogic;
		}

		private IInventoryStateHandler _handler;
	}
}

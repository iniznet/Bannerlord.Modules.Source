using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class CraftingState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public Crafting CraftingLogic { get; private set; }

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

		private ICraftingStateHandler _handler;
	}
}

﻿using System;
using TaleWorlds.GauntletUI;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryImageIdentifierWidget : ImageIdentifierWidget
	{
		public InventoryImageIdentifierWidget(UIContext context)
			: base(context)
		{
		}

		public void SetRenderRequestedPreviousFrame(bool isRequested)
		{
			this._isRenderRequestedPreviousFrame = isRequested;
		}
	}
}

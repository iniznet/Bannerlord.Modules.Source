using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftingMaterialVisualBrushWidget : BrushWidget
	{
		public CraftingMaterialVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._visualDirty)
			{
				this.UpdateVisual();
				this._visualDirty = false;
			}
		}

		private void UpdateVisual()
		{
			this.RegisterBrushStatesOfWidget();
			string text = this.MaterialType;
			if (this.IsBig)
			{
				text += "Big";
			}
			this.SetState(text);
		}

		public string MaterialType
		{
			get
			{
				return this._materialType;
			}
			set
			{
				if (this._materialType != value)
				{
					this._materialType = value;
					this._visualDirty = true;
				}
			}
		}

		public bool IsBig
		{
			get
			{
				return this._isBig;
			}
			set
			{
				if (this._isBig != value)
				{
					this._isBig = value;
					this._visualDirty = true;
				}
			}
		}

		private bool _visualDirty = true;

		private string _materialType;

		private bool _isBig;
	}
}

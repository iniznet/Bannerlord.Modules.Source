using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Quest
{
	// Token: 0x02000055 RID: 85
	public class QuestStageItemWidget : Widget
	{
		// Token: 0x06000479 RID: 1145 RVA: 0x0000DEC6 File Offset: 0x0000C0C6
		public QuestStageItemWidget(UIContext context)
			: base(context)
		{
			this._firstFrame = true;
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0000DED8 File Offset: 0x0000C0D8
		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			this._previousHoverBegan = this._hoverBegan;
			if (!this._firstFrame && this.IsNew)
			{
				bool flag = this.IsMouseOverWidget();
				if (flag && !this._hoverBegan)
				{
					this._hoverBegan = true;
				}
				else if (!flag && this._hoverBegan)
				{
					this._hoverBegan = false;
				}
			}
			this._firstFrame = false;
			if (this._previousHoverBegan && !this._hoverBegan)
			{
				base.EventFired("ResetGlow", Array.Empty<object>());
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0000DF5C File Offset: 0x0000C15C
		private bool IsMouseOverWidget()
		{
			Vector2 globalPosition = base.GlobalPosition;
			return this.IsBetween(base.EventManager.MousePosition.X, globalPosition.X, globalPosition.X + base.Size.X) && this.IsBetween(base.EventManager.MousePosition.Y, globalPosition.Y, globalPosition.Y + base.Size.Y);
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0000DFD0 File Offset: 0x0000C1D0
		private bool IsBetween(float number, float min, float max)
		{
			return number >= min && number <= max;
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x0600047D RID: 1149 RVA: 0x0000DFDF File Offset: 0x0000C1DF
		// (set) Token: 0x0600047E RID: 1150 RVA: 0x0000DFE7 File Offset: 0x0000C1E7
		[Editor(false)]
		public bool IsNew
		{
			get
			{
				return this._isNew;
			}
			set
			{
				if (this._isNew != value)
				{
					this._isNew = value;
					base.OnPropertyChanged(value, "IsNew");
				}
			}
		}

		// Token: 0x040001F3 RID: 499
		private bool _firstFrame;

		// Token: 0x040001F4 RID: 500
		private bool _previousHoverBegan;

		// Token: 0x040001F5 RID: 501
		private bool _hoverBegan;

		// Token: 0x040001F6 RID: 502
		private bool _isNew;
	}
}

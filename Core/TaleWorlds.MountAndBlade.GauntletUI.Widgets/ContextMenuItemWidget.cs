using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000013 RID: 19
	public class ContextMenuItemWidget : Widget
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000DE RID: 222 RVA: 0x0000459A File Offset: 0x0000279A
		// (set) Token: 0x060000DF RID: 223 RVA: 0x000045A2 File Offset: 0x000027A2
		public Widget TypeIconWidget { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x000045AB File Offset: 0x000027AB
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x000045B3 File Offset: 0x000027B3
		public ButtonWidget ActionButtonWidget { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x000045BC File Offset: 0x000027BC
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x000045C4 File Offset: 0x000027C4
		public string TypeIconState { get; set; }

		// Token: 0x060000E4 RID: 228 RVA: 0x000045CD File Offset: 0x000027CD
		public ContextMenuItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000045E0 File Offset: 0x000027E0
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				if (this.TypeIconWidget != null && !string.IsNullOrEmpty(this.TypeIconState))
				{
					this.TypeIconWidget.RegisterBrushStatesOfWidget();
					this.TypeIconWidget.SetState(this.TypeIconState);
				}
				this._isInitialized = true;
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00004634 File Offset: 0x00002834
		protected override void RefreshState()
		{
			base.RefreshState();
			if (!this.CanBeUsed)
			{
				this.SetGlobalAlphaRecursively(0.5f);
				return;
			}
			this.SetGlobalAlphaRecursively(1f);
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000465B File Offset: 0x0000285B
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x00004663 File Offset: 0x00002863
		public bool CanBeUsed
		{
			get
			{
				return this._canBeUsed;
			}
			set
			{
				if (value != this._canBeUsed)
				{
					this._canBeUsed = value;
					base.OnPropertyChanged(value, "CanBeUsed");
					this.RefreshState();
				}
			}
		}

		// Token: 0x0400006D RID: 109
		private const float _disabledAlpha = 0.5f;

		// Token: 0x0400006E RID: 110
		private const float _enabledAlpha = 1f;

		// Token: 0x04000072 RID: 114
		private bool _isInitialized;

		// Token: 0x04000073 RID: 115
		private bool _canBeUsed = true;
	}
}

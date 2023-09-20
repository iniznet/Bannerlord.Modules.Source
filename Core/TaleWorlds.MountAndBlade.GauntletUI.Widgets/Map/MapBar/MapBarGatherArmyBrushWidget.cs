using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	// Token: 0x02000109 RID: 265
	public class MapBarGatherArmyBrushWidget : BrushWidget
	{
		// Token: 0x06000D9E RID: 3486 RVA: 0x0002631C File Offset: 0x0002451C
		public MapBarGatherArmyBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x00026325 File Offset: 0x00024525
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.UpdateVisualState();
				this._initialized = true;
			}
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x00026344 File Offset: 0x00024544
		private void UpdateVisualState()
		{
			base.IsEnabled = this.IsGatherArmyVisible;
			if (!this.IsGatherArmyVisible)
			{
				this.SetState("Disabled");
				return;
			}
			if (this._isInfoBarExtended)
			{
				this.SetState("Extended");
				return;
			}
			this.SetState("Default");
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x00026390 File Offset: 0x00024590
		private void OnMapInfoBarExtendStateChange(bool newState)
		{
			this._isInfoBarExtended = newState;
			this.UpdateVisualState();
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06000DA2 RID: 3490 RVA: 0x0002639F File Offset: 0x0002459F
		// (set) Token: 0x06000DA3 RID: 3491 RVA: 0x000263A7 File Offset: 0x000245A7
		public MapInfoBarWidget InfoBarWidget
		{
			get
			{
				return this._infoBarWidget;
			}
			set
			{
				if (this._infoBarWidget != value)
				{
					this._infoBarWidget = value;
					this._infoBarWidget.OnMapInfoBarExtendStateChange += this.OnMapInfoBarExtendStateChange;
				}
			}
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x000263D0 File Offset: 0x000245D0
		// (set) Token: 0x06000DA5 RID: 3493 RVA: 0x000263D8 File Offset: 0x000245D8
		public bool IsGatherArmyEnabled
		{
			get
			{
				return this._isGatherArmyEnabled;
			}
			set
			{
				if (this._isGatherArmyEnabled != value)
				{
					this._isGatherArmyEnabled = value;
					this.UpdateVisualState();
				}
			}
		}

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x000263F0 File Offset: 0x000245F0
		// (set) Token: 0x06000DA7 RID: 3495 RVA: 0x000263F8 File Offset: 0x000245F8
		public bool IsGatherArmyVisible
		{
			get
			{
				return this._isGatherArmyVisible;
			}
			set
			{
				if (this._isGatherArmyVisible != value)
				{
					this._isGatherArmyVisible = value;
					this.UpdateVisualState();
				}
			}
		}

		// Token: 0x04000647 RID: 1607
		private bool _isInfoBarExtended;

		// Token: 0x04000648 RID: 1608
		private bool _initialized;

		// Token: 0x04000649 RID: 1609
		private MapInfoBarWidget _infoBarWidget;

		// Token: 0x0400064A RID: 1610
		private bool _isGatherArmyEnabled;

		// Token: 0x0400064B RID: 1611
		private bool _isGatherArmyVisible;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000ED RID: 237
	public class DevelopmentQueueVisualIconWidget : Widget
	{
		// Token: 0x06000C57 RID: 3159 RVA: 0x00022ABB File Offset: 0x00020CBB
		public DevelopmentQueueVisualIconWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x00022ACC File Offset: 0x00020CCC
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._animState == DevelopmentQueueVisualIconWidget.AnimState.Start)
			{
				this._tickCount += 1f;
				if (this._tickCount > 20f)
				{
					this._animState = DevelopmentQueueVisualIconWidget.AnimState.Starting;
					return;
				}
			}
			else if (this._animState == DevelopmentQueueVisualIconWidget.AnimState.Starting)
			{
				BrushWidget inProgressIconWidget = this.InProgressIconWidget;
				if (inProgressIconWidget != null)
				{
					inProgressIconWidget.BrushRenderer.RestartAnimation();
				}
				this._animState = DevelopmentQueueVisualIconWidget.AnimState.Playing;
			}
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x00022B38 File Offset: 0x00020D38
		private void UpdateVisual(int index)
		{
			if (this.InProgressIconWidget != null && this.QueueIconWidget != null)
			{
				base.IsVisible = index >= 0;
				this.InProgressIconWidget.IsVisible = index == 0;
				this._animState = (this.InProgressIconWidget.IsVisible ? DevelopmentQueueVisualIconWidget.AnimState.Start : DevelopmentQueueVisualIconWidget.AnimState.Idle);
				this._tickCount = 0f;
				this.QueueIconWidget.IsVisible = index > 0;
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x00022BA2 File Offset: 0x00020DA2
		// (set) Token: 0x06000C5B RID: 3163 RVA: 0x00022BAA File Offset: 0x00020DAA
		[Editor(false)]
		public int QueueIndex
		{
			get
			{
				return this._queueIndex;
			}
			set
			{
				if (this._queueIndex != value)
				{
					this._queueIndex = value;
					base.OnPropertyChanged(value, "QueueIndex");
					this.UpdateVisual(value);
				}
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x00022BCF File Offset: 0x00020DCF
		// (set) Token: 0x06000C5D RID: 3165 RVA: 0x00022BD7 File Offset: 0x00020DD7
		[Editor(false)]
		public Widget QueueIconWidget
		{
			get
			{
				return this._queueIconWidget;
			}
			set
			{
				if (this._queueIconWidget != value)
				{
					this._queueIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "QueueIconWidget");
					this.UpdateVisual(this.QueueIndex);
				}
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x00022C01 File Offset: 0x00020E01
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x00022C09 File Offset: 0x00020E09
		[Editor(false)]
		public BrushWidget InProgressIconWidget
		{
			get
			{
				return this._inProgressIconWidget;
			}
			set
			{
				if (this._inProgressIconWidget != value)
				{
					this._inProgressIconWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "InProgressIconWidget");
					this.UpdateVisual(this.QueueIndex);
				}
			}
		}

		// Token: 0x040005B2 RID: 1458
		private DevelopmentQueueVisualIconWidget.AnimState _animState;

		// Token: 0x040005B3 RID: 1459
		private float _tickCount;

		// Token: 0x040005B4 RID: 1460
		private int _queueIndex = -1;

		// Token: 0x040005B5 RID: 1461
		private Widget _queueIconWidget;

		// Token: 0x040005B6 RID: 1462
		private BrushWidget _inProgressIconWidget;

		// Token: 0x02000194 RID: 404
		public enum AnimState
		{
			// Token: 0x0400090A RID: 2314
			Idle,
			// Token: 0x0400090B RID: 2315
			Start,
			// Token: 0x0400090C RID: 2316
			Starting,
			// Token: 0x0400090D RID: 2317
			Playing
		}
	}
}

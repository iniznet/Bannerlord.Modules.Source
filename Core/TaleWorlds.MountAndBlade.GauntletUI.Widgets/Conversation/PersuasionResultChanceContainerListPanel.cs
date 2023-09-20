using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	// Token: 0x02000151 RID: 337
	public class PersuasionResultChanceContainerListPanel : BrushListPanel
	{
		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06001179 RID: 4473 RVA: 0x000303B2 File Offset: 0x0002E5B2
		// (set) Token: 0x0600117A RID: 4474 RVA: 0x000303BA File Offset: 0x0002E5BA
		public float StayTime { get; set; } = 1f;

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x0600117B RID: 4475 RVA: 0x000303C3 File Offset: 0x0002E5C3
		// (set) Token: 0x0600117C RID: 4476 RVA: 0x000303CB File Offset: 0x0002E5CB
		public Widget CritFailWidget { get; set; }

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x0600117D RID: 4477 RVA: 0x000303D4 File Offset: 0x0002E5D4
		// (set) Token: 0x0600117E RID: 4478 RVA: 0x000303DC File Offset: 0x0002E5DC
		public Widget FailWidget { get; set; }

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x0600117F RID: 4479 RVA: 0x000303E5 File Offset: 0x0002E5E5
		// (set) Token: 0x06001180 RID: 4480 RVA: 0x000303ED File Offset: 0x0002E5ED
		public Widget SuccessWidget { get; set; }

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06001181 RID: 4481 RVA: 0x000303F6 File Offset: 0x0002E5F6
		// (set) Token: 0x06001182 RID: 4482 RVA: 0x000303FE File Offset: 0x0002E5FE
		public Widget CritSuccessWidget { get; set; }

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06001183 RID: 4483 RVA: 0x00030407 File Offset: 0x0002E607
		// (set) Token: 0x06001184 RID: 4484 RVA: 0x0003040F File Offset: 0x0002E60F
		public bool IsResultReady { get; set; }

		// Token: 0x06001185 RID: 4485 RVA: 0x00030418 File Offset: 0x0002E618
		public PersuasionResultChanceContainerListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x00030438 File Offset: 0x0002E638
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.IsResultReady)
			{
				if (this._delayStartTime == -1f && base.AlphaFactor <= 0.001f)
				{
					this._delayStartTime = base.EventManager.Time;
				}
				float num = Mathf.Lerp(base.AlphaFactor, 0f, 0.35f);
				this.SetGlobalAlphaRecursively(num);
				Widget resultVisualWidget = this._resultVisualWidget;
				if (resultVisualWidget != null)
				{
					resultVisualWidget.SetGlobalAlphaRecursively(1f);
				}
				if (this._delayStartTime != -1f && base.EventManager.Time - this._delayStartTime > this.StayTime)
				{
					base.EventFired("OnReadyToContinue", Array.Empty<object>());
				}
			}
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06001187 RID: 4487 RVA: 0x000304EC File Offset: 0x0002E6EC
		// (set) Token: 0x06001188 RID: 4488 RVA: 0x000304F4 File Offset: 0x0002E6F4
		[Editor(false)]
		public int ResultIndex
		{
			get
			{
				return this._resultIndex;
			}
			set
			{
				if (value != this._resultIndex)
				{
					this._resultIndex = value;
					base.OnPropertyChanged(value, "ResultIndex");
					switch (value)
					{
					case 0:
						this._resultVisualWidget = this.CritFailWidget;
						this.SetState("CriticalFail");
						return;
					case 1:
						this._resultVisualWidget = this.FailWidget;
						this.SetState("Fail");
						return;
					case 2:
						this._resultVisualWidget = this.SuccessWidget;
						this.SetState("Success");
						return;
					case 3:
						this._resultVisualWidget = this.CritSuccessWidget;
						this.SetState("CriticalSuccess");
						break;
					default:
						return;
					}
				}
			}
		}

		// Token: 0x04000804 RID: 2052
		private Widget _resultVisualWidget;

		// Token: 0x04000806 RID: 2054
		private float _delayStartTime = -1f;

		// Token: 0x04000807 RID: 2055
		private int _resultIndex;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	// Token: 0x0200014F RID: 335
	public class ConversationPersuasionProgressRichTextWidget : RichTextWidget
	{
		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06001164 RID: 4452 RVA: 0x0002FF8D File Offset: 0x0002E18D
		// (set) Token: 0x06001165 RID: 4453 RVA: 0x0002FF95 File Offset: 0x0002E195
		public float FadeInTime { get; set; } = 1f;

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06001166 RID: 4454 RVA: 0x0002FF9E File Offset: 0x0002E19E
		// (set) Token: 0x06001167 RID: 4455 RVA: 0x0002FFA6 File Offset: 0x0002E1A6
		public float FadeOutTime { get; set; } = 1f;

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06001168 RID: 4456 RVA: 0x0002FFAF File Offset: 0x0002E1AF
		// (set) Token: 0x06001169 RID: 4457 RVA: 0x0002FFB7 File Offset: 0x0002E1B7
		public float StayTime { get; set; } = 2.5f;

		// Token: 0x0600116A RID: 4458 RVA: 0x0002FFC0 File Offset: 0x0002E1C0
		public ConversationPersuasionProgressRichTextWidget(UIContext context)
			: base(context)
		{
			base.PropertyChanged += this.OnSelfPropertyChanged;
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x00030014 File Offset: 0x0002E214
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._startTime == -1f)
			{
				this.SetGlobalAlphaRecursively(0f);
				return;
			}
			float num;
			if (base.EventManager.Time - this._startTime < this.FadeInTime)
			{
				num = Mathf.Lerp(0f, 1f, (base.EventManager.Time - this._startTime) / this.FadeInTime);
			}
			else if (base.EventManager.Time - this._startTime < this.StayTime + this.FadeInTime)
			{
				num = 1f;
			}
			else
			{
				num = Mathf.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, 0f, (base.EventManager.Time - (this._startTime + this.StayTime + this.FadeInTime)) / this.FadeOutTime);
				if (base.ReadOnlyBrush.GlobalAlphaFactor <= 0.001f)
				{
					this._startTime = -1f;
				}
			}
			this.SetGlobalAlphaRecursively(num);
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x00030119 File Offset: 0x0002E319
		private void OnSelfPropertyChanged(PropertyOwnerObject arg1, string propertyName, object newState)
		{
			if (propertyName == "Text" && !string.IsNullOrEmpty(newState as string))
			{
				this._startTime = base.EventManager.Time;
			}
		}

		// Token: 0x040007FA RID: 2042
		private float _startTime = -1f;
	}
}

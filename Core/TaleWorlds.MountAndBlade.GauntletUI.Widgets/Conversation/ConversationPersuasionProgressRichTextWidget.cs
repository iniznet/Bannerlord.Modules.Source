using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	public class ConversationPersuasionProgressRichTextWidget : RichTextWidget
	{
		public float FadeInTime { get; set; } = 1f;

		public float FadeOutTime { get; set; } = 1f;

		public float StayTime { get; set; } = 2.5f;

		public ConversationPersuasionProgressRichTextWidget(UIContext context)
			: base(context)
		{
			base.PropertyChanged += this.OnSelfPropertyChanged;
		}

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

		private void OnSelfPropertyChanged(PropertyOwnerObject arg1, string propertyName, object newState)
		{
			if (propertyName == "Text" && !string.IsNullOrEmpty(newState as string))
			{
				this._startTime = base.EventManager.Time;
			}
		}

		private float _startTime = -1f;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.AdminMessage
{
	public class MultiplayerAdminMessageWidget : Widget
	{
		public TextWidget MessageTextWidget { get; set; }

		public float MessageOnScreenStayTime
		{
			get
			{
				return 5f;
			}
		}

		public float MessageFadeInTime
		{
			get
			{
				return 0.4f;
			}
		}

		public float MessageFadeOutTime
		{
			get
			{
				return 0.2f;
			}
		}

		public MultiplayerAdminMessageWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.ChildCount <= 0)
			{
				this._currentTextOnScreenTime = 0f;
				return;
			}
			this._currentTextOnScreenTime += dt;
			if (this._currentTextOnScreenTime < this.MessageFadeInTime)
			{
				float num = MathF.Lerp(0f, 1f, this._currentTextOnScreenTime / this.MessageFadeInTime, 1E-05f);
				base.Children[0].SetGlobalAlphaRecursively(num);
				base.Children[0].IsVisible = true;
				return;
			}
			if (this._currentTextOnScreenTime > this.MessageFadeInTime && this._currentTextOnScreenTime < this.MessageOnScreenStayTime + this.MessageFadeInTime)
			{
				base.Children[0].SetGlobalAlphaRecursively(1f);
				return;
			}
			if (this._currentTextOnScreenTime < this.MessageFadeInTime + this.MessageOnScreenStayTime + this.MessageFadeOutTime)
			{
				float num2 = MathF.Lerp(1f, 0f, (this._currentTextOnScreenTime - (this.MessageFadeInTime + this.MessageOnScreenStayTime)) / this.MessageFadeOutTime, 1E-05f);
				base.Children[0].SetGlobalAlphaRecursively(num2);
				return;
			}
			MultiplayerAdminMessageItemWidget multiplayerAdminMessageItemWidget = base.Children[0] as MultiplayerAdminMessageItemWidget;
			if (multiplayerAdminMessageItemWidget != null)
			{
				multiplayerAdminMessageItemWidget.Remove();
			}
			this._currentTextOnScreenTime = 0f;
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
		}

		private float _currentTextOnScreenTime;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class SceneNotificationDescriptionTextWidget : TextWidget
	{
		public SceneNotificationDescriptionTextWidget(UIContext context)
			: base(context)
		{
			this._defaultAlignment = base.ReadOnlyBrush.TextHorizontalAlignment;
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._text.LineCount != this._cachedLineCount)
			{
				this._cachedLineCount = this._text.LineCount;
				if (this._cachedLineCount == 1)
				{
					base.ReadOnlyBrush.TextHorizontalAlignment = this._defaultAlignment;
					return;
				}
				base.ReadOnlyBrush.TextHorizontalAlignment = this.MultiLineAlignment;
			}
		}

		[Editor(false)]
		public TextHorizontalAlignment MultiLineAlignment
		{
			get
			{
				return this._multiLineAlignment;
			}
			set
			{
				this._multiLineAlignment = value;
			}
		}

		private int _cachedLineCount;

		private TextHorizontalAlignment _defaultAlignment;

		private TextHorizontalAlignment _multiLineAlignment;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	public class ClanFinanceTextWidget : TextWidget
	{
		public ClanFinanceTextWidget(UIContext context)
			: base(context)
		{
			base.intPropertyChanged += this.IntText_PropertyChanged;
		}

		private void IntText_PropertyChanged(PropertyOwnerObject widget, string propertyName, int propertyValue)
		{
			if (this.NegativeMarkWidget != null && propertyName == "IntText")
			{
				this.NegativeMarkWidget.IsVisible = propertyValue < 0;
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.Text != null && base.Text != string.Empty)
			{
				base.Text = MathF.Abs(base.IntText).ToString();
			}
		}

		[Editor(false)]
		public TextWidget NegativeMarkWidget
		{
			get
			{
				return this._negativeMarkWidget;
			}
			set
			{
				if (this._negativeMarkWidget != value)
				{
					this._negativeMarkWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NegativeMarkWidget");
				}
			}
		}

		private TextWidget _negativeMarkWidget;
	}
}

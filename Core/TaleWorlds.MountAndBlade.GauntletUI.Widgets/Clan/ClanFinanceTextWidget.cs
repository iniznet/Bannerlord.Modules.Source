using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	// Token: 0x02000153 RID: 339
	public class ClanFinanceTextWidget : TextWidget
	{
		// Token: 0x06001199 RID: 4505 RVA: 0x000308C6 File Offset: 0x0002EAC6
		public ClanFinanceTextWidget(UIContext context)
			: base(context)
		{
			base.intPropertyChanged += this.IntText_PropertyChanged;
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x000308E1 File Offset: 0x0002EAE1
		private void IntText_PropertyChanged(PropertyOwnerObject widget, string propertyName, int propertyValue)
		{
			if (this.NegativeMarkWidget != null && propertyName == "IntText")
			{
				this.NegativeMarkWidget.IsVisible = propertyValue < 0;
			}
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00030908 File Offset: 0x0002EB08
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.Text != null && base.Text != string.Empty)
			{
				base.Text = MathF.Abs(base.IntText).ToString();
			}
		}

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x0600119C RID: 4508 RVA: 0x0003094F File Offset: 0x0002EB4F
		// (set) Token: 0x0600119D RID: 4509 RVA: 0x00030957 File Offset: 0x0002EB57
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

		// Token: 0x0400080F RID: 2063
		private TextWidget _negativeMarkWidget;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200003C RID: 60
	public class WarningTextWidget : TextWidget
	{
		// Token: 0x06000334 RID: 820 RVA: 0x0000A4BF File Offset: 0x000086BF
		public WarningTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000335 RID: 821 RVA: 0x0000A4C8 File Offset: 0x000086C8
		// (set) Token: 0x06000336 RID: 822 RVA: 0x0000A4D0 File Offset: 0x000086D0
		[Editor(false)]
		public bool IsWarned
		{
			get
			{
				return this._isWarned;
			}
			set
			{
				if (this._isWarned != value)
				{
					this._isWarned = value;
					base.OnPropertyChanged(value, "IsWarned");
					this.SetState(this._isWarned ? "Warned" : "Default");
				}
			}
		}

		// Token: 0x04000150 RID: 336
		private bool _isWarned;
	}
}

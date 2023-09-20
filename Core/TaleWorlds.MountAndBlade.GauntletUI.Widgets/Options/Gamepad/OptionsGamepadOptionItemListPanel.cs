using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options.Gamepad
{
	// Token: 0x0200006F RID: 111
	public class OptionsGamepadOptionItemListPanel : ListPanel
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000606 RID: 1542 RVA: 0x00011D00 File Offset: 0x0000FF00
		// (remove) Token: 0x06000607 RID: 1543 RVA: 0x00011D38 File Offset: 0x0000FF38
		public event OptionsGamepadOptionItemListPanel.OnActionTextChangeEvent OnActionTextChanged;

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x00011D6D File Offset: 0x0000FF6D
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x00011D75 File Offset: 0x0000FF75
		public OptionsGamepadKeyLocationWidget TargetKey { get; private set; }

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x00011D7E File Offset: 0x0000FF7E
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x00011D86 File Offset: 0x0000FF86
		public string ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (this._actionText != value)
				{
					this._actionText = value;
					OptionsGamepadOptionItemListPanel.OnActionTextChangeEvent onActionTextChanged = this.OnActionTextChanged;
					if (onActionTextChanged == null)
					{
						return;
					}
					onActionTextChanged();
				}
			}
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00011DAD File Offset: 0x0000FFAD
		public OptionsGamepadOptionItemListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00011DB6 File Offset: 0x0000FFB6
		public void SetKeyProperties(OptionsGamepadKeyLocationWidget currentTarget, Widget parentAreaWidget)
		{
			this.TargetKey = currentTarget;
			this.TargetKey.SetKeyProperties(this.ActionText, parentAreaWidget);
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x00011DD1 File Offset: 0x0000FFD1
		// (set) Token: 0x0600060F RID: 1551 RVA: 0x00011DD9 File Offset: 0x0000FFD9
		public int KeyId
		{
			get
			{
				return this._keyId;
			}
			set
			{
				if (value != this._keyId)
				{
					this._keyId = value;
				}
			}
		}

		// Token: 0x0400029F RID: 671
		private string _actionText;

		// Token: 0x040002A0 RID: 672
		private int _keyId;

		// Token: 0x02000187 RID: 391
		// (Invoke) Token: 0x060012FA RID: 4858
		public delegate void OnActionTextChangeEvent();
	}
}

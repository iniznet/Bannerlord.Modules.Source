using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	// Token: 0x02000089 RID: 137
	public class MultiplayerScoreboardStatsParentWidget : Widget
	{
		// Token: 0x0600074C RID: 1868 RVA: 0x0001599E File Offset: 0x00013B9E
		public MultiplayerScoreboardStatsParentWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x000159A8 File Offset: 0x00013BA8
		private void RefreshActiveState()
		{
			float num = (this.IsActive ? this.ActiveAlpha : this.InactiveAlpha);
			foreach (Widget widget in base.AllChildren)
			{
				RichTextWidget richTextWidget;
				TextWidget textWidget;
				if ((richTextWidget = widget as RichTextWidget) != null)
				{
					richTextWidget.SetAlpha(num);
				}
				else if ((textWidget = widget as TextWidget) != null)
				{
					textWidget.SetAlpha(num);
				}
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x0600074E RID: 1870 RVA: 0x00015A2C File Offset: 0x00013C2C
		// (set) Token: 0x0600074F RID: 1871 RVA: 0x00015A34 File Offset: 0x00013C34
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
					this.RefreshActiveState();
				}
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000750 RID: 1872 RVA: 0x00015A58 File Offset: 0x00013C58
		// (set) Token: 0x06000751 RID: 1873 RVA: 0x00015A63 File Offset: 0x00013C63
		public bool IsInactive
		{
			get
			{
				return !this.IsActive;
			}
			set
			{
				if (value == this.IsActive)
				{
					this.IsActive = !value;
					base.OnPropertyChanged(value, "IsInactive");
				}
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000752 RID: 1874 RVA: 0x00015A84 File Offset: 0x00013C84
		// (set) Token: 0x06000753 RID: 1875 RVA: 0x00015A8C File Offset: 0x00013C8C
		public float ActiveAlpha
		{
			get
			{
				return this._activeAlpha;
			}
			set
			{
				if (value != this._activeAlpha)
				{
					this._activeAlpha = value;
					base.OnPropertyChanged(value, "ActiveAlpha");
				}
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x00015AAA File Offset: 0x00013CAA
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x00015AB2 File Offset: 0x00013CB2
		public float InactiveAlpha
		{
			get
			{
				return this._inactiveAlpha;
			}
			set
			{
				if (value != this._inactiveAlpha)
				{
					this._inactiveAlpha = value;
					base.OnPropertyChanged(value, "InactiveAlpha");
				}
			}
		}

		// Token: 0x0400034D RID: 845
		private bool _isActive;

		// Token: 0x0400034E RID: 846
		private float _activeAlpha;

		// Token: 0x0400034F RID: 847
		private float _inactiveAlpha;
	}
}

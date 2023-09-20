using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	// Token: 0x020000BB RID: 187
	public class MultiplayerClassLoadoutTroopCardBrushWidget : BrushWidget
	{
		// Token: 0x0600099B RID: 2459 RVA: 0x0001B8CE File Offset: 0x00019ACE
		public MultiplayerClassLoadoutTroopCardBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x0001B8D8 File Offset: 0x00019AD8
		private void OnCultureIDUpdated()
		{
			if (this.CultureID != null)
			{
				this.SetState(this.CultureID);
				BrushWidget border = this.Border;
				if (border != null)
				{
					border.SetState(this.CultureID);
				}
				BrushWidget classBorder = this.ClassBorder;
				if (classBorder != null)
				{
					classBorder.SetState(this.CultureID);
				}
				BrushWidget classFrame = this.ClassFrame;
				if (classFrame == null)
				{
					return;
				}
				classFrame.SetState(this.CultureID);
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x0600099D RID: 2461 RVA: 0x0001B93D File Offset: 0x00019B3D
		// (set) Token: 0x0600099E RID: 2462 RVA: 0x0001B945 File Offset: 0x00019B45
		[Editor(false)]
		public string CultureID
		{
			get
			{
				return this._cultureID;
			}
			set
			{
				if (value != this._cultureID)
				{
					this._cultureID = value;
					base.OnPropertyChanged<string>(value, "CultureID");
					this.OnCultureIDUpdated();
				}
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x0600099F RID: 2463 RVA: 0x0001B96E File Offset: 0x00019B6E
		// (set) Token: 0x060009A0 RID: 2464 RVA: 0x0001B976 File Offset: 0x00019B76
		[Editor(false)]
		public BrushWidget Border
		{
			get
			{
				return this._border;
			}
			set
			{
				if (value != this._border)
				{
					this._border = value;
					base.OnPropertyChanged<BrushWidget>(value, "Border");
					this.OnCultureIDUpdated();
				}
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x060009A1 RID: 2465 RVA: 0x0001B99A File Offset: 0x00019B9A
		// (set) Token: 0x060009A2 RID: 2466 RVA: 0x0001B9A2 File Offset: 0x00019BA2
		[Editor(false)]
		public BrushWidget ClassBorder
		{
			get
			{
				return this._classBorder;
			}
			set
			{
				if (value != this._classBorder)
				{
					this._classBorder = value;
					base.OnPropertyChanged<BrushWidget>(value, "ClassBorder");
					this.OnCultureIDUpdated();
				}
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x060009A3 RID: 2467 RVA: 0x0001B9C6 File Offset: 0x00019BC6
		// (set) Token: 0x060009A4 RID: 2468 RVA: 0x0001B9CE File Offset: 0x00019BCE
		[Editor(false)]
		public BrushWidget ClassFrame
		{
			get
			{
				return this._classFrame;
			}
			set
			{
				if (value != this._classFrame)
				{
					this._classFrame = value;
					base.OnPropertyChanged<BrushWidget>(value, "ClassFrame");
					base.OnPropertyChanged<BrushWidget>(value, "ClassFrame");
				}
			}
		}

		// Token: 0x04000469 RID: 1129
		private string _cultureID;

		// Token: 0x0400046A RID: 1130
		private BrushWidget _border;

		// Token: 0x0400046B RID: 1131
		private BrushWidget _classBorder;

		// Token: 0x0400046C RID: 1132
		private BrushWidget _classFrame;
	}
}

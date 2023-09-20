using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	// Token: 0x0200001F RID: 31
	public class LauncherBoolBrushWidget : BrushWidget
	{
		// Token: 0x0600012F RID: 303 RVA: 0x00005A18 File Offset: 0x00003C18
		public LauncherBoolBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00005A21 File Offset: 0x00003C21
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.BoolVariableUpdated();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00005A2F File Offset: 0x00003C2F
		private void BoolVariableUpdated()
		{
			(this.TargetWidget ?? this).Brush = (this.BoolVariable ? this.OnTrueBrush : this.OnFalseBrush);
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00005A57 File Offset: 0x00003C57
		// (set) Token: 0x06000133 RID: 307 RVA: 0x00005A5F File Offset: 0x00003C5F
		[DataSourceProperty]
		public bool BoolVariable
		{
			get
			{
				return this._boolVariable;
			}
			set
			{
				if (value != this._boolVariable)
				{
					this._boolVariable = value;
					base.OnPropertyChanged(value, "BoolVariable");
					this.BoolVariableUpdated();
				}
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000134 RID: 308 RVA: 0x00005A83 File Offset: 0x00003C83
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00005A8B File Offset: 0x00003C8B
		[DataSourceProperty]
		public BrushWidget TargetWidget
		{
			get
			{
				return this._targetWidget;
			}
			set
			{
				if (value != this._targetWidget)
				{
					this._targetWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "TargetWidget");
				}
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00005AA9 File Offset: 0x00003CA9
		// (set) Token: 0x06000137 RID: 311 RVA: 0x00005AB1 File Offset: 0x00003CB1
		[DataSourceProperty]
		public Brush OnTrueBrush
		{
			get
			{
				return this._onTrueBrush;
			}
			set
			{
				if (value != this._onTrueBrush)
				{
					this._onTrueBrush = value;
					base.OnPropertyChanged<Brush>(value, "OnTrueBrush");
				}
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000138 RID: 312 RVA: 0x00005ACF File Offset: 0x00003CCF
		// (set) Token: 0x06000139 RID: 313 RVA: 0x00005AD7 File Offset: 0x00003CD7
		[DataSourceProperty]
		public Brush OnFalseBrush
		{
			get
			{
				return this._onFalseBrush;
			}
			set
			{
				if (value != this._onFalseBrush)
				{
					this._onFalseBrush = value;
					base.OnPropertyChanged<Brush>(value, "OnFalseBrush");
				}
			}
		}

		// Token: 0x04000096 RID: 150
		private bool _boolVariable;

		// Token: 0x04000097 RID: 151
		private BrushWidget _targetWidget;

		// Token: 0x04000098 RID: 152
		private Brush _onTrueBrush;

		// Token: 0x04000099 RID: 153
		private Brush _onFalseBrush;
	}
}

using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000007 RID: 7
	public class BoolBrushChangerBrushWidget : BrushWidget
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000021CC File Offset: 0x000003CC
		public BoolBrushChangerBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000021D5 File Offset: 0x000003D5
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialUpdateHandled)
			{
				this.OnBooleanUpdated();
				this._initialUpdateHandled = true;
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000021F4 File Offset: 0x000003F4
		private void OnBooleanUpdated()
		{
			string text = (this.BooleanCheck ? this.TrueBrush : this.FalseBrush);
			Brush brush = base.Context.GetBrush(text);
			BrushWidget brushWidget = this.TargetWidget ?? this;
			brushWidget.Brush = brush;
			if (this.IncludeChildren)
			{
				using (IEnumerator<Widget> enumerator = brushWidget.AllChildren.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BrushWidget brushWidget2;
						if ((brushWidget2 = enumerator.Current as BrushWidget) != null)
						{
							brushWidget2.Brush = brush;
						}
					}
				}
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000228C File Offset: 0x0000048C
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002294 File Offset: 0x00000494
		[Editor(false)]
		public bool BooleanCheck
		{
			get
			{
				return this._booleanCheck;
			}
			set
			{
				if (this._booleanCheck != value)
				{
					this._booleanCheck = value;
					base.OnPropertyChanged(value, "BooleanCheck");
					this.OnBooleanUpdated();
				}
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000022B8 File Offset: 0x000004B8
		// (set) Token: 0x06000015 RID: 21 RVA: 0x000022C0 File Offset: 0x000004C0
		[Editor(false)]
		public string TrueBrush
		{
			get
			{
				return this._trueBrush;
			}
			set
			{
				if (this._trueBrush != value)
				{
					this._trueBrush = value;
					base.OnPropertyChanged<string>(value, "TrueBrush");
				}
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000016 RID: 22 RVA: 0x000022E3 File Offset: 0x000004E3
		// (set) Token: 0x06000017 RID: 23 RVA: 0x000022EB File Offset: 0x000004EB
		[Editor(false)]
		public string FalseBrush
		{
			get
			{
				return this._falseBrush;
			}
			set
			{
				if (this._falseBrush != value)
				{
					this._falseBrush = value;
					base.OnPropertyChanged<string>(value, "FalseBrush");
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000230E File Offset: 0x0000050E
		// (set) Token: 0x06000019 RID: 25 RVA: 0x00002316 File Offset: 0x00000516
		[Editor(false)]
		public BrushWidget TargetWidget
		{
			get
			{
				return this._targetWidget;
			}
			set
			{
				if (this._targetWidget != value)
				{
					this._targetWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "TargetWidget");
				}
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002334 File Offset: 0x00000534
		// (set) Token: 0x0600001B RID: 27 RVA: 0x0000233C File Offset: 0x0000053C
		[Editor(false)]
		public bool IncludeChildren
		{
			get
			{
				return this._includeChildren;
			}
			set
			{
				if (this._includeChildren != value)
				{
					this._includeChildren = value;
					base.OnPropertyChanged(value, "IncludeChildren");
				}
			}
		}

		// Token: 0x04000004 RID: 4
		private bool _initialUpdateHandled;

		// Token: 0x04000005 RID: 5
		private bool _booleanCheck;

		// Token: 0x04000006 RID: 6
		private string _trueBrush;

		// Token: 0x04000007 RID: 7
		private string _falseBrush;

		// Token: 0x04000008 RID: 8
		private BrushWidget _targetWidget;

		// Token: 0x04000009 RID: 9
		private bool _includeChildren;
	}
}

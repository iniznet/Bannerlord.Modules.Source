using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class BoolBrushChangerBrushWidget : BrushWidget
	{
		public BoolBrushChangerBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialUpdateHandled)
			{
				this.OnBooleanUpdated();
				this._initialUpdateHandled = true;
			}
		}

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

		private bool _initialUpdateHandled;

		private bool _booleanCheck;

		private string _trueBrush;

		private string _falseBrush;

		private BrushWidget _targetWidget;

		private bool _includeChildren;
	}
}

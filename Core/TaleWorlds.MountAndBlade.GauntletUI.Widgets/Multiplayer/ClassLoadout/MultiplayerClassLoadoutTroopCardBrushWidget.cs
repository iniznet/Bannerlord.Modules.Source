using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	public class MultiplayerClassLoadoutTroopCardBrushWidget : BrushWidget
	{
		public MultiplayerClassLoadoutTroopCardBrushWidget(UIContext context)
			: base(context)
		{
		}

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

		private string _cultureID;

		private BrushWidget _border;

		private BrushWidget _classBorder;

		private BrushWidget _classFrame;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	public class MultiplayerClassLoadoutTroopInfoBrushWidget : BrushWidget
	{
		public MultiplayerClassLoadoutTroopInfoBrushWidget(UIContext context)
			: base(context)
		{
			this.SetAlpha(this.DefaultAlpha);
		}

		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.SetAlpha(1f);
		}

		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.SetAlpha(this.DefaultAlpha);
		}

		public override void OnBrushChanged()
		{
			base.OnBrushChanged();
			this.SetAlpha(this.DefaultAlpha);
		}

		[Editor(false)]
		public float DefaultAlpha
		{
			get
			{
				return this._defaultAlpha;
			}
			set
			{
				if (value != this._defaultAlpha)
				{
					this._defaultAlpha = value;
					base.OnPropertyChanged(value, "DefaultAlpha");
				}
			}
		}

		private float _defaultAlpha = 0.7f;
	}
}

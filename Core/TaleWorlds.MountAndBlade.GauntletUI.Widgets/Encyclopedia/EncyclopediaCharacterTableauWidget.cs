using System;
using TaleWorlds.GauntletUI;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	public class EncyclopediaCharacterTableauWidget : CharacterTableauWidget
	{
		public EncyclopediaCharacterTableauWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateVisual(bool isDead)
		{
			base.Brush.SaturationFactor = (float)(isDead ? (-100) : 0);
		}

		[Editor(false)]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (this._isDead != value)
				{
					this._isDead = value;
					base.OnPropertyChanged(value, "IsDead");
					this.UpdateVisual(value);
				}
			}
		}

		private bool _isDead;
	}
}

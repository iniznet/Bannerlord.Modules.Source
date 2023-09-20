using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	public class MultiplayerClassLoadoutTroopTupleVisualWidget : Widget
	{
		public MultiplayerClassLoadoutTroopTupleVisualWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				base.Sprite = base.Context.SpriteData.GetSprite("MPClassLoadout\\TroopTupleImages\\" + this.TroopTypeCode + "1");
				base.Sprite = base.Sprite;
				base.SuggestedWidth = (float)base.Sprite.Width;
				base.SuggestedHeight = (float)base.Sprite.Height;
				this._initialized = true;
			}
		}

		public string FactionCode
		{
			get
			{
				return this._factionCode;
			}
			set
			{
				if (value != this._factionCode)
				{
					this._factionCode = value;
					base.OnPropertyChanged<string>(value, "FactionCode");
				}
			}
		}

		public string TroopTypeCode
		{
			get
			{
				return this._troopTypeCode;
			}
			set
			{
				if (value != this._troopTypeCode)
				{
					this._troopTypeCode = value;
					base.OnPropertyChanged<string>(value, "TroopTypeCode");
				}
			}
		}

		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChanged(value, "UseSecondary");
				}
			}
		}

		private bool _initialized;

		private string _factionCode;

		private string _troopTypeCode;

		private bool _useSecondary;
	}
}

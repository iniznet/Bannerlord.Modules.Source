using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftingWeaponTypeIconWidget : Widget
	{
		public CraftingWeaponTypeIconWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateIconVisual()
		{
			base.Sprite = base.Context.SpriteData.GetSprite("Crafting\\WeaponTypes\\" + this.WeaponType);
		}

		[Editor(false)]
		public string WeaponType
		{
			get
			{
				return this._weaponType;
			}
			set
			{
				if (value != this._weaponType)
				{
					this._weaponType = value;
					this.UpdateIconVisual();
					base.OnPropertyChanged<string>(value, "WeaponType");
				}
			}
		}

		private string _weaponType;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x0200014A RID: 330
	public class CraftingWeaponTypeIconWidget : Widget
	{
		// Token: 0x06001148 RID: 4424 RVA: 0x0002FD77 File Offset: 0x0002DF77
		public CraftingWeaponTypeIconWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x0002FD80 File Offset: 0x0002DF80
		private void UpdateIconVisual()
		{
			base.Sprite = base.Context.SpriteData.GetSprite("Crafting\\WeaponTypes\\" + this.WeaponType);
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x0600114A RID: 4426 RVA: 0x0002FDA8 File Offset: 0x0002DFA8
		// (set) Token: 0x0600114B RID: 4427 RVA: 0x0002FDB0 File Offset: 0x0002DFB0
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

		// Token: 0x040007EE RID: 2030
		private string _weaponType;
	}
}

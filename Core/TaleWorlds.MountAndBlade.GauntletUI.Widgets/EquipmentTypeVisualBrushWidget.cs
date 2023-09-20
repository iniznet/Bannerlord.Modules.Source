using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000019 RID: 25
	public class EquipmentTypeVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000121 RID: 289 RVA: 0x00005206 File Offset: 0x00003406
		public EquipmentTypeVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00005216 File Offset: 0x00003416
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._hasVisualDetermined)
			{
				this.RegisterBrushStatesOfWidget();
				this.UpdateVisual(this.Type);
				this._hasVisualDetermined = true;
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00005240 File Offset: 0x00003440
		private void UpdateVisual(int type)
		{
			switch (type)
			{
			case 0:
				this.SetState("Invalid");
				return;
			case 1:
				this.SetState("Horse");
				return;
			case 2:
				this.SetState("OneHandedWeapon");
				return;
			case 3:
				this.SetState("TwoHandedWeapon");
				return;
			case 4:
				this.SetState("Polearm");
				return;
			case 5:
				this.SetState("Arrows");
				return;
			case 6:
				this.SetState("Bolts");
				return;
			case 7:
				this.SetState("Shield");
				return;
			case 8:
				this.SetState("Bow");
				return;
			case 9:
				this.SetState("Crossbow");
				return;
			case 10:
				this.SetState("Thrown");
				return;
			case 11:
				this.SetState("Goods");
				return;
			case 12:
				this.SetState("HeadArmor");
				return;
			case 13:
				this.SetState("BodyArmor");
				return;
			case 14:
				this.SetState("LegArmor");
				return;
			case 15:
				this.SetState("HandArmor");
				return;
			case 16:
				this.SetState("Pistol");
				return;
			case 17:
				this.SetState("Musket");
				return;
			case 18:
				this.SetState("Bullets");
				return;
			case 19:
				this.SetState("Animal");
				return;
			case 20:
				this.SetState("Book");
				return;
			case 21:
				this.SetState("ChestArmor");
				return;
			case 22:
				this.SetState("Cape");
				return;
			case 23:
				this.SetState("HorseHarness");
				return;
			case 24:
				this.SetState("Banner");
				return;
			default:
				this.SetState("Invalid");
				return;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000124 RID: 292 RVA: 0x000053F3 File Offset: 0x000035F3
		// (set) Token: 0x06000125 RID: 293 RVA: 0x000053FB File Offset: 0x000035FB
		[Editor(false)]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (this._type != value)
				{
					this._type = value;
					base.OnPropertyChanged(value, "Type");
				}
			}
		}

		// Token: 0x0400008A RID: 138
		private bool _hasVisualDetermined;

		// Token: 0x0400008B RID: 139
		private int _type = -1;
	}
}

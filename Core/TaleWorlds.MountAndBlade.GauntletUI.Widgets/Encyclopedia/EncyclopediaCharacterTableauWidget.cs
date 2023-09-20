using System;
using TaleWorlds.GauntletUI;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	// Token: 0x02000135 RID: 309
	public class EncyclopediaCharacterTableauWidget : CharacterTableauWidget
	{
		// Token: 0x06001066 RID: 4198 RVA: 0x0002E178 File Offset: 0x0002C378
		public EncyclopediaCharacterTableauWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x0002E181 File Offset: 0x0002C381
		private void UpdateVisual(bool isDead)
		{
			base.Brush.SaturationFactor = (float)(isDead ? (-100) : 0);
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x06001068 RID: 4200 RVA: 0x0002E197 File Offset: 0x0002C397
		// (set) Token: 0x06001069 RID: 4201 RVA: 0x0002E19F File Offset: 0x0002C39F
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

		// Token: 0x0400078F RID: 1935
		private bool _isDead;
	}
}

using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.Conversation
{
	// Token: 0x020000E6 RID: 230
	public class PersuasionChanceVisualListPanel : ListPanel
	{
		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x000218BC File Offset: 0x0001FABC
		// (set) Token: 0x06000BFD RID: 3069 RVA: 0x000218C4 File Offset: 0x0001FAC4
		public bool IsFailChance { get; set; }

		// Token: 0x06000BFE RID: 3070 RVA: 0x000218CD File Offset: 0x0001FACD
		public PersuasionChanceVisualListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x000218D6 File Offset: 0x0001FAD6
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.IsVisible = !this.IsFailChance && this.ChanceValue > 0;
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06000C00 RID: 3072 RVA: 0x000218F9 File Offset: 0x0001FAF9
		// (set) Token: 0x06000C01 RID: 3073 RVA: 0x00021901 File Offset: 0x0001FB01
		public int ChanceValue
		{
			get
			{
				return this._chanceValue;
			}
			set
			{
				if (this._chanceValue != value)
				{
					this._chanceValue = value;
					base.OnPropertyChanged(value, "ChanceValue");
				}
			}
		}

		// Token: 0x0400058A RID: 1418
		private int _chanceValue;
	}
}

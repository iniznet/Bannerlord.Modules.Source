using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x02000103 RID: 259
	public class ClanLordStatusItemVM : ViewModel
	{
		// Token: 0x0600180E RID: 6158 RVA: 0x00058045 File Offset: 0x00056245
		public ClanLordStatusItemVM(ClanLordStatusItemVM.LordStatus status, TextObject hintText)
		{
			this.Type = (int)status;
			this.Hint = new HintViewModel(hintText, null);
		}

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x0600180F RID: 6159 RVA: 0x00058068 File Offset: 0x00056268
		// (set) Token: 0x06001810 RID: 6160 RVA: 0x00058070 File Offset: 0x00056270
		[DataSourceProperty]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x06001811 RID: 6161 RVA: 0x0005808E File Offset: 0x0005628E
		// (set) Token: 0x06001812 RID: 6162 RVA: 0x00058096 File Offset: 0x00056296
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x04000B6F RID: 2927
		private int _type = -1;

		// Token: 0x04000B70 RID: 2928
		private HintViewModel _hint;

		// Token: 0x02000235 RID: 565
		public enum LordStatus
		{
			// Token: 0x040010BC RID: 4284
			Dead,
			// Token: 0x040010BD RID: 4285
			Married,
			// Token: 0x040010BE RID: 4286
			Pregnant,
			// Token: 0x040010BF RID: 4287
			InBattle,
			// Token: 0x040010C0 RID: 4288
			InSiege,
			// Token: 0x040010C1 RID: 4289
			Child,
			// Token: 0x040010C2 RID: 4290
			Prisoner,
			// Token: 0x040010C3 RID: 4291
			Sick
		}
	}
}

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000E4 RID: 228
	public class ReloadPhaseItemVM : ViewModel
	{
		// Token: 0x060014C6 RID: 5318 RVA: 0x00043D6B File Offset: 0x00041F6B
		public ReloadPhaseItemVM(float progress, float relativeDurationToMaxDuration)
		{
			this.Update(progress, relativeDurationToMaxDuration);
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x00043D7B File Offset: 0x00041F7B
		public void Update(float progress, float relativeDurationToMaxDuration)
		{
			this.Progress = progress;
			this.RelativeDurationToMaxDuration = relativeDurationToMaxDuration;
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x060014C8 RID: 5320 RVA: 0x00043D8B File Offset: 0x00041F8B
		// (set) Token: 0x060014C9 RID: 5321 RVA: 0x00043D93 File Offset: 0x00041F93
		[DataSourceProperty]
		public float Progress
		{
			get
			{
				return this._progress;
			}
			set
			{
				if (value != this._progress)
				{
					this._progress = value;
					base.OnPropertyChangedWithValue(value, "Progress");
				}
			}
		}

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x060014CA RID: 5322 RVA: 0x00043DB1 File Offset: 0x00041FB1
		// (set) Token: 0x060014CB RID: 5323 RVA: 0x00043DB9 File Offset: 0x00041FB9
		[DataSourceProperty]
		public float RelativeDurationToMaxDuration
		{
			get
			{
				return this._relativeDurationToMaxDuration;
			}
			set
			{
				if (value != this._relativeDurationToMaxDuration)
				{
					this._relativeDurationToMaxDuration = value;
					base.OnPropertyChangedWithValue(value, "RelativeDurationToMaxDuration");
				}
			}
		}

		// Token: 0x040009F0 RID: 2544
		private float _progress;

		// Token: 0x040009F1 RID: 2545
		private float _relativeDurationToMaxDuration;
	}
}

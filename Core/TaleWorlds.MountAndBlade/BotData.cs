using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000296 RID: 662
	public class BotData
	{
		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x060023D5 RID: 9173 RVA: 0x00084C47 File Offset: 0x00082E47
		public int Score
		{
			get
			{
				return this.KillCount * 3 + this.AssistCount;
			}
		}

		// Token: 0x060023D6 RID: 9174 RVA: 0x00084C58 File Offset: 0x00082E58
		public BotData()
		{
		}

		// Token: 0x060023D7 RID: 9175 RVA: 0x00084C60 File Offset: 0x00082E60
		public BotData(int kill, int assist, int death, int alive)
		{
			this.KillCount = kill;
			this.DeathCount = death;
			this.AssistCount = assist;
			this.AliveCount = alive;
		}

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x060023D8 RID: 9176 RVA: 0x00084C85 File Offset: 0x00082E85
		public bool IsAnyValid
		{
			get
			{
				return this.KillCount != 0 || this.DeathCount != 0 || this.AssistCount != 0 || this.AliveCount != 0;
			}
		}

		// Token: 0x060023D9 RID: 9177 RVA: 0x00084CAA File Offset: 0x00082EAA
		public void ResetKillDeathAssist()
		{
			this.KillCount = 0;
			this.DeathCount = 0;
			this.AssistCount = 0;
		}

		// Token: 0x04000D1B RID: 3355
		public int AliveCount;

		// Token: 0x04000D1C RID: 3356
		public int KillCount;

		// Token: 0x04000D1D RID: 3357
		public int DeathCount;

		// Token: 0x04000D1E RID: 3358
		public int AssistCount;
	}
}

using System;

namespace TaleWorlds.MountAndBlade
{
	public class BotData
	{
		public int Score
		{
			get
			{
				return this.KillCount * 3 + this.AssistCount;
			}
		}

		public BotData()
		{
		}

		public BotData(int kill, int assist, int death, int alive)
		{
			this.KillCount = kill;
			this.DeathCount = death;
			this.AssistCount = assist;
			this.AliveCount = alive;
		}

		public bool IsAnyValid
		{
			get
			{
				return this.KillCount != 0 || this.DeathCount != 0 || this.AssistCount != 0 || this.AliveCount != 0;
			}
		}

		public void ResetKillDeathAssist()
		{
			this.KillCount = 0;
			this.DeathCount = 0;
			this.AssistCount = 0;
		}

		public int AliveCount;

		public int KillCount;

		public int DeathCount;

		public int AssistCount;
	}
}

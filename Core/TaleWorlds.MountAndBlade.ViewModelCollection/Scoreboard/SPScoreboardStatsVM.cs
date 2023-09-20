using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	// Token: 0x02000013 RID: 19
	public class SPScoreboardStatsVM : ViewModel
	{
		// Token: 0x06000178 RID: 376 RVA: 0x00006628 File Offset: 0x00004828
		public SPScoreboardStatsVM(TextObject name)
		{
			this._nameTextObject = name;
			this.Kill = 0;
			this.Dead = 0;
			this.Wounded = 0;
			this.Routed = 0;
			this.Remaining = 0;
			this.ReadyToUpgrade = 0;
			this.IsMainParty = false;
			this.IsMainHero = false;
			this.RefreshValues();
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000668B File Offset: 0x0000488B
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject nameTextObject = this._nameTextObject;
			this.NameText = ((nameTextObject != null) ? nameTextObject.ToString() : null) ?? "";
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000066B4 File Offset: 0x000048B4
		public void UpdateScores(int numberRemaining, int numberDead, int numberWounded, int numberRouted, int numberKilled, int numberReadyToUpgrade)
		{
			this.Kill += numberKilled;
			this.Dead += numberDead;
			this.Wounded += numberWounded;
			this.Routed += numberRouted;
			this.Remaining += numberRemaining;
			this.ReadyToUpgrade += numberReadyToUpgrade;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00006718 File Offset: 0x00004918
		public bool IsAnyStatRelevant()
		{
			return this.Kill > 1 || this.Dead > 1 || this.Wounded > 1 || this.Routed > 1 || this.Remaining > 1 || this.ReadyToUpgrade > 1;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00006754 File Offset: 0x00004954
		public SPScoreboardStatsVM GetScoreForOneAliveMember()
		{
			return new SPScoreboardStatsVM(TextObject.Empty)
			{
				Remaining = MathF.Min(1, this.Remaining),
				Dead = 0,
				Wounded = 0,
				Routed = MathF.Min(1, this.Routed),
				Kill = 0,
				ReadyToUpgrade = 0
			};
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600017D RID: 381 RVA: 0x000067AB File Offset: 0x000049AB
		// (set) Token: 0x0600017E RID: 382 RVA: 0x000067B3 File Offset: 0x000049B3
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600017F RID: 383 RVA: 0x000067D6 File Offset: 0x000049D6
		// (set) Token: 0x06000180 RID: 384 RVA: 0x000067DE File Offset: 0x000049DE
		[DataSourceProperty]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChangedWithValue(value, "IsMainHero");
				}
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000181 RID: 385 RVA: 0x000067FC File Offset: 0x000049FC
		// (set) Token: 0x06000182 RID: 386 RVA: 0x00006804 File Offset: 0x00004A04
		[DataSourceProperty]
		public bool IsMainParty
		{
			get
			{
				return this._isMainParty;
			}
			set
			{
				if (value != this._isMainParty)
				{
					this._isMainParty = value;
					base.OnPropertyChangedWithValue(value, "IsMainParty");
				}
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00006822 File Offset: 0x00004A22
		// (set) Token: 0x06000184 RID: 388 RVA: 0x0000682A File Offset: 0x00004A2A
		[DataSourceProperty]
		public int Kill
		{
			get
			{
				return this._kill;
			}
			set
			{
				if (value != this._kill)
				{
					this._kill = value;
					base.OnPropertyChangedWithValue(value, "Kill");
				}
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00006848 File Offset: 0x00004A48
		// (set) Token: 0x06000186 RID: 390 RVA: 0x00006850 File Offset: 0x00004A50
		[DataSourceProperty]
		public int Dead
		{
			get
			{
				return this._dead;
			}
			set
			{
				if (value != this._dead)
				{
					this._dead = value;
					base.OnPropertyChangedWithValue(value, "Dead");
				}
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000187 RID: 391 RVA: 0x0000686E File Offset: 0x00004A6E
		// (set) Token: 0x06000188 RID: 392 RVA: 0x00006876 File Offset: 0x00004A76
		[DataSourceProperty]
		public int Wounded
		{
			get
			{
				return this._wounded;
			}
			set
			{
				if (value != this._wounded)
				{
					this._wounded = value;
					base.OnPropertyChangedWithValue(value, "Wounded");
				}
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00006894 File Offset: 0x00004A94
		// (set) Token: 0x0600018A RID: 394 RVA: 0x0000689C File Offset: 0x00004A9C
		[DataSourceProperty]
		public int Routed
		{
			get
			{
				return this._routed;
			}
			set
			{
				if (value != this._routed)
				{
					this._routed = value;
					base.OnPropertyChangedWithValue(value, "Routed");
				}
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600018B RID: 395 RVA: 0x000068BA File Offset: 0x00004ABA
		// (set) Token: 0x0600018C RID: 396 RVA: 0x000068C2 File Offset: 0x00004AC2
		[DataSourceProperty]
		public int Remaining
		{
			get
			{
				return this._remaining;
			}
			set
			{
				if (value != this._remaining)
				{
					this._remaining = value;
					base.OnPropertyChangedWithValue(value, "Remaining");
				}
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600018D RID: 397 RVA: 0x000068E0 File Offset: 0x00004AE0
		// (set) Token: 0x0600018E RID: 398 RVA: 0x000068E8 File Offset: 0x00004AE8
		[DataSourceProperty]
		public int ReadyToUpgrade
		{
			get
			{
				return this._readyToUpgrade;
			}
			set
			{
				if (value != this._readyToUpgrade)
				{
					this._readyToUpgrade = value;
					base.OnPropertyChangedWithValue(value, "ReadyToUpgrade");
				}
			}
		}

		// Token: 0x040000AD RID: 173
		private TextObject _nameTextObject;

		// Token: 0x040000AE RID: 174
		private string _nameText = "";

		// Token: 0x040000AF RID: 175
		private int _kill;

		// Token: 0x040000B0 RID: 176
		private int _dead;

		// Token: 0x040000B1 RID: 177
		private int _wounded;

		// Token: 0x040000B2 RID: 178
		private int _routed;

		// Token: 0x040000B3 RID: 179
		private int _remaining;

		// Token: 0x040000B4 RID: 180
		private int _readyToUpgrade;

		// Token: 0x040000B5 RID: 181
		private bool _isMainParty;

		// Token: 0x040000B6 RID: 182
		private bool _isMainHero;
	}
}

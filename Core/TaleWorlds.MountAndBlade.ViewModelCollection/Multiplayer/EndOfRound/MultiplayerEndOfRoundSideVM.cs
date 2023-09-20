using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.EndOfRound
{
	// Token: 0x020000C5 RID: 197
	public class MultiplayerEndOfRoundSideVM : ViewModel
	{
		// Token: 0x06001280 RID: 4736 RVA: 0x0003CD47 File Offset: 0x0003AF47
		public void SetData(BasicCultureObject culture, int score, bool isWinner, bool useSecondary)
		{
			this._culture = culture;
			this.CultureID = culture.StringId;
			this.Score = score;
			this.IsWinner = isWinner;
			this.UseSecondary = useSecondary;
			this.RefreshValues();
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x0003CD78 File Offset: 0x0003AF78
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CultureName = this._culture.Name.ToString();
		}

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x06001282 RID: 4738 RVA: 0x0003CD96 File Offset: 0x0003AF96
		// (set) Token: 0x06001283 RID: 4739 RVA: 0x0003CD9E File Offset: 0x0003AF9E
		[DataSourceProperty]
		public bool IsWinner
		{
			get
			{
				return this._isWinner;
			}
			set
			{
				if (value != this._isWinner)
				{
					this._isWinner = value;
					base.OnPropertyChangedWithValue(value, "IsWinner");
				}
			}
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x06001284 RID: 4740 RVA: 0x0003CDBC File Offset: 0x0003AFBC
		// (set) Token: 0x06001285 RID: 4741 RVA: 0x0003CDC4 File Offset: 0x0003AFC4
		[DataSourceProperty]
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChangedWithValue(value, "UseSecondary");
				}
			}
		}

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x06001286 RID: 4742 RVA: 0x0003CDE2 File Offset: 0x0003AFE2
		// (set) Token: 0x06001287 RID: 4743 RVA: 0x0003CDEA File Offset: 0x0003AFEA
		[DataSourceProperty]
		public string CultureID
		{
			get
			{
				return this._cultureID;
			}
			set
			{
				if (value != this._cultureID)
				{
					this._cultureID = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureID");
				}
			}
		}

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x06001288 RID: 4744 RVA: 0x0003CE0D File Offset: 0x0003B00D
		// (set) Token: 0x06001289 RID: 4745 RVA: 0x0003CE15 File Offset: 0x0003B015
		[DataSourceProperty]
		public string CultureName
		{
			get
			{
				return this._cultureName;
			}
			set
			{
				if (value != this._cultureName)
				{
					this._cultureName = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureName");
				}
			}
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x0600128A RID: 4746 RVA: 0x0003CE38 File Offset: 0x0003B038
		// (set) Token: 0x0600128B RID: 4747 RVA: 0x0003CE40 File Offset: 0x0003B040
		[DataSourceProperty]
		public int Score
		{
			get
			{
				return this._score;
			}
			set
			{
				if (value != this._score)
				{
					this._score = value;
					base.OnPropertyChangedWithValue(value, "Score");
				}
			}
		}

		// Token: 0x040008D5 RID: 2261
		private BasicCultureObject _culture;

		// Token: 0x040008D6 RID: 2262
		private bool _isWinner;

		// Token: 0x040008D7 RID: 2263
		private bool _useSecondary;

		// Token: 0x040008D8 RID: 2264
		private string _cultureID;

		// Token: 0x040008D9 RID: 2265
		private string _cultureName;

		// Token: 0x040008DA RID: 2266
		private int _score;
	}
}

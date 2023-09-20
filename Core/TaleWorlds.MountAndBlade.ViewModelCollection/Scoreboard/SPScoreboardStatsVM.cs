using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	public class SPScoreboardStatsVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject nameTextObject = this._nameTextObject;
			this.NameText = ((nameTextObject != null) ? nameTextObject.ToString() : null) ?? "";
		}

		public void UpdateScores(int numberRemaining, int numberDead, int numberWounded, int numberRouted, int numberKilled, int numberReadyToUpgrade)
		{
			this.Kill += numberKilled;
			this.Dead += numberDead;
			this.Wounded += numberWounded;
			this.Routed += numberRouted;
			this.Remaining += numberRemaining;
			this.ReadyToUpgrade += numberReadyToUpgrade;
		}

		public bool IsAnyStatRelevant()
		{
			return this.Remaining >= 1 || this.Routed >= 1;
		}

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

		private TextObject _nameTextObject;

		private string _nameText = "";

		private int _kill;

		private int _dead;

		private int _wounded;

		private int _routed;

		private int _remaining;

		private int _readyToUpgrade;

		private bool _isMainParty;

		private bool _isMainHero;
	}
}

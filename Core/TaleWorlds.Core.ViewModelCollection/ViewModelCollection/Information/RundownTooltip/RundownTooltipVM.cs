using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Information.RundownTooltip
{
	public class RundownTooltipVM : TooltipBaseVM
	{
		public bool IsInitializedProperly { get; }

		public RundownTooltipVM(Type invokedType, object[] invokedArgs)
			: base(invokedType, invokedArgs)
		{
			this.Lines = new MBBindingList<RundownLineVM>();
			if (invokedArgs.Length == 5)
			{
				this._titleTextSource = invokedArgs[2] as TextObject;
				this._summaryTextSource = invokedArgs[3] as TextObject;
				this._valueCategorization = (RundownTooltipVM.ValueCategorization)invokedArgs[4];
				bool flag = !TextObject.IsNullOrEmpty(this._titleTextSource);
				bool flag2 = !TextObject.IsNullOrEmpty(this._summaryTextSource);
				this.IsInitializedProperly = flag && flag2;
			}
			else
			{
				Debug.FailedAssert("Unexpected number of arguments for rundown tooltip", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core.ViewModelCollection\\Information\\RundownTooltip\\RundownTooltipVM.cs", ".ctor", 46);
			}
			this.ValueCategorizationAsInt = (int)this._valueCategorization;
			this._isPeriodicRefreshEnabled = true;
			this._periodicRefreshDelay = 1f;
			this.Refresh();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject titleTextSource = this._titleTextSource;
			this.TitleText = ((titleTextSource != null) ? titleTextSource.ToString() : null);
			this.RefreshExtendText();
			this.RefreshExpectedChangeText();
		}

		protected override void OnPeriodicRefresh()
		{
			base.OnPeriodicRefresh();
			this.Refresh();
		}

		protected override void OnIsExtendedChanged()
		{
			base.OnIsExtendedChanged();
			base.IsActive = false;
			this.Refresh();
		}

		private void Refresh()
		{
			base.InvokeRefreshData<RundownTooltipVM>(this);
			this.RefreshExtendText();
			this.RefreshExpectedChangeText();
		}

		private void RefreshExpectedChangeText()
		{
			if (this._summaryTextSource != null)
			{
				string text = "DefaultChange";
				if (this._valueCategorization != RundownTooltipVM.ValueCategorization.None)
				{
					text = (((float)((this._valueCategorization == RundownTooltipVM.ValueCategorization.LargeIsBetter) ? 1 : (-1)) * this.CurrentExpectedChange < 0f) ? "NegativeChange" : "PositiveChange");
				}
				TextObject textObject = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null);
				textObject.SetTextVariable("LEFT", this._summaryTextSource.ToString());
				textObject.SetTextVariable("RIGHT", string.Concat(new string[]
				{
					"<span style=\"",
					text,
					"\">",
					string.Format("{0:0.##}", this.CurrentExpectedChange),
					"</span>"
				}));
				this.ExpectedChangeText = textObject.ToString();
			}
		}

		private void RefreshExtendText()
		{
			GameTexts.SetVariable("EXTEND_KEY", GameTexts.FindText("str_game_key_text", "anyalt").ToString());
			this.ExtendText = GameTexts.FindText("str_map_tooltip_info", null).ToString();
		}

		public static void RefreshGenericRundownTooltip(RundownTooltipVM rundownTooltip, object[] args)
		{
			rundownTooltip.IsActive = rundownTooltip.IsInitializedProperly;
			if (rundownTooltip.IsActive)
			{
				Func<List<RundownLineVM>> func = args[0] as Func<List<RundownLineVM>>;
				Func<List<RundownLineVM>> func2 = args[1] as Func<List<RundownLineVM>>;
				float num = 0f;
				rundownTooltip.Lines.Clear();
				Func<List<RundownLineVM>> func3 = ((rundownTooltip.IsExtended && func2 != null) ? func2 : func);
				List<RundownLineVM> list = ((func3 != null) ? func3() : null);
				if (list != null)
				{
					foreach (RundownLineVM rundownLineVM in list)
					{
						num += rundownLineVM.Value;
						rundownTooltip.Lines.Add(rundownLineVM);
					}
				}
				rundownTooltip.CurrentExpectedChange = num;
			}
		}

		[DataSourceProperty]
		public MBBindingList<RundownLineVM> Lines
		{
			get
			{
				return this._lines;
			}
			set
			{
				if (value != this._lines)
				{
					this._lines = value;
					base.OnPropertyChangedWithValue<MBBindingList<RundownLineVM>>(value, "Lines");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string ExpectedChangeText
		{
			get
			{
				return this._expectedChangeText;
			}
			set
			{
				if (value != this._expectedChangeText)
				{
					this._expectedChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExpectedChangeText");
				}
			}
		}

		[DataSourceProperty]
		public int ValueCategorizationAsInt
		{
			get
			{
				return this._valueCategorizationAsInt;
			}
			set
			{
				if (value != this._valueCategorizationAsInt)
				{
					this._valueCategorizationAsInt = value;
					base.OnPropertyChangedWithValue(value, "ValueCategorizationAsInt");
				}
			}
		}

		[DataSourceProperty]
		public string ExtendText
		{
			get
			{
				return this._extendText;
			}
			set
			{
				if (value != this._extendText)
				{
					this._extendText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExtendText");
				}
			}
		}

		public float CurrentExpectedChange;

		private readonly RundownTooltipVM.ValueCategorization _valueCategorization;

		private readonly TextObject _titleTextSource;

		private readonly TextObject _summaryTextSource;

		private MBBindingList<RundownLineVM> _lines;

		private string _titleText;

		private string _expectedChangeText;

		private int _valueCategorizationAsInt;

		private string _extendText;

		public enum ValueCategorization
		{
			None,
			LargeIsBetter,
			SmallIsBetter
		}
	}
}

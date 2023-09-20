using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.BoardGame
{
	// Token: 0x0200003B RID: 59
	public class BoardGameInstructionsVM : ViewModel
	{
		// Token: 0x06000433 RID: 1075 RVA: 0x00012B7C File Offset: 0x00010D7C
		public BoardGameInstructionsVM(CultureObject.BoardGameType boardGameType)
		{
			this._boardGameType = boardGameType;
			this.InstructionList = new MBBindingList<BoardGameInstructionVM>();
			for (int i = 0; i < this.GetNumberOfInstructions(this._boardGameType); i++)
			{
				this.InstructionList.Add(new BoardGameInstructionVM(this._boardGameType, i));
			}
			this._currentInstructionIndex = 0;
			if (this.InstructionList.Count > 0)
			{
				this.InstructionList[0].IsEnabled = true;
			}
			this.RefreshValues();
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00012BFC File Offset: 0x00010DFC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.InstructionsText = GameTexts.FindText("str_how_to_play", null).ToString();
			this.PreviousText = GameTexts.FindText("str_previous", null).ToString();
			this.NextText = GameTexts.FindText("str_next", null).ToString();
			this.InstructionList.ApplyActionOnAllItems(delegate(BoardGameInstructionVM x)
			{
				x.RefreshValues();
			});
			if (this._currentInstructionIndex >= 0 && this._currentInstructionIndex < this.InstructionList.Count)
			{
				TextObject textObject = new TextObject("{=hUSmlhNh}{CURRENT_PAGE}/{TOTAL_PAGES}", null);
				textObject.SetTextVariable("CURRENT_PAGE", (this._currentInstructionIndex + 1).ToString());
				textObject.SetTextVariable("TOTAL_PAGES", this.InstructionList.Count.ToString());
				this.CurrentPageText = textObject.ToString();
				this.IsPreviousButtonEnabled = this._currentInstructionIndex != 0;
				this.IsNextButtonEnabled = this._currentInstructionIndex < this.InstructionList.Count - 1;
			}
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00012D18 File Offset: 0x00010F18
		public void ExecuteShowPrevious()
		{
			if (this._currentInstructionIndex > 0 && this._currentInstructionIndex < this.InstructionList.Count)
			{
				this.InstructionList[this._currentInstructionIndex].IsEnabled = false;
				this._currentInstructionIndex--;
				this.InstructionList[this._currentInstructionIndex].IsEnabled = true;
				this.RefreshValues();
			}
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x00012D84 File Offset: 0x00010F84
		public void ExecuteShowNext()
		{
			if (this._currentInstructionIndex >= 0 && this._currentInstructionIndex < this.InstructionList.Count - 1)
			{
				this.InstructionList[this._currentInstructionIndex].IsEnabled = false;
				this._currentInstructionIndex++;
				this.InstructionList[this._currentInstructionIndex].IsEnabled = true;
				this.RefreshValues();
			}
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00012DF1 File Offset: 0x00010FF1
		private int GetNumberOfInstructions(CultureObject.BoardGameType game)
		{
			switch (game)
			{
			case 0:
				return 4;
			case 1:
				return 5;
			case 2:
				return 3;
			case 3:
				return 2;
			case 4:
				return 4;
			case 5:
				return 4;
			default:
				return 0;
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x00012E20 File Offset: 0x00011020
		// (set) Token: 0x06000439 RID: 1081 RVA: 0x00012E28 File Offset: 0x00011028
		[DataSourceProperty]
		public bool IsPreviousButtonEnabled
		{
			get
			{
				return this._isPreviousButtonEnabled;
			}
			set
			{
				if (value != this._isPreviousButtonEnabled)
				{
					this._isPreviousButtonEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPreviousButtonEnabled");
				}
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00012E46 File Offset: 0x00011046
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x00012E4E File Offset: 0x0001104E
		[DataSourceProperty]
		public bool IsNextButtonEnabled
		{
			get
			{
				return this._isNextButtonEnabled;
			}
			set
			{
				if (value != this._isNextButtonEnabled)
				{
					this._isNextButtonEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsNextButtonEnabled");
				}
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00012E6C File Offset: 0x0001106C
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x00012E74 File Offset: 0x00011074
		[DataSourceProperty]
		public string InstructionsText
		{
			get
			{
				return this._instructionsText;
			}
			set
			{
				if (value != this._instructionsText)
				{
					this._instructionsText = value;
					base.OnPropertyChangedWithValue<string>(value, "InstructionsText");
				}
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x00012E97 File Offset: 0x00011097
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x00012E9F File Offset: 0x0001109F
		[DataSourceProperty]
		public string PreviousText
		{
			get
			{
				return this._previousText;
			}
			set
			{
				if (value != this._previousText)
				{
					this._previousText = value;
					base.OnPropertyChangedWithValue<string>(value, "PreviousText");
				}
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00012EC2 File Offset: 0x000110C2
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x00012ECA File Offset: 0x000110CA
		[DataSourceProperty]
		public string NextText
		{
			get
			{
				return this._nextText;
			}
			set
			{
				if (value != this._nextText)
				{
					this._nextText = value;
					base.OnPropertyChangedWithValue<string>(value, "NextText");
				}
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00012EED File Offset: 0x000110ED
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x00012EF5 File Offset: 0x000110F5
		[DataSourceProperty]
		public string CurrentPageText
		{
			get
			{
				return this._currentPageText;
			}
			set
			{
				if (value != this._currentPageText)
				{
					this._currentPageText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentPageText");
				}
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x00012F18 File Offset: 0x00011118
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x00012F20 File Offset: 0x00011120
		[DataSourceProperty]
		public MBBindingList<BoardGameInstructionVM> InstructionList
		{
			get
			{
				return this._instructionList;
			}
			set
			{
				if (value != this._instructionList)
				{
					this._instructionList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BoardGameInstructionVM>>(value, "InstructionList");
				}
			}
		}

		// Token: 0x0400022F RID: 559
		private readonly CultureObject.BoardGameType _boardGameType;

		// Token: 0x04000230 RID: 560
		private int _currentInstructionIndex;

		// Token: 0x04000231 RID: 561
		private bool _isPreviousButtonEnabled;

		// Token: 0x04000232 RID: 562
		private bool _isNextButtonEnabled;

		// Token: 0x04000233 RID: 563
		private string _instructionsText;

		// Token: 0x04000234 RID: 564
		private string _previousText;

		// Token: 0x04000235 RID: 565
		private string _nextText;

		// Token: 0x04000236 RID: 566
		private string _currentPageText;

		// Token: 0x04000237 RID: 567
		private MBBindingList<BoardGameInstructionVM> _instructionList;
	}
}

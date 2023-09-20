using System;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x0200012D RID: 301
	public abstract class CharacterCreationStageBaseVM : ViewModel
	{
		// Token: 0x06001CA6 RID: 7334 RVA: 0x00066D94 File Offset: 0x00064F94
		protected CharacterCreationStageBaseVM(CharacterCreation characterCreation, Action affirmativeAction, TextObject affirmativeActionText, Action negativeAction, TextObject negativeActionText, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex)
		{
			this._characterCreation = characterCreation;
			this._goToIndex = goToIndex;
			this._affirmativeAction = affirmativeAction;
			this._negativeAction = negativeAction;
			this._affirmativeActionText = affirmativeActionText;
			this._negativeActionText = negativeActionText;
			this.TotalStageCount = totalStagesCount;
			this.CurrentStageIndex = currentStageIndex;
			this.FurthestIndex = furthestIndex;
		}

		// Token: 0x06001CA7 RID: 7335
		public abstract void OnNextStage();

		// Token: 0x06001CA8 RID: 7336
		public abstract void OnPreviousStage();

		// Token: 0x06001CA9 RID: 7337
		public abstract bool CanAdvanceToNextStage();

		// Token: 0x06001CAA RID: 7338 RVA: 0x00066E22 File Offset: 0x00065022
		public virtual void ExecuteGoToIndex(int index)
		{
			this._goToIndex(index);
		}

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x06001CAB RID: 7339 RVA: 0x00066E30 File Offset: 0x00065030
		public bool CanAdvance
		{
			get
			{
				return this.CanAdvanceToNextStage();
			}
		}

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x06001CAC RID: 7340 RVA: 0x00066E38 File Offset: 0x00065038
		public string NextStageText
		{
			get
			{
				return this._affirmativeActionText.ToString();
			}
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x06001CAD RID: 7341 RVA: 0x00066E45 File Offset: 0x00065045
		public string PreviousStageText
		{
			get
			{
				return this._negativeActionText.ToString();
			}
		}

		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x06001CAE RID: 7342 RVA: 0x00066E52 File Offset: 0x00065052
		// (set) Token: 0x06001CAF RID: 7343 RVA: 0x00066E5A File Offset: 0x0006505A
		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x06001CB0 RID: 7344 RVA: 0x00066E7D File Offset: 0x0006507D
		// (set) Token: 0x06001CB1 RID: 7345 RVA: 0x00066E85 File Offset: 0x00065085
		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x06001CB2 RID: 7346 RVA: 0x00066EA8 File Offset: 0x000650A8
		// (set) Token: 0x06001CB3 RID: 7347 RVA: 0x00066EB0 File Offset: 0x000650B0
		[DataSourceProperty]
		public string SelectionText
		{
			get
			{
				return this._selectionText;
			}
			set
			{
				if (value != this._selectionText)
				{
					this._selectionText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectionText");
				}
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x06001CB4 RID: 7348 RVA: 0x00066ED3 File Offset: 0x000650D3
		// (set) Token: 0x06001CB5 RID: 7349 RVA: 0x00066EDB File Offset: 0x000650DB
		[DataSourceProperty]
		public int TotalStageCount
		{
			get
			{
				return this._totalStageCount;
			}
			set
			{
				if (value != this._totalStageCount)
				{
					this._totalStageCount = value;
					base.OnPropertyChangedWithValue(value, "TotalStageCount");
				}
			}
		}

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x06001CB6 RID: 7350 RVA: 0x00066EF9 File Offset: 0x000650F9
		// (set) Token: 0x06001CB7 RID: 7351 RVA: 0x00066F01 File Offset: 0x00065101
		[DataSourceProperty]
		public int FurthestIndex
		{
			get
			{
				return this._furthestIndex;
			}
			set
			{
				if (value != this._furthestIndex)
				{
					this._furthestIndex = value;
					base.OnPropertyChangedWithValue(value, "FurthestIndex");
				}
			}
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x06001CB8 RID: 7352 RVA: 0x00066F1F File Offset: 0x0006511F
		// (set) Token: 0x06001CB9 RID: 7353 RVA: 0x00066F27 File Offset: 0x00065127
		[DataSourceProperty]
		public int CurrentStageIndex
		{
			get
			{
				return this._currentStageIndex;
			}
			set
			{
				if (value != this._currentStageIndex)
				{
					this._currentStageIndex = value;
					base.OnPropertyChangedWithValue(value, "CurrentStageIndex");
				}
			}
		}

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x06001CBA RID: 7354 RVA: 0x00066F45 File Offset: 0x00065145
		// (set) Token: 0x06001CBB RID: 7355 RVA: 0x00066F4D File Offset: 0x0006514D
		[DataSourceProperty]
		public bool AnyItemSelected
		{
			get
			{
				return this._anyItemSelected;
			}
			set
			{
				if (value != this._anyItemSelected)
				{
					this._anyItemSelected = value;
					base.OnPropertyChangedWithValue(value, "AnyItemSelected");
				}
			}
		}

		// Token: 0x04000D7F RID: 3455
		protected readonly CharacterCreation _characterCreation;

		// Token: 0x04000D80 RID: 3456
		protected readonly Action _affirmativeAction;

		// Token: 0x04000D81 RID: 3457
		protected readonly Action _negativeAction;

		// Token: 0x04000D82 RID: 3458
		protected readonly TextObject _affirmativeActionText;

		// Token: 0x04000D83 RID: 3459
		protected readonly TextObject _negativeActionText;

		// Token: 0x04000D84 RID: 3460
		private readonly Action<int> _goToIndex;

		// Token: 0x04000D85 RID: 3461
		private string _title = "";

		// Token: 0x04000D86 RID: 3462
		private string _description = "";

		// Token: 0x04000D87 RID: 3463
		private string _selectionText = "";

		// Token: 0x04000D88 RID: 3464
		private int _totalStageCount = -1;

		// Token: 0x04000D89 RID: 3465
		private int _currentStageIndex = -1;

		// Token: 0x04000D8A RID: 3466
		private int _furthestIndex = -1;

		// Token: 0x04000D8B RID: 3467
		private bool _anyItemSelected;
	}
}

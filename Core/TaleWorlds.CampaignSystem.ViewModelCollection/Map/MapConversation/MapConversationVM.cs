using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Conversation;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapConversation
{
	// Token: 0x0200004C RID: 76
	public class MapConversationVM : ViewModel
	{
		// Token: 0x06000570 RID: 1392 RVA: 0x0001B233 File Offset: 0x00019433
		public MapConversationVM(Action onContinue, Func<string> getContinueInputText)
		{
			this._onContinue = onContinue;
			this.DialogController = new MissionConversationVM(getContinueInputText, false);
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x0001B256 File Offset: 0x00019456
		public void ExecuteContinue()
		{
			Action onContinue = this._onContinue;
			if (onContinue == null)
			{
				return;
			}
			onContinue();
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0001B268 File Offset: 0x00019468
		public override void OnFinalize()
		{
			base.OnFinalize();
			MissionConversationVM dialogController = this.DialogController;
			if (dialogController != null)
			{
				dialogController.OnFinalize();
			}
			this.DialogController = null;
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x0001B288 File Offset: 0x00019488
		// (set) Token: 0x06000574 RID: 1396 RVA: 0x0001B290 File Offset: 0x00019490
		[DataSourceProperty]
		public MissionConversationVM DialogController
		{
			get
			{
				return this._dialogController;
			}
			set
			{
				if (value != this._dialogController)
				{
					this._dialogController = value;
					base.OnPropertyChangedWithValue<MissionConversationVM>(value, "DialogController");
				}
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000575 RID: 1397 RVA: 0x0001B2AE File Offset: 0x000194AE
		// (set) Token: 0x06000576 RID: 1398 RVA: 0x0001B2B6 File Offset: 0x000194B6
		[DataSourceProperty]
		public object TableauData
		{
			get
			{
				return this._tableauData;
			}
			set
			{
				if (value != this._tableauData)
				{
					this._tableauData = value;
					base.OnPropertyChangedWithValue<object>(value, "TableauData");
				}
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000577 RID: 1399 RVA: 0x0001B2D4 File Offset: 0x000194D4
		// (set) Token: 0x06000578 RID: 1400 RVA: 0x0001B2DC File Offset: 0x000194DC
		[DataSourceProperty]
		public bool IsTableauEnabled
		{
			get
			{
				return this._isTableauEnabled;
			}
			set
			{
				if (value != this._isTableauEnabled)
				{
					this._isTableauEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsTableauEnabled");
				}
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000579 RID: 1401 RVA: 0x0001B2FA File Offset: 0x000194FA
		// (set) Token: 0x0600057A RID: 1402 RVA: 0x0001B302 File Offset: 0x00019502
		[DataSourceProperty]
		public bool IsBarterActive
		{
			get
			{
				return this._isBarterActive;
			}
			set
			{
				if (value != this._isBarterActive)
				{
					this._isBarterActive = value;
					base.OnPropertyChangedWithValue(value, "IsBarterActive");
				}
			}
		}

		// Token: 0x04000250 RID: 592
		private readonly Action _onContinue;

		// Token: 0x04000251 RID: 593
		private MissionConversationVM _dialogController;

		// Token: 0x04000252 RID: 594
		private object _tableauData;

		// Token: 0x04000253 RID: 595
		private bool _isTableauEnabled = true;

		// Token: 0x04000254 RID: 596
		private bool _isBarterActive;
	}
}

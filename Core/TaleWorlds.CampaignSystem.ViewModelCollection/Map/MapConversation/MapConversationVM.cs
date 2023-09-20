using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Conversation;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapConversation
{
	public class MapConversationVM : ViewModel
	{
		public MapConversationVM(Action onContinue, Func<string> getContinueInputText)
		{
			this._onContinue = onContinue;
			this.DialogController = new MissionConversationVM(getContinueInputText, false);
		}

		public void ExecuteContinue()
		{
			Action onContinue = this._onContinue;
			if (onContinue == null)
			{
				return;
			}
			onContinue();
		}

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

		private readonly Action _onContinue;

		private MissionConversationVM _dialogController;

		private object _tableauData;

		private bool _isTableauEnabled = true;

		private bool _isBarterActive;
	}
}

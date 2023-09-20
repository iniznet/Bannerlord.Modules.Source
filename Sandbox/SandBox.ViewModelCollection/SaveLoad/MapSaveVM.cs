using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.SaveLoad
{
	public class MapSaveVM : ViewModel
	{
		public MapSaveVM(Action<bool> onActiveStateChange)
		{
			this._onActiveStateChange = onActiveStateChange;
			CampaignEvents.OnSaveStartedEvent.AddNonSerializedListener(this, new Action(this.OnSaveStarted));
			CampaignEvents.OnSaveOverEvent.AddNonSerializedListener(this, new Action<bool, string>(this.OnSaveOver));
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject textObject = TextObject.Empty;
			textObject = new TextObject("{=cp2XDjeq}Saving...", null);
			this.SavingText = textObject.ToString();
		}

		private void OnSaveOver(bool isSuccessful, string saveName)
		{
			this.IsActive = false;
			Action<bool> onActiveStateChange = this._onActiveStateChange;
			if (onActiveStateChange == null)
			{
				return;
			}
			onActiveStateChange(false);
		}

		private void OnSaveStarted()
		{
			this.IsActive = true;
			Action<bool> onActiveStateChange = this._onActiveStateChange;
			if (onActiveStateChange == null)
			{
				return;
			}
			onActiveStateChange(true);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnSaveStartedEvent.ClearListeners(this);
			CampaignEvents.OnSaveOverEvent.ClearListeners(this);
		}

		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		[DataSourceProperty]
		public string SavingText
		{
			get
			{
				return this._savingText;
			}
			set
			{
				if (value != this._savingText)
				{
					this._savingText = value;
					base.OnPropertyChangedWithValue<string>(value, "SavingText");
				}
			}
		}

		private readonly Action<bool> _onActiveStateChange;

		private string _savingText;

		private bool _isActive;
	}
}

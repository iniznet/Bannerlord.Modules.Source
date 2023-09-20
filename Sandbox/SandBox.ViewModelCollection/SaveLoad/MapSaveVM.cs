using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.SaveLoad
{
	// Token: 0x0200000D RID: 13
	public class MapSaveVM : ViewModel
	{
		// Token: 0x06000123 RID: 291 RVA: 0x00007618 File Offset: 0x00005818
		public MapSaveVM(Action<bool> onActiveStateChange)
		{
			this._onActiveStateChange = onActiveStateChange;
			CampaignEvents.OnSaveStartedEvent.AddNonSerializedListener(this, new Action(this.OnSaveStarted));
			CampaignEvents.OnSaveOverEvent.AddNonSerializedListener(this, new Action<bool, string>(this.OnSaveOver));
			this.RefreshValues();
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00007668 File Offset: 0x00005868
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject textObject = TextObject.Empty;
			textObject = new TextObject("{=cp2XDjeq}Saving...", null);
			this.SavingText = textObject.ToString();
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00007699 File Offset: 0x00005899
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

		// Token: 0x06000126 RID: 294 RVA: 0x000076B3 File Offset: 0x000058B3
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

		// Token: 0x06000127 RID: 295 RVA: 0x000076CD File Offset: 0x000058CD
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnSaveStartedEvent.ClearListeners(this);
			CampaignEvents.OnSaveOverEvent.ClearListeners(this);
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000128 RID: 296 RVA: 0x000076EB File Offset: 0x000058EB
		// (set) Token: 0x06000129 RID: 297 RVA: 0x000076F3 File Offset: 0x000058F3
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

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00007711 File Offset: 0x00005911
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00007719 File Offset: 0x00005919
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

		// Token: 0x04000071 RID: 113
		private readonly Action<bool> _onActiveStateChange;

		// Token: 0x04000072 RID: 114
		private string _savingText;

		// Token: 0x04000073 RID: 115
		private bool _isActive;
	}
}

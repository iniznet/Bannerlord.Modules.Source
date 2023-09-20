using System;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x0200008D RID: 141
	public class SettlementDailyProjectVM : SettlementProjectVM
	{
		// Token: 0x06000DD5 RID: 3541 RVA: 0x00037AE2 File Offset: 0x00035CE2
		public SettlementDailyProjectVM(Action<SettlementProjectVM, bool> onSelection, Action<SettlementProjectVM> onSetAsCurrent, Action onResetCurrent, Building building)
			: base(onSelection, onSetAsCurrent, onResetCurrent, building)
		{
			base.IsDaily = true;
			this.RefreshValues();
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x00037AFC File Offset: 0x00035CFC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DefaultText = GameTexts.FindText("str_default", null).ToString();
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x00037B1A File Offset: 0x00035D1A
		public override void RefreshProductionText()
		{
			base.RefreshProductionText();
			base.ProductionText = new TextObject("{=bd7oAQq6}Daily", null).ToString();
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x00037B38 File Offset: 0x00035D38
		public override void ExecuteAddToQueue()
		{
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x00037B3A File Offset: 0x00035D3A
		public override void ExecuteSetAsActiveDevelopment()
		{
			this._onSelection(this, false);
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x00037B49 File Offset: 0x00035D49
		public override void ExecuteSetAsCurrent()
		{
			Action<SettlementProjectVM> onSetAsCurrent = this._onSetAsCurrent;
			if (onSetAsCurrent == null)
			{
				return;
			}
			onSetAsCurrent(this);
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x00037B5C File Offset: 0x00035D5C
		public override void ExecuteResetCurrent()
		{
			Action onResetCurrent = this._onResetCurrent;
			if (onResetCurrent == null)
			{
				return;
			}
			onResetCurrent();
		}

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06000DDC RID: 3548 RVA: 0x00037B6E File Offset: 0x00035D6E
		// (set) Token: 0x06000DDD RID: 3549 RVA: 0x00037B76 File Offset: 0x00035D76
		[DataSourceProperty]
		public bool IsDefault
		{
			get
			{
				return this._isDefault;
			}
			set
			{
				if (value != this._isDefault)
				{
					this._isDefault = value;
					base.OnPropertyChangedWithValue(value, "IsDefault");
				}
			}
		}

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06000DDE RID: 3550 RVA: 0x00037B94 File Offset: 0x00035D94
		// (set) Token: 0x06000DDF RID: 3551 RVA: 0x00037B9C File Offset: 0x00035D9C
		[DataSourceProperty]
		public string DefaultText
		{
			get
			{
				return this._defaultText;
			}
			set
			{
				if (value != this._defaultText)
				{
					this._defaultText = value;
					base.OnPropertyChangedWithValue<string>(value, "DefaultText");
				}
			}
		}

		// Token: 0x0400066D RID: 1645
		private bool _isDefault;

		// Token: 0x0400066E RID: 1646
		private string _defaultText;
	}
}

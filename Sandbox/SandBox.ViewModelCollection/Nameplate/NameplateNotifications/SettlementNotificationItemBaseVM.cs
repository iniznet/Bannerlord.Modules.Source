using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications
{
	// Token: 0x0200001C RID: 28
	public class SettlementNotificationItemBaseVM : ViewModel
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000D278 File Offset: 0x0000B478
		// (set) Token: 0x060002A6 RID: 678 RVA: 0x0000D280 File Offset: 0x0000B480
		public int CreatedTick { get; set; }

		// Token: 0x060002A7 RID: 679 RVA: 0x0000D289 File Offset: 0x0000B489
		public SettlementNotificationItemBaseVM(Action<SettlementNotificationItemBaseVM> onRemove, int createdTick)
		{
			this._onRemove = onRemove;
			this.RelationType = 0;
			this.CreatedTick = createdTick;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000D2A6 File Offset: 0x0000B4A6
		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000D2B4 File Offset: 0x0000B4B4
		// (set) Token: 0x060002AA RID: 682 RVA: 0x0000D2BC File Offset: 0x0000B4BC
		public string CharacterName
		{
			get
			{
				return this._characterName;
			}
			set
			{
				if (value != this._characterName)
				{
					this._characterName = value;
					base.OnPropertyChangedWithValue<string>(value, "CharacterName");
				}
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060002AB RID: 683 RVA: 0x0000D2DF File Offset: 0x0000B4DF
		// (set) Token: 0x060002AC RID: 684 RVA: 0x0000D2E7 File Offset: 0x0000B4E7
		public int RelationType
		{
			get
			{
				return this._relationType;
			}
			set
			{
				if (value != this._relationType)
				{
					this._relationType = value;
					base.OnPropertyChangedWithValue(value, "RelationType");
				}
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060002AD RID: 685 RVA: 0x0000D305 File Offset: 0x0000B505
		// (set) Token: 0x060002AE RID: 686 RVA: 0x0000D30D File Offset: 0x0000B50D
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060002AF RID: 687 RVA: 0x0000D330 File Offset: 0x0000B530
		// (set) Token: 0x060002B0 RID: 688 RVA: 0x0000D338 File Offset: 0x0000B538
		public ImageIdentifierVM CharacterVisual
		{
			get
			{
				return this._characterVisual;
			}
			set
			{
				if (value != this._characterVisual)
				{
					this._characterVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "CharacterVisual");
				}
			}
		}

		// Token: 0x04000152 RID: 338
		private readonly Action<SettlementNotificationItemBaseVM> _onRemove;

		// Token: 0x04000154 RID: 340
		private ImageIdentifierVM _characterVisual;

		// Token: 0x04000155 RID: 341
		private string _text;

		// Token: 0x04000156 RID: 342
		private string _characterName;

		// Token: 0x04000157 RID: 343
		private int _relationType;
	}
}

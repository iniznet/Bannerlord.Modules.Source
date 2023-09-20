using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x02000092 RID: 146
	public class TownManagementDescriptionItemVM : ViewModel
	{
		// Token: 0x06000E2F RID: 3631 RVA: 0x00038B28 File Offset: 0x00036D28
		public TownManagementDescriptionItemVM(TextObject title, int value, int valueChange, TownManagementDescriptionItemVM.DescriptionType type, BasicTooltipViewModel hint = null)
		{
			this._titleObj = title;
			this.Value = value;
			this.ValueChange = valueChange;
			this.Type = (int)type;
			this.Hint = hint ?? new BasicTooltipViewModel();
			this.RefreshValues();
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x00038B76 File Offset: 0x00036D76
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Title = this._titleObj.ToString();
		}

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06000E31 RID: 3633 RVA: 0x00038B8F File Offset: 0x00036D8F
		// (set) Token: 0x06000E32 RID: 3634 RVA: 0x00038B97 File Offset: 0x00036D97
		[DataSourceProperty]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06000E33 RID: 3635 RVA: 0x00038BB5 File Offset: 0x00036DB5
		// (set) Token: 0x06000E34 RID: 3636 RVA: 0x00038BBD File Offset: 0x00036DBD
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

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06000E35 RID: 3637 RVA: 0x00038BE0 File Offset: 0x00036DE0
		// (set) Token: 0x06000E36 RID: 3638 RVA: 0x00038BE8 File Offset: 0x00036DE8
		[DataSourceProperty]
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue(value, "Value");
				}
			}
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06000E37 RID: 3639 RVA: 0x00038C06 File Offset: 0x00036E06
		// (set) Token: 0x06000E38 RID: 3640 RVA: 0x00038C0E File Offset: 0x00036E0E
		[DataSourceProperty]
		public int ValueChange
		{
			get
			{
				return this._valueChange;
			}
			set
			{
				if (value != this._valueChange)
				{
					this._valueChange = value;
					base.OnPropertyChangedWithValue(value, "ValueChange");
				}
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06000E39 RID: 3641 RVA: 0x00038C2C File Offset: 0x00036E2C
		// (set) Token: 0x06000E3A RID: 3642 RVA: 0x00038C34 File Offset: 0x00036E34
		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint && value != null)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x04000696 RID: 1686
		private readonly TextObject _titleObj;

		// Token: 0x04000697 RID: 1687
		private int _type = -1;

		// Token: 0x04000698 RID: 1688
		private string _title;

		// Token: 0x04000699 RID: 1689
		private int _value;

		// Token: 0x0400069A RID: 1690
		private int _valueChange;

		// Token: 0x0400069B RID: 1691
		private BasicTooltipViewModel _hint;

		// Token: 0x020001CE RID: 462
		public enum DescriptionType
		{
			// Token: 0x04000FB7 RID: 4023
			Gold,
			// Token: 0x04000FB8 RID: 4024
			Production,
			// Token: 0x04000FB9 RID: 4025
			Militia,
			// Token: 0x04000FBA RID: 4026
			Prosperity,
			// Token: 0x04000FBB RID: 4027
			Food,
			// Token: 0x04000FBC RID: 4028
			Loyalty,
			// Token: 0x04000FBD RID: 4029
			Security,
			// Token: 0x04000FBE RID: 4030
			Garrison
		}
	}
}

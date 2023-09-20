using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x02000122 RID: 290
	public class CharacterCreationCultureFeatVM : ViewModel
	{
		// Token: 0x06001C14 RID: 7188 RVA: 0x00064DB9 File Offset: 0x00062FB9
		public CharacterCreationCultureFeatVM(bool isPositive, string description)
		{
			this.IsPositive = isPositive;
			this.Description = description;
		}

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x06001C15 RID: 7189 RVA: 0x00064DCF File Offset: 0x00062FCF
		// (set) Token: 0x06001C16 RID: 7190 RVA: 0x00064DD7 File Offset: 0x00062FD7
		[DataSourceProperty]
		public bool IsPositive
		{
			get
			{
				return this._isPositive;
			}
			set
			{
				if (value != this._isPositive)
				{
					this._isPositive = value;
					base.OnPropertyChangedWithValue(value, "IsPositive");
				}
			}
		}

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x06001C17 RID: 7191 RVA: 0x00064DF5 File Offset: 0x00062FF5
		// (set) Token: 0x06001C18 RID: 7192 RVA: 0x00064DFD File Offset: 0x00062FFD
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

		// Token: 0x04000D40 RID: 3392
		private bool _isPositive;

		// Token: 0x04000D41 RID: 3393
		private string _description;
	}
}

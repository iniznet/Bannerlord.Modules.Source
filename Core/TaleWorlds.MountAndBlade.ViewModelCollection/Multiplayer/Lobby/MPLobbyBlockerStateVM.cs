using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x02000059 RID: 89
	public class MPLobbyBlockerStateVM : ViewModel
	{
		// Token: 0x06000799 RID: 1945 RVA: 0x0001DF40 File Offset: 0x0001C140
		public MPLobbyBlockerStateVM(Action<bool> setNavigationRestriction)
		{
			this._setNavigationRestriction = setNavigationRestriction;
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x0001DF4F File Offset: 0x0001C14F
		public void OnLobbyStateIsBlocker(TextObject description)
		{
			this._descriptionObj = description;
			this.IsEnabled = true;
			this.Description = this._descriptionObj.ToString();
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x0001DF70 File Offset: 0x0001C170
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject descriptionObj = this._descriptionObj;
			this.Description = ((descriptionObj != null) ? descriptionObj.ToString() : null);
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x0001DF90 File Offset: 0x0001C190
		public void OnLobbyStateNotBlocker()
		{
			this.IsEnabled = false;
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x0001DF99 File Offset: 0x0001C199
		// (set) Token: 0x0600079E RID: 1950 RVA: 0x0001DFA1 File Offset: 0x0001C1A1
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
					this._setNavigationRestriction(value);
				}
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x0600079F RID: 1951 RVA: 0x0001DFCB File Offset: 0x0001C1CB
		// (set) Token: 0x060007A0 RID: 1952 RVA: 0x0001DFD3 File Offset: 0x0001C1D3
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

		// Token: 0x040003DD RID: 989
		private Action<bool> _setNavigationRestriction;

		// Token: 0x040003DE RID: 990
		private TextObject _descriptionObj;

		// Token: 0x040003DF RID: 991
		private bool _isEnabled;

		// Token: 0x040003E0 RID: 992
		private string _description;
	}
}

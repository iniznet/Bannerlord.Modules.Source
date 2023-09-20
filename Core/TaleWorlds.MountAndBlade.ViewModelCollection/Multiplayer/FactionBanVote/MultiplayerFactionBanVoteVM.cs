using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FactionBanVote
{
	// Token: 0x020000C4 RID: 196
	public class MultiplayerFactionBanVoteVM : ViewModel
	{
		// Token: 0x06001278 RID: 4728 RVA: 0x0003CC8B File Offset: 0x0003AE8B
		public MultiplayerFactionBanVoteVM(BasicCultureObject culture, Action<MultiplayerFactionBanVoteVM> onSelect)
		{
			this.Culture = culture;
			this._onSelect = onSelect;
			this._isEnabled = true;
			this._name = culture.Name.ToString();
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06001279 RID: 4729 RVA: 0x0003CCB9 File Offset: 0x0003AEB9
		// (set) Token: 0x0600127A RID: 4730 RVA: 0x0003CCC1 File Offset: 0x0003AEC1
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					if (value)
					{
						this._onSelect(this);
					}
				}
			}
		}

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x0600127B RID: 4731 RVA: 0x0003CCEE File Offset: 0x0003AEEE
		// (set) Token: 0x0600127C RID: 4732 RVA: 0x0003CCF6 File Offset: 0x0003AEF6
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
				}
			}
		}

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x0600127D RID: 4733 RVA: 0x0003CD14 File Offset: 0x0003AF14
		// (set) Token: 0x0600127E RID: 4734 RVA: 0x0003CD1C File Offset: 0x0003AF1C
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x040008D0 RID: 2256
		private readonly Action<MultiplayerFactionBanVoteVM> _onSelect;

		// Token: 0x040008D1 RID: 2257
		public readonly BasicCultureObject Culture;

		// Token: 0x040008D2 RID: 2258
		private string _name;

		// Token: 0x040008D3 RID: 2259
		private bool _isEnabled;

		// Token: 0x040008D4 RID: 2260
		private bool _isSelected;
	}
}

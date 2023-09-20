using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000CB RID: 203
	public class HeroPerkVM : ViewModel
	{
		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06001309 RID: 4873 RVA: 0x0003E726 File Offset: 0x0003C926
		// (set) Token: 0x0600130A RID: 4874 RVA: 0x0003E72E File Offset: 0x0003C92E
		public IReadOnlyPerkObject SelectedPerk { get; private set; }

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x0600130B RID: 4875 RVA: 0x0003E737 File Offset: 0x0003C937
		// (set) Token: 0x0600130C RID: 4876 RVA: 0x0003E73F File Offset: 0x0003C93F
		public MPPerkVM SelectedPerkItem { get; private set; }

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x0600130D RID: 4877 RVA: 0x0003E748 File Offset: 0x0003C948
		public int PerkIndex { get; }

		// Token: 0x0600130E RID: 4878 RVA: 0x0003E750 File Offset: 0x0003C950
		public HeroPerkVM(Action<HeroPerkVM, MPPerkVM> onSelectPerk, IReadOnlyPerkObject perk, List<IReadOnlyPerkObject> candidatePerks, int perkIndex)
		{
			HeroPerkVM <>4__this = this;
			this.Hint = new BasicTooltipViewModel(() => <>4__this.SelectedPerkItem.Description);
			this.CandidatePerks = new MBBindingList<MPPerkVM>();
			this.PerkIndex = perkIndex;
			this._onSelectPerk = onSelectPerk;
			for (int i = 0; i < candidatePerks.Count; i++)
			{
				IReadOnlyPerkObject readOnlyPerkObject = candidatePerks[i];
				bool flag = readOnlyPerkObject != perk;
				this.CandidatePerks.Add(new MPPerkVM(new Action<MPPerkVM>(this.OnSelectPerk), readOnlyPerkObject, flag, i));
			}
			this.OnSelectPerk(this.CandidatePerks.SingleOrDefault((MPPerkVM x) => x.Perk == perk));
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0003E818 File Offset: 0x0003CA18
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.SelectedPerkItem.Name;
			this.CandidatePerks.ApplyActionOnAllItems(delegate(MPPerkVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x0003E868 File Offset: 0x0003CA68
		[UsedImplicitly]
		private void OnSelectPerk(MPPerkVM perkVm)
		{
			this.OnRefreshWithPerk(perkVm);
			foreach (MPPerkVM mpperkVM in this.CandidatePerks)
			{
				mpperkVM.IsSelectable = true;
			}
			perkVm.IsSelectable = false;
			this._onSelectPerk(this, perkVm);
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0003E8D0 File Offset: 0x0003CAD0
		private void OnRefreshWithPerk(MPPerkVM perk)
		{
			this.SelectedPerkItem = perk;
			MPPerkVM selectedPerkItem = this.SelectedPerkItem;
			this.SelectedPerk = ((selectedPerkItem != null) ? selectedPerkItem.Perk : null);
			if (perk == null)
			{
				this.Name = "";
				this.IconType = "";
				return;
			}
			this.IconType = perk.IconType;
			this.RefreshValues();
		}

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06001312 RID: 4882 RVA: 0x0003E928 File Offset: 0x0003CB28
		// (set) Token: 0x06001313 RID: 4883 RVA: 0x0003E930 File Offset: 0x0003CB30
		[DataSourceProperty]
		public MBBindingList<MPPerkVM> CandidatePerks
		{
			get
			{
				return this._candidatePerks;
			}
			set
			{
				if (value != this._candidatePerks)
				{
					this._candidatePerks = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPerkVM>>(value, "CandidatePerks");
				}
			}
		}

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x06001314 RID: 4884 RVA: 0x0003E94E File Offset: 0x0003CB4E
		// (set) Token: 0x06001315 RID: 4885 RVA: 0x0003E956 File Offset: 0x0003CB56
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

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x06001316 RID: 4886 RVA: 0x0003E979 File Offset: 0x0003CB79
		// (set) Token: 0x06001317 RID: 4887 RVA: 0x0003E981 File Offset: 0x0003CB81
		[DataSourceProperty]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconType");
				}
			}
		}

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x06001318 RID: 4888 RVA: 0x0003E9A4 File Offset: 0x0003CBA4
		// (set) Token: 0x06001319 RID: 4889 RVA: 0x0003E9AC File Offset: 0x0003CBAC
		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x04000926 RID: 2342
		private readonly Action<HeroPerkVM, MPPerkVM> _onSelectPerk;

		// Token: 0x04000928 RID: 2344
		private string _name = "";

		// Token: 0x04000929 RID: 2345
		private string _iconType;

		// Token: 0x0400092A RID: 2346
		private BasicTooltipViewModel _hint;

		// Token: 0x0400092B RID: 2347
		private MBBindingList<MPPerkVM> _candidatePerks;
	}
}

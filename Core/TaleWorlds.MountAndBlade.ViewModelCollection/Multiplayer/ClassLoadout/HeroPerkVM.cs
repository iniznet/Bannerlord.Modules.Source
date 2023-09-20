using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	public class HeroPerkVM : ViewModel
	{
		public IReadOnlyPerkObject SelectedPerk { get; private set; }

		public MPPerkVM SelectedPerkItem { get; private set; }

		public int PerkIndex { get; }

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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.SelectedPerkItem.Name;
			this.CandidatePerks.ApplyActionOnAllItems(delegate(MPPerkVM x)
			{
				x.RefreshValues();
			});
		}

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

		private readonly Action<HeroPerkVM, MPPerkVM> _onSelectPerk;

		private string _name = "";

		private string _iconType;

		private BasicTooltipViewModel _hint;

		private MBBindingList<MPPerkVM> _candidatePerks;
	}
}

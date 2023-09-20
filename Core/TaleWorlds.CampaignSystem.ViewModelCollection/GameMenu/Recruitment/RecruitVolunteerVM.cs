using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment
{
	// Token: 0x0200009D RID: 157
	public class RecruitVolunteerVM : ViewModel
	{
		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06000F82 RID: 3970 RVA: 0x0003C737 File Offset: 0x0003A937
		// (set) Token: 0x06000F83 RID: 3971 RVA: 0x0003C73F File Offset: 0x0003A93F
		public Hero OwnerHero { get; private set; }

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x0003C748 File Offset: 0x0003A948
		// (set) Token: 0x06000F85 RID: 3973 RVA: 0x0003C750 File Offset: 0x0003A950
		public List<CharacterObject> VolunteerTroops { get; private set; }

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0003C759 File Offset: 0x0003A959
		public int GoldCost { get; }

		// Token: 0x06000F87 RID: 3975 RVA: 0x0003C764 File Offset: 0x0003A964
		public RecruitVolunteerVM(Hero owner, List<CharacterObject> troops, Action<RecruitVolunteerVM, RecruitVolunteerTroopVM> onRecruit, Action<RecruitVolunteerVM, RecruitVolunteerTroopVM> onRemoveFromCart)
		{
			this.OwnerHero = owner;
			this.VolunteerTroops = troops;
			this._onRecruit = onRecruit;
			this._onRemoveFromCart = onRemoveFromCart;
			this.Owner = new RecruitVolunteerOwnerVM(owner, (int)owner.GetRelationWithPlayer());
			this.Troops = new MBBindingList<RecruitVolunteerTroopVM>();
			int num = 0;
			foreach (CharacterObject characterObject in troops)
			{
				RecruitVolunteerTroopVM recruitVolunteerTroopVM = new RecruitVolunteerTroopVM(this, characterObject, num, new Action<RecruitVolunteerTroopVM>(this.ExecuteRecruit), new Action<RecruitVolunteerTroopVM>(this.ExecuteRemoveFromCart));
				recruitVolunteerTroopVM.CanBeRecruited = false;
				recruitVolunteerTroopVM.PlayerHasEnoughRelation = false;
				if (HeroHelper.HeroCanRecruitFromHero(Hero.MainHero, this.OwnerHero, num))
				{
					recruitVolunteerTroopVM.PlayerHasEnoughRelation = true;
					if (characterObject != null)
					{
						recruitVolunteerTroopVM.CanBeRecruited = true;
					}
				}
				num++;
				this.Troops.Add(recruitVolunteerTroopVM);
			}
			this.RecruitHint = new HintViewModel();
			this.RefreshProperties();
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x0003C864 File Offset: 0x0003AA64
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RefreshProperties();
			RecruitVolunteerOwnerVM owner = this.Owner;
			if (owner != null)
			{
				owner.RefreshValues();
			}
			this.Troops.ApplyActionOnAllItems(delegate(RecruitVolunteerTroopVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0003C8B8 File Offset: 0x0003AAB8
		public void ExecuteRecruit(RecruitVolunteerTroopVM troop)
		{
			this._onRecruit(this, troop);
			this.RefreshProperties();
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x0003C8CD File Offset: 0x0003AACD
		public void ExecuteRemoveFromCart(RecruitVolunteerTroopVM troop)
		{
			this._onRemoveFromCart(this, troop);
			this.RefreshProperties();
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x0003C8E4 File Offset: 0x0003AAE4
		private void RefreshProperties()
		{
			this.RecruitText = this.GoldCost.ToString();
			if (this.RecruitableNumber == 0)
			{
				this.QuantityText = GameTexts.FindText("str_none", null).ToString();
				return;
			}
			GameTexts.SetVariable("QUANTITY", this.RecruitableNumber.ToString());
			this.QuantityText = GameTexts.FindText("str_x_quantity", null).ToString();
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x0003C950 File Offset: 0x0003AB50
		public void OnRecruitMoveToCart(RecruitVolunteerTroopVM troop)
		{
			MBInformationManager.HideInformations();
			this.Troops.RemoveAt(troop.Index);
			RecruitVolunteerTroopVM recruitVolunteerTroopVM = new RecruitVolunteerTroopVM(this, null, troop.Index, new Action<RecruitVolunteerTroopVM>(this.ExecuteRecruit), new Action<RecruitVolunteerTroopVM>(this.ExecuteRemoveFromCart));
			recruitVolunteerTroopVM.IsTroopEmpty = true;
			recruitVolunteerTroopVM.PlayerHasEnoughRelation = true;
			this.Troops.Insert(troop.Index, recruitVolunteerTroopVM);
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0003C9B9 File Offset: 0x0003ABB9
		public void OnRecruitRemovedFromCart(RecruitVolunteerTroopVM troop)
		{
			this.Troops.RemoveAt(troop.Index);
			this.Troops.Insert(troop.Index, troop);
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06000F8E RID: 3982 RVA: 0x0003C9DE File Offset: 0x0003ABDE
		// (set) Token: 0x06000F8F RID: 3983 RVA: 0x0003C9E6 File Offset: 0x0003ABE6
		[DataSourceProperty]
		public MBBindingList<RecruitVolunteerTroopVM> Troops
		{
			get
			{
				return this._troops;
			}
			set
			{
				if (value != this._troops)
				{
					this._troops = value;
					base.OnPropertyChangedWithValue<MBBindingList<RecruitVolunteerTroopVM>>(value, "Troops");
				}
			}
		}

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06000F90 RID: 3984 RVA: 0x0003CA04 File Offset: 0x0003AC04
		// (set) Token: 0x06000F91 RID: 3985 RVA: 0x0003CA0C File Offset: 0x0003AC0C
		[DataSourceProperty]
		public RecruitVolunteerOwnerVM Owner
		{
			get
			{
				return this._owner;
			}
			set
			{
				if (value != this._owner)
				{
					this._owner = value;
					base.OnPropertyChangedWithValue<RecruitVolunteerOwnerVM>(value, "Owner");
				}
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x0003CA2A File Offset: 0x0003AC2A
		// (set) Token: 0x06000F93 RID: 3987 RVA: 0x0003CA32 File Offset: 0x0003AC32
		[DataSourceProperty]
		public bool CanRecruit
		{
			get
			{
				return this._canRecruit;
			}
			set
			{
				if (value != this._canRecruit)
				{
					this._canRecruit = value;
					base.OnPropertyChangedWithValue(value, "CanRecruit");
				}
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06000F94 RID: 3988 RVA: 0x0003CA50 File Offset: 0x0003AC50
		// (set) Token: 0x06000F95 RID: 3989 RVA: 0x0003CA58 File Offset: 0x0003AC58
		[DataSourceProperty]
		public bool ButtonIsVisible
		{
			get
			{
				return this._buttonIsVisible;
			}
			set
			{
				if (value != this._buttonIsVisible)
				{
					this._buttonIsVisible = value;
					base.OnPropertyChangedWithValue(value, "ButtonIsVisible");
				}
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06000F96 RID: 3990 RVA: 0x0003CA76 File Offset: 0x0003AC76
		// (set) Token: 0x06000F97 RID: 3991 RVA: 0x0003CA7E File Offset: 0x0003AC7E
		[DataSourceProperty]
		public string QuantityText
		{
			get
			{
				return this._quantityText;
			}
			set
			{
				if (value != this._quantityText)
				{
					this._quantityText = value;
					base.OnPropertyChangedWithValue<string>(value, "QuantityText");
				}
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06000F98 RID: 3992 RVA: 0x0003CAA1 File Offset: 0x0003ACA1
		// (set) Token: 0x06000F99 RID: 3993 RVA: 0x0003CAA9 File Offset: 0x0003ACA9
		[DataSourceProperty]
		public string RecruitText
		{
			get
			{
				return this._recruitText;
			}
			set
			{
				if (value != this._recruitText)
				{
					this._recruitText = value;
					base.OnPropertyChangedWithValue<string>(value, "RecruitText");
				}
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06000F9A RID: 3994 RVA: 0x0003CACC File Offset: 0x0003ACCC
		// (set) Token: 0x06000F9B RID: 3995 RVA: 0x0003CAD4 File Offset: 0x0003ACD4
		[DataSourceProperty]
		public HintViewModel RecruitHint
		{
			get
			{
				return this._recruitHint;
			}
			set
			{
				if (value != this._recruitHint)
				{
					this._recruitHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RecruitHint");
				}
			}
		}

		// Token: 0x04000736 RID: 1846
		public int RecruitableNumber;

		// Token: 0x04000737 RID: 1847
		private readonly Action<RecruitVolunteerVM, RecruitVolunteerTroopVM> _onRecruit;

		// Token: 0x04000738 RID: 1848
		private readonly Action<RecruitVolunteerVM, RecruitVolunteerTroopVM> _onRemoveFromCart;

		// Token: 0x04000739 RID: 1849
		private string _quantityText;

		// Token: 0x0400073A RID: 1850
		private string _recruitText;

		// Token: 0x0400073B RID: 1851
		private bool _canRecruit;

		// Token: 0x0400073C RID: 1852
		private bool _buttonIsVisible;

		// Token: 0x0400073D RID: 1853
		private HintViewModel _recruitHint;

		// Token: 0x0400073E RID: 1854
		private RecruitVolunteerOwnerVM _owner;

		// Token: 0x0400073F RID: 1855
		private MBBindingList<RecruitVolunteerTroopVM> _troops;
	}
}

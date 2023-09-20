using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies
{
	// Token: 0x02000073 RID: 115
	public class KingdomArmyItemVM : KingdomItemVM
	{
		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06000A34 RID: 2612 RVA: 0x00028F09 File Offset: 0x00027109
		// (set) Token: 0x06000A35 RID: 2613 RVA: 0x00028F11 File Offset: 0x00027111
		public float DistanceToMainParty { get; set; }

		// Token: 0x06000A36 RID: 2614 RVA: 0x00028F1C File Offset: 0x0002711C
		public KingdomArmyItemVM(Army army, Action<KingdomArmyItemVM> onSelect)
		{
			this.Army = army;
			this._onSelect = onSelect;
			CampaignUIHelper.GetCharacterCode(army.ArmyOwner.CharacterObject, false);
			this.Leader = new HeroVM(this.Army.LeaderParty.LeaderHero, false);
			this.LordCount = army.Parties.Count;
			this.Strength = army.Parties.Sum((MobileParty p) => p.Party.NumberOfAllMembers);
			this.Location = army.GetBehaviorText(true).ToString();
			this.UpdateIsNew();
			this.Cohesion = (int)this.Army.Cohesion;
			this.Parties = new MBBindingList<KingdomArmyPartyItemVM>();
			foreach (MobileParty mobileParty in this.Army.Parties)
			{
				this.Parties.Add(new KingdomArmyPartyItemVM(mobileParty));
			}
			this.DistanceToMainParty = Campaign.Current.Models.MapDistanceModel.GetDistance(army.LeaderParty, MobileParty.MainParty);
			this.IsMainArmy = army.LeaderParty == MobileParty.MainParty;
			this.RefreshValues();
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x00029074 File Offset: 0x00027274
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ArmyName = this.Army.Name.ToString();
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_cohesion", null));
			GameTexts.SetVariable("STR2", this.Cohesion.ToString());
			this.CohesionLabel = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_men_count", null));
			GameTexts.SetVariable("RIGHT", this.Strength.ToString());
			this.StrengthLabel = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
			this.Parties.ApplyActionOnAllItems(delegate(KingdomArmyPartyItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x0002914D File Offset: 0x0002734D
		protected override void OnSelect()
		{
			base.OnSelect();
			this._onSelect(this);
			this.ExecuteResetNew();
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x00029167 File Offset: 0x00027367
		private void ExecuteResetNew()
		{
			if (base.IsNew)
			{
				PlayerUpdateTracker.Current.OnArmyExamined(this.Army);
				this.UpdateIsNew();
			}
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x00029187 File Offset: 0x00027387
		private void UpdateIsNew()
		{
			base.IsNew = PlayerUpdateTracker.Current.UnExaminedArmies.Any((Army a) => a == this.Army);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x000291AA File Offset: 0x000273AA
		protected void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06000A3C RID: 2620 RVA: 0x000291BC File Offset: 0x000273BC
		// (set) Token: 0x06000A3D RID: 2621 RVA: 0x000291C4 File Offset: 0x000273C4
		[DataSourceProperty]
		public MBBindingList<KingdomArmyPartyItemVM> Parties
		{
			get
			{
				return this._parties;
			}
			set
			{
				if (value != this._parties)
				{
					this._parties = value;
					base.OnPropertyChangedWithValue<MBBindingList<KingdomArmyPartyItemVM>>(value, "Parties");
				}
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06000A3E RID: 2622 RVA: 0x000291E2 File Offset: 0x000273E2
		// (set) Token: 0x06000A3F RID: 2623 RVA: 0x000291EA File Offset: 0x000273EA
		[DataSourceProperty]
		public HeroVM Leader
		{
			get
			{
				return this._leader;
			}
			set
			{
				if (value != this._leader)
				{
					this._leader = value;
					base.OnPropertyChanged("Visual");
				}
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06000A40 RID: 2624 RVA: 0x00029207 File Offset: 0x00027407
		// (set) Token: 0x06000A41 RID: 2625 RVA: 0x0002920F File Offset: 0x0002740F
		[DataSourceProperty]
		public string ArmyName
		{
			get
			{
				return this._armyName;
			}
			set
			{
				if (value != this._armyName)
				{
					this._armyName = value;
					base.OnPropertyChangedWithValue<string>(value, "ArmyName");
				}
			}
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06000A42 RID: 2626 RVA: 0x00029232 File Offset: 0x00027432
		// (set) Token: 0x06000A43 RID: 2627 RVA: 0x0002923A File Offset: 0x0002743A
		[DataSourceProperty]
		public int Cohesion
		{
			get
			{
				return this._cohesion;
			}
			set
			{
				if (value != this._cohesion)
				{
					this._cohesion = value;
					base.OnPropertyChangedWithValue(value, "Cohesion");
				}
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06000A44 RID: 2628 RVA: 0x00029258 File Offset: 0x00027458
		// (set) Token: 0x06000A45 RID: 2629 RVA: 0x00029260 File Offset: 0x00027460
		[DataSourceProperty]
		public string CohesionLabel
		{
			get
			{
				return this._cohesionLabel;
			}
			set
			{
				if (value != this._cohesionLabel)
				{
					this._cohesionLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "CohesionLabel");
				}
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06000A46 RID: 2630 RVA: 0x00029283 File Offset: 0x00027483
		// (set) Token: 0x06000A47 RID: 2631 RVA: 0x0002928B File Offset: 0x0002748B
		[DataSourceProperty]
		public int LordCount
		{
			get
			{
				return this._lordCount;
			}
			set
			{
				if (value != this._lordCount)
				{
					this._lordCount = value;
					base.OnPropertyChangedWithValue(value, "LordCount");
				}
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06000A48 RID: 2632 RVA: 0x000292A9 File Offset: 0x000274A9
		// (set) Token: 0x06000A49 RID: 2633 RVA: 0x000292B1 File Offset: 0x000274B1
		[DataSourceProperty]
		public int Strength
		{
			get
			{
				return this._strength;
			}
			set
			{
				if (value != this._strength)
				{
					this._strength = value;
					base.OnPropertyChangedWithValue(value, "Strength");
				}
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000A4A RID: 2634 RVA: 0x000292CF File Offset: 0x000274CF
		// (set) Token: 0x06000A4B RID: 2635 RVA: 0x000292D7 File Offset: 0x000274D7
		[DataSourceProperty]
		public string StrengthLabel
		{
			get
			{
				return this._strengthLabel;
			}
			set
			{
				if (value != this._strengthLabel)
				{
					this._strengthLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "StrengthLabel");
				}
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06000A4C RID: 2636 RVA: 0x000292FA File Offset: 0x000274FA
		// (set) Token: 0x06000A4D RID: 2637 RVA: 0x00029302 File Offset: 0x00027502
		[DataSourceProperty]
		public string Location
		{
			get
			{
				return this._location;
			}
			set
			{
				if (value != this._location)
				{
					this._location = value;
					base.OnPropertyChanged("Objective");
				}
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06000A4E RID: 2638 RVA: 0x00029324 File Offset: 0x00027524
		// (set) Token: 0x06000A4F RID: 2639 RVA: 0x0002932C File Offset: 0x0002752C
		[DataSourceProperty]
		public bool IsMainArmy
		{
			get
			{
				return this._isMainArmy;
			}
			set
			{
				if (value != this._isMainArmy)
				{
					this._isMainArmy = value;
					base.OnPropertyChangedWithValue(value, "IsMainArmy");
				}
			}
		}

		// Token: 0x04000499 RID: 1177
		private readonly Action<KingdomArmyItemVM> _onSelect;

		// Token: 0x0400049A RID: 1178
		public readonly Army Army;

		// Token: 0x0400049C RID: 1180
		private HeroVM _leader;

		// Token: 0x0400049D RID: 1181
		private MBBindingList<KingdomArmyPartyItemVM> _parties;

		// Token: 0x0400049E RID: 1182
		private string _armyName;

		// Token: 0x0400049F RID: 1183
		private int _strength;

		// Token: 0x040004A0 RID: 1184
		private int _cohesion;

		// Token: 0x040004A1 RID: 1185
		private string _strengthLabel;

		// Token: 0x040004A2 RID: 1186
		private int _lordCount;

		// Token: 0x040004A3 RID: 1187
		private string _location;

		// Token: 0x040004A4 RID: 1188
		private string _cohesionLabel;

		// Token: 0x040004A5 RID: 1189
		private bool _isMainArmy;
	}
}

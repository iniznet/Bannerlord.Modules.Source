using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar
{
	// Token: 0x0200004F RID: 79
	public class MapInfoVM : ViewModel
	{
		// Token: 0x0600059C RID: 1436 RVA: 0x0001B96C File Offset: 0x00019B6C
		public MapInfoVM()
		{
			this.DenarHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetGoldTooltip(Clan.PlayerClan));
			this.HealthHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPlayerHitpointsTooltip());
			this.InfluenceHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetInfluenceTooltip(Clan.PlayerClan));
			this.AvailableTroopsHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetMainPartyHealthTooltip());
			this.ExtendHint = new HintViewModel(GameTexts.FindText("str_map_extend_bar_hint", null), null);
			this.SpeedHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartySpeedTooltip());
			this.ViewDistanceHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetViewDistanceTooltip());
			this.TroopWageHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyWageTooltip());
			this.MoraleHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyMoraleTooltip(MobileParty.MainParty));
			this.DailyConsumptionHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyFoodTooltip(MobileParty.MainParty));
			this._viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			this.IsInfoBarExtended = this._viewDataTracker.GetMapBarExtendedState();
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0001BB84 File Offset: 0x00019D84
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdatePlayerInfo(true);
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x0001BB93 File Offset: 0x00019D93
		public void Tick()
		{
			this.IsMainHeroSick = Hero.MainHero != null && Hero.IsMainHeroIll;
			Hero mainHero = Hero.MainHero;
			this.IsInfoBarEnabled = mainHero != null && mainHero.IsAlive;
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0001BBC1 File Offset: 0x00019DC1
		public void Refresh()
		{
			this.UpdatePlayerInfo(false);
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0001BBCC File Offset: 0x00019DCC
		private void UpdatePlayerInfo(bool updateForced)
		{
			int totalWage = MobileParty.MainParty.TotalWage;
			ExplainedNumber explainedNumber = Campaign.Current.Models.ClanFinanceModel.CalculateClanGoldChange(Clan.PlayerClan, true, false, true);
			this.IsDenarTooltipWarning = (float)Hero.MainHero.Gold + explainedNumber.ResultNumber < 0f;
			this.IsInfluenceTooltipWarning = Hero.MainHero.Clan.Influence < -100f;
			this.IsMoraleTooltipWarning = MobileParty.MainParty.Morale < (float)Campaign.Current.Models.PartyDesertionModel.GetMoraleThresholdForTroopDesertion(MobileParty.MainParty);
			int numDaysForFoodToLast = MobileParty.MainParty.GetNumDaysForFoodToLast();
			this.IsDailyConsumptionTooltipWarning = numDaysForFoodToLast < 1;
			this.IsAvailableTroopsTooltipWarning = PartyBase.MainParty.PartySizeLimit < PartyBase.MainParty.NumberOfAllMembers || PartyBase.MainParty.PrisonerSizeLimit < PartyBase.MainParty.NumberOfPrisoners;
			this.IsHealthTooltipWarning = Hero.MainHero.IsWounded;
			if (this.Denars != Hero.MainHero.Gold || updateForced)
			{
				this.Denars = Hero.MainHero.Gold;
				this.DenarsWithAbbrText = CampaignUIHelper.GetAbbreviatedValueTextFromValue(this.Denars);
			}
			if (this.Influence != (int)Hero.MainHero.Clan.Influence || updateForced)
			{
				this.Influence = (int)Hero.MainHero.Clan.Influence;
				this.InfluenceWithAbbrText = CampaignUIHelper.GetAbbreviatedValueTextFromValue(this.Influence);
			}
			this.Morale = (int)MobileParty.MainParty.Morale;
			this.TotalFood = (int)((MobileParty.MainParty.Food > 0f) ? MobileParty.MainParty.Food : 0f);
			this.TotalTroops = PartyBase.MainParty.MemberRoster.TotalManCount;
			this.AvailableTroopsText = CampaignUIHelper.GetPartyNameplateText(PartyBase.MainParty);
			int num = (int)MathF.Clamp((float)(Hero.MainHero.HitPoints * 100 / CharacterObject.PlayerCharacter.MaxHitPoints()), 1f, 100f);
			if (this.Health != num || updateForced)
			{
				this.Health = num;
				GameTexts.SetVariable("NUMBER", this.Health);
				this.HealthTextWithPercentage = GameTexts.FindText("str_NUMBER_percent", null).ToString();
			}
			float num2 = MathF.Round(MobileParty.MainParty.Morale, 1);
			if (this._latestMorale != num2 || updateForced)
			{
				this._latestMorale = num2;
				MBTextManager.SetTextVariable("BASE_EFFECT", num2.ToString("0.0"), false);
			}
			float num3 = (MobileParty.MainParty.CurrentNavigationFace.IsValid() ? MobileParty.MainParty.Speed : 0f);
			if (this._latestSpeed != num3 || updateForced)
			{
				this._latestSpeed = num3;
				this.Speed = CampaignUIHelper.FloatToString(num3);
			}
			float seeingRange = MobileParty.MainParty.SeeingRange;
			if (this._latestSeeingRange != seeingRange || updateForced)
			{
				this._latestSeeingRange = seeingRange;
				this.ViewDistance = CampaignUIHelper.FloatToString(seeingRange);
			}
			if (this._latestTotalWage != totalWage || updateForced)
			{
				this._latestTotalWage = totalWage;
				this.TroopWage = totalWage.ToString();
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x0001BEEF File Offset: 0x0001A0EF
		// (set) Token: 0x060005A2 RID: 1442 RVA: 0x0001BEF7 File Offset: 0x0001A0F7
		[DataSourceProperty]
		public bool IsHealthTooltipWarning
		{
			get
			{
				return this._isHealthTooltipWarning;
			}
			set
			{
				if (value != this._isHealthTooltipWarning)
				{
					this._isHealthTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsHealthTooltipWarning");
				}
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x0001BF15 File Offset: 0x0001A115
		// (set) Token: 0x060005A4 RID: 1444 RVA: 0x0001BF1D File Offset: 0x0001A11D
		[DataSourceProperty]
		public bool IsMainHeroSick
		{
			get
			{
				return this._isMainHeroSick;
			}
			set
			{
				if (value != this._isMainHeroSick)
				{
					this._isMainHeroSick = value;
					base.OnPropertyChangedWithValue(value, "IsMainHeroSick");
				}
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x0001BF3B File Offset: 0x0001A13B
		// (set) Token: 0x060005A6 RID: 1446 RVA: 0x0001BF43 File Offset: 0x0001A143
		[DataSourceProperty]
		public HintViewModel ExtendHint
		{
			get
			{
				return this._extendHint;
			}
			set
			{
				if (value != this._extendHint)
				{
					this._extendHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ExtendHint");
				}
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x0001BF61 File Offset: 0x0001A161
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x0001BF69 File Offset: 0x0001A169
		[DataSourceProperty]
		public BasicTooltipViewModel DenarHint
		{
			get
			{
				return this._denarHint;
			}
			set
			{
				if (value != this._denarHint)
				{
					this._denarHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "DenarHint");
				}
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x0001BF87 File Offset: 0x0001A187
		// (set) Token: 0x060005AA RID: 1450 RVA: 0x0001BF8F File Offset: 0x0001A18F
		[DataSourceProperty]
		public BasicTooltipViewModel InfluenceHint
		{
			get
			{
				return this._influenceHint;
			}
			set
			{
				if (value != this._influenceHint)
				{
					this._influenceHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "InfluenceHint");
				}
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x0001BFAD File Offset: 0x0001A1AD
		// (set) Token: 0x060005AC RID: 1452 RVA: 0x0001BFB5 File Offset: 0x0001A1B5
		[DataSourceProperty]
		public BasicTooltipViewModel AvailableTroopsHint
		{
			get
			{
				return this._availableTroopsHint;
			}
			set
			{
				if (value != this._availableTroopsHint)
				{
					this._availableTroopsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "AvailableTroopsHint");
				}
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060005AD RID: 1453 RVA: 0x0001BFD3 File Offset: 0x0001A1D3
		// (set) Token: 0x060005AE RID: 1454 RVA: 0x0001BFDB File Offset: 0x0001A1DB
		[DataSourceProperty]
		public BasicTooltipViewModel HealthHint
		{
			get
			{
				return this._healthHint;
			}
			set
			{
				if (value != this._healthHint)
				{
					this._healthHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "HealthHint");
				}
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060005AF RID: 1455 RVA: 0x0001BFF9 File Offset: 0x0001A1F9
		// (set) Token: 0x060005B0 RID: 1456 RVA: 0x0001C001 File Offset: 0x0001A201
		[DataSourceProperty]
		public BasicTooltipViewModel DailyConsumptionHint
		{
			get
			{
				return this._dailyConsumptionHint;
			}
			set
			{
				if (value != this._dailyConsumptionHint)
				{
					this._dailyConsumptionHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "DailyConsumptionHint");
				}
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x060005B1 RID: 1457 RVA: 0x0001C01F File Offset: 0x0001A21F
		// (set) Token: 0x060005B2 RID: 1458 RVA: 0x0001C027 File Offset: 0x0001A227
		[DataSourceProperty]
		public BasicTooltipViewModel MoraleHint
		{
			get
			{
				return this._moraleHint;
			}
			set
			{
				if (value != this._moraleHint)
				{
					this._moraleHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "MoraleHint");
				}
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x060005B3 RID: 1459 RVA: 0x0001C045 File Offset: 0x0001A245
		// (set) Token: 0x060005B4 RID: 1460 RVA: 0x0001C04D File Offset: 0x0001A24D
		[DataSourceProperty]
		public BasicTooltipViewModel SpeedHint
		{
			get
			{
				return this._speedHint;
			}
			set
			{
				if (value != this._speedHint)
				{
					this._speedHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SpeedHint");
				}
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x060005B5 RID: 1461 RVA: 0x0001C06B File Offset: 0x0001A26B
		// (set) Token: 0x060005B6 RID: 1462 RVA: 0x0001C073 File Offset: 0x0001A273
		[DataSourceProperty]
		public BasicTooltipViewModel ViewDistanceHint
		{
			get
			{
				return this._viewDistanceHint;
			}
			set
			{
				if (value != this._viewDistanceHint)
				{
					this._viewDistanceHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ViewDistanceHint");
				}
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x060005B7 RID: 1463 RVA: 0x0001C091 File Offset: 0x0001A291
		// (set) Token: 0x060005B8 RID: 1464 RVA: 0x0001C099 File Offset: 0x0001A299
		[DataSourceProperty]
		public BasicTooltipViewModel TrainingFactorHint
		{
			get
			{
				return this._trainingFactorHint;
			}
			set
			{
				if (value != this._trainingFactorHint)
				{
					this._trainingFactorHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TrainingFactorHint");
				}
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x060005B9 RID: 1465 RVA: 0x0001C0B7 File Offset: 0x0001A2B7
		// (set) Token: 0x060005BA RID: 1466 RVA: 0x0001C0BF File Offset: 0x0001A2BF
		[DataSourceProperty]
		public BasicTooltipViewModel TroopWageHint
		{
			get
			{
				return this._troopWageHint;
			}
			set
			{
				if (value != this._troopWageHint)
				{
					this._troopWageHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TroopWageHint");
				}
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060005BB RID: 1467 RVA: 0x0001C0DD File Offset: 0x0001A2DD
		// (set) Token: 0x060005BC RID: 1468 RVA: 0x0001C0E5 File Offset: 0x0001A2E5
		[DataSourceProperty]
		public bool IsDenarTooltipWarning
		{
			get
			{
				return this._isDenarTooltipWarning;
			}
			set
			{
				if (value != this._isDenarTooltipWarning)
				{
					this._isDenarTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsDenarTooltipWarning");
				}
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x0001C103 File Offset: 0x0001A303
		// (set) Token: 0x060005BE RID: 1470 RVA: 0x0001C10B File Offset: 0x0001A30B
		[DataSourceProperty]
		public bool IsInfluenceTooltipWarning
		{
			get
			{
				return this._isInfluenceTooltipWarning;
			}
			set
			{
				if (value != this._isInfluenceTooltipWarning)
				{
					this._isInfluenceTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsInfluenceTooltipWarning");
				}
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0001C129 File Offset: 0x0001A329
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x0001C131 File Offset: 0x0001A331
		[DataSourceProperty]
		public bool IsMoraleTooltipWarning
		{
			get
			{
				return this._isMoraleTooltipWarning;
			}
			set
			{
				if (value != this._isMoraleTooltipWarning)
				{
					this._isMoraleTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsMoraleTooltipWarning");
				}
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0001C14F File Offset: 0x0001A34F
		// (set) Token: 0x060005C2 RID: 1474 RVA: 0x0001C157 File Offset: 0x0001A357
		[DataSourceProperty]
		public bool IsDailyConsumptionTooltipWarning
		{
			get
			{
				return this._isDailyConsumptionTooltipWarning;
			}
			set
			{
				if (value != this._isDailyConsumptionTooltipWarning)
				{
					this._isDailyConsumptionTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsDailyConsumptionTooltipWarning");
				}
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x0001C175 File Offset: 0x0001A375
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x0001C17D File Offset: 0x0001A37D
		[DataSourceProperty]
		public bool IsAvailableTroopsTooltipWarning
		{
			get
			{
				return this._isAvailableTroopsTooltipWarning;
			}
			set
			{
				if (value != this._isAvailableTroopsTooltipWarning)
				{
					this._isAvailableTroopsTooltipWarning = value;
					base.OnPropertyChangedWithValue(value, "IsAvailableTroopsTooltipWarning");
				}
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x0001C19B File Offset: 0x0001A39B
		// (set) Token: 0x060005C6 RID: 1478 RVA: 0x0001C1A3 File Offset: 0x0001A3A3
		[DataSourceProperty]
		public string DenarsWithAbbrText
		{
			get
			{
				return this._denarsWithAbbrText;
			}
			set
			{
				if (value != this._denarsWithAbbrText)
				{
					this._denarsWithAbbrText = value;
					base.OnPropertyChangedWithValue<string>(value, "DenarsWithAbbrText");
				}
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x0001C1C6 File Offset: 0x0001A3C6
		// (set) Token: 0x060005C8 RID: 1480 RVA: 0x0001C1CE File Offset: 0x0001A3CE
		[DataSourceProperty]
		public int Denars
		{
			get
			{
				return this._denars;
			}
			set
			{
				if (value != this._denars)
				{
					this._denars = value;
					base.OnPropertyChangedWithValue(value, "Denars");
				}
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x0001C1EC File Offset: 0x0001A3EC
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x0001C1F4 File Offset: 0x0001A3F4
		[DataSourceProperty]
		public int Influence
		{
			get
			{
				return this._influence;
			}
			set
			{
				if (value != this._influence)
				{
					this._influence = value;
					base.OnPropertyChangedWithValue(value, "Influence");
				}
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x0001C212 File Offset: 0x0001A412
		// (set) Token: 0x060005CC RID: 1484 RVA: 0x0001C21A File Offset: 0x0001A41A
		[DataSourceProperty]
		public string InfluenceWithAbbrText
		{
			get
			{
				return this._influenceWithAbbrText;
			}
			set
			{
				if (value != this._influenceWithAbbrText)
				{
					this._influenceWithAbbrText = value;
					base.OnPropertyChangedWithValue<string>(value, "InfluenceWithAbbrText");
				}
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x0001C23D File Offset: 0x0001A43D
		// (set) Token: 0x060005CE RID: 1486 RVA: 0x0001C245 File Offset: 0x0001A445
		[DataSourceProperty]
		public int Morale
		{
			get
			{
				return this._morale;
			}
			set
			{
				if (value != this._morale)
				{
					this._morale = value;
					base.OnPropertyChangedWithValue(value, "Morale");
				}
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060005CF RID: 1487 RVA: 0x0001C263 File Offset: 0x0001A463
		// (set) Token: 0x060005D0 RID: 1488 RVA: 0x0001C26B File Offset: 0x0001A46B
		[DataSourceProperty]
		public int TotalFood
		{
			get
			{
				return this._totalFood;
			}
			set
			{
				if (value != this._totalFood)
				{
					this._totalFood = value;
					base.OnPropertyChangedWithValue(value, "TotalFood");
				}
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x0001C289 File Offset: 0x0001A489
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x0001C291 File Offset: 0x0001A491
		[DataSourceProperty]
		public int Health
		{
			get
			{
				return this._health;
			}
			set
			{
				if (value != this._health)
				{
					this._health = value;
					base.OnPropertyChangedWithValue(value, "Health");
				}
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0001C2AF File Offset: 0x0001A4AF
		// (set) Token: 0x060005D4 RID: 1492 RVA: 0x0001C2B7 File Offset: 0x0001A4B7
		[DataSourceProperty]
		public string HealthTextWithPercentage
		{
			get
			{
				return this._healthTextWithPercentage;
			}
			set
			{
				if (value != this._healthTextWithPercentage)
				{
					this._healthTextWithPercentage = value;
					base.OnPropertyChangedWithValue<string>(value, "HealthTextWithPercentage");
				}
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060005D5 RID: 1493 RVA: 0x0001C2DA File Offset: 0x0001A4DA
		// (set) Token: 0x060005D6 RID: 1494 RVA: 0x0001C2E2 File Offset: 0x0001A4E2
		[DataSourceProperty]
		public string AvailableTroopsText
		{
			get
			{
				return this._availableTroopsText;
			}
			set
			{
				if (value != this._availableTroopsText)
				{
					this._availableTroopsText = value;
					base.OnPropertyChangedWithValue<string>(value, "AvailableTroopsText");
				}
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x0001C305 File Offset: 0x0001A505
		// (set) Token: 0x060005D8 RID: 1496 RVA: 0x0001C30D File Offset: 0x0001A50D
		[DataSourceProperty]
		public int TotalTroops
		{
			get
			{
				return this._totalTroops;
			}
			set
			{
				if (value != this._totalTroops)
				{
					this._totalTroops = value;
					base.OnPropertyChangedWithValue(value, "TotalTroops");
				}
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x0001C32B File Offset: 0x0001A52B
		// (set) Token: 0x060005DA RID: 1498 RVA: 0x0001C333 File Offset: 0x0001A533
		[DataSourceProperty]
		public string Speed
		{
			get
			{
				return this._speed;
			}
			set
			{
				if (value != this._speed)
				{
					this._speed = value;
					base.OnPropertyChangedWithValue<string>(value, "Speed");
				}
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x0001C356 File Offset: 0x0001A556
		// (set) Token: 0x060005DC RID: 1500 RVA: 0x0001C35E File Offset: 0x0001A55E
		[DataSourceProperty]
		public string ViewDistance
		{
			get
			{
				return this._viewDistance;
			}
			set
			{
				if (value != this._viewDistance)
				{
					this._viewDistance = value;
					base.OnPropertyChangedWithValue<string>(value, "ViewDistance");
				}
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x0001C381 File Offset: 0x0001A581
		// (set) Token: 0x060005DE RID: 1502 RVA: 0x0001C389 File Offset: 0x0001A589
		[DataSourceProperty]
		public string TrainingFactor
		{
			get
			{
				return this._trainingFactor;
			}
			set
			{
				if (value != this._trainingFactor)
				{
					this._trainingFactor = value;
					base.OnPropertyChangedWithValue<string>(value, "TrainingFactor");
				}
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060005DF RID: 1503 RVA: 0x0001C3AC File Offset: 0x0001A5AC
		// (set) Token: 0x060005E0 RID: 1504 RVA: 0x0001C3B4 File Offset: 0x0001A5B4
		[DataSourceProperty]
		public string TroopWage
		{
			get
			{
				return this._troopWage;
			}
			set
			{
				if (value != this._troopWage)
				{
					this._troopWage = value;
					base.OnPropertyChangedWithValue<string>(value, "TroopWage");
				}
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x0001C3D7 File Offset: 0x0001A5D7
		// (set) Token: 0x060005E2 RID: 1506 RVA: 0x0001C3DF File Offset: 0x0001A5DF
		[DataSourceProperty]
		public bool IsInfoBarExtended
		{
			get
			{
				return this._isInfoBarExtended;
			}
			set
			{
				if (value != this._isInfoBarExtended)
				{
					this._isInfoBarExtended = value;
					this._viewDataTracker.SetMapBarExtendedState(value);
					base.OnPropertyChangedWithValue(value, "IsInfoBarExtended");
				}
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060005E3 RID: 1507 RVA: 0x0001C409 File Offset: 0x0001A609
		// (set) Token: 0x060005E4 RID: 1508 RVA: 0x0001C411 File Offset: 0x0001A611
		[DataSourceProperty]
		public bool IsInfoBarEnabled
		{
			get
			{
				return this._isInfoBarEnabled;
			}
			set
			{
				if (value != this._isInfoBarEnabled)
				{
					this._isInfoBarEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsInfoBarEnabled");
				}
			}
		}

		// Token: 0x04000274 RID: 628
		private int _latestTotalWage = -1;

		// Token: 0x04000275 RID: 629
		private float _latestSeeingRange = -1f;

		// Token: 0x04000276 RID: 630
		private float _latestSpeed = -1f;

		// Token: 0x04000277 RID: 631
		private float _latestMorale = -1f;

		// Token: 0x04000278 RID: 632
		private IViewDataTracker _viewDataTracker;

		// Token: 0x04000279 RID: 633
		private string _speed;

		// Token: 0x0400027A RID: 634
		private string _viewDistance;

		// Token: 0x0400027B RID: 635
		private string _trainingFactor;

		// Token: 0x0400027C RID: 636
		private string _troopWage;

		// Token: 0x0400027D RID: 637
		private string _healthTextWithPercentage;

		// Token: 0x0400027E RID: 638
		private string _denarsWithAbbrText = "";

		// Token: 0x0400027F RID: 639
		private string _influenceWithAbbrText = "";

		// Token: 0x04000280 RID: 640
		private string _availableTroopsText;

		// Token: 0x04000281 RID: 641
		private int _denars = -1;

		// Token: 0x04000282 RID: 642
		private int _influence = -1;

		// Token: 0x04000283 RID: 643
		private int _morale = -1;

		// Token: 0x04000284 RID: 644
		private int _totalFood;

		// Token: 0x04000285 RID: 645
		private int _health;

		// Token: 0x04000286 RID: 646
		private int _totalTroops;

		// Token: 0x04000287 RID: 647
		private bool _isInfoBarExtended;

		// Token: 0x04000288 RID: 648
		private bool _isInfoBarEnabled;

		// Token: 0x04000289 RID: 649
		private bool _isDenarTooltipWarning;

		// Token: 0x0400028A RID: 650
		private bool _isHealthTooltipWarning;

		// Token: 0x0400028B RID: 651
		private bool _isInfluenceTooltipWarning;

		// Token: 0x0400028C RID: 652
		private bool _isMoraleTooltipWarning;

		// Token: 0x0400028D RID: 653
		private bool _isDailyConsumptionTooltipWarning;

		// Token: 0x0400028E RID: 654
		private bool _isAvailableTroopsTooltipWarning;

		// Token: 0x0400028F RID: 655
		private bool _isMainHeroSick;

		// Token: 0x04000290 RID: 656
		private BasicTooltipViewModel _denarHint;

		// Token: 0x04000291 RID: 657
		private BasicTooltipViewModel _influenceHint;

		// Token: 0x04000292 RID: 658
		private BasicTooltipViewModel _availableTroopsHint;

		// Token: 0x04000293 RID: 659
		private BasicTooltipViewModel _healthHint;

		// Token: 0x04000294 RID: 660
		private BasicTooltipViewModel _dailyConsumptionHint;

		// Token: 0x04000295 RID: 661
		private BasicTooltipViewModel _moraleHint;

		// Token: 0x04000296 RID: 662
		private BasicTooltipViewModel _trainingFactorHint;

		// Token: 0x04000297 RID: 663
		private BasicTooltipViewModel _troopWageHint;

		// Token: 0x04000298 RID: 664
		private BasicTooltipViewModel _speedHint;

		// Token: 0x04000299 RID: 665
		private BasicTooltipViewModel _viewDistanceHint;

		// Token: 0x0400029A RID: 666
		private HintViewModel _extendHint;
	}
}

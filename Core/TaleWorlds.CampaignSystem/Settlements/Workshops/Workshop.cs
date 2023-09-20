﻿using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements.Workshops
{
	public class Workshop : SettlementArea
	{
		internal static void AutoGeneratedStaticCollectObjectsWorkshop(object o, List<object> collectedObjects)
		{
			((Workshop)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._settlement);
			collectedObjects.Add(this._owner);
			collectedObjects.Add(this._customName);
			collectedObjects.Add(this._productionProgress);
			collectedObjects.Add(this.WorkshopType);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.LastRunCampaignTime, collectedObjects);
		}

		internal static object AutoGeneratedGetMemberValueWorkshopType(object o)
		{
			return ((Workshop)o).WorkshopType;
		}

		internal static object AutoGeneratedGetMemberValueLastRunCampaignTime(object o)
		{
			return ((Workshop)o).LastRunCampaignTime;
		}

		internal static object AutoGeneratedGetMemberValueCapital(object o)
		{
			return ((Workshop)o).Capital;
		}

		internal static object AutoGeneratedGetMemberValueInitialCapital(object o)
		{
			return ((Workshop)o).InitialCapital;
		}

		internal static object AutoGeneratedGetMemberValue_settlement(object o)
		{
			return ((Workshop)o)._settlement;
		}

		internal static object AutoGeneratedGetMemberValue_tag(object o)
		{
			return ((Workshop)o)._tag;
		}

		internal static object AutoGeneratedGetMemberValue_owner(object o)
		{
			return ((Workshop)o)._owner;
		}

		internal static object AutoGeneratedGetMemberValue_customName(object o)
		{
			return ((Workshop)o)._customName;
		}

		internal static object AutoGeneratedGetMemberValue_productionProgress(object o)
		{
			return ((Workshop)o)._productionProgress;
		}

		public override Settlement Settlement
		{
			get
			{
				return this._settlement;
			}
		}

		public override string Tag
		{
			get
			{
				return this._tag;
			}
		}

		public override Hero Owner
		{
			get
			{
				return this._owner;
			}
		}

		public override TextObject Name
		{
			get
			{
				TextObject textObject;
				if ((textObject = this._customName) == null)
				{
					WorkshopType workshopType = this.WorkshopType;
					textObject = ((workshopType != null) ? workshopType.Name : null) ?? new TextObject("{=xWoXL2FG}Empty Workshop", null);
				}
				return textObject;
			}
		}

		[SaveableProperty(105)]
		public WorkshopType WorkshopType { get; private set; }

		public int ProfitMade
		{
			get
			{
				return MathF.Max(this.Capital - this.InitialCapital, 0);
			}
		}

		public int Expense
		{
			get
			{
				return Campaign.Current.Models.WorkshopModel.DailyExpense;
			}
		}

		[SaveableProperty(115)]
		public CampaignTime LastRunCampaignTime { get; private set; }

		[SaveableProperty(111)]
		public int Capital { get; private set; }

		[SaveableProperty(112)]
		public int InitialCapital { get; private set; }

		public Workshop(Settlement settlement, string tag)
		{
			this._customName = null;
			this._settlement = settlement;
			this._tag = tag;
			this.Capital = 0;
			this.InitialCapital = 0;
		}

		public override int GetHashCode()
		{
			return this.Settlement.GetHashCode() + this._tag.GetHashCode();
		}

		public void InitializeWorkshop(Hero owner, WorkshopType type)
		{
			this.WorkshopType = type;
			this._owner = owner;
			this._owner.AddOwnedWorkshop(this);
			this.Capital = Campaign.Current.Models.WorkshopModel.InitialCapital;
			this.InitialCapital = Campaign.Current.Models.WorkshopModel.InitialCapital;
			this._productionProgress = new float[type.Productions.Count];
		}

		public void ChangeOwnerOfWorkshop(Hero newOwner, WorkshopType type, int capital)
		{
			this._owner.RemoveOwnedWorkshop(this);
			this._owner = newOwner;
			this._owner.AddOwnedWorkshop(this);
			this.Capital = capital;
			if (type != this.WorkshopType)
			{
				this.ChangeWorkshopProduction(type);
			}
		}

		public void ChangeWorkshopProduction(WorkshopType newWorkshopType)
		{
			this.WorkshopType = newWorkshopType;
			this._productionProgress = new float[newWorkshopType.Productions.Count];
		}

		public void SetCustomName(TextObject customName)
		{
			this._customName = customName;
		}

		public void UpdateLastRunTime()
		{
			this.LastRunCampaignTime = CampaignTime.Now;
		}

		public void SetProgress(int i, float value)
		{
			this._productionProgress[i] = value;
		}

		internal void AfterLoad()
		{
			if (this._productionProgress.Length != this.WorkshopType.Productions.Count)
			{
				this._productionProgress = new float[this.WorkshopType.Productions.Count];
			}
			if (this.LastRunCampaignTime == CampaignTime.Zero)
			{
				this.LastRunCampaignTime = CampaignTime.Now;
			}
		}

		public void ChangeGold(int goldChange)
		{
			this.Capital += goldChange;
		}

		public float GetProductionProgress(int index)
		{
			return this._productionProgress[index];
		}

		public override string ToString()
		{
			return this.Name.ToString() + " " + this.Settlement.ToString();
		}

		[SaveableField(100)]
		private readonly Settlement _settlement;

		[SaveableField(101)]
		private readonly string _tag;

		[SaveableField(102)]
		private Hero _owner;

		[SaveableField(103)]
		private TextObject _customName;

		[SaveableField(104)]
		private float[] _productionProgress;
	}
}

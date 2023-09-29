﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements
{
	public class Alley : SettlementArea
	{
		internal static void AutoGeneratedStaticCollectObjectsAlley(object o, List<object> collectedObjects)
		{
			((Alley)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._owner);
		}

		internal static object AutoGeneratedGetMemberValue_owner(object o)
		{
			return ((Alley)o)._owner;
		}

		public override Settlement Settlement
		{
			get
			{
				return this._settlement;
			}
		}

		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		public override Hero Owner
		{
			get
			{
				return this._owner;
			}
		}

		public override string Tag
		{
			get
			{
				return this._tag;
			}
		}

		public void SetOwner(Hero newOwner)
		{
			if (this._owner != null)
			{
				this._owner.OwnedAlleys.Remove(this);
			}
			Hero owner = this._owner;
			this._owner = newOwner;
			if (this._owner != null)
			{
				this._owner.OwnedAlleys.Add(this);
				this.State = ((this._owner == Hero.MainHero) ? Alley.AreaState.OccupiedByPlayer : Alley.AreaState.OccupiedByGangLeader);
			}
			else
			{
				this.State = Alley.AreaState.Empty;
			}
			CampaignEventDispatcher.Instance.OnAlleyOwnerChanged(this, newOwner, owner);
		}

		public Alley.AreaState State { get; private set; }

		public Alley(Settlement settlement, string tag, TextObject name)
		{
			this.Initialize(settlement, tag, name);
		}

		public void Initialize(Settlement settlement, string tag, TextObject name)
		{
			this._name = name;
			this._settlement = settlement;
			this._tag = tag;
		}

		internal void AfterLoad()
		{
			if (this._owner != null)
			{
				this.State = ((this._owner == Hero.MainHero) ? Alley.AreaState.OccupiedByPlayer : Alley.AreaState.OccupiedByGangLeader);
				this._owner.OwnedAlleys.Add(this);
				if (MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.0", 27066) && !this._owner.IsAlive)
				{
					this.SetOwner(null);
					this.State = Alley.AreaState.Empty;
					return;
				}
			}
			else
			{
				this.State = Alley.AreaState.Empty;
			}
		}

		private Settlement _settlement;

		[CachedData]
		private TextObject _name;

		[SaveableField(10)]
		private Hero _owner;

		private string _tag;

		public enum AreaState
		{
			Empty,
			OccupiedByGangLeader,
			OccupiedByPlayer
		}
	}
}

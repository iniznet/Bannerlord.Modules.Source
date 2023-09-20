﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Roster
{
	// Token: 0x02000292 RID: 658
	public struct FlattenedTroopRosterElement
	{
		// Token: 0x06002323 RID: 8995 RVA: 0x00094E80 File Offset: 0x00093080
		public static void AutoGeneratedStaticCollectObjectsFlattenedTroopRosterElement(object o, List<object> collectedObjects)
		{
			((FlattenedTroopRosterElement)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x00094E9C File Offset: 0x0009309C
		private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._troop);
			UniqueTroopDescriptor.AutoGeneratedStaticCollectObjectsUniqueTroopDescriptor(this._uniqueNo, collectedObjects);
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x00094EBC File Offset: 0x000930BC
		internal static object AutoGeneratedGetMemberValueState(object o)
		{
			return ((FlattenedTroopRosterElement)o).State;
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x00094EDC File Offset: 0x000930DC
		internal static object AutoGeneratedGetMemberValue_troop(object o)
		{
			return ((FlattenedTroopRosterElement)o)._troop;
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x00094EE9 File Offset: 0x000930E9
		internal static object AutoGeneratedGetMemberValue_xp(object o)
		{
			return ((FlattenedTroopRosterElement)o)._xp;
		}

		// Token: 0x06002328 RID: 9000 RVA: 0x00094EFB File Offset: 0x000930FB
		internal static object AutoGeneratedGetMemberValue_xpGain(object o)
		{
			return ((FlattenedTroopRosterElement)o)._xpGain;
		}

		// Token: 0x06002329 RID: 9001 RVA: 0x00094F0D File Offset: 0x0009310D
		internal static object AutoGeneratedGetMemberValue_uniqueNo(object o)
		{
			return ((FlattenedTroopRosterElement)o)._uniqueNo;
		}

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x0600232A RID: 9002 RVA: 0x00094F1F File Offset: 0x0009311F
		// (set) Token: 0x0600232B RID: 9003 RVA: 0x00094F27 File Offset: 0x00093127
		[SaveableProperty(5)]
		public RosterTroopState State { get; private set; }

		// Token: 0x0600232C RID: 9004 RVA: 0x00094F30 File Offset: 0x00093130
		public FlattenedTroopRosterElement(CharacterObject troop, RosterTroopState state = RosterTroopState.Active, int xp = 0, UniqueTroopDescriptor uniqueNo = default(UniqueTroopDescriptor), int xpGain = 0)
		{
			this = default(FlattenedTroopRosterElement);
			this._troop = troop;
			this._xp = xp;
			this._xpGain = xpGain;
			this.State = state;
			this._uniqueNo = ((!uniqueNo.IsValid) ? new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed) : uniqueNo);
		}

		// Token: 0x0600232D RID: 9005 RVA: 0x00094F83 File Offset: 0x00093183
		public FlattenedTroopRosterElement(FlattenedTroopRosterElement rosterElement, RosterTroopState state)
		{
			this = new FlattenedTroopRosterElement(rosterElement);
			this.State = state;
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x00094F94 File Offset: 0x00093194
		private FlattenedTroopRosterElement(FlattenedTroopRosterElement rosterElement)
		{
			this = default(FlattenedTroopRosterElement);
			this._troop = rosterElement._troop;
			this._xp = rosterElement._xp;
			this._xpGain = rosterElement._xpGain;
			this._uniqueNo = rosterElement._uniqueNo;
			this.State = rosterElement.State;
		}

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x0600232F RID: 9007 RVA: 0x00094FE5 File Offset: 0x000931E5
		public CharacterObject Troop
		{
			get
			{
				return this._troop;
			}
		}

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x06002330 RID: 9008 RVA: 0x00094FED File Offset: 0x000931ED
		// (set) Token: 0x06002331 RID: 9009 RVA: 0x00095021 File Offset: 0x00093221
		public bool IsWounded
		{
			get
			{
				if (this.Troop.IsHero)
				{
					return this.Troop.HeroObject.IsWounded;
				}
				return this.State == RosterTroopState.Wounded || this.State == RosterTroopState.WoundedInThisBattle;
			}
			set
			{
				this.State = (value ? RosterTroopState.Wounded : RosterTroopState.Active);
			}
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x06002332 RID: 9010 RVA: 0x00095030 File Offset: 0x00093230
		// (set) Token: 0x06002333 RID: 9011 RVA: 0x0009503B File Offset: 0x0009323B
		public bool IsRouted
		{
			get
			{
				return this.State == RosterTroopState.Routed;
			}
			set
			{
				this.State = (value ? RosterTroopState.Routed : RosterTroopState.Active);
			}
		}

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x06002334 RID: 9012 RVA: 0x0009504A File Offset: 0x0009324A
		// (set) Token: 0x06002335 RID: 9013 RVA: 0x00095055 File Offset: 0x00093255
		public bool IsKilled
		{
			get
			{
				return this.State == RosterTroopState.Killed;
			}
			set
			{
				this.State = (value ? RosterTroopState.Killed : RosterTroopState.Active);
			}
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x06002336 RID: 9014 RVA: 0x00095064 File Offset: 0x00093264
		public int Xp
		{
			get
			{
				return this._xp;
			}
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x06002337 RID: 9015 RVA: 0x0009506C File Offset: 0x0009326C
		public int XpGained
		{
			get
			{
				return this._xpGain;
			}
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x06002338 RID: 9016 RVA: 0x00095074 File Offset: 0x00093274
		public UniqueTroopDescriptor Descriptor
		{
			get
			{
				return this._uniqueNo;
			}
		}

		// Token: 0x06002339 RID: 9017 RVA: 0x0009507C File Offset: 0x0009327C
		public override string ToString()
		{
			return this.Troop.ToString();
		}

		// Token: 0x04000AC7 RID: 2759
		public static readonly FlattenedTroopRosterElement DefaultFlattenedTroopRosterElement;

		// Token: 0x04000AC8 RID: 2760
		[SaveableField(0)]
		private readonly CharacterObject _troop;

		// Token: 0x04000AC9 RID: 2761
		[SaveableField(1)]
		private readonly int _xp;

		// Token: 0x04000ACA RID: 2762
		[SaveableField(3)]
		private readonly int _xpGain;

		// Token: 0x04000ACB RID: 2763
		[SaveableField(4)]
		private readonly UniqueTroopDescriptor _uniqueNo;
	}
}

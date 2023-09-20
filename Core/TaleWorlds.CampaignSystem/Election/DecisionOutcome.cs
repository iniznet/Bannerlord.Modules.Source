﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
	// Token: 0x02000271 RID: 625
	public abstract class DecisionOutcome
	{
		// Token: 0x06002016 RID: 8214 RVA: 0x00089365 File Offset: 0x00087565
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this.SupporterList);
			collectedObjects.Add(this._sponsorClan);
		}

		// Token: 0x06002017 RID: 8215 RVA: 0x0008937F File Offset: 0x0008757F
		internal static object AutoGeneratedGetMemberValueInitialMerit(object o)
		{
			return ((DecisionOutcome)o).InitialMerit;
		}

		// Token: 0x06002018 RID: 8216 RVA: 0x00089391 File Offset: 0x00087591
		internal static object AutoGeneratedGetMemberValueSupporterList(object o)
		{
			return ((DecisionOutcome)o).SupporterList;
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x0008939E File Offset: 0x0008759E
		internal static object AutoGeneratedGetMemberValue_sponsorClan(object o)
		{
			return ((DecisionOutcome)o)._sponsorClan;
		}

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x0600201A RID: 8218 RVA: 0x000893AC File Offset: 0x000875AC
		public float Support
		{
			get
			{
				float num = 0f;
				using (List<Supporter>.Enumerator enumerator = this.SupporterList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current.SupportWeight)
						{
						case Supporter.SupportWeights.SlightlyFavor:
							num += 0.2f;
							break;
						case Supporter.SupportWeights.StronglyFavor:
							num += 0.4f;
							break;
						case Supporter.SupportWeights.FullyPush:
							num += 1f;
							break;
						}
					}
				}
				return num;
			}
		}

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x0600201B RID: 8219 RVA: 0x00089438 File Offset: 0x00087638
		public float Merit
		{
			get
			{
				return this.InitialMerit * (1f + this.Support);
			}
		}

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x0600201C RID: 8220 RVA: 0x0008944D File Offset: 0x0008764D
		public Clan SponsorClan
		{
			get
			{
				return this._sponsorClan;
			}
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x0600201D RID: 8221 RVA: 0x00089455 File Offset: 0x00087655
		// (set) Token: 0x0600201E RID: 8222 RVA: 0x0008945D File Offset: 0x0008765D
		public float InitialSupport { get; internal set; }

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x0600201F RID: 8223 RVA: 0x00089466 File Offset: 0x00087666
		// (set) Token: 0x06002020 RID: 8224 RVA: 0x0008946E File Offset: 0x0008766E
		public float Likelihood { get; internal set; }

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x06002021 RID: 8225 RVA: 0x00089477 File Offset: 0x00087677
		// (set) Token: 0x06002022 RID: 8226 RVA: 0x0008947F File Offset: 0x0008767F
		public float TotalSupportPoints { get; internal set; }

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x06002023 RID: 8227 RVA: 0x00089488 File Offset: 0x00087688
		// (set) Token: 0x06002024 RID: 8228 RVA: 0x00089490 File Offset: 0x00087690
		public float WinChance { get; internal set; }

		// Token: 0x06002025 RID: 8229
		public abstract TextObject GetDecisionTitle();

		// Token: 0x06002026 RID: 8230
		public abstract TextObject GetDecisionDescription();

		// Token: 0x06002027 RID: 8231
		public abstract string GetDecisionLink();

		// Token: 0x06002028 RID: 8232
		public abstract ImageIdentifier GetDecisionImageIdentifier();

		// Token: 0x06002029 RID: 8233 RVA: 0x00089499 File Offset: 0x00087699
		public void AddSupport(Supporter supporter)
		{
			this.SupporterList.Add(supporter);
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x000894A7 File Offset: 0x000876A7
		public void ResetSupport(Supporter supporter)
		{
			if (this.SupporterList.Contains(supporter))
			{
				this.SupporterList.Remove(supporter);
			}
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x000894C4 File Offset: 0x000876C4
		public void SetSponsor(Clan sponsorClan)
		{
			this._sponsorClan = sponsorClan;
		}

		// Token: 0x04000A37 RID: 2615
		[SaveableField(0)]
		public float InitialMerit;

		// Token: 0x04000A38 RID: 2616
		[SaveableField(1)]
		public List<Supporter> SupporterList = new List<Supporter>();

		// Token: 0x04000A39 RID: 2617
		[SaveableField(2)]
		private Clan _sponsorClan;
	}
}

﻿using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	// Token: 0x020002FC RID: 764
	public class IssueEffect : MBObjectBase
	{
		// Token: 0x06002C30 RID: 11312 RVA: 0x000B89BC File Offset: 0x000B6BBC
		internal static void AutoGeneratedStaticCollectObjectsIssueEffect(object o, List<object> collectedObjects)
		{
			((IssueEffect)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002C31 RID: 11313 RVA: 0x000B89CA File Offset: 0x000B6BCA
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x17000AC9 RID: 2761
		// (get) Token: 0x06002C32 RID: 11314 RVA: 0x000B89D3 File Offset: 0x000B6BD3
		public static MBReadOnlyList<IssueEffect> All
		{
			get
			{
				return Campaign.Current.AllIssueEffects;
			}
		}

		// Token: 0x17000ACA RID: 2762
		// (get) Token: 0x06002C33 RID: 11315 RVA: 0x000B89DF File Offset: 0x000B6BDF
		// (set) Token: 0x06002C34 RID: 11316 RVA: 0x000B89E7 File Offset: 0x000B6BE7
		public TextObject Name { get; private set; }

		// Token: 0x17000ACB RID: 2763
		// (get) Token: 0x06002C35 RID: 11317 RVA: 0x000B89F0 File Offset: 0x000B6BF0
		// (set) Token: 0x06002C36 RID: 11318 RVA: 0x000B89F8 File Offset: 0x000B6BF8
		public TextObject Description { get; private set; }

		// Token: 0x06002C37 RID: 11319 RVA: 0x000B8A01 File Offset: 0x000B6C01
		public IssueEffect(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06002C38 RID: 11320 RVA: 0x000B8A0A File Offset: 0x000B6C0A
		public void Initialize(TextObject name, TextObject description)
		{
			this.Name = name;
			this.Description = description;
			base.AfterInitialized();
		}

		// Token: 0x06002C39 RID: 11321 RVA: 0x000B8A20 File Offset: 0x000B6C20
		public override string ToString()
		{
			return this.Name.ToString();
		}
	}
}

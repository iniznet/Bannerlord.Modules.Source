﻿using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Siege
{
	// Token: 0x02000287 RID: 647
	public class SiegeStrategy : MBObjectBase
	{
		// Token: 0x0600220B RID: 8715 RVA: 0x00090E75 File Offset: 0x0008F075
		internal static void AutoGeneratedStaticCollectObjectsSiegeStrategy(object o, List<object> collectedObjects)
		{
			((SiegeStrategy)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x00090E83 File Offset: 0x0008F083
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x0600220D RID: 8717 RVA: 0x00090E8C File Offset: 0x0008F08C
		public static MBReadOnlyList<SiegeStrategy> All
		{
			get
			{
				return Campaign.Current.AllSiegeStrategies;
			}
		}

		// Token: 0x17000871 RID: 2161
		// (get) Token: 0x0600220E RID: 8718 RVA: 0x00090E98 File Offset: 0x0008F098
		// (set) Token: 0x0600220F RID: 8719 RVA: 0x00090EA0 File Offset: 0x0008F0A0
		public TextObject Name { get; private set; }

		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x06002210 RID: 8720 RVA: 0x00090EA9 File Offset: 0x0008F0A9
		// (set) Token: 0x06002211 RID: 8721 RVA: 0x00090EB1 File Offset: 0x0008F0B1
		public TextObject Description { get; private set; }

		// Token: 0x06002212 RID: 8722 RVA: 0x00090EBA File Offset: 0x0008F0BA
		public SiegeStrategy(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x00090EC3 File Offset: 0x0008F0C3
		public void Initialize(TextObject name, TextObject description)
		{
			base.Initialize();
			this.Name = name;
			this.Description = description;
			base.AfterInitialized();
		}
	}
}

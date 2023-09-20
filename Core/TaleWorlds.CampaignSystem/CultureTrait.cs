﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200005A RID: 90
	public class CultureTrait : PropertyObject
	{
		// Token: 0x06000A4B RID: 2635 RVA: 0x000392C8 File Offset: 0x000374C8
		internal static void AutoGeneratedStaticCollectObjectsCultureTrait(object o, List<object> collectedObjects)
		{
			((CultureTrait)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x000392D6 File Offset: 0x000374D6
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x000392DF File Offset: 0x000374DF
		public CultureTrait(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x000392E8 File Offset: 0x000374E8
		public void Initialize(TextObject name, TextObject description, string asdf)
		{
			base.Initialize(name, description);
		}
	}
}

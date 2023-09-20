﻿using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	// Token: 0x020002FE RID: 766
	public abstract class IssueCoolDownData
	{
		// Token: 0x06002C6C RID: 11372 RVA: 0x000B9FB4 File Offset: 0x000B81B4
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.ExpireTime, collectedObjects);
		}

		// Token: 0x06002C6D RID: 11373 RVA: 0x000B9FC7 File Offset: 0x000B81C7
		internal static object AutoGeneratedGetMemberValueExpireTime(object o)
		{
			return ((IssueCoolDownData)o).ExpireTime;
		}

		// Token: 0x06002C6E RID: 11374 RVA: 0x000B9FD9 File Offset: 0x000B81D9
		protected IssueCoolDownData(CampaignTime expireTime)
		{
			this.ExpireTime = expireTime;
		}

		// Token: 0x06002C6F RID: 11375 RVA: 0x000B9FE8 File Offset: 0x000B81E8
		public virtual bool IsValid()
		{
			return !this.ExpireTime.IsPast;
		}

		// Token: 0x06002C70 RID: 11376
		public abstract bool IsRelatedTo(object obj);

		// Token: 0x04000D6A RID: 3434
		[SaveableField(0)]
		public readonly CampaignTime ExpireTime;
	}
}

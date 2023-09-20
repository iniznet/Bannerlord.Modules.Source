using System;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.ClientApplication
{
	// Token: 0x02000056 RID: 86
	public abstract class DiamondClientApplicationObject
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600020B RID: 523 RVA: 0x000062EC File Offset: 0x000044EC
		public DiamondClientApplication Application
		{
			get
			{
				return this._application;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600020C RID: 524 RVA: 0x000062F4 File Offset: 0x000044F4
		public ApplicationVersion ApplicationVersion
		{
			get
			{
				return this.Application.ApplicationVersion;
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00006301 File Offset: 0x00004501
		protected DiamondClientApplicationObject(DiamondClientApplication application)
		{
			this._application = application;
		}

		// Token: 0x040000BD RID: 189
		private DiamondClientApplication _application;
	}
}

using System;
using Epic.OnlineServices;
using Epic.OnlineServices.Platform;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond.AccessProvider.Epic
{
	// Token: 0x02000003 RID: 3
	public class EpicLoginAccessProvider : ILoginAccessProvider
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public EpicLoginAccessProvider(PlatformInterface platform, EpicAccountId epicAccountId, string epicUserName, string accessToken, TextObject initFailReason)
		{
			this._platform = platform;
			this._epicAccountId = epicAccountId;
			this._accessToken = accessToken;
			this._epicUserName = epicUserName;
			this._initFailReason = initFailReason;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000207D File Offset: 0x0000027D
		void ILoginAccessProvider.Initialize(string preferredUserName, PlatformInitParams initParams)
		{
			this._initParams = initParams;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002086 File Offset: 0x00000286
		private string ExchangeCode
		{
			get
			{
				return (string)this._initParams["ExchangeCode"];
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000209D File Offset: 0x0000029D
		string ILoginAccessProvider.GetUserName()
		{
			return this._epicUserName;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020A8 File Offset: 0x000002A8
		private string EpicId
		{
			get
			{
				if (this._epicAccountId == null)
				{
					return null;
				}
				string text;
				this._epicAccountId.ToString(out text);
				return text;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020D4 File Offset: 0x000002D4
		PlayerId ILoginAccessProvider.GetPlayerId()
		{
			if (this.EpicId == null)
			{
				return PlayerId.Empty;
			}
			return new PlayerId(3, this.EpicId);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020F0 File Offset: 0x000002F0
		AccessObjectResult ILoginAccessProvider.CreateAccessObject()
		{
			if (this._initFailReason != null)
			{
				return AccessObjectResult.CreateFailed(this._initFailReason);
			}
			return AccessObjectResult.CreateSuccess(new EpicAccessObject
			{
				EpicId = this.EpicId,
				AccessToken = this._accessToken
			});
		}

		// Token: 0x04000008 RID: 8
		private string _epicUserName;

		// Token: 0x04000009 RID: 9
		private PlatformInitParams _initParams;

		// Token: 0x0400000A RID: 10
		private PlatformInterface _platform;

		// Token: 0x0400000B RID: 11
		private EpicAccountId _epicAccountId;

		// Token: 0x0400000C RID: 12
		private string _accessToken;

		// Token: 0x0400000D RID: 13
		private TextObject _initFailReason;
	}
}

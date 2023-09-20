using System;
using Epic.OnlineServices;
using Epic.OnlineServices.Platform;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond.AccessProvider.Epic
{
	public class EpicLoginAccessProvider : ILoginAccessProvider
	{
		public EpicLoginAccessProvider(PlatformInterface platform, EpicAccountId epicAccountId, string epicUserName, string accessToken, TextObject initFailReason)
		{
			this._platform = platform;
			this._epicAccountId = epicAccountId;
			this._accessToken = accessToken;
			this._epicUserName = epicUserName;
			this._initFailReason = initFailReason;
		}

		void ILoginAccessProvider.Initialize(string preferredUserName, PlatformInitParams initParams)
		{
			this._initParams = initParams;
		}

		private string ExchangeCode
		{
			get
			{
				return (string)this._initParams["ExchangeCode"];
			}
		}

		string ILoginAccessProvider.GetUserName()
		{
			return this._epicUserName;
		}

		private string EpicId
		{
			get
			{
				if (this._epicAccountId == null)
				{
					return null;
				}
				string text;
				this._epicAccountId.ToString(ref text);
				return text;
			}
		}

		PlayerId ILoginAccessProvider.GetPlayerId()
		{
			if (this.EpicId == null)
			{
				return PlayerId.Empty;
			}
			return new PlayerId(3, this.EpicId);
		}

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

		private string _epicUserName;

		private PlatformInitParams _initParams;

		private PlatformInterface _platform;

		private EpicAccountId _epicAccountId;

		private string _accessToken;

		private TextObject _initFailReason;
	}
}

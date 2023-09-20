using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond.AccessProvider.GDK
{
	public class GDKLoginAccessProvider : ILoginAccessProvider
	{
		public GDKLoginAccessProvider(string gamerTag, ulong xuid, string token, PlayerId playerId, TextObject initializationFailReason)
		{
			this._gamerTag = gamerTag;
			this._playerId = playerId;
			this._initializationFailReason = initializationFailReason;
			this._token = token;
			this._xuid = xuid;
		}

		void ILoginAccessProvider.Initialize(string preferredUserName, PlatformInitParams initParams)
		{
			this._initParams = initParams;
		}

		string ILoginAccessProvider.GetUserName()
		{
			return this._gamerTag;
		}

		PlayerId ILoginAccessProvider.GetPlayerId()
		{
			return this._playerId;
		}

		AccessObjectResult ILoginAccessProvider.CreateAccessObject()
		{
			if (this._initializationFailReason != null)
			{
				return AccessObjectResult.CreateFailed(this._initializationFailReason);
			}
			if (this._playerId == PlayerId.Empty)
			{
				return AccessObjectResult.CreateFailed(new TextObject("{=leU2EiDo}Could not initialize Xbox Live player on lobby server", null));
			}
			return AccessObjectResult.CreateSuccess(new GDKAccessObject
			{
				Id = this._xuid.ToString(),
				Token = this._token
			});
		}

		private PlatformInitParams _initParams;

		private string _gamerTag;

		private ulong _xuid;

		private PlayerId _playerId;

		private TextObject _initializationFailReason;

		private string _token;
	}
}

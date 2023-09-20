using System;
using System.Text;
using Steamworks;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond.AccessProvider.Steam
{
	public class SteamLoginAccessProvider : ILoginAccessProvider
	{
		void ILoginAccessProvider.Initialize(string preferredUserName, PlatformInitParams initParams)
		{
			if (SteamAPI.Init() && Packsize.Test())
			{
				this._appId = SteamUtils.GetAppID().m_AppId;
				this._steamId = SteamUser.GetSteamID().m_SteamID;
				this._steamUserName = SteamFriends.GetPersonaName();
				this._initParams = initParams;
				byte[] array = new byte[1042];
				SteamAPICall_t steamAPICall_t = SteamUser.RequestEncryptedAppTicket(Encoding.UTF8.GetBytes(""), array.Length);
				CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.EncryptedAppTicketResponseReceived)).Set(steamAPICall_t, null);
				while (this._appTicket == null)
				{
					SteamAPI.RunCallbacks();
				}
			}
		}

		private int AppId
		{
			get
			{
				return Convert.ToInt32(this._appId);
			}
		}

		string ILoginAccessProvider.GetUserName()
		{
			return this._steamUserName;
		}

		PlayerId ILoginAccessProvider.GetPlayerId()
		{
			return new PlayerId(2, 0UL, this._steamId);
		}

		AccessObjectResult ILoginAccessProvider.CreateAccessObject()
		{
			if (!SteamAPI.IsSteamRunning())
			{
				return AccessObjectResult.CreateFailed(new TextObject("{=uunRVBPN}Steam is not running.", null));
			}
			byte[] array = new byte[1024];
			uint num;
			if (SteamUser.GetAuthSessionTicket(array, 1024, out num) == HAuthTicket.Invalid)
			{
				return AccessObjectResult.CreateFailed(new TextObject("{=XSU8Bbbb}Invalid Steam session.", null));
			}
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			foreach (byte b in array)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			string text = stringBuilder.ToString();
			return AccessObjectResult.CreateSuccess(new SteamAccessObject(this._steamUserName, text, this.AppId, this._appTicket));
		}

		private void EncryptedAppTicketResponseReceived(EncryptedAppTicketResponse_t response, bool bIOFailure)
		{
			byte[] array = new byte[2048];
			uint num;
			SteamUser.GetEncryptedAppTicket(array, 2048, out num);
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, (long)((ulong)num));
			this._appTicket = BitConverter.ToString(array2).Replace("-", "");
		}

		private string _steamUserName;

		private ulong _steamId;

		private PlatformInitParams _initParams;

		private uint _appId;

		private string _appTicket;
	}
}

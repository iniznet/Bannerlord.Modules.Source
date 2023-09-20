using System;
using System.Text;
using System.Threading;
using Galaxy.Api;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond.AccessProvider.GOG
{
	public class GOGLoginAccessProvider : ILoginAccessProvider
	{
		void ILoginAccessProvider.Initialize(string preferredUserName, PlatformInitParams initParams)
		{
			this._initParams = initParams;
			if (GalaxyInstance.User().IsLoggedOn())
			{
				IUser user = GalaxyInstance.User();
				IFriends friends = GalaxyInstance.Friends();
				this._gogId = user.GetGalaxyID().GetRealID();
				this._oldId = user.GetGalaxyID().ToUint64();
				this._gogUserName = friends.GetPersonaName();
			}
		}

		string ILoginAccessProvider.GetUserName()
		{
			return this._gogUserName;
		}

		PlayerId ILoginAccessProvider.GetPlayerId()
		{
			return new PlayerId(5, 0UL, this._gogId);
		}

		AccessObjectResult ILoginAccessProvider.CreateAccessObject()
		{
			if (!GalaxyInstance.User().IsLoggedOn())
			{
				return AccessObjectResult.CreateFailed(new TextObject("{=hU361b7v}Not logged in on GOG.", null));
			}
			IUser user = GalaxyInstance.User();
			if (user.IsLoggedOn())
			{
				EncryptedAppTicketListener encryptedAppTicketListener = new EncryptedAppTicketListener();
				user.RequestEncryptedAppTicket(null, 0U, encryptedAppTicketListener);
				while (!encryptedAppTicketListener.GotResult)
				{
					GalaxyInstance.ProcessData();
					Thread.Sleep(5);
				}
				byte[] array = new byte[4096];
				uint num = 0U;
				user.GetEncryptedAppTicket(array, (uint)array.Length, ref num);
				byte[] array2 = new byte[num];
				Array.Copy(array, array2, (long)((ulong)num));
				string @string = Encoding.ASCII.GetString(array2);
				return AccessObjectResult.CreateSuccess(new GOGAccessObject(this._gogUserName, this._gogId, this._oldId, @string));
			}
			return AccessObjectResult.CreateFailed(new TextObject("{=hU361b7v}Not logged in on GOG.", null));
		}

		private string _gogUserName;

		private ulong _gogId;

		private ulong _oldId;

		private PlatformInitParams _initParams;
	}
}

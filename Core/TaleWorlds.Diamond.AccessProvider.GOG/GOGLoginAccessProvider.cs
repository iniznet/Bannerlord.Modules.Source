using System;
using System.Text;
using System.Threading;
using Galaxy.Api;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Diamond.AccessProvider.GOG
{
	// Token: 0x02000002 RID: 2
	public class GOGLoginAccessProvider : ILoginAccessProvider
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		void ILoginAccessProvider.Initialize(string preferredUserName, PlatformInitParams initParams)
		{
			this._initParams = initParams;
			if (GalaxyInstance.User().IsLoggedOn())
			{
				IUser user = GalaxyInstance.User();
				IFriends friends = GalaxyInstance.Friends();
				this._gogId = user.GetGalaxyID().ToUint64();
				this._gogUserName = friends.GetPersonaName();
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002091 File Offset: 0x00000291
		string ILoginAccessProvider.GetUserName()
		{
			return this._gogUserName;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002099 File Offset: 0x00000299
		PlayerId ILoginAccessProvider.GetPlayerId()
		{
			return new PlayerId(5, 0UL, this._gogId);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020AC File Offset: 0x000002AC
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
				string text = "I Own Bannerlord";
				byte[] bytes = Encoding.Unicode.GetBytes(text);
				user.RequestEncryptedAppTicket(bytes, (uint)bytes.Length, encryptedAppTicketListener);
				while (!encryptedAppTicketListener.GotResult)
				{
					GalaxyInstance.ProcessData();
					Thread.Sleep(5);
				}
				byte[] array = new byte[4096];
				uint num = 0U;
				user.GetEncryptedAppTicket(array, (uint)array.Length, ref num);
				return AccessObjectResult.CreateSuccess(new GOGAccessObject(this._gogUserName, this._gogId, array));
			}
			return AccessObjectResult.CreateFailed(new TextObject("{=hU361b7v}Not logged in on GOG.", null));
		}

		// Token: 0x04000001 RID: 1
		private string _gogUserName;

		// Token: 0x04000002 RID: 2
		private ulong _gogId;

		// Token: 0x04000003 RID: 3
		private PlatformInitParams _initParams;
	}
}

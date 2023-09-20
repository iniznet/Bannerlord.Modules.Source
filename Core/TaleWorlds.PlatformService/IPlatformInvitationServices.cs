using System;
using System.Threading.Tasks;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	// Token: 0x02000006 RID: 6
	public interface IPlatformInvitationServices
	{
		// Token: 0x06000016 RID: 22
		Task OnLogin();

		// Token: 0x06000017 RID: 23
		Task<bool> OnInviteToPlatformSession(PlayerId playerId);

		// Token: 0x06000018 RID: 24
		Task OnLeftParty();

		// Token: 0x06000019 RID: 25
		PlayerId GetInvitationPlayerId();

		// Token: 0x0600001A RID: 26
		Task<Tuple<bool, ulong>> JoinSession();

		// Token: 0x0600001B RID: 27
		Task LeaveSession(bool createNewSession);
	}
}

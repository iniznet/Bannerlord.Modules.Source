using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker.Targets
{
	// Token: 0x020000C1 RID: 193
	public class MissionPeerMarkerTargetVM : MissionMarkerTargetVM
	{
		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x0600125B RID: 4699 RVA: 0x0003C663 File Offset: 0x0003A863
		// (set) Token: 0x0600125C RID: 4700 RVA: 0x0003C66B File Offset: 0x0003A86B
		public MissionPeer TargetPeer { get; private set; }

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x0600125D RID: 4701 RVA: 0x0003C674 File Offset: 0x0003A874
		public override Vec3 WorldPosition
		{
			get
			{
				MissionPeer targetPeer = this.TargetPeer;
				if (((targetPeer != null) ? targetPeer.ControlledAgent : null) != null)
				{
					return this.TargetPeer.ControlledAgent.Position + new Vec3(0f, 0f, this.TargetPeer.ControlledAgent.GetEyeGlobalHeight(), -1f);
				}
				Debug.FailedAssert("No target found!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\FlagMarker\\Targets\\MissionPeerMarkerTargetVM.cs", "WorldPosition", 27);
				return Vec3.One;
			}
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x0600125E RID: 4702 RVA: 0x0003C6EA File Offset: 0x0003A8EA
		protected override float HeightOffset
		{
			get
			{
				return 0.75f;
			}
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x0003C6F1 File Offset: 0x0003A8F1
		public MissionPeerMarkerTargetVM(MissionPeer peer, bool isFriend)
			: base(MissionMarkerType.Peer)
		{
			this.TargetPeer = peer;
			this._isFriend = isFriend;
			base.Name = peer.DisplayedName;
			this.SetVisual();
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x0003C71C File Offset: 0x0003A91C
		private void SetVisual()
		{
			string text = "#FFFFFFFF";
			if (NetworkMain.GameClient.IsInParty && NetworkMain.GameClient.PlayersInParty.Any((PartyPlayerInLobbyClient p) => p.PlayerId.Equals(this.TargetPeer.Peer.Id)))
			{
				text = "#00FF00FF";
			}
			else if (this._isFriend)
			{
				text = "#FFFF00FF";
			}
			else if (NetworkMain.GameClient.IsInClan && NetworkMain.GameClient.PlayersInClan.Any((ClanPlayer p) => p.PlayerId.Equals(this.TargetPeer.Peer.Id)))
			{
				text = "#00FFFFFF";
			}
			uint num = TaleWorlds.Library.Color.ConvertStringToColor("#FFFFFFFF").ToUnsignedInteger();
			uint num2 = TaleWorlds.Library.Color.ConvertStringToColor(text).ToUnsignedInteger();
			base.RefreshColor(num, num2);
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x0003C7C9 File Offset: 0x0003A9C9
		public override void UpdateScreenPosition(Camera missionCamera)
		{
			MissionPeer targetPeer = this.TargetPeer;
			if (((targetPeer != null) ? targetPeer.ControlledAgent : null) == null)
			{
				return;
			}
			base.UpdateScreenPosition(missionCamera);
		}

		// Token: 0x040008C5 RID: 2245
		private const string _partyMemberColor = "#00FF00FF";

		// Token: 0x040008C6 RID: 2246
		private const string _friendColor = "#FFFF00FF";

		// Token: 0x040008C7 RID: 2247
		private const string _clanMemberColor = "#00FFFFFF";

		// Token: 0x040008C8 RID: 2248
		private bool _isFriend;
	}
}

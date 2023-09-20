using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker.Targets
{
	public class MissionPeerMarkerTargetVM : MissionMarkerTargetVM
	{
		public MissionPeer TargetPeer { get; private set; }

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

		protected override float HeightOffset
		{
			get
			{
				return 0.75f;
			}
		}

		public MissionPeerMarkerTargetVM(MissionPeer peer, bool isFriend)
			: base(MissionMarkerType.Peer)
		{
			this.TargetPeer = peer;
			this._isFriend = isFriend;
			base.Name = peer.DisplayedName;
			this.SetVisual();
		}

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

		public override void UpdateScreenPosition(Camera missionCamera)
		{
			MissionPeer targetPeer = this.TargetPeer;
			if (((targetPeer != null) ? targetPeer.ControlledAgent : null) == null)
			{
				return;
			}
			base.UpdateScreenPosition(missionCamera);
		}

		private const string _partyMemberColor = "#00FF00FF";

		private const string _friendColor = "#FFFF00FF";

		private const string _clanMemberColor = "#00FFFFFF";

		private bool _isFriend;
	}
}

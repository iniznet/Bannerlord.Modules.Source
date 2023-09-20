using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.FlagMarker.Targets
{
	public class MissionAlwaysVisibleMarkerTargetVM : MissionMarkerTargetVM
	{
		public MissionPeer TargetPeer { get; private set; }

		public override Vec3 WorldPosition
		{
			get
			{
				return this._position;
			}
		}

		protected override float HeightOffset
		{
			get
			{
				return 0.75f;
			}
		}

		public MissionAlwaysVisibleMarkerTargetVM(MissionPeer peer, Vec3 position, Action<MissionAlwaysVisibleMarkerTargetVM> onRemove)
			: base(MissionMarkerType.Peer)
		{
			this.TargetPeer = peer;
			this._position = position;
			this._onRemove = onRemove;
		}

		public void ExecuteRemove()
		{
			Action<MissionAlwaysVisibleMarkerTargetVM> onRemove = this._onRemove;
			if (onRemove == null)
			{
				return;
			}
			onRemove(this);
		}

		private Vec3 _position;

		private Action<MissionAlwaysVisibleMarkerTargetVM> _onRemove;
	}
}

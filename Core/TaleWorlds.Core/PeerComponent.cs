using System;

namespace TaleWorlds.Core
{
	public abstract class PeerComponent : IEntityComponent
	{
		public VirtualPlayer Peer
		{
			get
			{
				return this._peer;
			}
			set
			{
				this._peer = value;
			}
		}

		public virtual void Initialize()
		{
		}

		public string Name
		{
			get
			{
				return this.Peer.UserName;
			}
		}

		public bool IsMine
		{
			get
			{
				return this.Peer.IsMine;
			}
		}

		public T GetComponent<T>() where T : PeerComponent
		{
			return this.Peer.GetComponent<T>();
		}

		public virtual void OnInitialize()
		{
		}

		public virtual void OnFinalize()
		{
		}

		public uint TypeId { get; set; }

		private VirtualPlayer _peer;
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TaleWorlds.PlayerServices.Avatar
{
	public abstract class ApiAvatarServiceBase : IAvatarService
	{
		protected Dictionary<ulong, AvatarData> AvatarImageCache { get; }

		[TupleElementNames(new string[] { "accountId", "avatarData" })]
		protected List<ValueTuple<ulong, AvatarData>> WaitingAccounts
		{
			[return: TupleElementNames(new string[] { "accountId", "avatarData" })]
			get;
			[param: TupleElementNames(new string[] { "accountId", "avatarData" })]
			set;
		}

		[TupleElementNames(new string[] { "accountId", "avatarData" })]
		protected List<ValueTuple<ulong, AvatarData>> InProgressAccounts
		{
			[return: TupleElementNames(new string[] { "accountId", "avatarData" })]
			get;
			[param: TupleElementNames(new string[] { "accountId", "avatarData" })]
			set;
		}

		protected Task FetchAvatarsTask { get; set; }

		protected ApiAvatarServiceBase()
		{
			this.AvatarImageCache = new Dictionary<ulong, AvatarData>();
			this.WaitingAccounts = new List<ValueTuple<ulong, AvatarData>>();
			this.InProgressAccounts = new List<ValueTuple<ulong, AvatarData>>();
			this.FetchAvatarsTask = null;
		}

		public void Tick(float dt)
		{
		}

		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			ulong part = playerId.Part4;
			Dictionary<ulong, AvatarData> avatarImageCache = this.AvatarImageCache;
			AvatarData avatarData;
			lock (avatarImageCache)
			{
				if (this.AvatarImageCache.TryGetValue(part, out avatarData) && avatarData.Status != AvatarData.DataStatus.Failed)
				{
					return avatarData;
				}
				if (this.AvatarImageCache.Count > 300)
				{
					this.AvatarImageCache.Clear();
				}
				avatarData = new AvatarData();
				this.AvatarImageCache[part] = avatarData;
			}
			List<ValueTuple<ulong, AvatarData>> waitingAccounts = this.WaitingAccounts;
			lock (waitingAccounts)
			{
				this.WaitingAccounts.Add(new ValueTuple<ulong, AvatarData>(part, avatarData));
			}
			this.CheckWaitingAccounts();
			return avatarData;
		}

		private void CheckWaitingAccounts()
		{
			List<ValueTuple<ulong, AvatarData>> waitingAccounts = this.WaitingAccounts;
			lock (waitingAccounts)
			{
				if (this.FetchAvatarsTask == null || this.FetchAvatarsTask.IsCompleted)
				{
					Task fetchAvatarsTask = this.FetchAvatarsTask;
					if (fetchAvatarsTask != null && fetchAvatarsTask.IsFaulted)
					{
						this.FetchAvatarsTask = null;
						foreach (ValueTuple<ulong, AvatarData> valueTuple in this.InProgressAccounts)
						{
							AvatarData item = valueTuple.Item2;
							if (item.Status == AvatarData.DataStatus.NotReady)
							{
								item.SetFailed();
							}
						}
					}
					if (this.FetchAvatarsTask != null)
					{
						this.FetchAvatarsTask.Dispose();
						this.FetchAvatarsTask = null;
					}
					this.InProgressAccounts.Clear();
					if (this.WaitingAccounts.Count > 0)
					{
						this.FetchAvatarsTask = this.FetchAvatars();
						Task.Run(async delegate
						{
							await this.FetchAvatarsTask;
							this.CheckWaitingAccounts();
						});
					}
				}
			}
		}

		protected abstract Task FetchAvatars();

		public void Initialize()
		{
		}

		public void ClearCache()
		{
			Dictionary<ulong, AvatarData> avatarImageCache = this.AvatarImageCache;
			lock (avatarImageCache)
			{
				this.AvatarImageCache.Clear();
			}
			List<ValueTuple<ulong, AvatarData>> waitingAccounts = this.WaitingAccounts;
			lock (waitingAccounts)
			{
				this.WaitingAccounts.Clear();
			}
		}

		public bool IsInitialized()
		{
			return true;
		}
	}
}

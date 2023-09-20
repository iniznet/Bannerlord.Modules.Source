using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TaleWorlds.PlayerServices.Avatar
{
	public abstract class ApiAvatarServiceBaseSingleThread : IAvatarService
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

		protected ApiAvatarServiceBaseSingleThread()
		{
			this.AvatarImageCache = new Dictionary<ulong, AvatarData>();
			this.WaitingAccounts = new List<ValueTuple<ulong, AvatarData>>();
			this.InProgressAccounts = new List<ValueTuple<ulong, AvatarData>>();
			this.FetchAvatarsTask = null;
		}

		public void Tick(float dt)
		{
			this.CheckWaitingAccounts();
		}

		public AvatarData GetPlayerAvatar(PlayerId playerId)
		{
			ulong part = playerId.Part4;
			AvatarData avatarData;
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
			this.WaitingAccounts.Add(new ValueTuple<ulong, AvatarData>(part, avatarData));
			return avatarData;
		}

		private async void CheckWaitingAccounts()
		{
			if (this.FetchAvatarsTask == null && this.WaitingAccounts.Count > 0)
			{
				this.FetchAvatarsTask = this.FetchAvatars();
				await this.FetchAvatarsTask;
				if (this.FetchAvatarsTask.IsFaulted)
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
			}
		}

		protected abstract Task FetchAvatars();

		public void Initialize()
		{
		}

		public void ClearCache()
		{
			this.AvatarImageCache.Clear();
			this.WaitingAccounts.Clear();
		}

		public bool IsInitialized()
		{
			return true;
		}
	}
}

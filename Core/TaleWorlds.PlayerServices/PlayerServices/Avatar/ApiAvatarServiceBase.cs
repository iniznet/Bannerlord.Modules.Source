using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TaleWorlds.PlayerServices.Avatar
{
	// Token: 0x02000006 RID: 6
	public abstract class ApiAvatarServiceBase : IAvatarService
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002971 File Offset: 0x00000B71
		protected Dictionary<ulong, AvatarData> AvatarImageCache { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002979 File Offset: 0x00000B79
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002981 File Offset: 0x00000B81
		[TupleElementNames(new string[] { "accountId", "avatarData" })]
		protected List<ValueTuple<ulong, AvatarData>> WaitingAccounts
		{
			[return: TupleElementNames(new string[] { "accountId", "avatarData" })]
			get;
			[param: TupleElementNames(new string[] { "accountId", "avatarData" })]
			set;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002B RID: 43 RVA: 0x0000298A File Offset: 0x00000B8A
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002992 File Offset: 0x00000B92
		[TupleElementNames(new string[] { "accountId", "avatarData" })]
		protected List<ValueTuple<ulong, AvatarData>> InProgressAccounts
		{
			[return: TupleElementNames(new string[] { "accountId", "avatarData" })]
			get;
			[param: TupleElementNames(new string[] { "accountId", "avatarData" })]
			set;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002D RID: 45 RVA: 0x0000299B File Offset: 0x00000B9B
		// (set) Token: 0x0600002E RID: 46 RVA: 0x000029A3 File Offset: 0x00000BA3
		protected Task FetchAvatarsTask { get; set; }

		// Token: 0x0600002F RID: 47 RVA: 0x000029AC File Offset: 0x00000BAC
		protected ApiAvatarServiceBase()
		{
			this.AvatarImageCache = new Dictionary<ulong, AvatarData>();
			this.WaitingAccounts = new List<ValueTuple<ulong, AvatarData>>();
			this.InProgressAccounts = new List<ValueTuple<ulong, AvatarData>>();
			this.FetchAvatarsTask = null;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000029DC File Offset: 0x00000BDC
		public void Tick(float dt)
		{
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000029E0 File Offset: 0x00000BE0
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

		// Token: 0x06000032 RID: 50 RVA: 0x00002AB8 File Offset: 0x00000CB8
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

		// Token: 0x06000033 RID: 51
		protected abstract Task FetchAvatars();

		// Token: 0x06000034 RID: 52 RVA: 0x00002BC4 File Offset: 0x00000DC4
		public void Initialize()
		{
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002BC8 File Offset: 0x00000DC8
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

		// Token: 0x06000036 RID: 54 RVA: 0x00002C44 File Offset: 0x00000E44
		public bool IsInitialized()
		{
			return true;
		}
	}
}

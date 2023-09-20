using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TaleWorlds.PlayerServices.Avatar
{
	// Token: 0x02000007 RID: 7
	public abstract class ApiAvatarServiceBaseSingleThread : IAvatarService
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002C8D File Offset: 0x00000E8D
		protected Dictionary<ulong, AvatarData> AvatarImageCache { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002C95 File Offset: 0x00000E95
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002C9D File Offset: 0x00000E9D
		[TupleElementNames(new string[] { "accountId", "avatarData" })]
		protected List<ValueTuple<ulong, AvatarData>> WaitingAccounts
		{
			[return: TupleElementNames(new string[] { "accountId", "avatarData" })]
			get;
			[param: TupleElementNames(new string[] { "accountId", "avatarData" })]
			set;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002CA6 File Offset: 0x00000EA6
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002CAE File Offset: 0x00000EAE
		[TupleElementNames(new string[] { "accountId", "avatarData" })]
		protected List<ValueTuple<ulong, AvatarData>> InProgressAccounts
		{
			[return: TupleElementNames(new string[] { "accountId", "avatarData" })]
			get;
			[param: TupleElementNames(new string[] { "accountId", "avatarData" })]
			set;
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002CB7 File Offset: 0x00000EB7
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002CBF File Offset: 0x00000EBF
		protected Task FetchAvatarsTask { get; set; }

		// Token: 0x0600003F RID: 63 RVA: 0x00002CC8 File Offset: 0x00000EC8
		protected ApiAvatarServiceBaseSingleThread()
		{
			this.AvatarImageCache = new Dictionary<ulong, AvatarData>();
			this.WaitingAccounts = new List<ValueTuple<ulong, AvatarData>>();
			this.InProgressAccounts = new List<ValueTuple<ulong, AvatarData>>();
			this.FetchAvatarsTask = null;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002CF8 File Offset: 0x00000EF8
		public void Tick(float dt)
		{
			this.CheckWaitingAccounts();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002D00 File Offset: 0x00000F00
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

		// Token: 0x06000042 RID: 66 RVA: 0x00002D74 File Offset: 0x00000F74
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

		// Token: 0x06000043 RID: 67
		protected abstract Task FetchAvatars();

		// Token: 0x06000044 RID: 68 RVA: 0x00002DAD File Offset: 0x00000FAD
		public void Initialize()
		{
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002DAF File Offset: 0x00000FAF
		public void ClearCache()
		{
			this.AvatarImageCache.Clear();
			this.WaitingAccounts.Clear();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002DC7 File Offset: 0x00000FC7
		public bool IsInitialized()
		{
			return true;
		}
	}
}

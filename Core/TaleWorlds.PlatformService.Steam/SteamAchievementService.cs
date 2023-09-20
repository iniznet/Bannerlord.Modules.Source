using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using TaleWorlds.AchievementSystem;

namespace TaleWorlds.PlatformService.Steam
{
	public class SteamAchievementService : IAchievementService
	{
		public SteamAchievementService(SteamPlatformServices steamPlatformServices)
		{
			this._platform = steamPlatformServices;
		}

		bool IAchievementService.SetStat(string name, int value)
		{
			return this.SetStat(name, value);
		}

		Task<int> IAchievementService.GetStat(string name)
		{
			return this.GetStat(name);
		}

		Task<int[]> IAchievementService.GetStats(string[] names)
		{
			return this.GetStats(names);
		}

		bool IAchievementService.IsInitializationCompleted()
		{
			return true;
		}

		public void Tick(float dt)
		{
			this.StoreStats();
			if (this._statsInvalidatedElapsed != -1f)
			{
				this._statsInvalidatedElapsed += dt;
			}
			this._statsStoredElapsed += dt;
		}

		public void Initialize()
		{
			SteamUserStats.RequestCurrentStats();
			this._userStatsReceivedT = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.UserStatsReceived));
			this._userStatsStoredT = Callback<UserStatsStored_t>.Create(new Callback<UserStatsStored_t>.DispatchDelegate(this.UserStatsStored));
		}

		private void UserStatsReceived(UserStatsReceived_t userStatsReceivedT)
		{
			if ((ulong)SteamUtils.GetAppID().m_AppId == userStatsReceivedT.m_nGameID && userStatsReceivedT.m_eResult == EResult.k_EResultOK)
			{
				this._statsInitialized = true;
			}
		}

		private void UserStatsStored(UserStatsStored_t userStatsStoredT)
		{
		}

		internal bool SetStat(string name, int value)
		{
			if (!this._statsInitialized)
			{
				return false;
			}
			if (!SteamUserStats.SetStat(name, value))
			{
				return false;
			}
			this.InvalidateStats();
			return true;
		}

		internal Task<int> GetStat(string name)
		{
			if (!this._statsInitialized)
			{
				return Task.FromResult<int>(-1);
			}
			int num = -1;
			SteamUserStats.GetStat(name, out num);
			return Task.FromResult<int>(num);
		}

		internal Task<int[]> GetStats(string[] names)
		{
			if (!this._statsInitialized)
			{
				return Task.FromResult<int[]>(null);
			}
			List<int> list = new List<int>();
			foreach (string text in names)
			{
				int num = -1;
				SteamUserStats.GetStat(text, out num);
				list.Add(num);
			}
			return Task.FromResult<int[]>(list.ToArray());
		}

		private void InvalidateStats()
		{
			this._statsInvalidatedElapsed = 0f;
		}

		private void StoreStats()
		{
			if (this._statsInvalidatedElapsed > 5f && this._statsStoredElapsed > 60f)
			{
				this._statsStoredElapsed = 0f;
				if (SteamUserStats.StoreStats())
				{
					this._statsInvalidatedElapsed = -1f;
				}
			}
		}

		private SteamPlatformServices _platform;

		private float _statsInvalidatedElapsed = -1f;

		private float _statsStoredElapsed;

		private Callback<UserStatsReceived_t> _userStatsReceivedT;

		private Callback<UserStatsStored_t> _userStatsStoredT;

		private bool _statsInitialized;

		private const int StatInvalidationInterval = 5;

		private const int StoreStatsInterval = 60;
	}
}

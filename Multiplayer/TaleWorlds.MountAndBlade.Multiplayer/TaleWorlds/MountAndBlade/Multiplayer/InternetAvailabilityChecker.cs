using System;
using System.Threading.Tasks;

namespace TaleWorlds.MountAndBlade.Multiplayer
{
	public static class InternetAvailabilityChecker
	{
		public static bool InternetConnectionAvailable
		{
			get
			{
				return InternetAvailabilityChecker._internetConnectionAvailable;
			}
			private set
			{
				if (value != InternetAvailabilityChecker._internetConnectionAvailable)
				{
					InternetAvailabilityChecker._internetConnectionAvailable = value;
					Action<bool> onInternetConnectionAvailabilityChanged = InternetAvailabilityChecker.OnInternetConnectionAvailabilityChanged;
					if (onInternetConnectionAvailabilityChanged == null)
					{
						return;
					}
					onInternetConnectionAvailabilityChanged(value);
				}
			}
		}

		private static async void CheckInternetConnection()
		{
			if (NetworkMain.GameClient != null)
			{
				InternetAvailabilityChecker.InternetConnectionAvailable = await NetworkMain.GameClient.CheckConnection();
			}
			InternetAvailabilityChecker._lastInternetConnectionCheck = DateTime.Now.Ticks;
			InternetAvailabilityChecker._checkingConnection = false;
		}

		internal static void Tick(float dt)
		{
			long num = (InternetAvailabilityChecker.InternetConnectionAvailable ? 300000000L : 100000000L);
			if (Module.CurrentModule != null && Module.CurrentModule.StartupInfo.StartupType != 3 && !InternetAvailabilityChecker._checkingConnection && DateTime.Now.Ticks - InternetAvailabilityChecker._lastInternetConnectionCheck > num)
			{
				InternetAvailabilityChecker._checkingConnection = true;
				Task.Run(delegate
				{
					InternetAvailabilityChecker.CheckInternetConnection();
				});
			}
		}

		public static Action<bool> OnInternetConnectionAvailabilityChanged;

		private static bool _internetConnectionAvailable;

		private static long _lastInternetConnectionCheck;

		private static bool _checkingConnection;

		private const long InternetConnectionCheckIntervalShort = 100000000L;

		private const long InternetConnectionCheckIntervalLong = 300000000L;
	}
}

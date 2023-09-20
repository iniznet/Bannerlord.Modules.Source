using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.PlatformService
{
	// Token: 0x02000011 RID: 17
	public class PlatformServices
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000079 RID: 121 RVA: 0x000023D1 File Offset: 0x000005D1
		public static IPlatformServices Instance
		{
			get
			{
				return PlatformServices._platformServices;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000023D8 File Offset: 0x000005D8
		public static IPlatformInvitationServices InvitationServices
		{
			get
			{
				return PlatformServices._platformServices as IPlatformInvitationServices;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600007B RID: 123 RVA: 0x000023E4 File Offset: 0x000005E4
		// (set) Token: 0x0600007C RID: 124 RVA: 0x000023EB File Offset: 0x000005EB
		public static Action<SessionInvitationType> OnSessionInvitationAccepted { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600007D RID: 125 RVA: 0x000023F3 File Offset: 0x000005F3
		// (set) Token: 0x0600007E RID: 126 RVA: 0x000023FA File Offset: 0x000005FA
		public static Action OnPlatformRequestedMultiplayer { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00002402 File Offset: 0x00000602
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00002409 File Offset: 0x00000609
		public static bool IsPlatformRequestedMultiplayer { get; private set; } = false;

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00002411 File Offset: 0x00000611
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00002418 File Offset: 0x00000618
		public static SessionInvitationType SessionInvitationType { get; private set; } = SessionInvitationType.None;

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00002420 File Offset: 0x00000620
		// (set) Token: 0x06000084 RID: 132 RVA: 0x00002427 File Offset: 0x00000627
		public static bool IsPlatformRequestedContinueGame { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000085 RID: 133 RVA: 0x0000242F File Offset: 0x0000062F
		public static string ProviderName
		{
			get
			{
				return PlatformServices._platformServices.ProviderName;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000086 RID: 134 RVA: 0x0000243B File Offset: 0x0000063B
		public static string UserId
		{
			get
			{
				return PlatformServices._platformServices.UserId;
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000245F File Offset: 0x0000065F
		public static void Setup(IPlatformServices platformServices)
		{
			PlatformServices._platformServices = platformServices;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00002467 File Offset: 0x00000667
		public static bool Initialize(IFriendListService[] additionalFriendListServices)
		{
			return PlatformServices._platformServices.Initialize(additionalFriendListServices);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00002474 File Offset: 0x00000674
		public static void Terminate()
		{
			PlatformServices._platformServices.Terminate();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00002480 File Offset: 0x00000680
		public static void ConnectionStateChanged(bool isAuthenticated)
		{
			Action<bool> onConnectionStateChanged = PlatformServices.OnConnectionStateChanged;
			if (onConnectionStateChanged == null)
			{
				return;
			}
			onConnectionStateChanged(isAuthenticated);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00002492 File Offset: 0x00000692
		public static void MultiplayerGameStateChanged(bool isPlaying)
		{
			Action<bool> onMultiplayerGameStateChanged = PlatformServices.OnMultiplayerGameStateChanged;
			if (onMultiplayerGameStateChanged == null)
			{
				return;
			}
			onMultiplayerGameStateChanged(isPlaying);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000024A4 File Offset: 0x000006A4
		public static void LobbyClientStateChanged(bool atLobby, bool isPartyLeaderOrSolo)
		{
			Action<bool, bool> onLobbyClientStateChanged = PlatformServices.OnLobbyClientStateChanged;
			if (onLobbyClientStateChanged == null)
			{
				return;
			}
			onLobbyClientStateChanged(atLobby, isPartyLeaderOrSolo);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000024B8 File Offset: 0x000006B8
		public static void FireOnSessionInvitationAccepted(SessionInvitationType sessionInvitationType)
		{
			PlatformServices.SessionInvitationType = sessionInvitationType;
			if (PlatformServices.OnSessionInvitationAccepted != null)
			{
				Delegate[] invocationList = PlatformServices.OnSessionInvitationAccepted.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action<SessionInvitationType> action;
					if ((action = invocationList[i] as Action<SessionInvitationType>) != null)
					{
						action(sessionInvitationType);
					}
				}
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00002500 File Offset: 0x00000700
		public static void FireOnPlatformRequestedMultiplayer()
		{
			PlatformServices.IsPlatformRequestedMultiplayer = true;
			if (PlatformServices.OnPlatformRequestedMultiplayer != null)
			{
				Delegate[] invocationList = PlatformServices.OnPlatformRequestedMultiplayer.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action action;
					if ((action = invocationList[i] as Action) != null)
					{
						action();
					}
				}
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00002545 File Offset: 0x00000745
		public static void OnSessionInvitationHandled()
		{
			PlatformServices.SessionInvitationType = SessionInvitationType.None;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000254D File Offset: 0x0000074D
		public static void OnPlatformMultiplayerRequestHandled()
		{
			PlatformServices.IsPlatformRequestedMultiplayer = false;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00002555 File Offset: 0x00000755
		public static void SetIsPlatformRequestedContinueGame(bool isRequested)
		{
			PlatformServices.IsPlatformRequestedContinueGame = true;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00002560 File Offset: 0x00000760
		public static async Task<string> FilterString(string content, string defaultContent)
		{
			TaskAwaiter<bool> taskAwaiter = PlatformServices.Instance.VerifyString(content).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			string text;
			if (!taskAwaiter.GetResult())
			{
				text = defaultContent;
			}
			else
			{
				text = content;
			}
			return text;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000025B0 File Offset: 0x000007B0
		[CommandLineFunctionality.CommandLineArgumentFunction("trigger_invitation", "platform_services")]
		public static string TriggerInvitation(List<string> strings)
		{
			SessionInvitationType sessionInvitationType;
			if (strings.Count == 0 || !Enum.TryParse<SessionInvitationType>(strings[0], out sessionInvitationType))
			{
				sessionInvitationType = SessionInvitationType.Multiplayer;
			}
			PlatformServices.FireOnSessionInvitationAccepted(sessionInvitationType);
			return "Triggered invitation with " + sessionInvitationType;
		}

		// Token: 0x04000026 RID: 38
		private static IPlatformServices _platformServices = new NullPlatformServices();

		// Token: 0x04000027 RID: 39
		public static Action<bool> OnConnectionStateChanged;

		// Token: 0x04000028 RID: 40
		public static Action<bool> OnMultiplayerGameStateChanged;

		// Token: 0x04000029 RID: 41
		public static Action<bool, bool> OnLobbyClientStateChanged;
	}
}

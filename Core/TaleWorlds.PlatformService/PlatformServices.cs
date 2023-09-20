using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.PlatformService
{
	public class PlatformServices
	{
		public static IPlatformServices Instance
		{
			get
			{
				return PlatformServices._platformServices;
			}
		}

		public static IPlatformInvitationServices InvitationServices
		{
			get
			{
				return PlatformServices._platformServices as IPlatformInvitationServices;
			}
		}

		public static Action<SessionInvitationType> OnSessionInvitationAccepted { get; set; }

		public static Action OnPlatformRequestedMultiplayer { get; set; }

		public static bool IsPlatformRequestedMultiplayer { get; private set; } = false;

		public static SessionInvitationType SessionInvitationType { get; private set; } = SessionInvitationType.None;

		public static bool IsPlatformRequestedContinueGame { get; private set; }

		public static string ProviderName
		{
			get
			{
				return PlatformServices._platformServices.ProviderName;
			}
		}

		public static string UserId
		{
			get
			{
				return PlatformServices._platformServices.UserId;
			}
		}

		public static void Setup(IPlatformServices platformServices)
		{
			PlatformServices._platformServices = platformServices;
		}

		public static bool Initialize(IFriendListService[] additionalFriendListServices)
		{
			return PlatformServices._platformServices.Initialize(additionalFriendListServices);
		}

		public static void Terminate()
		{
			PlatformServices._platformServices.Terminate();
		}

		public static void ConnectionStateChanged(bool isAuthenticated)
		{
			Action<bool> onConnectionStateChanged = PlatformServices.OnConnectionStateChanged;
			if (onConnectionStateChanged == null)
			{
				return;
			}
			onConnectionStateChanged(isAuthenticated);
		}

		public static void MultiplayerGameStateChanged(bool isPlaying)
		{
			Action<bool> onMultiplayerGameStateChanged = PlatformServices.OnMultiplayerGameStateChanged;
			if (onMultiplayerGameStateChanged == null)
			{
				return;
			}
			onMultiplayerGameStateChanged(isPlaying);
		}

		public static void LobbyClientStateChanged(bool atLobby, bool isPartyLeaderOrSolo)
		{
			Action<bool, bool> onLobbyClientStateChanged = PlatformServices.OnLobbyClientStateChanged;
			if (onLobbyClientStateChanged == null)
			{
				return;
			}
			onLobbyClientStateChanged(atLobby, isPartyLeaderOrSolo);
		}

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

		public static void OnSessionInvitationHandled()
		{
			PlatformServices.SessionInvitationType = SessionInvitationType.None;
		}

		public static void OnPlatformMultiplayerRequestHandled()
		{
			PlatformServices.IsPlatformRequestedMultiplayer = false;
		}

		public static void SetIsPlatformRequestedContinueGame(bool isRequested)
		{
			PlatformServices.IsPlatformRequestedContinueGame = true;
		}

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

		private static IPlatformServices _platformServices = new NullPlatformServices();

		public static Action<bool> OnConnectionStateChanged;

		public static Action<bool> OnMultiplayerGameStateChanged;

		public static Action<bool, bool> OnLobbyClientStateChanged;
	}
}

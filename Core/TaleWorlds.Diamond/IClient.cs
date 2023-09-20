using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	public interface IClient
	{
		bool IsInCriticalState { get; }

		long AliveCheckTimeInMiliSeconds { get; }

		void HandleMessage(Message message);

		void OnConnected();

		void OnCantConnect();

		void OnDisconnected();

		Task<bool> CheckConnection();

		ILoginAccessProvider AccessProvider { get; }
	}
}

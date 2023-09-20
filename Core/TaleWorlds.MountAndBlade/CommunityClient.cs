using System;
using TaleWorlds.Library.Http;

namespace TaleWorlds.MountAndBlade
{
	public class CommunityClient
	{
		public bool IsInGame { get; private set; }

		public ICommunityClientHandler Handler { get; set; }

		public CommunityClient()
		{
			this._httpDriver = HttpDriverManager.GetDefaultHttpDriver();
		}

		public void QuitFromGame()
		{
			if (this.IsInGame)
			{
				this.IsInGame = false;
				ICommunityClientHandler handler = this.Handler;
				if (handler == null)
				{
					return;
				}
				handler.OnQuitFromGame();
			}
		}

		private IHttpDriver _httpDriver;
	}
}

using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class BannerEditorState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public IBannerEditorStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		public Clan GetClan()
		{
			return Clan.PlayerClan;
		}

		public CharacterObject GetCharacter()
		{
			return CharacterObject.PlayerCharacter;
		}

		private IBannerEditorStateHandler _handler;
	}
}

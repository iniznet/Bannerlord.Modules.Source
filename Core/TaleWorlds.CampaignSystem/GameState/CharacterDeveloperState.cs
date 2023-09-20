using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class CharacterDeveloperState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public Hero InitialSelectedHero { get; private set; }

		public CharacterDeveloperState()
		{
		}

		public CharacterDeveloperState(Hero initialSelectedHero)
		{
			this.InitialSelectedHero = initialSelectedHero;
		}

		public ICharacterDeveloperStateHandler Handler
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

		private ICharacterDeveloperStateHandler _handler;
	}
}

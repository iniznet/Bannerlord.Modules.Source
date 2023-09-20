using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class BarberState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public IFaceGeneratorCustomFilter Filter { get; private set; }

		public BarberState()
		{
		}

		public BarberState(BasicCharacterObject character, IFaceGeneratorCustomFilter filter)
		{
			this.Character = character;
			this.Filter = filter;
		}

		public BasicCharacterObject Character;
	}
}

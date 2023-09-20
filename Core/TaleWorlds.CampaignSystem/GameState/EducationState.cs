﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class EducationState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public Hero Child { get; private set; }

		public IEducationStateHandler Handler
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

		public EducationState()
		{
			Debug.FailedAssert("Do not use EducationState with default constructor!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\GameState\\EducationState.cs", ".ctor", 22);
		}

		public EducationState(Hero child)
		{
			this.Child = child;
		}

		private IEducationStateHandler _handler;
	}
}

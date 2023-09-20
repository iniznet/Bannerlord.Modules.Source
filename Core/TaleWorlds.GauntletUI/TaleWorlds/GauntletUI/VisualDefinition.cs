using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI
{
	public class VisualDefinition
	{
		public string Name { get; private set; }

		public float TransitionDuration { get; private set; }

		public float DelayOnBegin { get; private set; }

		public bool EaseIn { get; private set; }

		public Dictionary<string, VisualState> VisualStates { get; private set; }

		public VisualDefinition(string name, float transitionDuration, float delayOnBegin, bool easeIn)
		{
			this.Name = name;
			this.TransitionDuration = transitionDuration;
			this.DelayOnBegin = delayOnBegin;
			this.EaseIn = easeIn;
			this.VisualStates = new Dictionary<string, VisualState>();
		}

		public void AddVisualState(VisualState visualState)
		{
			this.VisualStates.Add(visualState.State, visualState);
		}

		public VisualState GetVisualState(string state)
		{
			if (this.VisualStates.ContainsKey(state))
			{
				return this.VisualStates[state];
			}
			return null;
		}
	}
}

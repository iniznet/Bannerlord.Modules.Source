﻿using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public struct TacticalDecision
	{
		public TacticComponent DecidingComponent { get; private set; }

		public byte DecisionCode { get; private set; }

		public Formation SubjectFormation { get; private set; }

		public Formation TargetFormation { get; private set; }

		public WorldPosition? TargetPosition { get; private set; }

		public MissionObject TargetObject { get; private set; }

		public TacticalDecision(TacticComponent decidingComponent, byte decisionCode, Formation subjectFormation = null, Formation targetFormation = null, WorldPosition? targetPosition = null, MissionObject targetObject = null)
		{
			this.DecidingComponent = decidingComponent;
			this.DecisionCode = decisionCode;
			this.SubjectFormation = subjectFormation;
			this.TargetFormation = targetFormation;
			this.TargetPosition = targetPosition;
			this.TargetObject = targetObject;
		}
	}
}

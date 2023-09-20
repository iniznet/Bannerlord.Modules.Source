using System;

namespace TaleWorlds.Library
{
	public class EngineMethod : Attribute
	{
		public EngineMethod(string engineMethodName, bool activateTelemetryProfiling = false)
		{
			this.EngineMethodName = engineMethodName;
			this.ActivateTelemetryProfiling = activateTelemetryProfiling;
		}

		public string EngineMethodName { get; private set; }

		public bool ActivateTelemetryProfiling { get; private set; }
	}
}

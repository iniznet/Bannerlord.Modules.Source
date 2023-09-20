using System;
using TaleWorlds.Library.EventSystem;

namespace SandBox.View.Map
{
	public class MainMapCameraMoveEvent : EventBase
	{
		public bool RotationChanged { get; private set; }

		public bool PositionChanged { get; private set; }

		public MainMapCameraMoveEvent(bool rotationChanged, bool positionChanged)
		{
			this.RotationChanged = rotationChanged;
			this.PositionChanged = positionChanged;
		}
	}
}

using System;

namespace TaleWorlds.MountAndBlade
{
	public class CompassMarker
	{
		public string Id { get; private set; }

		public float Angle { get; private set; }

		public bool IsPrimary { get; private set; }

		public CompassMarker(string id, float angle, bool isPrimary)
		{
			this.Id = id;
			this.Angle = angle % 360f;
			this.IsPrimary = isPrimary;
		}
	}
}

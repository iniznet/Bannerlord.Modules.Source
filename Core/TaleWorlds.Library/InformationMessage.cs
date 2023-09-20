using System;

namespace TaleWorlds.Library
{
	public class InformationMessage
	{
		public string Information { get; set; }

		public string Detail { get; set; }

		public Color Color { get; set; }

		public string SoundEventPath { get; set; }

		public string Category { get; set; }

		public InformationMessage(string information)
		{
			this.Information = information;
			this.Color = Color.White;
		}

		public InformationMessage(string information, Color color)
		{
			this.Information = information;
			this.Color = color;
		}

		public InformationMessage(string information, Color color, string category)
		{
			this.Information = information;
			this.Color = color;
			this.Category = category;
		}

		public InformationMessage(string information, string soundEventPath)
		{
			this.Information = information;
			this.SoundEventPath = soundEventPath;
			this.Color = Color.White;
		}

		public InformationMessage()
		{
			this.Information = "";
		}
	}
}

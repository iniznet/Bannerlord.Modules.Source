using System;
using System.Collections.Generic;

namespace TaleWorlds.Engine
{
	public class Highlights
	{
		public static void Initialize()
		{
			EngineApplicationInterface.IHighlights.Initialize();
		}

		public static void OpenGroup(string id)
		{
			EngineApplicationInterface.IHighlights.OpenGroup(id);
		}

		public static void CloseGroup(string id, bool destroy = false)
		{
			EngineApplicationInterface.IHighlights.CloseGroup(id, destroy);
		}

		public static void SaveScreenshot(string highlightId, string groupId)
		{
			EngineApplicationInterface.IHighlights.SaveScreenshot(highlightId, groupId);
		}

		public static void SaveVideo(string highlightId, string groupId, int startDelta, int endDelta)
		{
			EngineApplicationInterface.IHighlights.SaveVideo(highlightId, groupId, startDelta, endDelta);
		}

		public static void OpenSummary(List<string> groups)
		{
			string text = string.Join("::", groups);
			EngineApplicationInterface.IHighlights.OpenSummary(text);
		}

		public static void AddHighlight(string id, string name)
		{
			EngineApplicationInterface.IHighlights.AddHighlight(id, name);
		}

		public static void RemoveHighlight(string id)
		{
			EngineApplicationInterface.IHighlights.RemoveHighlight(id);
		}

		public enum Significance
		{
			None,
			ExtremelyBad,
			VeryBad,
			Bad = 4,
			Neutral = 16,
			Good = 256,
			VeryGood = 512,
			ExtremelyGoods = 1024,
			Max = 2048
		}

		public enum Type
		{
			None,
			Milestone,
			Achievement,
			Incident = 4,
			StateChange = 8,
			Unannounced = 16,
			Max = 32
		}
	}
}

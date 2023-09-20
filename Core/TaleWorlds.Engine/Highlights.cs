using System;
using System.Collections.Generic;

namespace TaleWorlds.Engine
{
	// Token: 0x0200004B RID: 75
	public class Highlights
	{
		// Token: 0x060006A2 RID: 1698 RVA: 0x00004C9D File Offset: 0x00002E9D
		public static void Initialize()
		{
			EngineApplicationInterface.IHighlights.Initialize();
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x00004CA9 File Offset: 0x00002EA9
		public static void OpenGroup(string id)
		{
			EngineApplicationInterface.IHighlights.OpenGroup(id);
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x00004CB6 File Offset: 0x00002EB6
		public static void CloseGroup(string id, bool destroy = false)
		{
			EngineApplicationInterface.IHighlights.CloseGroup(id, destroy);
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x00004CC4 File Offset: 0x00002EC4
		public static void SaveScreenshot(string highlightId, string groupId)
		{
			EngineApplicationInterface.IHighlights.SaveScreenshot(highlightId, groupId);
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x00004CD2 File Offset: 0x00002ED2
		public static void SaveVideo(string highlightId, string groupId, int startDelta, int endDelta)
		{
			EngineApplicationInterface.IHighlights.SaveVideo(highlightId, groupId, startDelta, endDelta);
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x00004CE4 File Offset: 0x00002EE4
		public static void OpenSummary(List<string> groups)
		{
			string text = string.Join("::", groups);
			EngineApplicationInterface.IHighlights.OpenSummary(text);
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x00004D08 File Offset: 0x00002F08
		public static void AddHighlight(string id, string name)
		{
			EngineApplicationInterface.IHighlights.AddHighlight(id, name);
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x00004D16 File Offset: 0x00002F16
		public static void RemoveHighlight(string id)
		{
			EngineApplicationInterface.IHighlights.RemoveHighlight(id);
		}

		// Token: 0x020000B4 RID: 180
		public enum Significance
		{
			// Token: 0x04000376 RID: 886
			None,
			// Token: 0x04000377 RID: 887
			ExtremelyBad,
			// Token: 0x04000378 RID: 888
			VeryBad,
			// Token: 0x04000379 RID: 889
			Bad = 4,
			// Token: 0x0400037A RID: 890
			Neutral = 16,
			// Token: 0x0400037B RID: 891
			Good = 256,
			// Token: 0x0400037C RID: 892
			VeryGood = 512,
			// Token: 0x0400037D RID: 893
			ExtremelyGoods = 1024,
			// Token: 0x0400037E RID: 894
			Max = 2048
		}

		// Token: 0x020000B5 RID: 181
		public enum Type
		{
			// Token: 0x04000380 RID: 896
			None,
			// Token: 0x04000381 RID: 897
			Milestone,
			// Token: 0x04000382 RID: 898
			Achievement,
			// Token: 0x04000383 RID: 899
			Incident = 4,
			// Token: 0x04000384 RID: 900
			StateChange = 8,
			// Token: 0x04000385 RID: 901
			Unannounced = 16,
			// Token: 0x04000386 RID: 902
			Max = 32
		}
	}
}

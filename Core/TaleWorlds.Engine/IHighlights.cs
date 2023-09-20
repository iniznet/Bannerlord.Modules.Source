using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000035 RID: 53
	[ApplicationInterfaceBase]
	internal interface IHighlights
	{
		// Token: 0x06000473 RID: 1139
		[EngineMethod("initialize", false)]
		void Initialize();

		// Token: 0x06000474 RID: 1140
		[EngineMethod("open_group", false)]
		void OpenGroup(string id);

		// Token: 0x06000475 RID: 1141
		[EngineMethod("close_group", false)]
		void CloseGroup(string id, bool destroy = false);

		// Token: 0x06000476 RID: 1142
		[EngineMethod("save_screenshot", false)]
		void SaveScreenshot(string highlightId, string groupId);

		// Token: 0x06000477 RID: 1143
		[EngineMethod("save_video", false)]
		void SaveVideo(string highlightId, string groupId, int startDelta, int endDelta);

		// Token: 0x06000478 RID: 1144
		[EngineMethod("open_summary", false)]
		void OpenSummary(string groups);

		// Token: 0x06000479 RID: 1145
		[EngineMethod("add_highlight", false)]
		void AddHighlight(string id, string name);

		// Token: 0x0600047A RID: 1146
		[EngineMethod("remove_highlight", false)]
		void RemoveHighlight(string id);
	}
}

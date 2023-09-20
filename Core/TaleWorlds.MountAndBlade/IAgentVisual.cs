using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B5 RID: 693
	public interface IAgentVisual
	{
		// Token: 0x06002690 RID: 9872
		void SetAction(ActionIndexCache actionName, float startProgress = 0f, bool forceFaceMorphRestart = true);

		// Token: 0x06002691 RID: 9873
		MBAgentVisuals GetVisuals();

		// Token: 0x06002692 RID: 9874
		MatrixFrame GetFrame();

		// Token: 0x06002693 RID: 9875
		BodyProperties GetBodyProperties();

		// Token: 0x06002694 RID: 9876
		void SetBodyProperties(BodyProperties bodyProperties);

		// Token: 0x06002695 RID: 9877
		bool GetIsFemale();

		// Token: 0x06002696 RID: 9878
		string GetCharacterObjectID();

		// Token: 0x06002697 RID: 9879
		void SetCharacterObjectID(string id);

		// Token: 0x06002698 RID: 9880
		Equipment GetEquipment();

		// Token: 0x06002699 RID: 9881
		void SetClothingColors(uint color1, uint color2);

		// Token: 0x0600269A RID: 9882
		void GetClothingColors(out uint color1, out uint color2);

		// Token: 0x0600269B RID: 9883
		AgentVisualsData GetCopyAgentVisualsData();

		// Token: 0x0600269C RID: 9884
		void Refresh(bool needBatchedVersionForWeaponMeshes, AgentVisualsData data, bool forceUseFaceCache = false);
	}
}

using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B6 RID: 694
	public class AgentVisualHolder : IAgentVisual
	{
		// Token: 0x0600269D RID: 9885 RVA: 0x000910E3 File Offset: 0x0008F2E3
		public AgentVisualHolder(MatrixFrame frame, Equipment equipment, string name, BodyProperties bodyProperties)
		{
			this.SetFrame(ref frame);
			this._equipment = equipment;
			this._characterObjectStringID = name;
			this._bodyProperties = bodyProperties;
		}

		// Token: 0x0600269E RID: 9886 RVA: 0x00091109 File Offset: 0x0008F309
		public void SetAction(ActionIndexCache actionName, float startProgress = 0f, bool forceFaceMorphRestart = true)
		{
		}

		// Token: 0x0600269F RID: 9887 RVA: 0x0009110B File Offset: 0x0008F30B
		public GameEntity GetEntity()
		{
			return null;
		}

		// Token: 0x060026A0 RID: 9888 RVA: 0x0009110E File Offset: 0x0008F30E
		public MBAgentVisuals GetVisuals()
		{
			return null;
		}

		// Token: 0x060026A1 RID: 9889 RVA: 0x00091111 File Offset: 0x0008F311
		public void SetFrame(ref MatrixFrame frame)
		{
			this._frame = frame;
		}

		// Token: 0x060026A2 RID: 9890 RVA: 0x0009111F File Offset: 0x0008F31F
		public MatrixFrame GetFrame()
		{
			return this._frame;
		}

		// Token: 0x060026A3 RID: 9891 RVA: 0x00091127 File Offset: 0x0008F327
		public BodyProperties GetBodyProperties()
		{
			return this._bodyProperties;
		}

		// Token: 0x060026A4 RID: 9892 RVA: 0x0009112F File Offset: 0x0008F32F
		public void SetBodyProperties(BodyProperties bodyProperties)
		{
			this._bodyProperties = bodyProperties;
		}

		// Token: 0x060026A5 RID: 9893 RVA: 0x00091138 File Offset: 0x0008F338
		public bool GetIsFemale()
		{
			return false;
		}

		// Token: 0x060026A6 RID: 9894 RVA: 0x0009113B File Offset: 0x0008F33B
		public string GetCharacterObjectID()
		{
			return this._characterObjectStringID;
		}

		// Token: 0x060026A7 RID: 9895 RVA: 0x00091143 File Offset: 0x0008F343
		public void SetCharacterObjectID(string id)
		{
			this._characterObjectStringID = id;
		}

		// Token: 0x060026A8 RID: 9896 RVA: 0x0009114C File Offset: 0x0008F34C
		public Equipment GetEquipment()
		{
			return this._equipment;
		}

		// Token: 0x060026A9 RID: 9897 RVA: 0x00091154 File Offset: 0x0008F354
		public void RefreshWithNewEquipment(Equipment equipment)
		{
			this._equipment = equipment;
		}

		// Token: 0x060026AA RID: 9898 RVA: 0x0009115D File Offset: 0x0008F35D
		public void SetClothingColors(uint color1, uint color2)
		{
		}

		// Token: 0x060026AB RID: 9899 RVA: 0x0009115F File Offset: 0x0008F35F
		public void GetClothingColors(out uint color1, out uint color2)
		{
			color1 = uint.MaxValue;
			color2 = uint.MaxValue;
		}

		// Token: 0x060026AC RID: 9900 RVA: 0x00091167 File Offset: 0x0008F367
		public AgentVisualsData GetCopyAgentVisualsData()
		{
			return null;
		}

		// Token: 0x060026AD RID: 9901 RVA: 0x0009116A File Offset: 0x0008F36A
		public void Refresh(bool needBatchedVersionForWeaponMeshes, AgentVisualsData data, bool forceUseFaceCache = false)
		{
		}

		// Token: 0x04000E4F RID: 3663
		private MatrixFrame _frame;

		// Token: 0x04000E50 RID: 3664
		private Equipment _equipment;

		// Token: 0x04000E51 RID: 3665
		private string _characterObjectStringID;

		// Token: 0x04000E52 RID: 3666
		private BodyProperties _bodyProperties;
	}
}

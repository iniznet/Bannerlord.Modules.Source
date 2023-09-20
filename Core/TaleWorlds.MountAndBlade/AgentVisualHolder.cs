using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class AgentVisualHolder : IAgentVisual
	{
		public AgentVisualHolder(MatrixFrame frame, Equipment equipment, string name, BodyProperties bodyProperties)
		{
			this.SetFrame(ref frame);
			this._equipment = equipment;
			this._characterObjectStringID = name;
			this._bodyProperties = bodyProperties;
		}

		public void SetAction(ActionIndexCache actionName, float startProgress = 0f, bool forceFaceMorphRestart = true)
		{
		}

		public GameEntity GetEntity()
		{
			return null;
		}

		public MBAgentVisuals GetVisuals()
		{
			return null;
		}

		public void SetFrame(ref MatrixFrame frame)
		{
			this._frame = frame;
		}

		public MatrixFrame GetFrame()
		{
			return this._frame;
		}

		public BodyProperties GetBodyProperties()
		{
			return this._bodyProperties;
		}

		public void SetBodyProperties(BodyProperties bodyProperties)
		{
			this._bodyProperties = bodyProperties;
		}

		public bool GetIsFemale()
		{
			return false;
		}

		public string GetCharacterObjectID()
		{
			return this._characterObjectStringID;
		}

		public void SetCharacterObjectID(string id)
		{
			this._characterObjectStringID = id;
		}

		public Equipment GetEquipment()
		{
			return this._equipment;
		}

		public void RefreshWithNewEquipment(Equipment equipment)
		{
			this._equipment = equipment;
		}

		public void SetClothingColors(uint color1, uint color2)
		{
		}

		public void GetClothingColors(out uint color1, out uint color2)
		{
			color1 = uint.MaxValue;
			color2 = uint.MaxValue;
		}

		public AgentVisualsData GetCopyAgentVisualsData()
		{
			return null;
		}

		public void Refresh(bool needBatchedVersionForWeaponMeshes, AgentVisualsData data, bool forceUseFaceCache = false)
		{
		}

		private MatrixFrame _frame;

		private Equipment _equipment;

		private string _characterObjectStringID;

		private BodyProperties _bodyProperties;
	}
}

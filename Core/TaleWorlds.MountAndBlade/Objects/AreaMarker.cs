using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Objects
{
	public class AreaMarker : MissionObject, ITrackableBase
	{
		public virtual string Tag
		{
			get
			{
				return "area_marker_" + this.AreaIndex;
			}
		}

		protected internal override void OnEditorTick(float dt)
		{
			if (this.CheckToggle)
			{
				MBEditor.HelpersEnabled();
			}
		}

		protected internal override void OnEditorInit()
		{
			this.CheckToggle = false;
		}

		public bool IsPositionInRange(Vec3 position)
		{
			return position.DistanceSquared(base.GameEntity.GlobalPosition) <= this.AreaRadius * this.AreaRadius;
		}

		public virtual List<UsableMachine> GetUsableMachinesInRange(string excludeTag = null)
		{
			float distanceSquared = this.AreaRadius * this.AreaRadius;
			return (from x in Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>()
				where !x.IsDeactivated && x.GameEntity.GlobalPosition.DistanceSquared(this.GameEntity.GlobalPosition) <= distanceSquared && !x.GameEntity.HasTag(excludeTag)
				select x).ToList<UsableMachine>();
		}

		public virtual List<UsableMachine> GetUsableMachinesWithTagInRange(string tag)
		{
			float distanceSquared = this.AreaRadius * this.AreaRadius;
			return (from x in Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>()
				where x.GameEntity.GlobalPosition.DistanceSquared(this.GameEntity.GlobalPosition) <= distanceSquared && x.GameEntity.HasTag(tag)
				select x).ToList<UsableMachine>();
		}

		public virtual List<GameEntity> GetGameEntitiesWithTagInRange(string tag)
		{
			float distanceSquared = this.AreaRadius * this.AreaRadius;
			return (from x in Mission.Current.Scene.FindEntitiesWithTag(tag)
				where x.GlobalPosition.DistanceSquared(this.GameEntity.GlobalPosition) <= distanceSquared && x.HasTag(tag)
				select x).ToList<GameEntity>();
		}

		public virtual TextObject GetName()
		{
			return new TextObject(base.GameEntity.Name, null);
		}

		public virtual Vec3 GetPosition()
		{
			return base.GameEntity.GlobalPosition;
		}

		public float GetTrackDistanceToMainAgent()
		{
			if (Agent.Main == null)
			{
				return -1f;
			}
			return this.GetPosition().Distance(Agent.Main.Position);
		}

		public bool CheckTracked(BasicCharacterObject basicCharacter)
		{
			return false;
		}

		public float AreaRadius = 3f;

		public int AreaIndex;

		public bool CheckToggle;
	}
}

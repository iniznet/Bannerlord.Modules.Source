using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Objects
{
	// Token: 0x0200039F RID: 927
	public class AreaMarker : MissionObject, ITrackableBase
	{
		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x06003296 RID: 12950 RVA: 0x000D1644 File Offset: 0x000CF844
		public virtual string Tag
		{
			get
			{
				return "area_marker_" + this.AreaIndex;
			}
		}

		// Token: 0x06003297 RID: 12951 RVA: 0x000D165B File Offset: 0x000CF85B
		protected internal override void OnEditorTick(float dt)
		{
			if (this.CheckToggle)
			{
				MBEditor.HelpersEnabled();
			}
		}

		// Token: 0x06003298 RID: 12952 RVA: 0x000D166B File Offset: 0x000CF86B
		protected internal override void OnEditorInit()
		{
			this.CheckToggle = false;
		}

		// Token: 0x06003299 RID: 12953 RVA: 0x000D1674 File Offset: 0x000CF874
		public bool IsPositionInRange(Vec3 position)
		{
			return position.DistanceSquared(base.GameEntity.GlobalPosition) <= this.AreaRadius * this.AreaRadius;
		}

		// Token: 0x0600329A RID: 12954 RVA: 0x000D169C File Offset: 0x000CF89C
		public virtual List<UsableMachine> GetUsableMachinesInRange(string excludeTag = null)
		{
			float distanceSquared = this.AreaRadius * this.AreaRadius;
			return (from x in Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>()
				where !x.IsDeactivated && x.GameEntity.GlobalPosition.DistanceSquared(this.GameEntity.GlobalPosition) <= distanceSquared && !x.GameEntity.HasTag(excludeTag)
				select x).ToList<UsableMachine>();
		}

		// Token: 0x0600329B RID: 12955 RVA: 0x000D16F8 File Offset: 0x000CF8F8
		public virtual List<UsableMachine> GetUsableMachinesWithTagInRange(string tag)
		{
			float distanceSquared = this.AreaRadius * this.AreaRadius;
			return (from x in Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>()
				where x.GameEntity.GlobalPosition.DistanceSquared(this.GameEntity.GlobalPosition) <= distanceSquared && x.GameEntity.HasTag(tag)
				select x).ToList<UsableMachine>();
		}

		// Token: 0x0600329C RID: 12956 RVA: 0x000D1754 File Offset: 0x000CF954
		public virtual List<GameEntity> GetGameEntitiesWithTagInRange(string tag)
		{
			float distanceSquared = this.AreaRadius * this.AreaRadius;
			return (from x in Mission.Current.Scene.FindEntitiesWithTag(tag)
				where x.GlobalPosition.DistanceSquared(this.GameEntity.GlobalPosition) <= distanceSquared && x.HasTag(tag)
				select x).ToList<GameEntity>();
		}

		// Token: 0x0600329D RID: 12957 RVA: 0x000D17B3 File Offset: 0x000CF9B3
		public virtual TextObject GetName()
		{
			return new TextObject(base.GameEntity.Name, null);
		}

		// Token: 0x0600329E RID: 12958 RVA: 0x000D17C6 File Offset: 0x000CF9C6
		public virtual Vec3 GetPosition()
		{
			return base.GameEntity.GlobalPosition;
		}

		// Token: 0x0600329F RID: 12959 RVA: 0x000D17D4 File Offset: 0x000CF9D4
		public float GetTrackDistanceToMainAgent()
		{
			if (Agent.Main == null)
			{
				return -1f;
			}
			return this.GetPosition().Distance(Agent.Main.Position);
		}

		// Token: 0x060032A0 RID: 12960 RVA: 0x000D1806 File Offset: 0x000CFA06
		public bool CheckTracked(BasicCharacterObject basicCharacter)
		{
			return false;
		}

		// Token: 0x04001553 RID: 5459
		public float AreaRadius = 3f;

		// Token: 0x04001554 RID: 5460
		public int AreaIndex;

		// Token: 0x04001555 RID: 5461
		public bool CheckToggle;
	}
}

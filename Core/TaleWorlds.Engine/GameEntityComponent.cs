using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000048 RID: 72
	[EngineClass("rglEntity_component")]
	public abstract class GameEntityComponent : NativeObject
	{
		// Token: 0x06000679 RID: 1657 RVA: 0x00004885 File Offset: 0x00002A85
		internal GameEntityComponent(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00004894 File Offset: 0x00002A94
		public GameEntity GetEntity()
		{
			return EngineApplicationInterface.IGameEntityComponent.GetEntity(this);
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x000048A1 File Offset: 0x00002AA1
		public virtual MetaMesh GetFirstMetaMesh()
		{
			return EngineApplicationInterface.IGameEntityComponent.GetFirstMetaMesh(this);
		}
	}
}

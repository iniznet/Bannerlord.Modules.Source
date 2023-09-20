using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	public class CampaignEntityComponent : IEntityComponent
	{
		void IEntityComponent.OnInitialize()
		{
			this.OnInitialize();
		}

		void IEntityComponent.OnFinalize()
		{
			this.OnFinalize();
		}

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnFinalize()
		{
		}

		public virtual void OnTick(float realDt, float dt)
		{
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class BannerComponent : WeaponComponent
	{
		public int BannerLevel { get; private set; }

		public BannerEffect BannerEffect { get; private set; }

		public BannerComponent(ItemObject item)
			: base(item)
		{
		}

		public override ItemComponent GetCopy()
		{
			return new BannerComponent(this.Item)
			{
				BannerLevel = this.BannerLevel,
				BannerEffect = this.BannerEffect
			};
		}

		public float GetBannerEffectBonus()
		{
			return this.BannerEffect.GetBonusAtLevel(this.BannerLevel);
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.BannerLevel = ((node.Attributes["banner_level"] != null) ? int.Parse(node.Attributes["banner_level"].Value) : 1);
			this.BannerEffect = MBObjectManager.Instance.GetObject<BannerEffect>(node.Attributes["effect"].Value);
		}

		internal static void AutoGeneratedStaticCollectObjectsBannerComponent(object o, List<object> collectedObjects)
		{
			((BannerComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}
	}
}

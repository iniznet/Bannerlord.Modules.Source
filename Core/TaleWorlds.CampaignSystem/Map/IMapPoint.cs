using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Map
{
	public interface IMapPoint
	{
		void OnGameInitialized();

		TextObject Name { get; }

		Vec2 Position2D { get; }

		PathFaceRecord CurrentNavigationFace { get; }

		Vec3 GetLogicalPosition();

		IFaction MapFaction { get; }

		bool IsInspected { get; }

		bool IsVisible { get; }

		bool IsActive { get; set; }
	}
}

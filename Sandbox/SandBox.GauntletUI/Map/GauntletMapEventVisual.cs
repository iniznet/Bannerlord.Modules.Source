using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.GauntletUI.Map
{
	public class GauntletMapEventVisual : IMapEventVisual
	{
		public MapEvent MapEvent { get; private set; }

		public Vec2 WorldPosition { get; private set; }

		public bool IsVisible { get; private set; }

		private Scene MapScene
		{
			get
			{
				if (this._mapScene == null)
				{
					Campaign campaign = Campaign.Current;
					if (((campaign != null) ? campaign.MapSceneWrapper : null) != null)
					{
						this._mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
					}
				}
				return this._mapScene;
			}
		}

		public GauntletMapEventVisual(MapEvent mapEvent, Action<GauntletMapEventVisual> onInitialized, Action<GauntletMapEventVisual> onVisibilityChanged, Action<GauntletMapEventVisual> onDeactivate)
		{
			this._onDeactivate = onDeactivate;
			this._onInitialized = onInitialized;
			this._onVisibilityChanged = onVisibilityChanged;
			this.MapEvent = mapEvent;
		}

		public void Initialize(Vec2 position, int battleSizeValue, bool hasSound, bool isVisible)
		{
			this.WorldPosition = position;
			this.IsVisible = isVisible;
			Action<GauntletMapEventVisual> onInitialized = this._onInitialized;
			if (onInitialized != null)
			{
				onInitialized(this);
			}
			if (hasSound)
			{
				if (GauntletMapEventVisual._battleSountEventIndex == -1)
				{
					GauntletMapEventVisual._battleSountEventIndex = SoundEvent.GetEventIdFromString(this.MapEvent.IsRaid ? "event:/map/ambient/node/battle_raid" : "event:/map/ambient/node/battle");
				}
				this._battleSound = SoundEvent.CreateEvent(GauntletMapEventVisual._battleSountEventIndex, this.MapScene);
				this._battleSound.SetParameter("battle_size", (float)battleSizeValue);
				float num = 0f;
				this.MapScene.GetHeightAtPoint(position, 2208137, ref num);
				this._battleSound.PlayInPosition(new Vec3(position.x, position.y, num + 2f, -1f));
				if (!isVisible)
				{
					this._battleSound.Pause();
				}
			}
		}

		public void OnMapEventEnd()
		{
			Action<GauntletMapEventVisual> onDeactivate = this._onDeactivate;
			if (onDeactivate != null)
			{
				onDeactivate(this);
			}
			if (this._battleSound != null)
			{
				this._battleSound.Stop();
				this._battleSound = null;
			}
		}

		public void SetVisibility(bool isVisible)
		{
			this.IsVisible = isVisible;
			Action<GauntletMapEventVisual> onVisibilityChanged = this._onVisibilityChanged;
			if (onVisibilityChanged != null)
			{
				onVisibilityChanged(this);
			}
			SoundEvent battleSound = this._battleSound;
			if (battleSound != null && battleSound.IsValid)
			{
				if (isVisible && this._battleSound.IsPaused())
				{
					this._battleSound.Resume();
					return;
				}
				if (!isVisible && !this._battleSound.IsPaused())
				{
					this._battleSound.Pause();
				}
			}
		}

		private static int _battleSountEventIndex = -1;

		private const string BattleSoundPath = "event:/map/ambient/node/battle";

		private const string RaidSoundPath = "event:/map/ambient/node/battle_raid";

		private readonly Action<GauntletMapEventVisual> _onDeactivate;

		private readonly Action<GauntletMapEventVisual> _onInitialized;

		private readonly Action<GauntletMapEventVisual> _onVisibilityChanged;

		private SoundEvent _battleSound;

		private Scene _mapScene;
	}
}

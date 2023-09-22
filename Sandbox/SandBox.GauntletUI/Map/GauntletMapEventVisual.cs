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
				if (this.MapEvent.IsFieldBattle)
				{
					if (GauntletMapEventVisual._battleSoundEventIndex == -1)
					{
						GauntletMapEventVisual._battleSoundEventIndex = SoundEvent.GetEventIdFromString("event:/map/ambient/node/battle");
					}
					this._battleSound = SoundEvent.CreateEvent(GauntletMapEventVisual._battleSoundEventIndex, this.MapScene);
					this._battleSound.SetParameter("battle_size", (float)battleSizeValue);
					float num = 0f;
					this.MapScene.GetHeightAtPoint(position, 2208137, ref num);
					this._battleSound.PlayInPosition(new Vec3(position.x, position.y, num + 2f, -1f));
					if (!isVisible)
					{
						this._battleSound.Pause();
						return;
					}
				}
				else
				{
					if (this.MapEvent.IsSiegeAssault || this.MapEvent.IsSiegeOutside || this.MapEvent.IsSiegeAmbush)
					{
						float num2 = 0f;
						Vec2 vec = ((this.MapEvent.MapEventSettlement != null) ? this.MapEvent.MapEventSettlement.GatePosition : this.MapEvent.Position);
						Campaign.Current.MapSceneWrapper.GetHeightAtPoint(vec, ref num2);
						Vec3 vec2;
						vec2..ctor(vec.X, vec.Y, num2, -1f);
						SoundEvent siegeSoundEvent = this._siegeSoundEvent;
						if (siegeSoundEvent != null)
						{
							siegeSoundEvent.Stop();
						}
						this._siegeSoundEvent = SoundEvent.CreateEventFromString("event:/map/ambient/node/battle_siege", this.MapScene);
						this._siegeSoundEvent.SetParameter("battle_size", 4f);
						this._siegeSoundEvent.SetPosition(vec2);
						this._siegeSoundEvent.Play();
						return;
					}
					if (this.MapEvent.IsRaid)
					{
						if (this.MapEvent.MapEventSettlement.IsRaided && this._raidedSoundEvent == null)
						{
							this._raidedSoundEvent = SoundEvent.CreateEventFromString("event:/map/ambient/node/burning_village", this.MapScene);
							this._raidedSoundEvent.SetParameter("battle_size", 4f);
							this._raidedSoundEvent.SetPosition(this.MapEvent.MapEventSettlement.GetPosition());
							this._raidedSoundEvent.Play();
							return;
						}
						if (!this.MapEvent.MapEventSettlement.IsRaided && this._raidedSoundEvent != null)
						{
							this._raidedSoundEvent.Stop();
							this._raidedSoundEvent = null;
						}
					}
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
			if (this._siegeSoundEvent != null)
			{
				this._siegeSoundEvent.Stop();
				this._siegeSoundEvent = null;
			}
			if (this._raidedSoundEvent != null)
			{
				this._raidedSoundEvent.Stop();
				this._raidedSoundEvent = null;
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
				}
				else if (!isVisible && !this._battleSound.IsPaused())
				{
					this._battleSound.Pause();
				}
			}
			SoundEvent siegeSoundEvent = this._siegeSoundEvent;
			if (siegeSoundEvent != null && siegeSoundEvent.IsValid)
			{
				if (isVisible && this._siegeSoundEvent.IsPaused())
				{
					this._siegeSoundEvent.Resume();
				}
				else if (!isVisible && !this._siegeSoundEvent.IsPaused())
				{
					this._siegeSoundEvent.Pause();
				}
			}
			SoundEvent raidedSoundEvent = this._raidedSoundEvent;
			if (raidedSoundEvent != null && raidedSoundEvent.IsValid)
			{
				if (isVisible && this._raidedSoundEvent.IsPaused())
				{
					this._raidedSoundEvent.Resume();
					return;
				}
				if (!isVisible && !this._raidedSoundEvent.IsPaused())
				{
					this._raidedSoundEvent.Pause();
				}
			}
		}

		private static int _battleSoundEventIndex = -1;

		private const string BattleSoundPath = "event:/map/ambient/node/battle";

		private const string RaidSoundPath = "event:/map/ambient/node/battle_raid";

		private const string SiegeSoundPath = "event:/map/ambient/node/battle_siege";

		private SoundEvent _siegeSoundEvent;

		private SoundEvent _raidedSoundEvent;

		private SoundEvent _battleSound;

		private readonly Action<GauntletMapEventVisual> _onDeactivate;

		private readonly Action<GauntletMapEventVisual> _onInitialized;

		private readonly Action<GauntletMapEventVisual> _onVisibilityChanged;

		private Scene _mapScene;
	}
}

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x0200002A RID: 42
	public class GauntletMapEventVisual : IMapEventVisual
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000184 RID: 388 RVA: 0x0000B869 File Offset: 0x00009A69
		// (set) Token: 0x06000185 RID: 389 RVA: 0x0000B871 File Offset: 0x00009A71
		public MapEvent MapEvent { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000186 RID: 390 RVA: 0x0000B87A File Offset: 0x00009A7A
		// (set) Token: 0x06000187 RID: 391 RVA: 0x0000B882 File Offset: 0x00009A82
		public Vec2 WorldPosition { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000188 RID: 392 RVA: 0x0000B88B File Offset: 0x00009A8B
		// (set) Token: 0x06000189 RID: 393 RVA: 0x0000B893 File Offset: 0x00009A93
		public bool IsVisible { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600018A RID: 394 RVA: 0x0000B89C File Offset: 0x00009A9C
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

		// Token: 0x0600018B RID: 395 RVA: 0x0000B8EA File Offset: 0x00009AEA
		public GauntletMapEventVisual(MapEvent mapEvent, Action<GauntletMapEventVisual> onInitialized, Action<GauntletMapEventVisual> onVisibilityChanged, Action<GauntletMapEventVisual> onDeactivate)
		{
			this._onDeactivate = onDeactivate;
			this._onInitialized = onInitialized;
			this._onVisibilityChanged = onVisibilityChanged;
			this.MapEvent = mapEvent;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000B910 File Offset: 0x00009B10
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

		// Token: 0x0600018D RID: 397 RVA: 0x0000B9E9 File Offset: 0x00009BE9
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

		// Token: 0x0600018E RID: 398 RVA: 0x0000BA18 File Offset: 0x00009C18
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

		// Token: 0x040000C5 RID: 197
		private static int _battleSountEventIndex = -1;

		// Token: 0x040000C6 RID: 198
		private const string BattleSoundPath = "event:/map/ambient/node/battle";

		// Token: 0x040000C7 RID: 199
		private const string RaidSoundPath = "event:/map/ambient/node/battle_raid";

		// Token: 0x040000CB RID: 203
		private readonly Action<GauntletMapEventVisual> _onDeactivate;

		// Token: 0x040000CC RID: 204
		private readonly Action<GauntletMapEventVisual> _onInitialized;

		// Token: 0x040000CD RID: 205
		private readonly Action<GauntletMapEventVisual> _onVisibilityChanged;

		// Token: 0x040000CE RID: 206
		private SoundEvent _battleSound;

		// Token: 0x040000CF RID: 207
		private Scene _mapScene;
	}
}

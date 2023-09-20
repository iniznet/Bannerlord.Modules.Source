using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	// Token: 0x020000B2 RID: 178
	public class HUDExtensionBrushWidget : BrushWidget
	{
		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000924 RID: 2340 RVA: 0x0001A32B File Offset: 0x0001852B
		// (set) Token: 0x06000925 RID: 2341 RVA: 0x0001A333 File Offset: 0x00018533
		public float AlphaChangeDuration { get; set; } = 0.15f;

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06000926 RID: 2342 RVA: 0x0001A33C File Offset: 0x0001853C
		// (set) Token: 0x06000927 RID: 2343 RVA: 0x0001A344 File Offset: 0x00018544
		public float OrderEnabledAlpha { get; set; } = 0.3f;

		// Token: 0x06000928 RID: 2344 RVA: 0x0001A34D File Offset: 0x0001854D
		public HUDExtensionBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x0001A390 File Offset: 0x00018590
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._currentAlpha - this._targetAlpha > 1E-45f)
			{
				if (this._alphaChangeTimeElapsed < this.AlphaChangeDuration)
				{
					this._currentAlpha = MathF.Lerp(this._initialAlpha, this._targetAlpha, this._alphaChangeTimeElapsed / this.AlphaChangeDuration, 1E-05f);
					this.SetGlobalAlphaRecursively(this._currentAlpha);
					this._alphaChangeTimeElapsed += dt;
					return;
				}
			}
			else
			{
				this._currentAlpha = this._targetAlpha;
				this.SetGlobalAlphaRecursively(this._targetAlpha);
			}
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x0001A422 File Offset: 0x00018622
		private void OnIsOrderEnabledChanged()
		{
			this._alphaChangeTimeElapsed = 0f;
			this._targetAlpha = (this.IsOrderActive ? this.OrderEnabledAlpha : 1f);
			this._initialAlpha = this._currentAlpha;
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x0600092B RID: 2347 RVA: 0x0001A456 File Offset: 0x00018656
		// (set) Token: 0x0600092C RID: 2348 RVA: 0x0001A45E File Offset: 0x0001865E
		[Editor(false)]
		public bool IsOrderActive
		{
			get
			{
				return this._isOrderActive;
			}
			set
			{
				if (this._isOrderActive != value)
				{
					this._isOrderActive = value;
					base.OnPropertyChanged(value, "IsOrderActive");
					this.OnIsOrderEnabledChanged();
				}
			}
		}

		// Token: 0x04000429 RID: 1065
		private float _alphaChangeTimeElapsed;

		// Token: 0x0400042A RID: 1066
		private float _initialAlpha = 1f;

		// Token: 0x0400042B RID: 1067
		private float _targetAlpha = 1f;

		// Token: 0x0400042C RID: 1068
		private float _currentAlpha = 1f;

		// Token: 0x0400042D RID: 1069
		private bool _isOrderActive;
	}
}

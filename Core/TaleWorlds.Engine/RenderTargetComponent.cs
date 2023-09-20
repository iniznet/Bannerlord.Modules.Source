using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000077 RID: 119
	public sealed class RenderTargetComponent : DotNetObject
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x00008D5A File Offset: 0x00006F5A
		public Texture RenderTarget
		{
			get
			{
				return (Texture)this._renderTargetWeakReference.GetNativeObject();
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060008BA RID: 2234 RVA: 0x00008D6C File Offset: 0x00006F6C
		// (set) Token: 0x060008BB RID: 2235 RVA: 0x00008D74 File Offset: 0x00006F74
		public object UserData { get; internal set; }

		// Token: 0x060008BC RID: 2236 RVA: 0x00008D7D File Offset: 0x00006F7D
		internal RenderTargetComponent(Texture renderTarget)
		{
			this._renderTargetWeakReference = new WeakNativeObjectReference(renderTarget);
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x00008D91 File Offset: 0x00006F91
		internal void OnTargetReleased()
		{
			this.PaintNeeded = null;
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x00008D9A File Offset: 0x00006F9A
		[EngineCallback]
		internal static RenderTargetComponent CreateRenderTargetComponent(Texture renderTarget)
		{
			return new RenderTargetComponent(renderTarget);
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060008BF RID: 2239 RVA: 0x00008DA4 File Offset: 0x00006FA4
		// (remove) Token: 0x060008C0 RID: 2240 RVA: 0x00008DDC File Offset: 0x00006FDC
		internal event RenderTargetComponent.TextureUpdateEventHandler PaintNeeded;

		// Token: 0x060008C1 RID: 2241 RVA: 0x00008E11 File Offset: 0x00007011
		[EngineCallback]
		internal void OnPaintNeeded()
		{
			RenderTargetComponent.TextureUpdateEventHandler paintNeeded = this.PaintNeeded;
			if (paintNeeded == null)
			{
				return;
			}
			paintNeeded(this.RenderTarget, EventArgs.Empty);
		}

		// Token: 0x04000179 RID: 377
		private readonly WeakNativeObjectReference _renderTargetWeakReference;

		// Token: 0x020000C1 RID: 193
		// (Invoke) Token: 0x06000C61 RID: 3169
		public delegate void TextureUpdateEventHandler(Texture sender, EventArgs e);
	}
}

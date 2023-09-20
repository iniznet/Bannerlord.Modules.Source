using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	public sealed class RenderTargetComponent : DotNetObject
	{
		public Texture RenderTarget
		{
			get
			{
				return (Texture)this._renderTargetWeakReference.GetNativeObject();
			}
		}

		public object UserData { get; internal set; }

		internal RenderTargetComponent(Texture renderTarget)
		{
			this._renderTargetWeakReference = new WeakNativeObjectReference(renderTarget);
		}

		internal void OnTargetReleased()
		{
			this.PaintNeeded = null;
		}

		[EngineCallback]
		internal static RenderTargetComponent CreateRenderTargetComponent(Texture renderTarget)
		{
			return new RenderTargetComponent(renderTarget);
		}

		internal event RenderTargetComponent.TextureUpdateEventHandler PaintNeeded;

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

		private readonly WeakNativeObjectReference _renderTargetWeakReference;

		public delegate void TextureUpdateEventHandler(Texture sender, EventArgs e);
	}
}

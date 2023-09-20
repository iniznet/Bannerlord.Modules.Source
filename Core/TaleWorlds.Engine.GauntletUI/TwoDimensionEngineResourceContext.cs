using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.Engine.GauntletUI
{
	public class TwoDimensionEngineResourceContext : ITwoDimensionResourceContext
	{
		Texture ITwoDimensionResourceContext.LoadTexture(ResourceDepot resourceDepot, string name)
		{
			string[] array = name.Split(new char[] { '\\' });
			Texture fromResource = Texture.GetFromResource(array[array.Length - 1]);
			fromResource.SetTextureAsAlwaysValid();
			bool flag = true;
			fromResource.PreloadTexture(flag);
			return new Texture(new EngineTexture(fromResource));
		}
	}
}

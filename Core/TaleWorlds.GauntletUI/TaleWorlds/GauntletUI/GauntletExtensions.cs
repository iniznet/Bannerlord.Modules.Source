using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200001F RID: 31
	public static class GauntletExtensions
	{
		// Token: 0x06000298 RID: 664 RVA: 0x0000DBF4 File Offset: 0x0000BDF4
		public static void SetGlobalAlphaRecursively(this Widget widget, float alphaFactor)
		{
			widget.SetAlpha(alphaFactor);
			List<Widget> children = widget.Children;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].SetGlobalAlphaRecursively(alphaFactor);
			}
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000DC30 File Offset: 0x0000BE30
		public static void SetAlpha(this Widget widget, float alphaFactor)
		{
			BrushWidget brushWidget;
			if ((brushWidget = widget as BrushWidget) != null)
			{
				brushWidget.Brush.GlobalAlphaFactor = alphaFactor;
			}
			TextureWidget textureWidget;
			if ((textureWidget = widget as TextureWidget) != null)
			{
				textureWidget.Brush.GlobalAlphaFactor = alphaFactor;
			}
			widget.AlphaFactor = alphaFactor;
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000DC70 File Offset: 0x0000BE70
		public static void RegisterBrushStatesOfWidget(this Widget widget)
		{
			BrushWidget brushWidget;
			if ((brushWidget = widget as BrushWidget) != null)
			{
				foreach (Style style in brushWidget.ReadOnlyBrush.Styles)
				{
					if (!widget.ContainsState(style.Name))
					{
						widget.AddState(style.Name);
					}
				}
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000DCE8 File Offset: 0x0000BEE8
		public static string GetFullIDPath(this Widget widget)
		{
			StringBuilder stringBuilder = new StringBuilder(string.IsNullOrEmpty(widget.Id) ? widget.GetType().Name : widget.Id);
			for (Widget widget2 = widget.ParentWidget; widget2 != null; widget2 = widget2.ParentWidget)
			{
				stringBuilder.Insert(0, (string.IsNullOrEmpty(widget2.Id) ? widget2.GetType().Name : widget2.Id) + "\\");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000DD68 File Offset: 0x0000BF68
		public static void ApplyActionForThisAndAllChildren(this Widget widget, Action<Widget> action)
		{
			action(widget);
			List<Widget> children = widget.Children;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].ApplyActionForThisAndAllChildren(action);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	public static class GauntletExtensions
	{
		public static void SetGlobalAlphaRecursively(this Widget widget, float alphaFactor)
		{
			widget.SetAlpha(alphaFactor);
			List<Widget> children = widget.Children;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].SetGlobalAlphaRecursively(alphaFactor);
			}
		}

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

		public static string GetFullIDPath(this Widget widget)
		{
			StringBuilder stringBuilder = new StringBuilder(string.IsNullOrEmpty(widget.Id) ? widget.GetType().Name : widget.Id);
			for (Widget widget2 = widget.ParentWidget; widget2 != null; widget2 = widget2.ParentWidget)
			{
				stringBuilder.Insert(0, (string.IsNullOrEmpty(widget2.Id) ? widget2.GetType().Name : widget2.Id) + "\\");
			}
			return stringBuilder.ToString();
		}

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

using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	public class BindingPathTargetDetails
	{
		public BindingPath BindingPath { get; private set; }

		public bool IsRoot
		{
			get
			{
				if (this.BindingPath == null)
				{
				}
				return this.BindingPath.Path == "Root";
			}
		}

		public BindingPathTargetDetails Parent { get; private set; }

		public List<BindingPathTargetDetails> Children { get; private set; }

		public List<WidgetCodeGenerationInfoDatabindingExtension> WidgetDatabindingInformations { get; private set; }

		public BindingPathTargetDetails(BindingPath bindingPath)
		{
			this.BindingPath = bindingPath;
			this.Children = new List<BindingPathTargetDetails>();
			this.WidgetDatabindingInformations = new List<WidgetCodeGenerationInfoDatabindingExtension>();
		}

		public void SetParent(BindingPathTargetDetails parent)
		{
			this.Parent = parent;
			this.Parent.Children.Add(this);
		}
	}
}

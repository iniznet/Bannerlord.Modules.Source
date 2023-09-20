using System;

namespace SandBox.View.Map
{
	public interface IMapConversationDataProvider
	{
		string GetAtmosphereNameFromData(MapConversationTableauData data);
	}
}

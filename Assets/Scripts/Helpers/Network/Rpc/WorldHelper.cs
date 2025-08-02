using Unity.Entities;

namespace Helpers.Network
{
	public static class WorldHelper
    {
        public static World GetClientWorld()
		{
			for (int w = 0; w < World.All.Count; w++)
				if (World.All[w].Name == "ClientWorld")
					return World.All[w];

			return default;
		}
    }
}

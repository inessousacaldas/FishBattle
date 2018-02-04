using AssetPipeline;

namespace PathologicalGames
{
    public static class PoolManagerConfig
    {

        public static PrefabPoolOption World2DMaPoolOption = new PrefabPoolOption()
        {
            preloadAmount = 30,
            preloadTime = true,
            preloadFrames = 10,

            cullAbove = 50,
            cullDelay = 10,
            cullDespawned = false,
            cullMaxPerPass = 5,

            limitFIFO = false,

        };

    }
}


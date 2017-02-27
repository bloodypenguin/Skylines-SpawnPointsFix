using System;
using ICities;

namespace SpawnPointsFix
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                return;
            }
            for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
            {
                var info = PrefabCollection<BuildingInfo>.GetLoaded(i);
                if (info == null || !info.m_isCustomContent)
                {
                    continue;
                }
                try
                {
                    SpawnPoints.RecalculateSpawnPoints(info);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"SpawnPointsFix - Exception happened when setting spawn points for asset {info.name}");
                    UnityEngine.Debug.LogException(e);
                }

            }
        }
    }
}
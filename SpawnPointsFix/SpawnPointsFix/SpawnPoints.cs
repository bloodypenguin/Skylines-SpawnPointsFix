using System.Collections.Generic;
using System.Linq;
using SpawnPointsFix.Extensions;
using UnityEngine;

namespace SpawnPointsFix
{
    public class SpawnPoints
    {

        public static void RecalculateSpawnPoints(BuildingInfo info)
        {
            var buildingAI = info?.GetComponent<TransportStationAI>();
            var paths = info?.m_paths;
            if (buildingAI == null || paths == null)
            {
                return;
            }
            var spawnPoints = (from path in paths
                where IsVehicleStop(path)
                select GetMiddle(path)).Distinct().ToArray();
            switch (spawnPoints.Length)
            {
                case 0:
                    buildingAI.m_spawnPosition = Vector3.zero;
                    buildingAI.m_spawnTarget = Vector3.zero;
                    buildingAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                    break;
                case 1:
                    buildingAI.m_spawnPosition = spawnPoints[0];
                    buildingAI.m_spawnTarget = spawnPoints[0];
                    buildingAI.m_spawnPoints = new[]
                    {
                        new DepotAI.SpawnPoint
                        {
                            m_position =  spawnPoints[0],
                            m_target =  spawnPoints[0]
                        }
                    };
                    break;
                default:
                    buildingAI.m_spawnPosition = Vector3.zero;
                    buildingAI.m_spawnTarget = Vector3.zero;
                    buildingAI.m_spawnPoints = spawnPoints.Select(p => new DepotAI.SpawnPoint
                    {
                        m_position = p,
                        m_target = p
                    }).ToArray();
                    break;
            }
        }

        private static bool IsVehicleStop(BuildingInfo.PathInfo path)
        {
            return (path?.m_nodes?.Length ?? 0) > 1 && (path?.m_netInfo?.IsStation() ?? false);
        }


        private static Vector3 GetMiddle(BuildingInfo.PathInfo path)
        {
            return (path.m_nodes.First() + path.m_nodes.Last()) / 2;
        }
    }
}

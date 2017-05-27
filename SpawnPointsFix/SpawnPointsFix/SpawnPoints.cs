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
            if (buildingAI == null || paths == null || buildingAI.m_transportLineInfo == null)
            {
                return;
            }
            var allSpawnPoints = (from path in paths
                    where IsVehicleStop(path)
                    select new KeyValuePair<Vector3, ItemClass>(GetMiddle(path), path.m_netInfo.m_class)).Distinct()
                .ToArray();
            Vector3[] primarySpawnPoints;
            Vector3[] secondarySpawnPoints;

            if (buildingAI.m_secondaryTransportInfo == null)
            {
                primarySpawnPoints = allSpawnPoints.Select(kvp => kvp.Key).ToArray();
                secondarySpawnPoints = new Vector3[] { };
            }
            else
            {
                primarySpawnPoints = allSpawnPoints
                    .Where(kvp => kvp.Value.m_subService == buildingAI.m_transportInfo.m_class.m_subService ||
                                  kvp.Value.m_subService == ItemClass.SubService.None &&
                                  kvp.Value.m_service == ItemClass.Service.Road &&
                                  buildingAI.m_transportInfo.m_class.m_subService ==
                                  ItemClass.SubService.PublicTransportBus)
                    .Select(kvp => kvp.Key)
                    .ToArray();
                secondarySpawnPoints = allSpawnPoints
                    .Where(kvp => kvp.Value.m_subService == buildingAI.m_secondaryTransportInfo.m_class.m_subService ||
                                  kvp.Value.m_subService == ItemClass.SubService.None &&
                                  kvp.Value.m_service == ItemClass.Service.Road &&
                                  buildingAI.m_secondaryTransportInfo.m_class.m_subService ==
                                  ItemClass.SubService.PublicTransportBus)
                    .Select(kvp => kvp.Key)
                    .ToArray();
            }


            if (!(buildingAI.m_transportLineInfo.m_class.m_service == ItemClass.Service.PublicTransport &&
                  (buildingAI.m_transportLineInfo.m_class.m_subService == ItemClass.SubService.PublicTransportShip ||
                   buildingAI.m_transportLineInfo.m_class.m_subService == ItemClass.SubService.PublicTransportPlane) &&
                  buildingAI.m_transportLineInfo.m_class.m_level == ItemClass.Level.Level2))
            {
                switch (primarySpawnPoints.Length)
                {
                    case 0:
                        buildingAI.m_spawnPosition = Vector3.zero;
                        buildingAI.m_spawnTarget = Vector3.zero;
                        buildingAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                        break;
                    case 1:
                        buildingAI.m_spawnPosition = primarySpawnPoints[0];
                        buildingAI.m_spawnTarget = primarySpawnPoints[0];
                        buildingAI.m_spawnPoints = new[]
                        {
                            new DepotAI.SpawnPoint
                            {
                                m_position = primarySpawnPoints[0],
                                m_target = primarySpawnPoints[0]
                            }
                        };
                        break;
                    default:
                        buildingAI.m_spawnPosition = Vector3.zero;
                        buildingAI.m_spawnTarget = Vector3.zero;
                        buildingAI.m_spawnPoints = primarySpawnPoints.Select(p => new DepotAI.SpawnPoint
                            {
                                m_position = p,
                                m_target = p
                            })
                            .ToArray();
                        break;
                }
            }

            if (buildingAI.m_secondaryTransportInfo != null && 
                !(buildingAI.m_secondaryTransportInfo.m_class.m_service == ItemClass.Service.PublicTransport &&
                  (buildingAI.m_secondaryTransportInfo.m_class.m_subService == ItemClass.SubService.PublicTransportShip ||
                   buildingAI.m_secondaryTransportInfo.m_class.m_subService == ItemClass.SubService.PublicTransportPlane) &&
                  buildingAI.m_secondaryTransportInfo.m_class.m_level == ItemClass.Level.Level2))
            {
                switch (secondarySpawnPoints.Length)
                {
                    case 0:
                        buildingAI.m_spawnPosition2 = Vector3.zero;
                        buildingAI.m_spawnTarget2 = Vector3.zero;
                        buildingAI.m_spawnPoints2 = new DepotAI.SpawnPoint[] { };
                        break;
                    case 1:
                        buildingAI.m_spawnPosition2 = secondarySpawnPoints[0];
                        buildingAI.m_spawnTarget2 = secondarySpawnPoints[0];
                        buildingAI.m_spawnPoints2 = new[]
                        {
                            new DepotAI.SpawnPoint
                            {
                                m_position = secondarySpawnPoints[0],
                                m_target = secondarySpawnPoints[0]
                            }
                        };
                        break;
                    default:
                        buildingAI.m_spawnPosition2 = Vector3.zero;
                        buildingAI.m_spawnTarget2 = Vector3.zero;
                        buildingAI.m_spawnPoints2 = secondarySpawnPoints.Select(p => new DepotAI.SpawnPoint
                            {
                                m_position = p,
                                m_target = p
                            })
                            .ToArray();
                        break;
                }
            }
        }

        private static bool IsVehicleStop(BuildingInfo.PathInfo path)
        {
            return (path?.m_nodes?.Length ?? 0) > 1 && (path?.m_netInfo?.IsStation() ?? false);
        }


        private static Vector3 GetMiddle(BuildingInfo.PathInfo path) //TODO(earalov): properly handle curved paths
        {
            return (path.m_nodes.First() + path.m_nodes.Last()) / 2;
        }
    }
}

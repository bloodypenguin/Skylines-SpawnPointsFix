namespace SpawnPointsFix.Extensions
{
    public static class NetInfoExtensions
    {
        public static bool IsStation(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            var name = netInfo.name;
            if (name == "Bus Station Way")
            {
                return false;
            }
            return name.Contains("Station") || name == "Airplane Stop";
        }
    }
}
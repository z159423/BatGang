namespace MondayOFF {
    public static class SingularAdDataSender {
        static int IS_COUNT = 0;
        static int RV_COUNT = 0;

        public static void SendAdData(MaxSdk.AdInfo adInfo) {
            SingularAdData data = new SingularAdData("AppLovin", "USD", adInfo.Revenue);

            data.WithAdUnitId(adInfo.AdUnitIdentifier).
                WithNetworkName(adInfo.NetworkName).
                WithAdPlacmentName(adInfo.Placement);

            SingularSDK.AdRevenue(data);

            // REWARDED RV_1 RV_3 RV_10
            if (adInfo.AdFormat.Equals("REWARDED")) {
                var currentCount = ++RV_COUNT;
                switch (currentCount) {
                    case 1:
                    case 3:
                    case 10:
                        SingularSDK.Event($"RV_{currentCount}");
                        break;
                }
            }
            // INTER IS_3 IS_5 IS_10
            else if (adInfo.AdFormat.Equals("INTER")) {
                var currentCount = ++IS_COUNT;
                switch (currentCount) {
                    case 3:
                    case 5:
                    case 10:
                        SingularSDK.Event($"IS_{currentCount}");
                        break;
                }
            }
        }
    }
}
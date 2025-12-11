namespace DentialClinic.Server.Models
{
    public class LocationDependencyCounts
    {
        public int MaterialRates { get; set; }
        public int PurchaseRates { get; set; }
        // add more if you need (Trips, Orders, etc.)
        public int Total => MaterialRates + PurchaseRates;
    }
}

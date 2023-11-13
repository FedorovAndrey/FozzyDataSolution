namespace SLPDBLibrary
{
    public class Meter : IDisposable
    {
        public Meter()
        {
            _data = new List<MeterData>();
        }
        public string? Vendor { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? MarkingPosition { get; set; }
        public string? Legend { get; set; }
        public List<MeterData> _data { get; set; }
        public void Dispose()
        {
            _data.Clear();
        }
    }
}

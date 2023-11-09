namespace SLPDBLibrary
{
    public class Meter : IDisposable
    {
        public Meter()
        {
            _data = new List<object>();
        }

        public string? Vendor { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? MarkingPosition { get; set; }
        public string? Legend { get; set; }
        public List<object> _data { get; set; }

        public void Dispose()
        {
            _data.Clear();
        }
    }
}

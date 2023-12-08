using SLPDBLibrary.Models;

namespace SLPDBLibrary
{
    public class MeterData : IDisposable
    {
        private List<TrendValue> _values = new List<TrendValue>();
        public MeterData()
        {


        }
        public string? Source { get; set; }
        public int SourceId { get; set; }
        public List<TrendValue> Values { get => _values; set => _values = value; }

        public void Dispose()
        {
            Values.Clear();
            GC.Collect();
        }
    }
}

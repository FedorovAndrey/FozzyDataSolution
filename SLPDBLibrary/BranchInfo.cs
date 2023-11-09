namespace SLPDBLibrary
{
    public class BranchInformation : IDisposable
    {
        private bool disposedValue;

        public BranchInformation()
        {
            this.Meters = new List<Meter>();
        }

        public int id { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? ServerName { get; set; }
        public List<Meter> Meters { get; set; }
        public int meterCount { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BranchInformation()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

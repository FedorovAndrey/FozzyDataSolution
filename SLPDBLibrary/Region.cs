using NLog;

namespace SLPDBLibrary
{
    public class Region
    {
        private List<BranchInformation> branches = new List<BranchInformation>();
        private int _id;
        private string _name;
        public Region()
        {


        }

        public bool AddBranch(BranchInformation branchInformation)
        {
            bool bResult = false;

            try
            {
                branches.Add(branchInformation);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("logger").Error(ex.Message);
                LogManager.GetLogger("logger").Error(ex.Source);
            }

            return bResult;
        }
        public List<BranchInformation> Branches
        {
            get
            {
                return this.branches;
            }
            set { this.branches = value; }


        }
        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        public DateTime TimestampBegin { get; set; }
        public DateTime TimestampEnd { get; set; }

    }
}

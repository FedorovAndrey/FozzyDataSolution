using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPDBLibrary
{
    public class Region
    {
        private List<BranchInformation> branches = new List<BranchInformation>();
        private int _id;
        private string _name;

        public Region() { }
        public Region(int id, string name)
        {
            this._name = name;
            this._id   = id;

        }

        public bool AddBranch(BranchInformation branchInformation)
        {
            bool bResult = false;

            try { 
                branches.Add(branchInformation);
            }
            catch(Exception ex) 
            {
                
            }

            return bResult;
        }
        public List<BranchInformation> Branches {
            get { 
                return this.branches;
            }


        }
        public int ID { 
            get { return this._id; }
        }
        public string Name
        {
            get { return this._name; }
        }
       

    }
}

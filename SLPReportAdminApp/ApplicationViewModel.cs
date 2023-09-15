using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using SLPDBLibrary;

namespace SLPReportAdminApp
{
    class ApplicationViewModel
    {
        DatabaseContext database = new DatabaseContext();
        public ObservableCollection<tbRegions> Regions { get; set; }
        public ObservableCollection<tbCities> Cities { get; set; }
        public ApplicationViewModel() 
        {
            database.tbRegions.Load();
            Regions = new ObservableCollection<tbRegions>();
            Cities = new ObservableCollection<tbCities>();
        }
    }
}

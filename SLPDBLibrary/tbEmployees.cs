using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLPDBLibrary
{
    public class tbEmployees
    {
        public int ID { get; set; }
        public int Mailing { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }   
        public string? MiddleName { get; set; } 
        public string? Position { get; set; }   
        public string? Email { get; set; }  

    }
}

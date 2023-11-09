// See https://aka.ms/new-console-template for more information

using SLPDBLibrary.Models;

Console.WriteLine("Hello, World!");

using (EboDbContext db = new EboDbContext())
{
    var query = (from meters in db.TbMeters
                 where meters.BranchId == 23
                 select meters).ToList();

}
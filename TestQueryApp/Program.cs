// See https://aka.ms/new-console-template for more information

using SLPDBLibrary;

Console.WriteLine("Hello, World!");

using (DatabaseContext database = new DatabaseContext())
{
    int regionID = 1;

    var query = (from branch in database.tbBranche
                 join city in database.tbCities on branch.City equals city.ID
                 where branch.Region == regionID
                 select new 
                 { 
                     branch.ID,
                     city.Name,
                     branch.Address

                 }).ToList();

    foreach (var item in query)
    {
        Console.WriteLine(item);
    }
}


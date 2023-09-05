// See https://aka.ms/new-console-template for more information
using SLPDBLibrary;

Console.WriteLine("Hello, World!");
using (DatabaseContext database = new DatabaseContext())
{
    var query = database.tbBranche.ToList();

    foreach (tbBranche branche in query)
    {
        Console.WriteLine("Address - {0} : Server Name - {1}", branche.Address, branche.ServerName);
    }
}

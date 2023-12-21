// See https://aka.ms/new-console-template for more information

using System.Diagnostics.Metrics;
using SLPDBLibrary;
using SLPDBLibrary.Models;
using SLPHelper;

Console.WriteLine("Hello, World!");



using (EboDbContext db = new EboDbContext())
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    var query = (
        from trend in db.TrendMeta
        where trend.Source.Contains("o-zh-zhitn1-em1") &&
              trend.Source.Contains("Загальний") &&
              (trend.Source.Contains("експорт") || trend.Source.Contains("імпорт"))
        orderby trend.Source
        select new MeterData
        {
            Source = trend.Source,
            SourceId = trend.Externallogid,

            
        }
        ).ToList();

    foreach (var item in query)
    {
        var query1 = (from data in db.TrendData
                     where data.Externallogid == data.Externallogid &&
                     data.Timestamp >= DateTime.Parse("2023-12-01 00:00:00") &&
                     data.Timestamp <= DateTime.Parse("2023-12-02 00:00:00") &&
                     data.Value != null
                     select data).ToList();
    }
}
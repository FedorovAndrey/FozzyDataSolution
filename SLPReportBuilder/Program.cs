// See https://aka.ms/new-console-template for more information

using System.Configuration;
using SLPDBLibrary;
using NLog;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();
logger.LogInformation("Program has started.");



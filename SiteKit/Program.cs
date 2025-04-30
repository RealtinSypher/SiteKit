using Cocona;
using SiteKit;

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

app.AddCommands<ProjectCommands>();

app.Run();
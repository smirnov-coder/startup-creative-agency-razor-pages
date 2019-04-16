#addin "Cake.Yarn&version=0.4.5"

var target = Argument("target", "Default");
var configuration = Argument("Configuration", "Release");

string solutionFilePath = "../../StartupCreativeAgency.sln";

Task("Clean")
	.Does(() =>
{
	DotNetCoreClean(solutionFilePath, new DotNetCoreCleanSettings
    {
        Configuration = configuration
    });
	Information("Solution cleaned.");
});

Task("NuGet-Restore")
	.Does(() =>
{
	DotNetCoreRestore(solutionFilePath);
	Information("NuGet packages restored.");
});

Task("Solution-Build")
	.IsDependentOn("Clean")
	.IsDependentOn("NuGet-Restore")
	.IsDependentOn("NodeJS-Restore")
	.IsDependentOn("Bundles-Build")
	.Does(() =>
{
	DotNetCoreBuild(solutionFilePath, new DotNetCoreBuildSettings
    {
        Configuration = configuration,
		NoRestore = true,
		EnvironmentVariables = new Dictionary<string, string>
		{
			["ASPNETCORE_ENVIRONMENT"] = "Production"
		}
    });
	Information("Solution built.");
});

Task("NodeJS-Restore")
	.Does(() =>
{
	Yarn.Install();
	Information("NodeJS packages restored.");
});

Task("Bundles-Build")
	.Does(() => 
{
	Yarn.RunScript("lib");
	Information("Vendor bundle built.");
	Yarn.RunScript("build");
	Information("Bundles built.");
});

// Опциональный параметр пути к папке для развёртывания приложения. По умолчанию создаётся папка 'publish' в корневой папке решения.
// Пример использования: ./build.ps1 -Target=Build-And-Publish -publish_dir=C:\my_publish_dir
var publishDirectory = Argument("publish_dir", "../../publish");

Task("Publish")
	.Does(() =>
{
	DotNetCorePublish(".", new DotNetCorePublishSettings
	{
		NoBuild = true,
		NoRestore = true,
		Configuration = configuration,
		OutputDirectory = publishDirectory
	});
	Information("Application published.");
});

Task("Test")
	.Does(() =>
{
	var solution = ParseSolution(solutionFilePath);
	var testProjects = solution.Projects.Where(project => project.Name.Contains("Tests") && project.Name != "Tests.Shared");
	foreach (var project in testProjects)
	{
		Information($"Testing project {project.Name}...");
		DotNetCoreTest(project.Path.ToString(), new DotNetCoreTestSettings
		{
			NoBuild = true,
			NoRestore = true,
			Configuration = configuration
		});
	}
	Information("Tests completed.");
});

Task("Build-And-Test")
	.IsDependentOn("Solution-Build")
	.IsDependentOn("Test");

Task("Build-And-Publish")
	.IsDependentOn("Build-And-Test")
	.IsDependentOn("Publish");

Task("Default")
	.IsDependentOn("Solution-Build");

RunTarget(target);

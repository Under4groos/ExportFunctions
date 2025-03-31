
using ExportFunctions.Helper;
using ExportFunctions.Structures;

string pathProject = string.Empty;
string pathExport = string.Empty;
string defineName = string.Empty;
string consoleArgs = string.Empty;

main:
if (!args.Any())
{
    Console.WriteLine("-p direcotry project");
    Console.WriteLine("-d name #define export ");
    Console.WriteLine("-e direcotry export files");

    Console.WriteLine("Write commands");
head:
    consoleArgs = Console.ReadLine() ?? string.Empty;
    if (string.IsNullOrEmpty(consoleArgs))
    {
        goto head;
    }
}
else
{
    consoleArgs = string.Join(" ", args);
}

foreach (string command in
    consoleArgs.
    Split("-").
    Where(l => !string.IsNullOrEmpty(l)).
    Select(l => l.Trim()))
{
    char coommand = command.First();
    string arg = command.Substring(1).Replace("\"", "").Trim();
    switch (coommand)
    {
        case 'p':
            if (!Directory.Exists(arg))
            {
                Console.WriteLine("Directory project not found!");
                Console.WriteLine($"->{arg}");
                goto main;
            }
            pathProject = arg;

            continue;
        case 'd':
            if (string.IsNullOrEmpty(arg))
            {
                Console.WriteLine($"Define is null!");
                goto main;
            }
            defineName = arg;

            continue;
        case 'e':
            pathExport = arg;

            continue;

    }


    return;
}
Console.WriteLine($"-p {pathProject}");
Console.WriteLine($"-d {defineName}");
Console.WriteLine($"-e {pathExport}");


using (SolutionNativeExport solutionExport = new SolutionNativeExport())
{
    // #define ExportFunction extern "C" __declspec(dllexport)
    solutionExport.DefineExportFunction = defineName;

    // Дирректоиря для экспорта.
    solutionExport.Exports = Path.GetFullPath(pathExport);
    solutionExport.OnCompleted += (StructResultExtern result) =>
    {
        Console.WriteLine(result.Span);
        Console.WriteLine(result.FileStructures);
        Console.WriteLine(result.FileFunctions);
    };
    solutionExport.Load(pathProject);
    solutionExport.Export();
}


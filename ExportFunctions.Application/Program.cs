using ExportFunctions.Helper;
using ExportFunctions.Structures;

string path = string.Empty;
#if DEBUG
path = @"E:\VisualStudio\repos\WinExportedFuncs\WinExportedFuncs";
#else
writepath:
Console.WriteLine("Write directory project:");
path = Console.ReadLine().Replace("\"", "");
if (!Directory.Exists(path))
{

    Console.WriteLine("Direcotry not found!");
    goto writepath;
}
#endif
using (SolutionNativeExport solutionExport = new SolutionNativeExport())
{
    // #define ExportFunction extern "C" __declspec(dllexport)
    solutionExport.DefineExportFunction = "ExportFunction";

    // Дирректоиря для экспорта.
    solutionExport.Exports = "Exports";
    solutionExport.OnCompleted += (StructResultExtern result) =>
    {
        Console.WriteLine(result.Span);
        Console.WriteLine(result.FileStructures);
        Console.WriteLine(result.FileFunctions);
    };
    solutionExport.Load(path);
    solutionExport.Export();
}
Console.ReadLine();
#if !DEBUG
goto writepath;
#endif



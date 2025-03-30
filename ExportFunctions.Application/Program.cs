using ExportFunctions.Application;

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
    solutionExport.Load(path);
    solutionExport.ExportFunctions((span) =>
    {
        
        Console.WriteLine(span);
    }, 
    (span) =>
    {
        Console.WriteLine(span);
    });


    Console.ReadLine();
}
#if !DEBUG
goto writepath;

#endif
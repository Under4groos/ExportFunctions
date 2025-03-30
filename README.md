# ExportFunctions

Конвертирует нативные функции и структуры в C# для их вызова. 


## Пример С++ функции
```C++
#define ExportFunction extern "C" __declspec(dllexport)
ExportFunction WinProcStruct TestFunctionResult(int i , int a , int b , int c) {
	return ListWinProcStruct[i];
}
```



```cs

using ExportFunctions.Application.Helper;
using ExportFunctions.Application.Structures;

string path = string.Empty;
#if DEBUG
// Путь к проекту. Не к решению, а к проекту!
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
```
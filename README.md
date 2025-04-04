# ExportFunctions

Конвертирует нативные функции и структуры в C# для их вызова.

# expo command

Args

```
-p direcotry project"
-d name #define export
-e direcotry export files
```

```
expo -p "E:\VisualStudio\repos\WinExportedFuncs\WinExportedFuncs" -d "ExportFunctionDebug" -e "C:\Users\UnderKo\Downloads\Exports"
```


# Using C# code lib 

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
```

## Результат

ExternFunctions.cs

```cs
using System.Runtime.InteropServices;
namespace Interop
{
	public static class Interop
	{
		//[0] E:\VisualStudio\repos\WinExportedFuncs\WinExportedFuncs\dllmain.cpp
		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TestFunctionResult")]
		public static extern WinProcStruct TestFunctionResult(int i,int a,int b,int c);
    }
}
```

ExternStructures

```cs
// E:\VisualStudio\repos\WinExportedFuncs\WinExportedFuncs\WinProcStruct.h
public struct WinProcStruct
{
   public IntPtr hwnd;
   public IntPtr lParam;
   public uint processId;
   public override string ToString()
   {
      return $" { this.hwnd.ToString()} { this.lParam.ToString()} { this.processId.ToString()}";
   }
}
```

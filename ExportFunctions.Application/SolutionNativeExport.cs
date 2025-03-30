using ExportFunctions.Application.Structures;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ExportFunctions.Application
{
    public class SolutionNativeExport : IDisposable
    {

        public List<StructNativeStructure> nativeStructures
        {
            get; private set;
        } = new List<StructNativeStructure>();

        public List<StructNativeFunction> structNativeFunctions
        {
            get; private set;
        } = new List<StructNativeFunction>();






        public string PathSolution
        {
            get; private set;
        }
        public Dictionary<string, string> FileData
        {
            get; private set;
        } = new Dictionary<string, string>();
        public List<string> Files { get; private set; } = new List<string>();

        public string MainFile
        {
            get; private set;
        }

        private const string Exports = "Exports";


        // export function in C# code 
        public string DirectoryExport
        {
            get; set;
        } = string.Empty;

        public string? NameProject;

        public string? FullPathStructures { get; set; }
        public string? FullPathHelper { get; set; }


        public string DefineExportFunction = "ExportFunction";


        public virtual void Load(string path)
        {
            if (!Directory.Exists(path))
                throw new FileNotFoundException("Directory not found!");

            NameProject = Path.GetFileName(path);

            DirectoryExport = Path.Combine(Exports, NameProject).CreateDirectory();
            FullPathStructures = Path.Combine(DirectoryExport, "Structures").CreateDirectory();
            FullPathHelper = Path.Combine(DirectoryExport, "Helper").CreateDirectory();




            this.Clear();

            PathSolution = path;


            Files = Directory.GetFiles(PathSolution, "*", SearchOption.AllDirectories)
                .Where(file => file.EndsWith(".cpp") || file.EndsWith(".h"))
                .Select(file => Path.GetFullPath(file)).ToList();

            string[] mainFiles = Files.Where(file => file.Contains("main")).ToArray();
            if (Files.Any())
                MainFile = mainFiles.First();


            FileInfo mainFileOne = new FileInfo(mainFiles.First());



            FileData.Add(mainFileOne.FullName, File.ReadAllText(mainFileOne.FullName));

            var includes = FileData.First().Value.Split("\n")

                .Where(line => line.StartsWith("#include"))
                .Select(
                    line => Path.Combine(
                        mainFileOne.DirectoryName,
                        Regex.Replace(line, "<|>|\"", string.Empty)
                        .Replace("#include", string.Empty).Trim()))
                .Where(file => File.Exists(file))
                .Select(f => f)
                .ToList();

            Console.WriteLine($"Main: {mainFileOne.FullName}");

            foreach (var f in includes)
            {
                FileData.Add(f, File.ReadAllText(f));

            }


            Console.WriteLine($"Finish read files.");

        }

        public void ExportFunctions(Action<TimeSpan> FinishStructures, Action<TimeSpan> FinishExportFunctions)
        {
            nativeStructures.Clear();

            var sw = new Stopwatch();
            sw.Start();
            foreach (var file in FileData)
            {
                string fileRead = file.Value;
                // https://regex101.com/r/T5xNuH/1
                // struct.+?[\w]+[\W]+?\{[\w\W]+?}
                FindStrucutures(fileRead, file.Key);

                FindExportFunctions(fileRead, file.Key);
            }
            SaveStructuresToFile();
            SaveExternFunctionsToFile();
            sw.Stop();
            FinishStructures?.Invoke(sw.Elapsed);

        }

        private void FindExportFunctions(string fileStringData, string fileFullPath)
        {
            var arrayFunctions = fileStringData.
                 Split("\n").Select(line => line.Trim()).
                 Where(line => !line.StartsWith("#define") && Regex.IsMatch(line, $"{DefineExportFunction}? ")).
                 ToArray();

            foreach (var func in arrayFunctions)
            {
                List<string> functions = new List<string>();
                string funcAll = func.Substring(DefineExportFunction.Length).Trim();
                string typeReturn = Converter.GetTypeFunction(funcAll);
                string functionHead = funcAll.Substring(typeReturn.Length).Trim();
                if (functionHead.EndsWith("{"))
                    functionHead = functionHead.Substring(0, functionHead.Length - 2);
                string nameFunction = Converter.GetNameFunction(functionHead);
                foreach (string[] item in Converter.GetArgumentsFunction(functionHead).
                    Select(f => f.Split(" ")).ToArray())
                {
                    if (item.Length == 2)
                    {
                        functions.Add($"{Converter.ConvertTypeCppToCsharp(item[0])} {item[1]}");
                    }

                }
                foreach (var item in functions)
                {
                    Console.WriteLine($"-> {item}");
                }
           
                structNativeFunctions.Add(new StructNativeFunction()
                {
                    FullPath = fileFullPath,
                    Name = nameFunction,
                    Variables = functions,
                    ReturnType = typeReturn
                });
            }


        }

        private void FindStrucutures(string fileStringData, string fileFullPath)
        {
            var mathes = Regex.Matches(fileStringData, "struct.+?[\\w]+[\\W]+?\\{[\\w\\W]+?}");


            var arrayStructuresMatch = mathes.Select(m => m.Value.Trim()).ToArray();
            foreach (var strStrucutre in arrayStructuresMatch)
            {
                string[] lines = strStrucutre.Split("\n").
                Select(line => line.Trim()).
                Where(line => !string.IsNullOrEmpty(line)).
                Where(line => !(line.StartsWith("{") || line.StartsWith("}"))).
                ToArray();


                if (lines.Length < 3)
                {
                    Console.WriteLine($"Ignored: {fileStringData}");
                    return;
                }
                StructNativeStructure strucure = new StructNativeStructure();

                foreach (var line in lines)
                {

                    if (line.StartsWith("struct"))
                    {
                        strucure.Name = line.Replace("struct", string.Empty).Trim();
                        strucure.FullPath = fileFullPath;
                        continue;
                    }


                    string[] variable = line.
                        Substring(0, line.Length - 1).
                        Split(" ").
                        Select(l => l.Trim()).
                        ToArray();
                    if (variable.Length < 2)
                        return;

                    if (strucure.Variables == null)
                        strucure.Variables = new List<StrucVariable>();
                    strucure.Variables.Add(new StrucVariable()
                    {
                        Name = variable[1],
                        Type = variable[0],

                    });

                }





                nativeStructures.Add(strucure);
            }


        }

        private void SaveExternFunctionsToFile(string nameFile = $"ExternFunctions.cs")
        {
            if (!Directory.Exists(FullPathHelper))
            {
                throw new Exception($"Directory not found! {FullPathHelper}");
            }
            string filePath = Path.GetFullPath(Path.Combine(FullPathHelper, $"{nameFile}"));



            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("using System.Runtime.InteropServices;");
                stringBuilder.AppendLine("namespace Interop\r\n{");
                stringBuilder.AppendLine("\tpublic static class Interop\r\n\t{");
                foreach (var structures in structNativeFunctions)
                {
                    stringBuilder.AppendLine($"\t{structures.ToCSharpCode()}");
                }
                stringBuilder.AppendLine("\t\n}\n}\n\n");
                File.WriteAllText(filePath, stringBuilder.ToString());

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SaveStructuresToFile(string nameFile = $"ExternStructures.cs")
        {
            if (!Directory.Exists(FullPathStructures))
            {
                throw new Exception($"Directory not found! {FullPathStructures}");
            }
            string filePath = Path.GetFullPath(Path.Combine(FullPathStructures, $"{nameFile}"));



            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var structures in nativeStructures)
                {
                    stringBuilder.AppendLine($"// {structures.FullPath}");
                    stringBuilder.AppendLine(structures.ToCSharpCode());


                }
                File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (Exception)
            {

                throw;
            }


        }

        public void Clear()
        {
            FileData.Clear();
            Files.Clear();
            PathSolution = MainFile = string.Empty;

        }

        public void Dispose()
        {
            this.Clear();
        }
    }
}

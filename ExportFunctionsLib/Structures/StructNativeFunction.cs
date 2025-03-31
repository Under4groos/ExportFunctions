using System.Text;

namespace ExportFunctions.Structures
{
    

    public struct StructNativeFunction
    {
        public string Name;
        public string ReturnType;
        public List<string> Variables;
        public string FullPath;
        public int IntLine;
       
        public string ToCSharpCode( string returnTypeRe = "")
        {
            if (!string.IsNullOrEmpty(returnTypeRe))
                ReturnType = returnTypeRe;
            StringBuilder stringBuilder = new StringBuilder();
            // [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetProcessFilePath")]

            stringBuilder.AppendLine($"\t//[{IntLine}] {FullPath}");
            stringBuilder.AppendLine($"\t\t[DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = \"{Name}\")]");

            // public static extern string GetProcessPathF(IntPtr hwnd);
            
            stringBuilder.AppendLine($"\t\tpublic static extern {ReturnType} {Name}({string.Join("," , Variables)});");


            return stringBuilder.ToString();
        }
    }
}

﻿using System.Text;

namespace ExportFunctions.Structures
{
    public struct StructNativeStructure
    {
        public string Name;
        public List<StrucVariable> Variables;
        public string FullPath;

        public string ToCSharpCode( bool isOverideToString = true)
        {
            StringBuilder buildStructure = new StringBuilder();

            // [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            buildStructure.AppendLine($"[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]");
            buildStructure.AppendLine($"public struct {Name}");
            buildStructure.AppendLine("{");
            foreach (var v in Variables)
            {

                buildStructure.AppendLine($"   public {v.ToStringConvert()};");

            }


            //public override string ToString()
            //{
            //    return $"[{hwnd}][{processId}]:{text}\n  {path}";
            //}
            buildStructure.AppendLine($"   public override string ToString()");

            string strReturn = "   {\n      return $\"_values_\";";

            string values = "";
            foreach (var v in Variables)
            {
                values += " {" + $" this.{v.Name}.ToString()" + "}";
            }
            buildStructure.AppendLine(strReturn.Replace("_values_", values));
            buildStructure.AppendLine("   }");

            buildStructure.AppendLine("}");
            return buildStructure.ToString();
        }

    }
}

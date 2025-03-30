namespace ExportFunctions.Application.Structures
{
    public struct StrucVariable
    {
        public string Type;
        public string Name;
        public string TypeConvert;

       
        public string ToStringConvert()
        {
            TypeConvert = Converter.ConvertTypeCppToCsharp(Type);
            return $"{TypeConvert} {Name}";
        }

        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}

namespace ExportFunctions.Application
{
    public static class Converter
    {

        public enum _IntPtr
        {
            HMODULE,

            HWND,
            LPARAM
        }
        public enum _uint
        {
            DWORD,
        }


        public enum _string
        {
            std__string,
            char_
        }

        public enum _int
        {
            ULONG_PTR,
            
        }

        public static string ToTypeof(string type, Type type1, int rep)
        {
            foreach (var item in Enum.GetValues(type1))
            {
                if (item.ToString() == type.
                    Replace("::", "__").
                    Replace("*", "_"))
                    return type1.Name.Substring(rep);
            }


            return string.Empty;

        }

        public static string GetTypeFunction(string func)
        {
            string result = string.Empty;
            foreach (var item in func)
            {
                if (item == ' ')
                    break;
                result += item;
            }
            return result;
        }
        public static string GetNameFunction(string func)
        {
            string result = string.Empty;
            foreach (var item in func)
            {
                if (item == '(')
                    break;
                result += item;
            }
            return result;
        }

        public static List<string> GetArgumentsFunction(string func)
        {
            
            string strResult = string.Empty;
            bool result = false;
            foreach (var char_ in func)
            {
                if(char_ == '(')
                {
                    result = true;
                    continue;
                }
                if (char_ == ')')
                {
                    result = false;
                    continue;
                }
                if (result)
                {
                    strResult += char_;
                }
            }
            return strResult.Split(",").Select(x => x.Trim()).ToList();
        }


        
        public static string ConvertTypeCppToCsharp(string type)
        {
            if(type == "int")
                return "int";


            string _type = ToTypeof(type, typeof(_IntPtr), 1);
            if (string.IsNullOrEmpty(_type))
                _type = ToTypeof(type, typeof(_int), 1);

            if (string.IsNullOrEmpty(_type))
                _type = ToTypeof(type, typeof(_uint), 1);


            if (string.IsNullOrEmpty(_type))
                _type = ToTypeof(type, typeof(_string), 1);

            if (!string.IsNullOrEmpty(_type))
                return _type;


            return "void";

        }
    }
}

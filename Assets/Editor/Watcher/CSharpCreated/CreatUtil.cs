

    public class CreatUtil
    {
        public const string savePath = "Assets/Scripts/Auto/Engine";
        const int leftSpace = 66;

        public static string SetStringField(string fieldName, string value, int order = 0)
        {
            fieldName = fieldName.Replace(' ', '_');
            var field = ForceFormat($"{GetFormat(order)}public const string {fieldName}", leftSpace);
            return ($"{field} = \"{value}\";");
        }

        public static string ConstField(string fieldName, string value, int order = 0)
        {
            fieldName = fieldName.Replace(' ', '_');
            var field = ForceFormat($"{GetFormat(order)}public const string {fieldName}", leftSpace);
            return ($"{field} = \"{value}\";");
        }
        public static string StaticField(string fieldName, string value, int order = 0)
        {
            fieldName = fieldName.Replace(' ', '_');
            var field = ForceFormat($"{GetFormat(order)}public static string {fieldName}", leftSpace);
            return ($"{field} = \"{value}\";");
        }



    public static string StaticMethod(string fieldName, string value, int order = 0)
    {
        fieldName = fieldName.Replace(' ', '_');
        var field = ForceFormat($"{GetFormat(order)}public static string {fieldName}", leftSpace);
        return ($"{field} = \"{value}\";");
    }


    public static string SetStringFieldSuf(string fieldName, string value, int order = 0)
        {
            fieldName = fieldName.Replace(' ', '_');
            var field = ForceFormat($"{GetFormat(order)}public const string {fieldName}_Suf", leftSpace);
            return ($"{field} = \"{value}\";");
        }


        public static string ForceFormat(string souce, int count)
        {
            if (souce.Length < count)
            {
                int num = count - souce.Length;

                while (num > 0)
                {
                    souce += " ";
                    num--;
                }
            }
            return souce;
        }



        public static string LeftSymbol => "{";
        public static string RightSymbol => "}";

        public static string GetFormat(int count)
        {
            string num = string.Empty;
            for (int i = 0; i < count; i++)
            {
                num += " ";
            }
            return num;
        }

        public static string SubstringSingle(string source, string replace)
        {
            var resulf = string.Empty;

            if (source.Contains(replace))
            {
                bool isFind = false;
                var vs = source.Replace('\\', '/').Split('/');

                foreach (var item in vs)
                {
                    if (isFind)
                    {
                        resulf += $"{item}/";
                        continue;
                    }

                    if (item == replace)
                    {
                        isFind = true;
                    }
                }
            }

            return resulf.TrimEnd('/').TrimStart('/');
        }

        public static string GetClassString(string directoryName)
        {
            directoryName = directoryName.Replace(' ', '_');
            return $"{directoryName}";
        }

        public static string NewClassString(string directoryName)
        {
            return $"new {directoryName}()";
        }
    }


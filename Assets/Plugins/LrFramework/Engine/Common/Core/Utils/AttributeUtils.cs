using System.Collections.Generic;
using System.Reflection;
using Object = System.Object;

namespace Sailfish
{
    public class AttributeUtils
    {
        public static List<System.Type> FindType<T>(bool includeSelf = true)
        {
            return FindType<T>(typeof(AttributeUtils).Assembly, includeSelf);
        }
        public static List<System.Type> FindType<T>(System.Reflection.Assembly assembly, bool includeSelf = true)
        {
            List<System.Type> types = null;
            System.Type superType = typeof(T);

            System.Type[] list = assembly.GetTypes();
            foreach (var type in list)
            {
                if (superType.IsAssignableFrom(type) && (includeSelf || (!includeSelf && superType != type)))
                {
                    if (types == null)
                    {
                        types = new List<System.Type>();
                    }
                    types.Add(type);
                }
            }

            return types;
        }

        public static T GetClassAttribute<T>(System.Type type) where T : System.Attribute
        {
            var attributes = type.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is T)
                {
                    return (T)attribute;
                }
            }

            return default(T);
        }

        public static object FindFieldByType(object obj, System.Type fieldType)
        {
            System.Type objType = obj.GetType();

            var properties = objType.GetProperties(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var propertyInfos = _Find<PropertyInfo>(properties, info => fieldType.IsAssignableFrom(info.PropertyType));
            if (propertyInfos != null && propertyInfos.Count > 0)
            {
                return propertyInfos[0].GetGetMethod(true).Invoke(obj, null);
            }

            var fields = objType.GetFields(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldInfos = _Find<FieldInfo>(fields, info => fieldType.IsAssignableFrom(info.FieldType));
            if (fieldInfos != null && fieldInfos.Count > 0)
            {
                return fieldInfos[0].GetValue(obj);
            }

            return null;
        }

        private static List<T> _Find<T>(T[] ts, System.Func<T, bool> isValid)
        {
            List<T> list = null;
            if (ts != null)
            {
                list = new List<T>();
                foreach (var t in ts)
                {
                    if (isValid(t))
                    {
                        list.Add(t);
                    }
                }

            }
            return list;
        }

        public static List<System.Type> FindTypeWithAttributes<T>()
        {
            List<System.Type> types = null;
            System.Type attrType = typeof(T);

            foreach (var type in typeof(AttributeUtils).Assembly.GetTypes())
            {
                var attrs = type.GetCustomAttributes(attrType, true);
                if (attrs != null && attrs.Length > 0)
                {
                    if (types == null)
                    {
                        types = new List<System.Type>();
                    }
                    types.Add(type);
                }
            }

            return types;
        }

        public static void DoWithMethodWithAttrubute<T>(object obj, System.Action<T, MethodInfo> action)
        {
            var objType = obj.GetType();
            var attrType = typeof(T);
            var methods = objType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (methods == null)
            {
                return;
            }

            for (int i = 0; i < methods.Length; i++)
            {
                var attrs = methods[i].GetCustomAttributes(attrType, true);
                if (attrs == null || attrs.Length == 0)
                {
                    continue;
                }

                for (int j = 0; j < attrs.Length; j++)
                {
                    action((T)attrs[j], methods[i]);
                }
            }
        }
    }
}

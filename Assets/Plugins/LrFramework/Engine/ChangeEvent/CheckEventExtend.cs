using System.Collections;
using System.Collections.Generic;


public class CheckValuePool
{
    public static Dictionary<int, Dictionary<ushort, CheckValue>> values
            = new Dictionary<int, Dictionary<ushort, CheckValue>>();


    public class CheckValue
    {
        public string defaultValue = string.Empty;

        public bool IsCheckValue(string value)
        {
            if (defaultValue != value)
            {
                defaultValue  = value;
                return true;
            }
            return false;
        }
    }

    public static bool IsCheckValue(int id, string value, ushort uid = 0)
    {
        if (values.TryGetValue(id, out Dictionary<ushort, CheckValue> ushortValues))
        {
            if (ushortValues.TryGetValue(uid, out CheckValue check))
            {
                return check.IsCheckValue(value);
            }
            else
            {
                var checkNew= new CheckValue() { };
                ushortValues[uid] = checkNew;
                checkNew.IsCheckValue(value);
            }
        }
        else
        {
            var childs  = new Dictionary<ushort, CheckValue>();
            childs[uid] = new CheckValue() { };
            values[id]  = childs;
        }
        return false;
    }


    public static void Release(int id)
    {
        if (values.TryGetValue(id, out Dictionary<ushort, CheckValue> ushortValues))
        {
            values.Remove(id);
            ushortValues.Clear();
        }
    }
}


public static partial class Extend 
{
    public static bool CheckValue<T, U>(this T target, U value, ushort uid = 0)
    {
        return CheckValuePool.IsCheckValue(target.GetHashCode(), value.ToString(), uid);
    }

    public static void ChackRelease<T>(this T target)
    {
        CheckValuePool.Release(target.GetHashCode());
    }

    public static void AddCheckEvent<T>(this T target)
    {
        ChangeEventController.InitChangeEvent(target);
    }

    public static void RemoveCheckEvent<T>(this T target)
    {
        ChangeEventController.RemoveChangeEvent(target);
    }

}

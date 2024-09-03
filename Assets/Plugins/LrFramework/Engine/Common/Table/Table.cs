using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

namespace Table
{
    public abstract class Row<K>
    {
        public abstract K ID { get; }
    }

    [ProtoContract]
    public class DataSrc<K, V> where V : Row<K>
    {
        [ProtoMember(1)]
        public V[] array;
    }

    public class Table<K, V> where V : Row<K>
    {
        Dictionary<K, V> table;
        V[] array;

        public Table(DataSrc<K, V> dataSrc)
        {
            array = dataSrc.array;
            table = new Dictionary<K, V>(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                V row = array[i];
                table.Add(row.ID, row);
            }
        }

        public V[] GetArray()
        {
            return array;
        }

        public V GetRow(K id)
        {
            V t;
            table.TryGetValue(id, out t);
            return t;
        }

        public bool Contains(K id)
        {
            return table.ContainsKey(id);
        }
    }

}

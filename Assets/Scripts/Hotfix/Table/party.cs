
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class party : Row<int>
    {

        /*
        序号
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型
        */
        [ProtoMember(2)]
        public string type { get; set; }

        /*
        怪物ID
        */
        [ProtoMember(3)]
        public int enemyID { get; set; }

        /*
        怪物ID
        */
        [ProtoMember(4)]
        public float speed { get; set; }

        /*
        奖励数值
        */
        [ProtoMember(5)]
        public int hit { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, party> party { get; private set; }

    }
#endif
}

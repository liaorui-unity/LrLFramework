
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class taskSystem : Row<int>
    {

        /*
        怪物id
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型
        */
        [ProtoMember(2)]
        public string type { get; set; }

        /*
        数值数组
        */
        [ProtoMember(3)]
        public string types { get; set; }

        /*
        
        */
        [ProtoMember(4)]
        public string keys { get; set; }

        /*
        概率权重
        */
        [ProtoMember(5)]
        public int weight { get; set; }

        /*
        团队出现规则
        */
        [ProtoMember(6)]
        public int[] teamRule { get; set; }

        /*
        单体出现规则
        */
        [ProtoMember(7)]
        public int[] singleRule { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, taskSystem> taskSystem { get; private set; }

    }
#endif
}

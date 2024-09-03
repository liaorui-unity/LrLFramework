
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class littleTurntable : Row<int>
    {

        /*
        怪物id
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型（1-彩票；2-子弹）
        */
        [ProtoMember(2)]
        public int type { get; set; }

        /*
        数值
        */
        [ProtoMember(3)]
        public int num { get; set; }

        /*
        概率权重
        */
        [ProtoMember(4)]
        public int weight { get; set; }

        /*
        特效等级
        */
        [ProtoMember(5)]
        public int lv { get; set; }

        /*
        UI等级
        */
        [ProtoMember(6)]
        public int bg { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, littleTurntable> littleTurntable { get; private set; }

    }
#endif
}


using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class Task : Row<int>
    {

        /*
        序号
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        等级
（1-低级，2-中级，3-高级）
        */
        [ProtoMember(2)]
        public int level { get; set; }

        /*
        怪物1类型
（2-低级怪；1-特殊效果怪；0-高级怪）
        */
        [ProtoMember(3)]
        public int[] monster { get; set; }

        /*
        奖励类型
（1-彩票，2-奖励币，3-特殊球）
        */
        [ProtoMember(4)]
        public int rewardPro { get; set; }

        /*
        奖励数值

        */
        [ProtoMember(5)]
        public int rewardNum { get; set; }

        /*
        奖励星星数
        */
        [ProtoMember(6)]
        public int rewardStar { get; set; }

        /*
        出现概率权重
        */
        [ProtoMember(7)]
        public int Weight { get; set; }

        /*
        一个周期允许的次数
        */
        [ProtoMember(8)]
        public int count { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, Task> Task { get; private set; }

    }
#endif
}

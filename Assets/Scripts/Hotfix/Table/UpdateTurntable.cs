
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class UpdateTurntable : Row<int>
    {

        /*
        序号
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型
（1-彩票，2-游戏币，3-推盘奖励币，4-低级弹药，5-高级弹药，6-补充特殊球，7-补充JP球）
        */
        [ProtoMember(2)]
        public int type { get; set; }

        /*
        奖励数值
        */
        [ProtoMember(3)]
        public int num { get; set; }

        /*
        抽中权重
        */
        [ProtoMember(4)]
        public int[] percent { get; set; }

        /*
        奖励等级
        */
        [ProtoMember(5)]
        public int lv { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, UpdateTurntable> UpdateTurntable { get; private set; }

    }
#endif
}

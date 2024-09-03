
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class rePlayTurntable : Row<int>
    {

        /*
        序号
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型
（0-彩票，1-推盘奖励币，2-子弹，3-超级攻击球，4-小游戏球，5-JP球，6-宝箱派对，8-重玩jp，9-进入jp，10-小游戏1,11-小游戏2）
        */
        [ProtoMember(2)]
        public int type { get; set; }

        /*
        奖励数值
        */
        [ProtoMember(3)]
        public int num { get; set; }

        /*
        数值类型
        */
        [ProtoMember(4)]
        public int numType { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, rePlayTurntable> rePlayTurntable { get; private set; }

    }
#endif
}

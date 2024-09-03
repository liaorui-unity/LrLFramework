
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class shareTable : Row<int>
    {

        /*
        序号
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型
 (None=-1, Star = 0 星星，Lottery = 1 彩票，Task = 2 任务,Energy = 3 能量,Share = 4 共享,Ultimate = 5 终极,Bead = 6 宝珠,Jp = 7 章鱼大奖,Ball =8 补珠,BigBall =9 大额度补珠)
        */
        [ProtoMember(2)]
        public int[] type { get; set; }

        /*
        奖励数值
        */
        [ProtoMember(3)]
        public int[] num { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, shareTable> shareTable { get; private set; }

    }
#endif
}

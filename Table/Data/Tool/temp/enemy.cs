
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class enemy : Row<int>
    {

        /*
        序号
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        sign
（1-低级，2-中级，3-高级）
        */
        [ProtoMember(2)]
        public string key { get; set; }

        /*
        怪物ID
        */
        [ProtoMember(3)]
        public int enemyID { get; set; }

        /*
        怪物权重
        */
        [ProtoMember(4)]
        public int Weight { get; set; }

        /*
        能量条权重
        */
        [ProtoMember(5)]
        public int energyWeight { get; set; }

        /*
        奖励类型
 (None=-1, Star = 0 星星，Lottery = 1 彩票，Task = 2 任务,Energy = 3 能量,Share = 4 共享,Ultimate = 5 终极,Bead = 6 宝珠,Jp = 7 章鱼大奖,Ball =8 补珠,BigBall =9 大额度补珠)
        */
        [ProtoMember(6)]
        public int type { get; set; }

        /*
        奖励数值

        */
        [ProtoMember(7)]
        public int[] rewardNums { get; set; }

        /*
        奖励权重

        */
        [ProtoMember(8)]
        public int[] rewardWeights { get; set; }

        /*
        奖励能量
        */
        [ProtoMember(9)]
        public int rewardEnergy { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, enemy> enemy { get; private set; }

    }
#endif
}

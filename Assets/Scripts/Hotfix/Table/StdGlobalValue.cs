
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class StdGlobalValue : Row<int>
    {

        /*
        限时任务id
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        任务目标类型(1-怪物，2-任务)
        */
        [ProtoMember(2)]
        public int TaskType { get; set; }

        /*
        任务目标（怪物id，任务id）
        */
        [ProtoMember(3)]
        public int[] Tasks { get; set; }

        /*
        任务奖励类型(1-彩票，2-币，3-jp进度）
        */
        [ProtoMember(4)]
        public int AwardType { get; set; }

        /*
        奖励数值
        */
        [ProtoMember(5)]
        public int[] AwardValue { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, StdGlobalValue> StdGlobalValue { get; private set; }

    }
#endif
}

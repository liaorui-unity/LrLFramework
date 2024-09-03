
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class jpGame : Row<int>
    {

        /*
        节点
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励类型
（1-彩票，2-推盘奖励币，3-jp球）
        */
        [ProtoMember(2)]
        public int[] type { get; set; }

        /*
        奖励数值
        */
        [ProtoMember(3)]
        public int[] num { get; set; }

        /*
        
        */
        [ProtoMember(4)]
        public float[] Pro { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, jpGame> jpGame { get; private set; }

    }
#endif
}

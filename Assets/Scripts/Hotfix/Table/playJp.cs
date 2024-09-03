
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class playJp : Row<int>
    {

        /*
        序号
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        卡牌（0-剪刀，1-石头，2-布）
        */
        [ProtoMember(2)]
        public int[] Cards { get; set; }

        /*
        双倍卡牌
        */
        [ProtoMember(3)]
        public int doubleCard { get; set; }

        /*
        三倍卡牌(-1为不出现）
        */
        [ProtoMember(4)]
        public int tripleCard { get; set; }

        /*
        出现的回合数
        */
        [ProtoMember(5)]
        public int round { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, playJp> playJp { get; private set; }

    }
#endif
}

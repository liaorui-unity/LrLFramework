
using ProtoBuf;

namespace Table
{
    [ProtoContract]
    public class pushCoin : Row<int>
    {

        /*
        序号
        */
        [ProtoMember(1)]
        public int id { get; set; }

        /*
        奖励币
        */
        [ProtoMember(2)]
        public int coin { get; set; }

        /*
        奖励彩票
        */
        [ProtoMember(3)]
        public int ticket { get; set; }

        /*
        策划说明
        */
        [ProtoMember(4)]
        public string des { get; set; }

        public override int ID{ get { return id; } }
    }

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static Table<int, pushCoin> pushCoin { get; private set; }

    }
#endif
}

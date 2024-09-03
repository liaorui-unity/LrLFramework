
namespace Table
{

#if UNITY_2017_1_OR_NEWER
    public partial class TableLib
    {

        public static void InitTable()
        {

                TableManager.instance.LoadMethod<int,enemy>(typeof(TableLib));

                TableManager.instance.LoadMethod<int,taskSystem>(typeof(TableLib));

                TableManager.instance.LoadMethod<int,shareTable>(typeof(TableLib));

                TableManager.instance.LoadMethod<int,ultimateTable>(typeof(TableLib));

                TableManager.instance.LoadMethod<int,energy>(typeof(TableLib));

                TableManager.instance.LoadMethod<int,party>(typeof(TableLib));

        }

    }

#endif

}

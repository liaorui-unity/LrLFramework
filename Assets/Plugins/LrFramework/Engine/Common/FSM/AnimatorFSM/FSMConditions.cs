//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{
	public interface IFSMConditions<Key>
	{
		void Transfer(Key state);
		bool Condition(out Key resulf);
	}

	public class FSMConditions<Key> : IFSMConditions<Key>
	{
		bool judge;
		Key value;

		bool IFSMConditions<Key>.Condition(out Key resulf)
		{
			if (judge)
			{
				judge = false;
				resulf = value;
				return true;
			}
			else
			{
				resulf = FSMMonitor<Key>.DefaultKey;
				return false;
			}
		}

        void IFSMConditions<Key>.Transfer(Key state)
        {
			value = state;
			judge = true;
		}
    }

}

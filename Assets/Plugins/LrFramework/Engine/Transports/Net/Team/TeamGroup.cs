using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;

namespace Mirror
{

    public enum TeamOperate
    {
        Creat = 0,
        Replace,
        Cancel,
        Update
    }

    public enum GroupOperate
    {
        AddMember,
        ExitMember,
        Update,
    }

    public class Member
    {
        public Identify identify;
        public IdentifyGroup teamId;
        public NetConnect connect;
    }


    [Serializable]
    public class TeamGroup 
    {
        public TeamFrame_Respond frame;
        public IdentifyGroup identify;
        public List<Identify> members;

        public TeamGroup(IdentifyGroup identify)
        {
            this.identify = identify;
            frame  = new TeamFrame_Respond();
            members = new List<Identify>();
        }
    }



}

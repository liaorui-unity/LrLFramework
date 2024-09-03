using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

namespace Mirror
{


    [System.Serializable]
    [ProtoContract]
    public class Identify
    {
        [ProtoMember(1)] public System.Guid guid;
        [ProtoMember(2)] public string name;

        int _code;
        int Code
        {
            get
            {
                if (_code == default(int))
                {
                    _code = 23;
                    var guidValue = guid.ToString();
                    unchecked
                    {
                        foreach (char c in guidValue)
                            _code = _code * 31 + c;
                    }
                }
                return _code;
            }
        }


        public override bool Equals(object obj)
        {
            return (obj as Identify) == this;
        }
        public override int GetHashCode()
        {
            return Code;
        }

        public override string ToString()
        {
            return $"Guid => {guid} \n name => {name}";
        }

        public static bool operator ==(Identify a, Identify b)
        {
            if (a.guid == b.guid)
                return true;

            if (!string.IsNullOrEmpty(a.name) && !string.IsNullOrEmpty(b.name))
                if (a.name == b.name)
                    return true;

            return false;
        }
        public static bool operator !=(Identify a, Identify b)
        {
            if (a.guid == b.guid)
                return false;

            if (!string.IsNullOrEmpty(a.name) && !string.IsNullOrEmpty(b.name))
                if (a.name == b.name)
                    return false;

            return true;
        }
    }


    [System.Serializable]
    [ProtoContract]
    public class IdentifyGroup
    {
        [ProtoMember(1)] public int sort;
        [ProtoMember(2)] public string name;
        [ProtoMember(3)] public string match;

        public override bool Equals(object obj)
        {
            return (obj as IdentifyGroup) == this;
        }
        public override int GetHashCode()
        {
            return sort;
        }

        public override string ToString()
        {
            return $"sort => {sort} \n name => {name} \n match => {match}";
        }

        public static bool operator ==(IdentifyGroup a, IdentifyGroup b)
        {
            if (a.sort == b.sort && (a.sort != 0 || b.sort != 0))
                return true;

            if (!string.IsNullOrEmpty(a.name) && !string.IsNullOrEmpty(b.name))
                if (a.name == b.name)
                    return true;

            if (!string.IsNullOrEmpty(a.match) && !string.IsNullOrEmpty(b.match))
                if (a.match == b.match)
                    return true;


            if (a.sort == 0 && string.IsNullOrEmpty(a.name) && string.IsNullOrEmpty(a.match))
            {
                if (b.sort == 0 && string.IsNullOrEmpty(b.name) && string.IsNullOrEmpty(b.match))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool operator !=(IdentifyGroup a, IdentifyGroup b)
        {
            if (a.sort == b.sort && (a.sort != 0||b.sort!=0))
                return false;

            if (!string.IsNullOrEmpty(a.name) && !string.IsNullOrEmpty(b.name))
                if (a.name == b.name)
                    return false;

            if (!string.IsNullOrEmpty(a.match) && !string.IsNullOrEmpty(b.match))
                if (a.match == b.match)
                    return false;

            if (a.sort == 0 && string.IsNullOrEmpty(a.name) && string.IsNullOrEmpty(a.match))
            {
                if (b.sort == 0 && string.IsNullOrEmpty(b.name) && string.IsNullOrEmpty(b.match))
                {
                    return false;
                }
            }

            return true;
        }
    }

    [ProtoContract]
    public class TeamFrame_Respond : NetMessage
    {
        [ProtoMember(1)] public long count;
    }


    [ProtoContract]
    public class TeamStatus_Respond : NetMessage
    {
        [ProtoMember(1)] public IdentifyGroup identify;
        [ProtoMember(2)] public TeamFrame_Respond frame;
        [ProtoMember(3)] public List<MemberStatus> memberStatus;

        public override string ToString()
        {
            return $"identify=>{identify} \n";
        }
    }

    [ProtoContract]
    public class MemberStatus : NetMessage
    {
        [ProtoMember(1)] public Identify identify;
    }

    [ProtoContract]
    public class TeamOperateRequest : NetMessage
    {
        [ProtoMember(1)] public TeamOperate operate;
        [ProtoMember(2)] public Identify mebIdentify;

        public override string ToString()
        {
            return $"operate=>{operate} \n mebIdentify=>{mebIdentify}";
        }
    }

    [ProtoContract]
    public class TeamOperateRespond : NetMessage
    {
        [ProtoMember(1)] public TeamOperate operate;
        [ProtoMember(2)] public Identify mebIdentify;
        [ProtoMember(3)] public Dictionary<IdentifyGroup, TeamStatus_Respond> groups;

        public override string ToString()
        {
            return $"operate=>{operate} \n mebIdentify=>{mebIdentify} \n groups.count=>{groups.Count}";
        }
    }

    [ProtoContract]
    public class GroupOperateRequest : NetMessage
    {
        [ProtoMember(1)] public GroupOperate operate;
        [ProtoMember(2)] public Identify mebIdentify;
        [ProtoMember(3)] public IdentifyGroup teamIdentify;

        public override string ToString()
        {
            return $"operate=>{operate} \n mebIdentify=>{mebIdentify} \n teamIdentify=>{teamIdentify}";
        }
    }

    [ProtoContract]
    public class GroupOperateRespond : NetMessage
    {
        [ProtoMember(1)] public GroupOperate operate;
        [ProtoMember(2)] public Identify mebIdentify;
        [ProtoMember(3)] public Dictionary<IdentifyGroup, TeamStatus_Respond> groups;

        public override string ToString()
        {
            return $"operate=>{operate} \n mebIdentify=>{mebIdentify} \n groups.count=>{groups.Count}";
        }
    }
}


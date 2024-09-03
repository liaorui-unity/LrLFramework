using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;

namespace Mirror
{

 



    public class TeamServer : MonoBehaviour
    {
        public static GroupOperateRespond teamRefresh = new GroupOperateRespond();
        public static Dictionary<IdentifyGroup,TeamGroup>  groups = new Dictionary<IdentifyGroup, TeamGroup>();
        public static Dictionary<Identify,   Member>   memberIds  = new Dictionary<Identify, Member>();
        public static Dictionary<NetConnect, Identify> connectIds = new Dictionary<NetConnect, Identify>();

 
        public static TeamGroup FindTeamGroup(Identify identify)
        {
            if (memberIds.TryGetValue(identify, out Member meb))
            {
                if (groups.TryGetValue(meb.teamId, out TeamGroup team))
                {
                    return team;
                }
            }
            return null;
        }


       void Start()
        {    
            NetworkMsgServer.Register<TeamOperateRequest>(AcceptTeamMsg);
            NetworkMsgServer.Register<GroupOperateRequest>(AcceptGroupMsg);

            teamRefresh.groups = new Dictionary<IdentifyGroup, TeamStatus_Respond>();
        }



        public void AcceptTeamMsg(NetConnect connect, TeamOperateRequest operateMsg)
        {
            Debug.Log($"operateTeam:{operateMsg.operate}");
            switch (operateMsg.operate)
            {
                case TeamOperate.Creat:
                    Create(connect, operateMsg);
                    break;
                case TeamOperate.Cancel:
                    Cancel(connect, operateMsg);
                    break;
                case TeamOperate.Replace:
                    Replace(connect, operateMsg);
                    break;
                case TeamOperate.Update:
                    Refresh(connect, operateMsg);
                    break;
                default: break;
            }
        }

        public void AcceptGroupMsg(NetConnect connect, GroupOperateRequest operateMsg)
        {

            Debug.Log($"operateGroup:{operateMsg.operate}");
            switch (operateMsg.operate)
            {
                case GroupOperate.AddMember:
                    JoinTeam(connect,  operateMsg.mebIdentify, operateMsg.teamIdentify);
                    break;
                case GroupOperate.ExitMember:
                    ExitTeam(connect, operateMsg);
                    break;
                case GroupOperate.Update:
                    UpdateGroups(operateMsg.teamIdentify, operateMsg.mebIdentify, GroupOperate.Update);
                    break;
                default: break;
            }
        }



        public void Create (NetConnect connect, TeamOperateRequest request)
        {
            var group = FindTeamGroup(request.mebIdentify);

            if (group is null)
            {
                /// 处理 IdentifyGroup
                var team = new IdentifyGroup()
                {
                    sort = groups.Count,
                    name = request.mebIdentify.name,
                    match = groups.Count.ToString()
                };


                /// 处理 TeamGroup
                group = new TeamGroup(team);
                groups.Add(team, group);

                group.members = new List<Identify>();
                group.members.Add(request.mebIdentify);


                ///处理 memberIds 
                if (!memberIds.TryGetValue(request.mebIdentify, out Member member))
                {
                    member = new Member();
    
                    memberIds[request.mebIdentify] = member;
                }

                /// 自身 member 
                member.teamId   = team;
                member.identify = request.mebIdentify;
                member.connect = connect;

                UpdateTeams(connect, request, team);

            }
        }

        public void Replace(NetConnect connect, TeamOperateRequest request)
        {
            if (memberIds.TryGetValue(request.mebIdentify, out Member member))
            {
                if (groups.TryGetValue(member.teamId, out TeamGroup group))
                {
                    groups.Remove(member.teamId);
                    group.identify.name = request.mebIdentify.name;
                    groups[member.teamId] = group;

                    UpdateTeams(connect, request,member.teamId);
                }
            }
        }
     
        public void Cancel (NetConnect connect, TeamOperateRequest request)
        {
            if (memberIds.TryGetValue(request.mebIdentify, out Member member))
            {
                UpdateTeams(connect, request, member.teamId);
            }
        }

        public void Refresh(NetConnect connect, TeamOperateRequest request)
        {
            UpdateTeams(connect, request,null);
        }

     

        public void JoinTeam(NetConnect connect, Identify mebId,IdentifyGroup teamId)
        {
            if (groups.TryGetValue(teamId, out TeamGroup team))
            {
                if (!team.members.Contains(mebId))
                {
                    if (!memberIds.TryGetValue(mebId, out Member member))
                    {
                        member = new Member();
                        memberIds[mebId]= member;
                    }

                    member.identify = mebId;
                    member.connect  = connect;
                    member.teamId   = teamId;
                    team.members.Add(mebId);

                    UpdateGroups(teamId, mebId, GroupOperate.AddMember);
                }
            }
        }

        public void ExitTeam(NetConnect connect, GroupOperateRequest request)
        {
            if (groups.TryGetValue(request.teamIdentify, out TeamGroup team))
            {
                if (team.members.Contains(request.mebIdentify))
                {
                    UpdateGroups(request.teamIdentify, request.mebIdentify, GroupOperate.ExitMember);
                }

                if (team.members.Count <= 0)
                {
                    Debug.Log("不存在");

                    UpdateTeams(connect, new TeamOperateRequest()
                    {
                        operate = TeamOperate.Cancel,
                        mebIdentify= request.mebIdentify
                    }, request.teamIdentify);
                }
            }
        }



        public void Connected(NetConnect connect)
        {
           
        }

        public void Disconnect(NetConnect connect)
        {

        }


        void UpdateGroups(IdentifyGroup teamId, Identify mebId, GroupOperate operate)
        {
            if (groups.TryGetValue(teamId, out TeamGroup group))
            {
                if (teamRefresh.groups.TryGetValue(teamId, out TeamStatus_Respond respond))
                {
                    respond.frame    = group.frame;
                    respond.identify = group.identify;

                    switch (operate)
                    {
                        case GroupOperate.AddMember:
                            {
                                respond.memberStatus.Add(new MemberStatus()
                                {
                                    identify = mebId
                                });

                                var msgRespond = NetworkPacketPools.Get<GroupOperateRespond>();
                                msgRespond.operate = GroupOperate.AddMember;
                                msgRespond.groups = new Dictionary<IdentifyGroup, TeamStatus_Respond>();
                                msgRespond.groups[teamId] = respond;
                                msgRespond.mebIdentify = mebId;
                              
                                foreach (var item in group.members)
                                {
                                    var meb = memberIds[item];
                                    NetworkMsgServer.Send(meb.connect, msgRespond);
                                }
                            }
                            break;
                        case GroupOperate.ExitMember:
                            {
                                var state = respond.memberStatus
                                    .Where(_ => _.identify == mebId)
                                    .First();
                                respond.memberStatus.Remove(state);

                                var msgRespond = NetworkPacketPools.Get<GroupOperateRespond>();
                                msgRespond.operate = GroupOperate.ExitMember;
                                msgRespond.groups = new Dictionary<IdentifyGroup, TeamStatus_Respond>();
                                msgRespond.groups[teamId] = respond;
                                msgRespond.mebIdentify = mebId;

                                Debug.Log(group.members.Count);
                                foreach (var item in group.members)
                                {
                                    var meb = memberIds[item];
                                    NetworkMsgServer.Send(meb.connect, msgRespond);
                                }
                                group.members.Remove(mebId);
                            }
                            break;
                        case GroupOperate.Update:
                            {
                                respond.memberStatus.Clear();
                                foreach (var meb in group.members)
                                {
                                    respond.memberStatus.Add(new MemberStatus()
                                    {
                                        identify = meb
                                    });
                                }
                            }
                            break;
                        default:
                            break;
                    }
  
                } 
            }
        }

        void UpdateTeams (NetConnect connect, TeamOperateRequest request ,IdentifyGroup teamId)
        {
            switch (request.operate)
            {
                case TeamOperate.Creat:
                    {

                        if (!teamRefresh.groups.TryGetValue(teamId, out TeamStatus_Respond respond))
                        {
                            respond = new TeamStatus_Respond();
                            respond.frame = groups[teamId].frame;
                            respond.identify = groups[teamId].identify;
                            respond.memberStatus = new List<MemberStatus>();
                            respond.memberStatus.Add(new MemberStatus() 
                            {
                             identify= request.mebIdentify
                            });

                            var msgRespond = NetworkPacketPools.Get<TeamOperateRespond>();
                            msgRespond.operate = TeamOperate.Creat;
                            msgRespond.mebIdentify = request.mebIdentify;
                            msgRespond.groups  = new Dictionary<IdentifyGroup, TeamStatus_Respond>();
                            msgRespond.groups[teamId] = respond;

                            teamRefresh.groups[teamId]= respond;

                            Debug.Log(connect.valueId);

                            NetworkMsgServer.Send(connect, msgRespond);
                        }
                    }
                    break;
                case TeamOperate.Cancel:
                    {
                        if (teamRefresh.groups.TryGetValue(teamId, out TeamStatus_Respond respond))
                        {
                            respond.frame = null;
                            respond.identify = null;
                            respond.memberStatus.Clear();
                            teamRefresh.groups.Remove(teamId);
                        }

                        Debug.Log("取消："+teamId);

                        if (groups.TryGetValue(teamId, out TeamGroup group))
                        {
                            groups.Remove(teamId);

                            foreach (var item in group.members)
                            {
                                var meb = memberIds[item];
                                meb.teamId = null;

                                var msgRespond = NetworkPacketPools.Get<TeamOperateRespond>();
                                msgRespond.operate = TeamOperate.Cancel;
                                NetworkMsgServer.Send(meb.connect, msgRespond);
                            }

                            group.members.Clear();
                        }

                    }
                    break;

                case TeamOperate.Replace:
                    {
                        if (teamRefresh.groups.TryGetValue(teamId, out TeamStatus_Respond respond))
                        {
                            teamRefresh.groups.Remove(teamId);
                            var group = FindTeamGroup(request.mebIdentify);
                            respond.identify = group.identify;
                            teamRefresh.groups[respond.identify] = respond;
                        }
                    }
                    break;

                case TeamOperate.Update:
                    {
                        var msgRespond = NetworkPacketPools.Get<TeamOperateRespond>();
                        msgRespond.operate = TeamOperate.Update;
                        msgRespond.mebIdentify = request.mebIdentify;
                        msgRespond.groups = teamRefresh.groups;
              
                        Debug.Log("connect:"+connect.valueId);

                        foreach (var item in teamRefresh.groups)
                        {
                            Debug.Log($"{item.Value.identify}");
                        }

                        NetworkMsgServer.Send(connect, msgRespond);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

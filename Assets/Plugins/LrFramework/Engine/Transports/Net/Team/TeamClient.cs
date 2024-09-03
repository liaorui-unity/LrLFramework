using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Mirror
{
    public class TeamClient : MonoBehaviour
    {
        public TeamGroup selfGroup;
        public Identify selfIdentify;

        public IdentifyGroup teamIdentify;

        Subject<TeamOperateRespond>  waitOperate = new Subject<TeamOperateRespond>();
        Subject<GroupOperateRespond> waitGroupOperate = new Subject<GroupOperateRespond>();

        public Action<Identify> joinCall;
        public Action<Identify> exitCall;

        public  Dictionary<IdentifyGroup, TeamStatus_Respond> groups = new Dictionary<IdentifyGroup, TeamStatus_Respond>();

        void Start()
        {
            teamIdentify = new IdentifyGroup();
            selfIdentify = new Identify();
            selfIdentify.guid = Guid.NewGuid();
            selfIdentify.name = Guid.NewGuid().ToString();

            NetworkMsgClient.Register<TeamOperateRespond> (AcceptTeamOperateMsg);
            NetworkMsgClient.Register<GroupOperateRespond>(AcceptGroupOperateMsg);
        }



        public void AcceptTeamOperateMsg(TeamOperateRespond refreshMsg)
        {
            if (refreshMsg.mebIdentify == selfIdentify)
                waitOperate.OnNext(refreshMsg);

            switch (refreshMsg.operate)
            {
                case TeamOperate.Creat:   
                    break;
                case TeamOperate.Cancel:
                    break;
                case TeamOperate.Replace:
                    break;
                case TeamOperate.Update:
                    break;
                default:
                    break;
            }
        }

        public void AcceptGroupOperateMsg(GroupOperateRespond refreshMsg)
        {

            if (refreshMsg.mebIdentify == selfIdentify)
                waitGroupOperate.OnNext(refreshMsg);

            switch (refreshMsg.operate)
            {
                case GroupOperate.AddMember:
                    joinCall?.Invoke(refreshMsg.mebIdentify);
                    break;
                case GroupOperate.ExitMember:
                    exitCall?.Invoke(refreshMsg.mebIdentify);
                    break;
                case GroupOperate.Update:
                    break;
                default:
                    break;
            }
        }


        async Task<T> WaitServerRespond<T>(Subject<T> subject)
        {
            var disposable = WaitRespond(subject);
            var respond = await subject;
            disposable.Dispose();
            return respond;
        }

        IDisposable WaitRespond<T>(Subject<T> subject)
        {
            return Observable.Timer(TimeSpan.FromSeconds(10))
                    .Subscribe((_) =>
                    {
                        subject.OnNext(default);
                    });
        }


        public async Task<bool> Creat()
        {
            TeamOperateRequest msg = NetworkPacketPools.Get<TeamOperateRequest>();
            msg.operate        = TeamOperate.Creat;
            msg.mebIdentify    = selfIdentify;
            NetworkMsgClient.Send(msg);

            var respond = await WaitServerRespond(waitOperate);

            if (respond != null)
            {
                if (TeamOperate.Creat == respond.operate)
                {
                    Debug.Log(respond.groups.Count);
                    foreach (var item in respond.groups)
                    {
                        groups[item.Key] = item.Value;
                        selfGroup = new TeamGroup(item.Key);
                        selfGroup.frame = item.Value.frame;
                        selfGroup.identify = item.Value.identify;

                        teamIdentify = item.Key;

                        foreach (var member in item.Value.memberStatus)
                        {
                            selfGroup.members.Add(member.identify);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<bool> Cancel()
        {
            TeamOperateRequest msg = NetworkPacketPools.Get<TeamOperateRequest>();
            msg.operate = TeamOperate.Cancel;
            msg.mebIdentify = selfIdentify;
            NetworkMsgClient.Send(msg);

            var respond = await WaitServerRespond(waitOperate);

            if (respond != null)
            {
                if (TeamOperate.Cancel == respond.operate)
                {
                    groups.Remove(teamIdentify);
                    teamIdentify = null;

                    selfGroup.members.Clear();
                    selfGroup = null;
                }
                return true;
            }
            return false;
        }

        public async Task<bool> Join(IdentifyGroup identify)
        {
            GroupOperateRequest msg = NetworkPacketPools.Get<GroupOperateRequest>();
            msg.operate = GroupOperate.AddMember;
            msg.mebIdentify = selfIdentify;
            msg.teamIdentify = identify;
            NetworkMsgClient.Send(msg);

            var respond = await WaitServerRespond(waitGroupOperate);

            if (respond != null)
            {
                if (GroupOperate.AddMember == respond.operate)
                {
                  

                    foreach (var item in respond.groups)
                    {
                        groups[item.Key] = item.Value;
                        selfGroup = new TeamGroup(item.Key);
                        selfGroup.frame = item.Value.frame;
                        selfGroup.identify = item.Value.identify;

                        teamIdentify = item.Key;

                        foreach (var member in selfGroup.members)
                        {
                            selfGroup.members.Add(member);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<bool> Exit()
        {
            if (teamIdentify == new IdentifyGroup()) return false;

            GroupOperateRequest msg = NetworkPacketPools.Get<GroupOperateRequest>();
            msg.operate = GroupOperate.ExitMember;
            msg.mebIdentify  = selfIdentify;
            msg.teamIdentify = teamIdentify;
            NetworkMsgClient.Send(msg);

            Debug.Log("·¢ËÍ");

            var respond = await WaitServerRespond(waitGroupOperate);

            Debug.Log("respond:"+respond.operate);
            if (respond != null)
            {
                if (GroupOperate.ExitMember == respond.operate)
                {   
                    foreach (var item in respond.groups)
                    {
                        groups[item.Key] = item.Value;
                    }
                    teamIdentify = null;
                    selfGroup = null;
                }
                return true;
            }
            return false;
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Creat();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                UpdateTeam();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                MatchTeam();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Exit();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("±È¶Ô");
                Dictionary<Identify, string> valuePairs = new Dictionary<Identify, string>();
                Identify identify = new Identify() { name = "t" };
                Identify identify1 = new Identify() { name = "t" };
                valuePairs[identify]=("Y");

                if (valuePairs.ContainsKey(identify1))
                {
                    Debug.Log("g");
                }
            }
        }
        public void Replace()
        {
            TeamOperateRequest msg = new TeamOperateRequest();
            msg.operate = TeamOperate.Replace;
            msg.mebIdentify = selfIdentify;
            NetworkMsgClient.Send(msg);
        }

        public async Task<bool> UpdateTeam()
        {
            TeamOperateRequest msg = new TeamOperateRequest();
            msg.operate = TeamOperate.Update;
            msg.mebIdentify = selfIdentify;
            NetworkMsgClient.Send(msg);

            var respond = await WaitServerRespond(waitOperate);
            if (respond.operate == TeamOperate.Update)
            {
                groups.Clear();
                foreach (var item in respond.groups)
                {
                    groups[item.Key] = item.Value;
                }

                Debug.Log("groups:"+ groups.Count);
            }

            return respond.operate == TeamOperate.Update;
        }


        public async void MatchTeam()
        {
            if (teamIdentify == new IdentifyGroup())
            {
                var list = groups.Keys.ToList();
                var id = list[UnityEngine.Random.Range(0, list.Count)];
                Debug.Log(id.name);
                await Join(id);
            }
        }

        public void UpdateGroup()
        {
            GroupOperateRequest msg = new GroupOperateRequest();
            msg.operate = GroupOperate.Update;
            msg.mebIdentify = selfIdentify;
            NetworkMsgClient.Send(msg);
        }
    }
}

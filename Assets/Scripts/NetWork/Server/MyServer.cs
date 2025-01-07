//说是服务端实例，但其实只是形式上像
//由于框架的P2P方式,其实每个客户端都有一个服务端实例
//但是也只有在所有客户端中仅有一个而已


using FishNet;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GamePlay.Core;
using NetWork.Client;

namespace NetWork.Server
{
    /// <summary>
    /// 服务端实例
    /// </summary>
    public class MyServer : NetworkBehaviour
    {
        [AllowMutableSyncType] public readonly SyncVar<int> Seed = new();
        public static MyServer Instance { get; private set; }

        [Server]
        public void StartGame()
        {
            
        }

        [ServerRpc]
        public void HandleAddTouziRequest(int playerId, int id, int score, NetworkConnection conn = null)
        {
            //因为自己服务端也是客户端
            //所以可以先处理自己这一边，这样速度较快
            //当然，如果想规范一点的话可以在response统一处理所有客户端
            if (conn == Owner)
            {
                GameManager.Instance.AddTouzi(playerId, id, score);
            }
            else
            {
                GameManager.Instance.AddTouzi(MyTool.GetNextPlayerId(playerId), id, score);
            }

            MyClient.Instance.AddTouziResponse(playerId, id, score);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Instance = this;
            if (!IsServerStarted) gameObject.SetActive(false);
            Seed.Value = MyGlobal.CurSeed;
        }
    }
}
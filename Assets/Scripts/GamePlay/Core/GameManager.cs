// // ********************************************************************************************
// //     /\_/\                           @file       GameManager.cs
// //    ( o.o )                          @brief     Game07
// //     > ^ <                           @author     Basya
// //    /     \
// //   (       )                         @Modified   2024111820
// //   (___)___)                         @Copyright  Copyright (c) 2024, Basya
// // ********************************************************************************************

using System;
using System.Collections.Generic;
using GamePlay.Node;
using NetWork;
using UI.Panel;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GamePlay.Core
{
    public enum GameMode
    {
        Native = 0,
        Online = 1
    }

    public enum GameState
    {
        Idle = 0,
        Gaming = 1
    }

    public class GameManager : MonoBehaviour
    {
        public static GameState GameState { get; private set; } = GameState.Idle;
        public static GameMode GameMode { get; set; } = GameMode.Native;
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// 当前玩家Id
        /// </summary>
        public static int CurPlayerId => Instance.curPlayerId;

        /// <summary>
        /// 当前骰子点数大小
        /// </summary>
        public static int CurScore => Instance.curScore;

        [SerializeField] private int curPlayerId = 0;

        private int NextPlayerId => MyTool.GetNextPlayerId(curPlayerId);

        [SerializeField] private Sprite[] touzi;

        [SerializeField] private NodeQueueManager[] nodeQueueManagers;

        public IReadOnlyList<Sprite> Touzi => touzi;

        public IReadOnlyList<NodeQueueManager> NodeQueueManagers => nodeQueueManagers;

        [SerializeField] private int curScore;

        private void Awake()
        {
            Application.targetFrameRate = 9999;
            Debug.Log(NetWorkMgr.CloseServer()); //激活静态构造函数，测试用test
            Instance = this;
            for (int i = 0; i < nodeQueueManagers.Length; i++)
            {
                nodeQueueManagers[i].Init(i);
            }
            // StartGame(123);
        }

        private void Start()
        {
            // StartGame(123);
        }

        public void NextToPlayerId()
        {
            curPlayerId++;
            curPlayerId %= MyGlobal.MAX_PLAYER_COUNT;
        }

        public void StartGame(int seed)
        {
            GameState = GameState.Gaming;
            Random.InitState(seed);
            curScore = Random.Range(1, 7);
            // GameUIPanel.Instance.RollDiceAnimation(curScore);
            Debug.Log(GameUIPanel.Instance);
            HoleCard.Instance.HoleCardsInit();
        }

        /// <summary>
        /// 下一回合，更新玩家id，得到这次的骰子点数，播放动画
        /// </summary>
        public void NextTurn()
        {
            NextToPlayerId();
            curScore = Random.Range(1, 7);
            // GameUIPanel.Instance.RollDiceAnimation(curScore);
        }

        /// <summary>
        /// 添加骰子,将骰子添加到场上
        /// </summary>
        /// <param name="playerId">玩家id</param>
        /// <param name="id">第几行</param>
        /// <param name="score">骰子点数大小</param>
        public void AddTouzi(int playerId, int id, int score)
        {
            if (GameState == GameState.Idle) return;
            NodeQueueManager playerNodeQueueManager = nodeQueueManagers[playerId];
            if (!playerNodeQueueManager.AddTouzi(id, score)) return;
            GameUIPanel.Instance.UpdateScoreUI(curPlayerId, playerNodeQueueManager);
            if (RemoveTouzi(NextPlayerId, id, score))
                GameUIPanel.Instance.UpdateScoreUI(NextPlayerId, nodeQueueManagers[NextPlayerId]);
            if (playerNodeQueueManager.CheckIsGameOver())
            {
                GameOver();
                return;
            }

            NextTurn();
        }

        public void AddTouzi(int id)
        {
            AddTouzi(curPlayerId, id, curScore);
        }


        public bool RemoveTouzi(int playerId, int id, int score)
        {
            NodeQueueManager playerNodeQueueManager = nodeQueueManagers[playerId];
            return playerNodeQueueManager.RemoveTouzi(id, score);
        }

        private void GameOver()
        {
            Reset();
        }
        //重置
        private void Reset()
        {
            GameState = GameState.Idle;
            curScore = Random.Range(1, 7);
            curPlayerId = 0;
            foreach (var nodeQueueManager in nodeQueueManagers)
            {
                nodeQueueManager.Reset();
            }

            GameUIPanel.Instance.UpdateScoreUI(0, nodeQueueManagers[0]);
            GameUIPanel.Instance.UpdateScoreUI(1, nodeQueueManagers[1]);
        }

        #region Debug

        [Space(10)] [SerializeField] private int t1 = 0;
        [SerializeField] private int t2 = 0;
        [SerializeField] private int t3 = 0;

        [ContextMenu("startGame")]
        private void TestStartGame()
        {
            StartGame(123);
        } 

        [ContextMenu("add")]
        private void Test()
        {
            AddTouzi(t1, t2, t3);
        }

        [ContextMenu("add1")]
        private void Test1()
        {
            AddTouzi(curPlayerId, t2, t3);
        }

        [ContextMenu("clear")]
        private void Test2()
        {
            Reset();
        }

        #endregion
    }
}
// // ********************************************************************************************
// //     /\_/\                           @file       NodeQueue.cs
// //    ( o.o )                          @brief     Game07
// //     > ^ <                           @author     Basya
// //    /     \
// //   (       )                         @Modified   2024111619
// //   (___)___)                         @Copyright  Copyright (c) 2024, Basya
// // ********************************************************************************************

using System.Collections.Generic;
using DG.Tweening;
using GamePlay.Core;
using UnityEngine;
using UnityEngine.EventSystems;


namespace GamePlay.Node
{
    public class NodeQueue : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerExitHandler
    {
        public int playerId = -1;
        public int id = -1;
        private const float HOVER_SCALE_FACTOR = 1.1f;
        private static int MaxQueueNode => NodeQueueManager.MAX_QUEUE_NODE;
        [SerializeField] private int sumScore;

        public int SumScore
        {
            get => sumScore;
            private set => sumScore = value;
        }

        [SerializeField] private Vector3 initialScale;
        [SerializeField] private Transform[] nodePos;
        [SerializeField] private List<int> scores;
        [SerializeField] private List<GameObject> nodeObjs;
        public IReadOnlyList<int> Scores => scores;


        private readonly Dictionary<int, int> _scoreCounts = new();

        public void OnPointerEnter(PointerEventData data)
        {
            if (playerId != GameManager.Instance.CurPlayerId) return;
            transform.DOKill();
            transform.DOScale(initialScale * HOVER_SCALE_FACTOR, 0.3f);
        }

        public void OnPointerExit(PointerEventData data)
        {
            transform.DOKill();
            transform.DOScale(initialScale, 0.3f);
        }

        public void OnPointerDown(PointerEventData data)
        {
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (data.pointerCurrentRaycast.gameObject != gameObject || playerId != GameManager.Instance.CurPlayerId)
                return;
            GameManager.Instance.AddTouzi(id);
        }

        public bool AddNode(int score)
        {
            if (scores.Count == MaxQueueNode) return false;
            GameObject node = NodeFactory.CreateNode(score, nodePos[scores.Count]);
            nodeObjs.Add(node);
            score++;
            scores.Add(score);
            if (!_scoreCounts.TryAdd(score, 1))
            {
                _scoreCounts[score]++;
            }

            UpdateSumScore();
            return true;
        }

        public bool RemoveNode(int removedScore)
        {
            removedScore++;
            if (scores.Count == 0) return false;

            bool found = false;
            for (int i = scores.Count - 1; i >= 0; i--)
            {
                var curScore = scores[i];

                if (curScore == removedScore)
                {
                    Destroy(nodeObjs[i]);
                    nodeObjs.RemoveAt(i);
                    scores.RemoveAt(i);
                    found = true;
                }
                else
                {
                    if (_scoreCounts.TryGetValue(curScore, out int count))
                    {
                        if (count == 1)
                        {
                            _scoreCounts.Remove(curScore);
                        }
                        else
                        {
                            _scoreCounts[curScore]--;
                        }
                    }
                }
            }

            if (found)
            {
                UpdateSumScore();
            }

            return found;
        }

        public void Clear()
        {
            scores.Clear();
            for (int i = nodeObjs.Count - 1; i >= 0; i--)
            {
                Destroy(nodeObjs[i]);
            }
            _scoreCounts.Clear();
            nodeObjs.Clear();
        }

        private void UpdateSumScore()
        {
            SumScore = 0;
            foreach (var kv in _scoreCounts)
            {
                int score = kv.Key;
                int count = kv.Value;
                if (count >= 2)
                {
                    SumScore += score * count * count;
                }
                else
                {
                    SumScore += score;
                }
            }
        }
    }
}
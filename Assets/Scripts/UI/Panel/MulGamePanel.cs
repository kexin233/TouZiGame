// // ********************************************************************************************
// //     /\_/\                           @file       MulGamePanel.cs
// //    ( o.o )                          @brief     Game07
// //     > ^ <                           @author     Basya
// //    /     \
// //   (       )                         @Modified   2024111617
// //   (___)___)                         @Copyright  Copyright (c) 2024, Basya
// // ********************************************************************************************

using NetWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Panel
{
    public class MulGamePanel : BasePanel<MulGamePanel>
    {
        [SerializeField] private TMP_InputField ip;
        [SerializeField] private TMP_InputField port;

        public override void Init()
        {
            base.Init();
            GetControl<Button>("btn1").onClick.AddListener(CreateRoomBtn);
            GetControl<Button>("btn2").onClick.AddListener(JoinRoomBtn);
        }

        public void CreateRoomBtn()
        {
            string portText = port.text;
            if (string.IsNullOrEmpty(portText)) return;
            NetWorkMgr.CreateServer("test", ushort.Parse(portText));
        }

        public void JoinRoomBtn()
        {
            string portText = port.text;
            if (string.IsNullOrEmpty(portText)) return;
            NetWorkMgr.JoinServer(ip.text, ushort.Parse(portText));
        }
    }
}
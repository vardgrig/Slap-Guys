using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    bool startTimer = false;
    double startTime;
    double timer = 3;
    ExitGames.Client.Photon.Hashtable CustomValue;

    void Update()
    {
        if (!startTimer)
        {
            if (GameManager.instance.CheckLoadedPlayers())
            {
                startTimer = true;
            }
            return;
        }

        if (PhotonNetwork.Time - startTime >= timer)
        {
            Time.timeScale = 1;
        }
    }
    public void StartTimer()
    {
        Time.timeScale = 0;
        PhotonNetwork.LocalPlayer.isSceneLoaded = true;
        if (PhotonNetwork.IsMasterClient)
        {
            CustomValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            CustomValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
        }
    }
}

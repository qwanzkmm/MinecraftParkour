using System.Runtime.InteropServices;
using UnityEngine;

public class LeaderBoardController : MonoBehaviour
{
    [DllImport("__Internal")] private static extern void SetToLeaderboard(int value);

    public int passedLevelsAmount = 0;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("PassedLevelsAmount"))
        {
            passedLevelsAmount = PlayerPrefs.GetInt("PassedLevelsAmount");
        }
        SetToLeader(passedLevelsAmount);
    }

    public void SetToLeader(int value)
    {
#if !UNITY_EDITOR
        SetToLeaderboard(value);
#endif
    }
}

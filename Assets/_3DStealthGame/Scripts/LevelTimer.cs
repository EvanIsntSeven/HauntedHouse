using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LevelTimer : MonoBehaviour
{
[SerializeField]
private float levelTime = 50;
[SerializeField]
private TextMeshProUGUI timerDisplay;
private float timer;
public GameEnding gameEnding;

    void Start()
    {
        //I did 5 time trials, recording my times for each one, and I found 50 seconds gives the player enough time to complete the level in most scenarios unless they are taking their time without any sense of urgency. It is a stealth game after all, so I don't want the player to have to speedrun, it is jsut meant to encourage them to move forward without lingering since avoiding the enemies is very easy if you wait for them to be far away.
        timer = levelTime;
    }

    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer);
        }
        else
        {
            gameEnding.CaughtPlayer();
        }
    }

    void UpdateTimerDisplay(float time)
    {
        int timer = Mathf.FloorToInt(time);
        timerDisplay.text = timer.ToString();
    }
}

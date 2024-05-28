using UnityEngine;

public class GameManager:SingletonMonobehaviour<GameManager>
{
    //start is called before the first option screen
    protected override void Awake()
    {
        base.Awake();

        //TOOO:need a resolution setting option screen
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);
    }
}

public class CopyOfGameManager : SingletonMonobehaviour<CopyOfGameManager>
{
    //start is called before the first option screen
    protected override void Awake()
    {
        base.Awake();

        //TOOO:need a resolution setting option screen
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System;

public class TimeManager : SingletonMonobehaviour<TimeManager>,ISaveable
{
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";
    
    private bool gameClockPaused = false;

    private float gameTick = 0f;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHander.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHander.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHander.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHander.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    private void Start()
    {
        EventHander.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;

        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;

            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;

                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;

                    if (gameDay > 30)
                    {
                        gameDay = 1;
                        int gs = (int)gameSeason;
                        gs++;

                        gameSeason = (Season)gs;

                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;

                            gameYear++;

                            if (gameYear > 9999)
                            {
                                gameYear = 1;
                            }

                            EventHander.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }
                        EventHander.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHander.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                }
                EventHander.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }
            EventHander.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

        }
    }

    private string GetDayOfWeek()
    {
        int totalDays = ((int)gameSeason * 30) + gameDay;
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek) 
        {
            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wed";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            case 7:
                return "Sun";
            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);

        return gameTime;
    }

    public void TestAdvanceGameMinute()
    {
        for(int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for(int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        //Delete existing scene sace if exists
        GameObjectSave.sceneDate.Remove(Settings.PersistentScene);

        //Create new scnen save
        SceneSave sceneSave = new SceneSave();

        //Create new int dictionary
        sceneSave.intDictonary = new Dictionary<string, int>();

        //Create new string dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //Add values to the int dictionary
        sceneSave.intDictonary.Add("gameYear", gameYear);
        sceneSave.intDictonary.Add("gameDay", gameDay);
        sceneSave.intDictonary.Add("gameHour", gameHour);
        sceneSave.intDictonary.Add("gameMinute", gameMinute);
        sceneSave.intDictonary.Add("gameSecond", gameSecond);

        //Add values to the string dictionary
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        //Add scene save to game object for persistent scene
        GameObjectSave.sceneDate.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        //Get saved gameobject from gameSave data
        if (gameSave.gameObjectDate.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            //Get savedscene data for gameObject
            if (GameObjectSave.sceneDate.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                //if int and string dictionaries are found
                if (sceneSave.intDictonary != null && sceneSave.stringDictionary != null)
                {
                    //popilate saved int values
                    if (sceneSave.intDictonary.TryGetValue("gameYear", out int saveGameYear))
                        gameYear = saveGameYear;
                    if (sceneSave.intDictonary.TryGetValue("gameDay", out int saveGameDay))
                        gameDay = saveGameDay;
                    if (sceneSave.intDictonary.TryGetValue("gameHour", out int saveGameHour))
                        gameHour = saveGameHour;
                    if (sceneSave.intDictonary.TryGetValue("gameMinute", out int saveGameMinute))
                        gameMinute = saveGameMinute;
                    if (sceneSave.intDictonary.TryGetValue("gameSecond", out int saveGameSecond))
                        gameSecond = saveGameSecond;

                    //populate sreing saved values
                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string saveGameDayOfWeek))
                        gameDayOfWeek = saveGameDayOfWeek;

                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string saveGameSeason))
                    {
                        if (Enum.TryParse<Season>(saveGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    //Zero gametick
                    gameTick = 0f;

                    //Trigger advance minute event
                    EventHander.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {

    }

    public void ISaveableRestoreScene(string sceneName)
    {

    }
}

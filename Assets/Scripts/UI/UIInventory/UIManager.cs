using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    private bool _pauseMenuOn = false;
    [SerializeField] private UIINventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    //update is called once per frame
    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        //toggle pause menu if escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    private void EnablePauseMenu()
    {
        //destory any currently dragged item
        uiInventoryBar.DestroyCurrentlyDraggedItems();

        //clear currently selected item
        uiInventoryBar.ClearCurrentlySelectedItem();

        PauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        //trigger garbage collector,System.GC.Collect()方法是一种强制进行垃圾回收的手段
        System.GC.Collect();

        //highlight selected button
        HighlightButtonForSelectedTab();
    }


    public void DisablePauseMenu()
    {
        //destroy any currently dragged items
        pauseMenuInventoryManagement.DestoryCurrentlyDraggedItem();

        PauseMenuOn = false;
        Player.Instance.PlayerInputIsDisabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
    
    
    
    private void HighlightButtonForSelectedTab()
    {
        for(int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToAction(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInaction(menuButtons[i]);
            }

        }

    }

    private void SetButtonColorToInaction(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.disabledColor;

        button.colors = colors;
    }

    private void SetButtonColorToAction(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.pressedColor;

        button.colors = colors;
    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for(int i = 0; i < menuTabs.Length; i++)
        {
            if (i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }

        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementInput : MonoBehaviour

{
    public static ElementInput instance;
    public int startRow;
    public int startCol;
    public bool clockwise;

    public int row;
    public int column;

    private void Awake()
    {
        instance = this;
    }
    void OnMouseOver()
    {
       if (Input.GetMouseButtonDown(0)) // Clic gauche de la souris
        {

            GameManager.instance.ElementPressed(row, column);
       }

       
    }
  
    public void ButtonPressedQuadrant1Clockwise()
    {
        GameManager.instance.ButtonPressed(0, 0, true);
    }
    public void ButtonPressedQuadrant1AntiClockwise()
    {
        GameManager.instance.ButtonPressed(0, 0, false);
    }

    public void ButtonPressedQuadrant2Clockwise()
    {
        GameManager.instance.ButtonPressed(0, 3, true);
    }
    public void ButtonPressedQuadrant2AntiClockwise()
    {
        GameManager.instance.ButtonPressed(0, 3, false);
    }

    public void ButtonPressedQuadrant3Clockwise()
    {
        GameManager.instance.ButtonPressed(3, 0, true);
    }
    public void ButtonPressedQuadrant3AntiClockwise()
    {
        GameManager.instance.ButtonPressed(3, 0, false);
    }

    public void ButtonPressedQuadrant4Clockwise()
    {
        GameManager.instance.ButtonPressed(3, 3, true);
    }
    public void ButtonPressedQuadrant4AntiClockwise()
    {
        GameManager.instance.ButtonPressed(3, 3, false);
    }







}

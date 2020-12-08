/*
 * Author: Matteo
 * Last Modified by: Matteo
 * Date Last Modified: 2020-04-14
 * Program Description: Manages the icons that are used in Game
 * Revision History:
 *      - Initial Setup
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconController : MonoBehaviour
{
    /// <summary>
    /// The type of white cell we should display in this button
    /// </summary>
    public WhiteCellObject whiteCellProperty;

    /// <summary>
    /// This button image
    /// </summary>
    private Image _buttonImage;

    /// <summary>
    /// This button 
    /// </summary>
    private Button _button;

    private void OnEnable()
    {
        _buttonImage = GetComponent<Image>();
        _button = GetComponent<Button>();

        _button.onClick.AddListener(() => { GameController.SelectedWhiteCell = whiteCellProperty; });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if (whiteCellProperty == null)
            return;

        //_buttonImage.sprite = whiteCellProperty.cellSprite;
    }
}

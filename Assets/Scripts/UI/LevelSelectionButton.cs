/*
 * Author: Matteo
 * Last Modified by: Matteo
 * Date Last Modified: 2020-04-14
 * Program Description: Manages the levels that are used in Game
 * Revision History:
 *      - Initial Setup
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var button = GetComponent<Button>();

        Debug.Log("LevelSelectionButton-Start-GameController.maxUnlockedLevel: " + GameController.maxUnlockedLevel);
        Debug.Log("LevelSelectionButton-Start-gameObject.name: " + gameObject.name);

        if (gameObject.name != "Tutorial" && GameController.maxUnlockedLevel != "Tutorial")
        {
            var maxUnlockedLevel = Int32.Parse(GameController.maxUnlockedLevel.Substring(GameController.maxUnlockedLevel.LastIndexOf("_", StringComparison.Ordinal) + 1));
            var buttonLevel = Int32.Parse(gameObject.name.Substring(gameObject.name.LastIndexOf("_", StringComparison.Ordinal) + 1));

            if (buttonLevel <= maxUnlockedLevel)
                button.interactable = true;
        }

        var gameController = FindObjectOfType<GameController>();
        button.onClick.AddListener(() => gameController.SetLevel(gameObject.name));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

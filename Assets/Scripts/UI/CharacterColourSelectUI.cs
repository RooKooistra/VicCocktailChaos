using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColourSelectUI : MonoBehaviour
{
    [SerializeField] private Transform colourButtonTransform;

    private void Start()
    {
        var playerColourList = GameMultiplayer.Instance.GetPlayerColorList();

        foreach (var color in playerColourList)
        {
            Transform colourButton = Instantiate(colourButtonTransform);
            colourButton.SetParent(transform);

            colourButton.GetComponent<CharacterColourSelectSingleUI>().SetUpButton(color, playerColourList.IndexOf(color));
        }
    }
}

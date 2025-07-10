using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ModConfigMenu.Components;

public class InputFocusComponent : MonoBehaviour, IPointerClickHandler
{
    public TMP_InputField InputField;
    private GameObject lastSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!InputField.interactable || !InputField.isActiveAndEnabled)
            return;

        StartCoroutine(FocusNextFrame());
    }
    
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != lastSelected)
        {
            Plugin.Logger.LogDebug($"[SelectionChange] New selected object: {EventSystem.current.currentSelectedGameObject?.name}");
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventSystem.current.SetSelectedGameObject(InputField.gameObject);
            InputField.Select();
            InputField.ActivateInputField();
            Plugin.Logger.LogDebug("Manually forced selection via Spacebar");
        }
    }

    private IEnumerator FocusNextFrame()
    {
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame(); // wait until all UI scripts finish

        EventSystem.current.SetSelectedGameObject(InputField.gameObject);
        InputField.OnSelect(null); // force Unity lifecycle
        InputField.Select();
        InputField.ActivateInputField();

        Plugin.Logger.LogDebug($"[InputFocus] Focus triggered. isFocused: {InputField.isFocused}, selected: {EventSystem.current.currentSelectedGameObject?.name}");
    }
}
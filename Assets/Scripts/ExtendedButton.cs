using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Класс реализует расширение стандартной кнопки UnityEngine.UI.Button
/// </summary>
public class ExtendedButton : Button
{
	public bool IsPressed; // Кнопка в данный момент зажата

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		if (interactable)
			IsPressed = true;
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		IsPressed = false;
	}
}
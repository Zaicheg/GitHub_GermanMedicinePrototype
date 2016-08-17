using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Метод управляет графическим интерфейсом
/// </summary>
public class GUIManager : Singleton<GUIManager>
{
	protected GUIManager() { }

	public Toggle EdgesLengthOneToggle;
	public Toggle EdgesLengthTwoToggle;
	public Toggle EdgesLengthThreeToggle;

	public ExtendedButton CameraControl_RotateLeftButton;
	public ExtendedButton CameraControl_RotateRightButton;
	public ExtendedButton CameraControl_RotateDownButton;
	public ExtendedButton CameraControl_RotateUpButton;
	public ExtendedButton CameraControl_ZoomOutButton;
	public ExtendedButton CameraControl_ZoomInButton;

	public RectTransform ContextMenu;
	public Button ContextMenu_DeleteButton;

	public Text BudgetText;

	public Button ResetButton;

	public Color selectedItemTextColor;

	/// <summary>
	/// Метод обрабатывает событие OnEnable
	/// </summary>
	public void OnEnableEvent()
	{
		InitElements();
	}

	/// <summary>
	/// Метод обрабатывает событие Update
	/// </summary>
	public void UpdateEvent()
	{
		CheckRepeatButtons();
	}

	/// <summary>
	/// Метод инициализирует элементы интерфейса
	/// </summary>
	private void InitElements()
	{
		EdgesLengthOneToggle.onValueChanged.AddListener(EdgesLengthOneToggleOnValueChangedEvent);
		EdgesLengthTwoToggle.onValueChanged.AddListener(EdgesLengthTwoToggleOnValueChangedEvent);
		EdgesLengthThreeToggle.onValueChanged.AddListener(EdgesLengthThreeToggleOnValueChangedEvent);

		ContextMenu_DeleteButton.onClick.AddListener(ContextMenu_DeleteButtonOnClickEvent);

		ResetButton.onClick.AddListener(ResetButtonOnClickEvent);
	}

	/// <summary>
	/// Метод проверяет состояние зажимаемых кнопок
	/// </summary>
	private void CheckRepeatButtons()
	{
		const float rotateSpeed = 2f;
		const float zoomSpeed = 0.005f;

		if (CameraControl_RotateLeftButton.IsPressed)
			CameraManager.Instance.DragMouseScript.Rotate(-1f * rotateSpeed, 0f);
		if (CameraControl_RotateRightButton.IsPressed)
			CameraManager.Instance.DragMouseScript.Rotate(1f * rotateSpeed, 0f);
		if (CameraControl_RotateDownButton.IsPressed)
			CameraManager.Instance.DragMouseScript.Rotate(0f * rotateSpeed, -1f);
		if (CameraControl_RotateUpButton.IsPressed)
			CameraManager.Instance.DragMouseScript.Rotate(0f * rotateSpeed, 1f);

		if (CameraControl_ZoomOutButton.IsPressed)
			CameraManager.Instance.DragMouseScript.Zoom(-1f * zoomSpeed);
		if (CameraControl_ZoomInButton.IsPressed)
			CameraManager.Instance.DragMouseScript.Zoom(1f * zoomSpeed);
	}

	/// <summary>
	/// Метод обрабатывает нажатие на кнопку "400 мм"
	/// </summary>
	private void EdgesLengthOneToggleOnValueChangedEvent(bool value)
	{
		General.Instance.SelectEdgesLength(400f);
	}

	/// <summary>
	/// Метод обрабатывает нажатие на кнопку "500 мм"
	/// </summary>
	private void EdgesLengthTwoToggleOnValueChangedEvent(bool value)
	{
		General.Instance.SelectEdgesLength(500f);
	}

	/// <summary>
	/// Метод обрабатывает нажатие на кнопку "700 мм"
	/// </summary>
	private void EdgesLengthThreeToggleOnValueChangedEvent(bool value)
	{
		General.Instance.SelectEdgesLength(700f);
	}

	/// <summary>
	/// Метод обрабатывает нажатие кнопки "Reset"
	/// </summary>
	private void ResetButtonOnClickEvent()
	{
		General.Instance.Reset();
	}

	/// <summary>
	/// Метод обрабатывает нажатие "Удалить" в контекстном меню
	/// </summary>
	private void ContextMenu_DeleteButtonOnClickEvent()
	{
		General.Instance.DeleteSelectedItem();
		General.Instance.CloseContextMenu();
	}

	/// <summary>
	/// Метод обрабатывает событие "изменена смета"
	/// </summary>
	public void BudgetChangedEvent(string text)
	{
		BudgetText.text = text;
	}

	/// <summary>
	/// Метод обрабатывает событие "открытие/закрытие контекстного меню"
	/// </summary>
	/// <param name="value">Значение</param>
	public void ContextMenuStateChanged(bool value)
	{
		ContextMenu.gameObject.SetActive(value);

		if (value)
		{
			ContextMenu.transform.position = new Vector2(
				Input.mousePosition.x + ContextMenu.sizeDelta.x / 2f,
				Input.mousePosition.y - ContextMenu.sizeDelta.y / 2f);
		}
	}
}

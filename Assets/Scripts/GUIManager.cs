using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Метод управляет графическим интерфейсом
/// </summary>
public class GUIManager : Singleton<GUIManager>
{
	protected GUIManager() { }

	public Button EdgesLengthOneButton;
	public Button EdgesLengthTwoButton;
	public Button EdgesLengthThreeButton;

	public ExtendedButton CameraControl_RotateLeftButton;
	public ExtendedButton CameraControl_RotateRightButton;
	public ExtendedButton CameraControl_RotateDownButton;
	public ExtendedButton CameraControl_RotateUpButton;
	public ExtendedButton CameraControl_ZoomOutButton;
	public ExtendedButton CameraControl_ZoomInButton;

	public Text BudgetText;

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
		EdgesLengthOneButton.onClick.AddListener(EdgesLengthOneButtonOnClickEvent);
		EdgesLengthTwoButton.onClick.AddListener(EdgesLengthTwoButtonOnClickEvent);
		EdgesLengthThreeButton.onClick.AddListener(EdgesLengthThreeButtonOnClickEvent);
	}

	/// <summary>
	/// Метод проверяет состояние зажимаемых кнопок
	/// </summary>
	private void CheckRepeatButtons()
	{
		if (CameraControl_RotateLeftButton.IsPressed)
			CameraManager.Instance.RotateCamera(new Vector2(-1f, 0f));
		if (CameraControl_RotateRightButton.IsPressed)
			CameraManager.Instance.RotateCamera(new Vector2(1f, 0f));
		if (CameraControl_RotateDownButton.IsPressed)
			CameraManager.Instance.RotateCamera(new Vector2(0f, -1f));
		if (CameraControl_RotateUpButton.IsPressed)
			CameraManager.Instance.RotateCamera(new Vector2(0f, 1f));

		if (CameraControl_ZoomOutButton.IsPressed)
			CameraManager.Instance.ZoomCamera(-1f);
		if (CameraControl_ZoomInButton.IsPressed)
			CameraManager.Instance.ZoomCamera(1f);
	}

	/// <summary>
	/// Метод обрабатывает нажатие на кнопку "400 мм"
	/// </summary>
	private void EdgesLengthOneButtonOnClickEvent()
	{
		General.Instance.SelectEdgesLength(400f);
	}

	/// <summary>
	/// Метод обрабатывает нажатие на кнопку "500 мм"
	/// </summary>
	private void EdgesLengthTwoButtonOnClickEvent()
	{
		General.Instance.SelectEdgesLength(500f);
	}

	/// <summary>
	/// Метод обрабатывает нажатие на кнопку "700 мм"
	/// </summary>
	private void EdgesLengthThreeButtonOnClickEvent()
	{
		General.Instance.SelectEdgesLength(700f);
	}

	/// <summary>
	/// Метод обрабатывает событие "длина ребра изменена"
	/// </summary>
	/// <param name="edgesLength"></param>
	public void EdgesLengthOnChangedEvent(float edgesLength)
	{
		EdgesLengthOneButton.GetComponentInChildren<Text>().color = EdgesLengthTwoButton.GetComponentInChildren<Text>().color = EdgesLengthThreeButton.GetComponentInChildren<Text>().color = Color.white;

		if (edgesLength == 400f)
			EdgesLengthOneButton.GetComponentInChildren<Text>().color = selectedItemTextColor;
		if (edgesLength == 500f)
			EdgesLengthTwoButton.GetComponentInChildren<Text>().color = selectedItemTextColor;
		if (edgesLength == 700f)
			EdgesLengthThreeButton.GetComponentInChildren<Text>().color = selectedItemTextColor;
	}

	/// <summary>
	/// Метод обрабатывает событие "изменена смета"
	/// </summary>
	public void BudgetChangedEvent(string text)
	{
		BudgetText.text = text;
	}
}

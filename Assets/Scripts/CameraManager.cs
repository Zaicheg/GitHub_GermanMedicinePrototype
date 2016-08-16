using UnityEngine;
using System.Collections;

/// <summary>
/// Класс управляет камерами
/// </summary>
public class CameraManager : Singleton<CameraManager>
{
	protected CameraManager() { }

	public Camera Spectator;
	public DragMouseOrbitCSharp DragMouseScript;

	/// <summary>
	/// Метод для расчёта отступа левого края камеры
	/// </summary>
	public RectTransform CanvasRect;
	public RectTransform MainMenuRect;

	/*
	/// <summary>
	/// Метод обрабатывает событие Update
	/// </summary>
	public void UpdateEvent()
	{
		FrustrumOffset();
		//LookAtCamera();
	}

	/// <summary>
	/// Метод настраивает положение камеры на экране
	/// </summary>
	public void FrustrumOffset()
	{
		var scaleFactor = CanvasRect.localScale.x * MainMenuRect.rect.width;
		var screenWidth = (float)Screen.width;
		var rectOffset = scaleFactor / (screenWidth / 100f) / 100f;
		Spectator.rect = new Rect(rectOffset, 0f, 1f, 1f);
	}*/

	/// <summary>
	/// Метод фокусирует камеру на определенной точке
	/// </summary>
	public void LookAtCamera()
	{
		Spectator.transform.LookAt(General.Instance.GetCenter());
	}

	/// <summary>
	/// Метод ставит камеру в дефолтную позицию
	/// </summary>
	public void MoveSpectatorInDefaultPosition()
	{
		const float distance = 0.5f;

		Spectator.transform.position = General.Instance.GetCenter();
		Spectator.transform.position += Vector3.forward * distance;
		Spectator.transform.position += Vector3.right * distance;
		Spectator.transform.position += Vector3.up * distance;
		Spectator.transform.LookAt(General.Instance.GetCenter());

		DragMouseScript.Target.position = Vector3.zero;
	}
}

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

	//public void UpdateEvent()
	//{
	//	LookAtCamera();
	//	AutoZoomCamera();
	//}

	/// <summary>
	/// Метод фокусирует камеру на определенной точке
	/// </summary>
	public void LookAtCamera()
	{
		DragMouseScript.Target.position = General.Instance.GetCenter();
	}

	public void AutoZoomCamera()
	{
		var maxOffset = Mathf.NegativeInfinity;

		foreach (var node in General.Instance.Nodes)
		{
			var a = Spectator.WorldToScreenPoint(node.transform.position);
			if (a.x > Screen.width)
				a.x = a.x - Screen.width;
			else
				a.x = 0f;
			if (a.y > Screen.height)
				a.y = a.y - Screen.height;
			else
				a.y = 0f;

			if (Mathf.Abs(a.x) > maxOffset)
				maxOffset = a.x;
			if (Mathf.Abs(a.y) > maxOffset)
				maxOffset = a.y;
		}

		if (maxOffset > 1f)
			DragMouseScript.Zoom(-1f * (Screen.width / maxOffset) * 0.3f);
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

using UnityEngine;
using System.Collections;

/// <summary>
/// Класс управляет камерами
/// </summary>
public class CameraManager : Singleton<CameraManager>
{
	protected CameraManager() { }

	public Camera Spectator;

	/// <summary>
	/// Метод для расчёта отступа левого края камеры
	/// </summary>
	public RectTransform CanvasRect;
	public RectTransform MainMenuRect;

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
	}

	/// <summary>
	/// Метод поворачивает камеру
	/// </summary>
	/// <param name="direction"></param>
	public void RotateCamera(Vector2 direction)
	{
		var targetPoint = Vector3.zero;

		const float speedX = 2f;
		const float speedY = 1f;

		var cameraAngleX = Spectator.transform.eulerAngles.y;
		var cameraAngleY = Spectator.transform.eulerAngles.x;

		cameraAngleX += direction.x * speedX;
		cameraAngleY += direction.y * speedY;

		if (cameraAngleY < -360f)
			cameraAngleY += 360f;
		if (cameraAngleY > 360f)
			cameraAngleY -= 360f;

		var distance = Vector3.Distance(Spectator.transform.position, targetPoint);
		var rotation = Quaternion.Euler(cameraAngleY, cameraAngleX, 0);
		var position = rotation * new Vector3(0f, 0f, -distance) + targetPoint;

		Spectator.transform.rotation = rotation;
		Spectator.transform.position = position;
	}

	/// <summary>
	/// Метод зумирует камеру
	/// </summary>
	/// <param name="direction"></param>
	public void ZoomCamera(float direction)
	{
		Spectator.transform.position += Spectator.transform.forward.normalized * Time.deltaTime * 1f * direction;
	}

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
		const float distance = 0.1f;

		Spectator.transform.position = General.Instance.GetCenter();
		Spectator.transform.position += Vector3.forward * distance;
		Spectator.transform.position += Vector3.right * distance;
		Spectator.transform.position += Vector3.up * distance;
		Spectator.transform.LookAt(General.Instance.GetCenter());
	}
}

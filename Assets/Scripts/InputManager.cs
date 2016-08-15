using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputManager : Singleton<InputManager>
{
	protected InputManager() { }

	private MouseRaycastResult _mouseRaycastResult; // Результат рейкаста по курсору

	public void UpdateEvent()
	{
		GetKeyboardInput();
		GetMouseInput();
	}

	#region Приём ввода через клавиатуру

	/// <summary>
	/// Метод принимает ввод с клавиатуры
	/// </summary>
	private void GetKeyboardInput()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			CameraManager.Instance.RotateCamera(new Vector2(-1f, 0f));
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			CameraManager.Instance.RotateCamera(new Vector2(1f, 0f));
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			CameraManager.Instance.RotateCamera(new Vector2(0f, -1f));
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			CameraManager.Instance.RotateCamera(new Vector2(0f, 1f));
		}
		if (Input.GetKey(KeyCode.PageDown))
		{
			CameraManager.Instance.ZoomCamera(-1f);
		}
		if (Input.GetKey(KeyCode.PageUp))
		{
			CameraManager.Instance.ZoomCamera(1f);
		}
	}

	#endregion

	#region Приём ввода через мышь
	
	/// <summary>
	/// Метод принимает ввод с мыши
	/// </summary>
	private void GetMouseInput()
	{
		if (Input.GetMouseButtonDown(0))
			MouseLeftDown();

		if (Input.GetMouseButton(1))
			MouseRightStay();

		GetMouseScroll();
	}

	/// <summary>
	/// Метод принимает ввод с колесика мыши
	/// </summary>
	private void GetMouseScroll()
	{
		var scrollValue = Input.GetAxis("Mouse ScrollWheel");
		if (scrollValue != 0f)
			CameraManager.Instance.ZoomCamera(Mathf.Sign(scrollValue) * 8f);
	}

	/// <summary>
	/// Ввод при нажатии ЛКМ
	/// </summary>
	private void MouseLeftDown()
	{
		_mouseRaycastResult = MouseRaycast(new List<RaycastedObjectType>() { RaycastedObjectType.NodeSide });
		if (_mouseRaycastResult == null)
			return;

		if (_mouseRaycastResult.Type == RaycastedObjectType.NodeSide)
			General.Instance.CreateEdgeForNodeSide(_mouseRaycastResult.GameObject.GetComponent<NodeSide>());
	}

	/// <summary>
	/// Ввод при зажатии ПКМ
	/// </summary>
	private void MouseRightStay()
	{
		var horizontalValue = Input.GetAxis("Mouse X");
		var verticalValue = Input.GetAxis("Mouse Y");

		if (horizontalValue != 0f)
			CameraManager.Instance.RotateCamera(new Vector2(Mathf.Sign(horizontalValue), 0f));
		if (verticalValue != 0f)
			CameraManager.Instance.RotateCamera(new Vector2(0f, -Mathf.Sign(verticalValue)));
	}

	#endregion

	#region Вспомогательные методы

	/// <summary>
	/// Метод реализует рейкаст от курсора в сцену с определением того, объект какого типа будет подхвачен
	/// </summary>
	/// <param name="priorities">Приоритеты слоёв</param>
	/// <returns>Результат рейкаста</returns>
	private MouseRaycastResult MouseRaycast(List<RaycastedObjectType> priorities)
	{
		return MouseRaycast(priorities, new List<GameObject>());
	}

	/// <summary>
	/// Метод реализует рейкаст от курсора в сцену с определением того, объект какого типа будет подхвачен
	/// </summary>
	/// <param name="priorities">Приоритеты слоёв</param>
	/// <param name="ignoredObjects">Игнорируемые геймобъекты</param>
	/// <returns>Результат рейкаста</returns>
	private MouseRaycastResult MouseRaycast(List<RaycastedObjectType> priorities, List<GameObject> ignoredObjects)
	{
		//Debug.Log("MouseRaycast");

		var targetCamera = CameraManager.Instance.Spectator;

		var mousePositionOnScreen = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		if (CheckMouseInCamera(targetCamera) == false)
			return null;

		var ray = targetCamera.ScreenPointToRay(mousePositionOnScreen);
		var hits = Physics.RaycastAll(ray, Mathf.Infinity);
		//Array.Reverse(hits);

		MouseRaycastResult result = null;

		foreach (var hit in hits)
		{
			if (ignoredObjects.Contains(hit.transform.gameObject))
				continue;

			var resultTemp = new MouseRaycastResult();

			var type = RaycastedObjectType.None;
			var layer = hit.transform.gameObject.layer;
			if (layer == LayerMask.NameToLayer("NodeSide"))
				type = RaycastedObjectType.NodeSide;
			resultTemp.Type = type;
			resultTemp.GameObject = hit.transform.gameObject;
			resultTemp.Hit = hit;

			if (priorities.Contains(resultTemp.Type) &&
				(result == null ||
				priorities.IndexOf(resultTemp.Type) < priorities.IndexOf(result.Type) ||
				(priorities.IndexOf(resultTemp.Type) == priorities.IndexOf(result.Type) && Vector3.Distance(result.Hit.point, targetCamera.transform.position) > Vector3.Distance(resultTemp.Hit.point, targetCamera.transform.position))))
				result = resultTemp;
		}

		return result;
	}

	/// <summary>
	/// Метод проверяет, находится ли курсор мыши в границах камеры
	/// </summary>
	/// <param name="targetCamera">Целевая камера</param>
	/// <returns>Результат проверки</returns>
	private bool CheckMouseInCamera(Camera targetCamera)
	{
		var mousePositionOnScreen = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		var mousePositionOnViewport = targetCamera.ScreenToViewportPoint(mousePositionOnScreen);

		return !(mousePositionOnViewport.x < 0f) && !(mousePositionOnViewport.x > 1f) && !(mousePositionOnViewport.y < 0f) && !(mousePositionOnViewport.y > 1f);
	}
	#endregion
}

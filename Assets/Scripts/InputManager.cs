using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputManager : Singleton<InputManager>
{
	protected InputManager() { }

	private MouseRaycastResult _mouseRaycastResult; // Результат рейкаста по курсору

	private GameObject MouseDownGameObject;

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
		const float rorateSpeed = 2f;
		const float zoomSpeed = 0.005f;

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			CameraManager.Instance.DragMouseScript.Rotate(-1f * rorateSpeed, 0f);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			CameraManager.Instance.DragMouseScript.Rotate(1f * rorateSpeed, 0f);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			CameraManager.Instance.DragMouseScript.Rotate(0f, -1f * rorateSpeed);
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			CameraManager.Instance.DragMouseScript.Rotate(0f, 1f * rorateSpeed);
		}
		if (Input.GetKey(KeyCode.PageDown))
		{
			CameraManager.Instance.DragMouseScript.Zoom(-1f * zoomSpeed);
		}
		if (Input.GetKey(KeyCode.PageUp))
		{
			CameraManager.Instance.DragMouseScript.Zoom(1f * zoomSpeed);
		}
	}

	#endregion

	#region Приём ввода через мышь

	/// <summary>
	/// Метод принимает ввод с мыши
	/// </summary>
	private void GetMouseInput()
	{
		if (General.Instance.ContextMenuOpened)
		{
			if (Input.GetMouseButtonUp(0))
				General.Instance.CloseContextMenu();

			return;
		}

		if (Input.GetMouseButtonDown(0))
			MouseLeftDown();

		if (Input.GetMouseButton(0))
			MouseLeftStay();

		if (Input.GetMouseButtonUp(0))
			MouseLeftUp();

		if (Input.GetMouseButtonDown(1))
			MouseRightDown();

		if (Input.GetMouseButtonUp(1))
			MouseRightUp();

		GetMouseScroll();
	}

	/// <summary>
	/// Метод обрабатывает событие "Прокрутка колесика мыши"
	/// </summary>
	private void GetMouseScroll()
	{
		var scrollValue = Input.GetAxis("Mouse ScrollWheel");
		if (scrollValue != 0f)
			CameraManager.Instance.DragMouseScript.Zoom(scrollValue);
	}

	/// <summary>
	/// Метод обрабатывает событие "Нажали ЛКМ"
	/// </summary>
	private void MouseLeftDown()
	{
		_mouseRaycastResult = MouseRaycast(new List<RaycastedObjectType>() { RaycastedObjectType.NodeSide });
		if (_mouseRaycastResult == null)
			return;

		MouseDownGameObject = _mouseRaycastResult.GameObject;
	}

	/// <summary>
	/// Метод обрабатывает событие "Зажали ЛКМ"
	/// </summary>
	private void MouseLeftStay()
	{
		const float rotateSpeed = 25f;

		var horizontalValue = Input.GetAxis("Mouse X");
		var verticalValue = Input.GetAxis("Mouse Y");

		if (horizontalValue != 0f)
			CameraManager.Instance.DragMouseScript.Rotate(horizontalValue * rotateSpeed, 0f);
		if (verticalValue != 0f)
			CameraManager.Instance.DragMouseScript.Rotate(0f, verticalValue * rotateSpeed);
	}


	/// <summary>
	/// Метод обрабатывает событие "Отпустили ЛКМ"
	/// </summary>
	private void MouseLeftUp()
	{
		_mouseRaycastResult = MouseRaycast(new List<RaycastedObjectType>() { RaycastedObjectType.NodeSide });
		if (_mouseRaycastResult == null)
			return;

		if (_mouseRaycastResult.GameObject != MouseDownGameObject)
			return;

		if (_mouseRaycastResult.Type == RaycastedObjectType.NodeSide)
			General.Instance.CreateEdgeForNodeSide(_mouseRaycastResult.GameObject.GetComponent<NodeSide>());
	}

	/// <summary>
	/// Метод обрабатывает событие "Нажали ПКМ"
	/// </summary>
	private void MouseRightDown()
	{
		_mouseRaycastResult = MouseRaycast(new List<RaycastedObjectType>() { RaycastedObjectType.NodeSide, RaycastedObjectType.Edge });
		if (_mouseRaycastResult == null)
			return;

		MouseDownGameObject = _mouseRaycastResult.GameObject;
	}

	/// <summary>
	/// Метод обрабатывает событие "Отпустили ПКМ"
	/// </summary>
	private void MouseRightUp()
	{
		_mouseRaycastResult = MouseRaycast(new List<RaycastedObjectType>() { RaycastedObjectType.NodeSide, RaycastedObjectType.Edge });
		if (_mouseRaycastResult == null)
			return;

		if (_mouseRaycastResult.GameObject != MouseDownGameObject)
			return;

		if (_mouseRaycastResult.Type == RaycastedObjectType.NodeSide)
			General.Instance.OpenContextMenuForNode(_mouseRaycastResult.GameObject.GetComponent<NodeSide>().Node);
		if (_mouseRaycastResult.Type == RaycastedObjectType.Edge)
			General.Instance.OpenContextMenuForEdge(_mouseRaycastResult.GameObject.GetComponent<Edge>());
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
			if (layer == LayerMask.NameToLayer("Edge"))
				type = RaycastedObjectType.Edge;
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

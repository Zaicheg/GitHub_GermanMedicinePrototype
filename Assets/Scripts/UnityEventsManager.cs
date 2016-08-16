using UnityEngine;
using System.Collections;

/// <summary>
/// Класс управляет приёмом стандартных событий
/// </summary>
public class UnityEventsManager : Singleton<UnityEventsManager>
{
	protected UnityEventsManager() { }

	/// <summary>
	/// Событие Start
	/// </summary>
	private void Start()
	{
		General.Instance.StartEvent();
	}

	/// <summary>
	/// Событие OnEnable
	/// </summary>
	private void OnEnable()
	{
		GUIManager.Instance.OnEnableEvent();
	}

	/// <summary>
	/// Событие Update
	/// </summary>
	private void Update()
	{
		InputManager.Instance.UpdateEvent();
		GUIManager.Instance.UpdateEvent();
		CameraManager.Instance.UpdateEvent();
	}
}

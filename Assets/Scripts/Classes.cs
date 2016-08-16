using UnityEngine;
using System.Collections;

#region Реализация синглтона

/// <summary>
/// Класс реализует механизм синглтона
/// Имейте в виду, что всё ещё возможно испольнование несинглтонового конструктора `T myT = new T();`
/// Для предотвращения этого, используйте `protected T () {}` в вашем классе-одиночке.
/// 
/// Это реализовано как MonoBehaviour, чтобы была возможность использовать корутины
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();

	public static T Instance
	{
		get
		{
			//if (_applicationIsQuitting)
			//{
			//	Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
			//		"' already destroyed on application quit." +
			//		" Won't create again - returning null.");
			//	return null;
			//}

			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
						Debug.LogError("[Singleton] Something went really wrong " +
							" - there should never be more than 1 singleton!" +
							" Reopening the scene might fix it.");
						return _instance;
					}

					if (_instance == null)
					{
						var singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) " + typeof(T);

						DontDestroyOnLoad(singleton);

						//Debug.Log("[Singleton] An instance of " + typeof(T) +
						//	" is needed in the scene, so '" + singleton +
						//	"' was created with DontDestroyOnLoad.");
					}
					else
					{
						//Debug.Log("[Singleton] Using instance already created: " +
						//	_instance.gameObject.name);
					}
				}

				return _instance;
			}
		}
	}

	//private static bool _applicationIsQuitting;
	///// <summary>
	///// When Unity quits, it destroys objects in a random order.
	///// In principle, a Singleton is only destroyed when application quits.
	///// If any script calls Instance after it have been destroyed, 
	/////   it will create a buggy ghost object that will stay on the Editor scene
	/////   even after stopping playing the Application. Really bad!
	///// So, this was made to be sure we're not creating that buggy ghost object.
	///// </summary>
	//public void OnDestroy()
	//{
	//	_applicationIsQuitting = true;
	//}
}

/// <summary>
/// Вспомогательный класс для Singleton.cs
/// </summary>
public static class MethodExtensionForMonoBehaviourTransform
{
	/// <summary>
	/// Получает или создаёт компонент. Используется так же, как GetComponent
	/// </summary>
	public static T GetOrAddComponent<T>(this Component child) where T : Component
	{
		return child.GetComponent<T>() ?? child.gameObject.AddComponent<T>();
	}
}

#endregion

#region Приём ввода

/// <summary>
/// Результат рейкаста через курсор
/// </summary>
public class MouseRaycastResult
{
	public GameObject GameObject;
	public RaycastedObjectType Type;
	public RaycastHit Hit;
}

#endregion
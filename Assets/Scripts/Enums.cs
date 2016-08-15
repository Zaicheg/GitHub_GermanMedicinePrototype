using UnityEngine;
using System.Collections;

/// <summary>
/// Класс содержит перечисления
/// </summary>
public class Enums : MonoBehaviour
{

}

#region Приём ввода

/// <summary>
/// Тип объекта, найденного рейкастом
/// </summary>
public enum RaycastedObjectType
{
	None,
	Ground,
	NodeSide,
	Edge,
	Wall
}

#endregion

#region Подсветка

/// <summary>
/// Состояние грани ноды
/// </summary>
public enum NodeSideState
{
	Normal,
	Disabled,
	Occupied
}

#endregion
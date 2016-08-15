using UnityEngine;
using System.Collections;

/// <summary>
/// Класс описывает грань ноды
/// </summary>
public class NodeSide : MonoBehaviour
{
	public Node Node;
	public Edge Edge;
	public GameObject Cap;

	public NodeSideState NodeSideState = NodeSideState.Normal;

	public bool IsSpecial;
}

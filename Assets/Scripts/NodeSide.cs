using UnityEngine;
using System.Collections;

/// <summary>
/// Класс описывает грань ноды
/// </summary>
public class NodeSide : MonoBehaviour
{
	public Node Node;
	public Edge Edge;
	public Edge NextEdge;
	public Edge PreviousEdge;
	public GameObject Cap;

	public NodeSideState NodeSideState = NodeSideState.Normal;

	public bool IsSpecial;
}

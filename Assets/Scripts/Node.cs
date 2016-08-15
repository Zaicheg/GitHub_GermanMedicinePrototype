using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс описывает ноду
/// </summary>
public class Node : MonoBehaviour
{
	public NodeSide LeftNodeSide;
	public NodeSide RightNodeSide;
	public NodeSide BackwardNodeSide;
	public NodeSide ForwardNodeSide;
	public NodeSide BottomNodeSide;
	public NodeSide TopNodeSide;

	public List<NodeSide> NodeSides = new List<NodeSide>();
}
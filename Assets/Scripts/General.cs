using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Метод управляет основной логикой
/// </summary>
public class General : Singleton<General>
{
	protected General() { }

	public GameObject ItemsRoot;

	public GameObject NodePrefab;
	public GameObject EdgePrefab;

	public List<Node> Nodes = new List<Node>();
	public List<Edge> Edges = new List<Edge>();

	public float EdgesLength;

	private const float NodeSize = 0.03f;
	private const float EdgeDiameter = 0.02f;

	public Node NodeForContextMenu;
	public Edge EdgeForContextMenu;

	public bool ContextMenuOpened;

	/// <summary>
	/// Метод обрабатывает событие Start
	/// </summary>
	public void StartEvent()
	{
		CreateNode(Vector3.zero, null);
		CameraManager.Instance.MoveSpectatorInDefaultPosition();

		SelectEdgesLength(500f);
	}

	/// <summary>
	/// Метод создаёт ноду
	/// </summary>
	/// <param name="position">Позиция для ноды</param>
	/// <param name="edge">Ребро, для которого создаётся нода</param>
	private Node CreateNode(Vector3 position, Edge edge)
	{
		var node = Instantiate(NodePrefab).GetComponent<Node>();
		node.transform.parent = ItemsRoot.transform;
		node.transform.name = "Node_" + Nodes.Count.ToString("D2");

		//var targetScale = new Vector3(NodeSize, NodeSize, NodeSize);
		//node.transform.localScale = targetScale;

		node.transform.position = position;

		Nodes.Add(node);

		node.NodeSides = new List<NodeSide>
		{
			node.LeftNodeSide,
			node.RightNodeSide,
			node.ForwardNodeSide,
			node.BackwardNodeSide,
			node.BottomNodeSide,
			node.TopNodeSide
		};

		foreach (var nodeSide in node.NodeSides)
		{
			ChangeNodeSideState(nodeSide, NodeSideState.Normal);
		}

		NodesOnUpdatedEvent();

		return node;
	}

	private void NodesOnUpdatedEvent()
	{
		CheckNodeSidesForAvailability();
		CalculateBudget();

		CameraManager.Instance.LookAtCamera();
		CameraManager.Instance.AutoZoomCamera();
	}

	/// <summary>
	/// Метод создаёт ребро от стороны ноды
	/// </summary>
	public void CreateEdgeForNodeSide(NodeSide nodeSide)
	{
		if (nodeSide.NodeSideState == NodeSideState.Occupied)
			return;
		if (nodeSide.NodeSideState == NodeSideState.Disabled)
			return;

		var edge = Instantiate(EdgePrefab).GetComponent<Edge>();
		edge.transform.parent = ItemsRoot.transform;
		edge.transform.name = "Edge_" + nodeSide.Node.transform.name.Replace("Node_", "");

		var targetScale = new Vector3(EdgeDiameter, EdgeDiameter, EdgesLength / 1000f);
		edge.transform.localScale = targetScale;

		var targetPosition = nodeSide.transform.position;
		targetPosition += nodeSide.transform.forward * edge.transform.localScale.z * 0.5f;
		edge.transform.position = targetPosition;

		edge.transform.forward = nodeSide.transform.forward;

		nodeSide.Edge = edge;
		nodeSide.NextEdge = edge;
		edge.StartNodeSide = nodeSide;

		Edges.Add(edge);

		CreateEndNodeForEdge(edge);

		ChangeNodeSideState(nodeSide, NodeSideState.Occupied);
	}

	/// <summary>
	/// Метод создаёт конечную ноду для ребра
	/// </summary>
	/// <param name="edge">Ребро</param>
	private void CreateEndNodeForEdge(Edge edge)
	{
		var targetPosition = edge.transform.position;
		targetPosition += edge.transform.forward * edge.transform.localScale.z * 0.5f;
		targetPosition += edge.transform.forward * NodeSize * 0.5f;

		Node foundedNode = null;
		foreach (var node in Nodes)
			if (node.transform.position == targetPosition)
			{
				foundedNode = node;
				break;
			}

		if (foundedNode == null)
			foundedNode = CreateNode(targetPosition, edge);

		foreach (var nodeSide in foundedNode.NodeSides)
		{
			if (edge.transform.forward == nodeSide.transform.forward * -1f)
			{
				edge.EndNodeSide = nodeSide;
				nodeSide.Edge = edge;
				nodeSide.PreviousEdge = edge;

				ChangeNodeSideState(nodeSide, NodeSideState.Occupied);

				break;
			}
		}
	}

	/// <summary>
	/// Метод меняет состояние ноды
	/// </summary>
	/// <param name="nodeSide"></param>
	/// <param name="nodeSideState"></param>
	private void ChangeNodeSideState(NodeSide nodeSide, NodeSideState nodeSideState)
	{
		nodeSide.NodeSideState = nodeSideState;

		HighlightNodeSide(nodeSide, nodeSideState);
	}

	/// <summary>
	/// Метод выбирает текущую длину создаваемых рёбер
	/// </summary>
	public void SelectEdgesLength(float length)
	{
		EdgesLength = length;

		CheckNodeSidesForAvailability();
	}

	/// <summary>
	/// Метод ищет грани нод, которые должны быть неактивны при текущей длине ребра
	/// </summary>
	private void CheckNodeSidesForAvailability()
	{
		var nodeSides = new List<NodeSide>();
		foreach (var node in Nodes)
			nodeSides.AddRange(node.NodeSides);

		//for (var i = 0; i < nodeSides.Count; i++)
		//{
		//	if (nodeSides[i].IsSpecial == false)
		//	{
		//		nodeSides.RemoveAt(i);
		//		i--;
		//	}
		//}

		foreach (var nodeSideA in nodeSides)
			if (nodeSideA.NodeSideState == NodeSideState.Disabled)
				ChangeNodeSideState(nodeSideA, NodeSideState.Normal);

		foreach (var nodeSideA in nodeSides)
		{
			if (nodeSideA.NodeSideState == NodeSideState.Occupied || nodeSideA.NodeSideState == NodeSideState.Disabled)
				continue;

			foreach (var nodeSideB in nodeSides)
			{
				if (nodeSideB.NodeSideState == NodeSideState.Disabled)
					continue;

				if (nodeSideA == nodeSideB || nodeSideA.Node == nodeSideB.Node)
					continue;

				if (nodeSideA.transform.forward != nodeSideB.transform.forward && nodeSideA.transform.forward != nodeSideB.transform.forward * -1f)
					continue;

				if (nodeSideB.Edge == null)
					continue;

				if (nodeSideB.Edge.StartNodeSide == null || nodeSideB.Edge.EndNodeSide == null)
					continue;

				if (nodeSideB.Edge.StartNodeSide.Node == null || nodeSideB.Edge.EndNodeSide.Node == null)
					continue;

				if (nodeSideB.Edge.StartNodeSide.Node == nodeSideA.Node || nodeSideB.Edge.EndNodeSide.Node == nodeSideA.Node)
					continue;

				var a = nodeSideA.transform.InverseTransformPoint(nodeSideB.Edge.transform.position);
				var b = a.z * nodeSideA.transform.localScale.z;
				if (Apr_01(b, nodeSideB.Edge.transform.localScale.z / 2f) == false)
					continue;

				if (Apr_01(nodeSideB.Edge.transform.localScale.z, EdgesLength / 1000f) == false)
				{
					//Debug.Log(nodeSideA.name + " - " + nodeSideA.Node.name + " / " + nodeSideB.name + " - " + nodeSideB.Node.name);
					ChangeNodeSideState(nodeSideA, NodeSideState.Disabled);
				}
			}
		}
	}

	/// <summary>
	/// Метод подсвечивает сторону ноды в зависимости от доступности
	/// </summary>
	/// <param name="nodeSide">Сторона ноды</param>
	/// <param name="mode">Режим подсветки</param>
	private void HighlightNodeSide(NodeSide nodeSide, NodeSideState mode)
	{
		if (mode == NodeSideState.Normal)
			nodeSide.Cap.SetActive(false);
		if (mode == NodeSideState.Disabled)
			nodeSide.Cap.SetActive(true);
	}

	/// <summary>
	/// Метод возврвщает центр всей конструкции
	/// </summary>
	/// <returns>Центр</returns>
	public Vector3 GetCenter()
	{
		float left = Mathf.Infinity, right = Mathf.NegativeInfinity, back = Mathf.Infinity, front = Mathf.NegativeInfinity, bottom = Mathf.Infinity, top = Mathf.NegativeInfinity;

		foreach (var node in Nodes)
		{
			if (node.transform.position.x < left)
				left = node.transform.position.x;
			if (node.transform.position.x > right)
				right = node.transform.position.x;
			if (node.transform.position.y < bottom)
				bottom = node.transform.position.y;
			if (node.transform.position.y > top)
				top = node.transform.position.y;
			if (node.transform.position.z < back)
				back = node.transform.position.z;
			if (node.transform.position.z > front)
				front = node.transform.position.z;
		}

		return new Vector3(left + (right - left) / 2f, bottom + (top - bottom) / 2f, back + (front - back) / 2f);
	}

	/// <summary>
	/// Метод подсчитывает смету
	/// </summary>
	private void CalculateBudget()
	{
		var edgesCount = Edges.Count;
		var nodesCount = Nodes.Count;

		var edgePrice = 100;
		var nodePrice = 150;

		var edgesCost = edgesCount * edgePrice;
		var nodesCost = nodesCount * nodePrice;

		var text = "Edges: " + edgesCount + ", €" + edgesCost + "\n" + "Nodes: " + nodesCount + ", €" + nodesCost + "\n" + "Total: €" + (edgesCost + nodesCost);
		GUIManager.Instance.BudgetChangedEvent(text);
	}

	/// <summary>
	/// Метод сбрасывает сцену на начальное состояние
	/// </summary>
	public void Reset()
	{
		SceneManager.LoadScene(0);
	}

	/// <summary>
	/// Метод открывает контекстное меню для ноды
	/// </summary>
	/// <param name="node">Нода</param>
	public void OpenContextMenuForNode(Node node)
	{
		ContextMenuOpened = true;

		NodeForContextMenu = node;

		GUIManager.Instance.ContextMenuStateChanged(ContextMenuOpened);
	}

	/// <summary>
	/// Метод открывает контекстное меню для ребра
	/// </summary>
	/// <param name="edge"></param>
	public void OpenContextMenuForEdge(Edge edge)
	{
		ContextMenuOpened = true;

		EdgeForContextMenu = edge;

		GUIManager.Instance.ContextMenuStateChanged(ContextMenuOpened);
	}

	/// <summary>
	/// Метод закрывает контекстное меню
	/// </summary>
	public void CloseContextMenu()
	{
		ContextMenuOpened = false;

		NodeForContextMenu = null;
		EdgeForContextMenu = null;

		GUIManager.Instance.ContextMenuStateChanged(ContextMenuOpened);
	}

	/// <summary>
	/// Метод удаляет выбранный объект
	/// </summary>
	public void DeleteSelectedItem()
	{
		if (NodeForContextMenu)
			DeleteNode(NodeForContextMenu);
		if (EdgeForContextMenu)
			DeleteEdge(EdgeForContextMenu);
	}

	/// <summary>
	/// Метод удаляет ноду
	/// </summary>
	/// <param name="node"></param>
	private void DeleteNode(Node node)
	{
		foreach (var nodeSide in node.NodeSides)
			if (nodeSide.Edge)
				DeleteEdge(nodeSide.Edge);

		if (Nodes.IndexOf(node) != 0)
		{
			Nodes.Remove(node);
			Destroy(node.gameObject);
		}

		NodesOnUpdatedEvent();
	}

	/// <summary>
	/// Метод удаляет ребро
	/// </summary>
	/// <param name="edge"></param>
	private void DeleteEdge(Edge edge)
	{
		Edges.Remove(edge);
		Destroy(edge.gameObject);

		if (edge.StartNodeSide)
			edge.StartNodeSide.NodeSideState = NodeSideState.Normal;
		if (edge.EndNodeSide)
			edge.EndNodeSide.NodeSideState = NodeSideState.Normal;
	}

	/// <summary>
	/// Метод сравнивает два числа с определенной погрешностью
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public bool Apr_01(float a, float b)
	{
		return Mathf.Abs(a - b) < 0.0001f;
	}
}
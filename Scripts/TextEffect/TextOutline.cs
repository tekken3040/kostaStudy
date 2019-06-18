using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class TextOutline : BaseMeshEffect
{
	[SerializeField]
	[Range(0f, 10f)]
	public float Width = 0f;
	
	[SerializeField]
	public Color32 OutlineColor = Color.white;

	// 추가 코드(기만)
	public void ReDraw()
	{
		enabled = false;
		enabled = true;
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!this.IsActive ())
			return;

		List<UIVertex> vertexList = new List<UIVertex> ();
		vh.GetUIVertexStream (vertexList);

		ModifyVertices (vertexList);

		vh.Clear ();
		vh.AddUIVertexTriangleStream (vertexList);
	}

	public void ModifyVertices(List<UIVertex> _vertexList)
	{
		if (!IsActive())
			return;
		
		int nCount = _vertexList.Count;

		for (int i = nCount - 1; i >= 0; --i)
		{
			UIVertex uiVertex = _vertexList[i];
//			uiVertex.
//			uiVertex.color = Color32.Lerp(EndColor, StartColor, (uiVertex.position.y - fBottomY) * fUIElementHeight - Offset);
			DebugMgr.Log(uiVertex.position.y);
			_vertexList[i] = uiVertex;
		}
	}
}
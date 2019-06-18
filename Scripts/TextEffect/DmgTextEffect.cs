using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class DmgTextEffect : BaseMeshEffect {
	[SerializeField]
	private bool Bold = false;

	[SerializeField]
	public Color32 FontStartColor = Color.white;
	[SerializeField]
	public Color32 FontEndColor = Color.black;

	[SerializeField]
	[Range(-1.5f, 1.5f)]
	public float Offset = 0f;

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

		// Bold
		if(Bold)
		{
			Color textColor = GetComponent<Text>().color;
			int start = 0;
			int end = _vertexList.Count;
			ApplyShadow(_vertexList, textColor, start, _vertexList.Count, 1, 1);
			
			start = end;
			end = _vertexList.Count;
			ApplyShadow(_vertexList, textColor, start, _vertexList.Count, 1, -1);
			
			start = end;
			end = _vertexList.Count;
			ApplyShadow(_vertexList, textColor, start, _vertexList.Count, -1, 1);
			
			start = end;
			end = _vertexList.Count;
			ApplyShadow(_vertexList, textColor, start, _vertexList.Count, -1, -1);
		}
	}

	protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
	{
		UIVertex vt;
		
		var neededCpacity = verts.Count * 2;
		if (verts.Capacity < neededCpacity)
			verts.Capacity = neededCpacity;
		
		for (int i = start; i < end; ++i)
		{
			vt = verts[i];
			verts.Add(vt);
			
			Vector3 v = vt.position;
			v.x += x;
			v.y += y;
			vt.position = v;
			var newColor = color;
			vt.color = newColor;
			verts[i] = vt;
		}
	}
}

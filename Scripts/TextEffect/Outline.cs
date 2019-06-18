using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/Outline", 15)]
    public class Outline : ModifiedShadow
    {
        //protected Outline()
        //{}
        //
		//public  override  void  ModifyMesh (Mesh mesh)
		//{
		//	if  (! IsActive ())
		//		return ;
        //
		//	List <UIVertex> list = new  List <UIVertex> ();
        //
		//	// 1.Mesh List <UIVertex>를 얻을
		//	using  (VertexHelper vertexHelper = new  VertexHelper (mesh))
		//	{
		//		vertexHelper.GetUIVertexStream (list);
		//	}
        //
		//	// 2.Unity 5.1 ~ 이전 ModifyVertices ()를 호출
		//	ModifyVertices (list);
        //
		//	// 3.List <UIVertex> 변경 Mesh에 반영
		//	using  (VertexHelper vertexHelper2 = new  VertexHelper ())
		//	{
		//		vertexHelper2.AddUIVertexTriangleStream (list);
		//		vertexHelper2.FillMesh (mesh);
		//	}
		//}

        public override void ModifyVertices(List<UIVertex> verts)
        {
            if (!IsActive())
                return;
            verts.Capacity = verts.Count * 9;
            var original = verts.Count;
            var count = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        var next = count + original;
                        ApplyShadow(verts, effectColor, count, next, effectDistance.x * x, effectDistance.y * y);
                        count = next;
                    }
                }
            }
            //var start = 0;
            //var end = verts.Count;
            //ApplyShadow(verts, effectColor, start, verts.Count, effectDistance.x, effectDistance.y);
            //
            //start = end;
            //end = verts.Count;
            //ApplyShadow(verts, effectColor, start, verts.Count, effectDistance.x, -effectDistance.y);
            //
            //start = end;
            //end = verts.Count;
            //ApplyShadow(verts, effectColor, start, verts.Count, -effectDistance.x, effectDistance.y);
            //
            //start = end;
            //end = verts.Count;
            //ApplyShadow(verts, effectColor, start, verts.Count, -effectDistance.x, -effectDistance.y);
        }
    }
}

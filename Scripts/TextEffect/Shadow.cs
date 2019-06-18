using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/Shadow", 14)]
    public class Shadow : BaseMeshEffect
    {
        [SerializeField]
        private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);

        [SerializeField]
        private Vector2 m_EffectDistance = new Vector2(1f, -1f);

        [SerializeField]
        private bool m_UseGraphicAlpha = true;

        protected Shadow()
        {}

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            effectDistance = m_EffectDistance;
            base.OnValidate();
        }

#endif

        public Color effectColor
        {
            get { return m_EffectColor; }
            set
            {
                m_EffectColor = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public Vector2 effectDistance
        {
            get { return m_EffectDistance; }
            set
            {
                if (value.x > 600)
                    value.x = 600;
                if (value.x < -600)
                    value.x = -600;

                if (value.y > 600)
                    value.y = 600;
                if (value.y < -600)
                    value.y = -600;

                if (m_EffectDistance == value)
                    return;

                m_EffectDistance = value;

                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public bool useGraphicAlpha
        {
            get { return m_UseGraphicAlpha; }
            set
            {
                m_UseGraphicAlpha = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
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
                if (m_UseGraphicAlpha)
                    newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
                vt.color = newColor;
                verts[i] = vt;
            }
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

//		public  override  void  ModifyMesh (Mesh mesh)
//		{
//			if  (! IsActive ())
//				return ;
//
//			List <UIVertex> list = new  List <UIVertex> ();
//
//			// 1.Mesh List <UIVertex>를 얻을
//			using  (VertexHelper vertexHelper = new  VertexHelper (mesh))
//			{
//				vertexHelper.GetUIVertexStream (list);
//			}
//
//			// 2.Unity 5.1 ~ 이전 ModifyVertices ()를 호출
//			ModifyVertices (list);
//
//			// 3.List <UIVertex> 변경 Mesh에 반영
//			using  (VertexHelper vertexHelper2 = new  VertexHelper ())
//			{
//				vertexHelper2.AddUIVertexTriangleStream (list);
//				vertexHelper2.FillMesh (mesh);
//			}
//		}

        public void ModifyVertices(List<UIVertex> verts)
        {
            if (!IsActive())
                return;

            ApplyShadow(verts, effectColor, 0, verts.Count, effectDistance.x, effectDistance.y);
        }
    }
}

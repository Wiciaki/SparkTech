namespace SparkTech.SDK.Clipper
{
    using System.Collections.Generic;

    public class PolyNode
    {
        #region Fields

        internal List<PolyNode> m_Childs = new List<PolyNode>();

        internal EndType m_endtype;

        internal int m_Index;

        internal JoinType m_jointype;

        internal PolyNode m_Parent;

        internal List<IntPoint> m_polygon = new List<IntPoint>();

        #endregion

        #region Public Properties

        public int ChildCount => this.m_Childs.Count;

        public List<PolyNode> Childs => this.m_Childs;

        public List<IntPoint> Contour => this.m_polygon;

        public bool IsHole => this.IsHoleNode();

        public bool IsOpen { get; set; }

        public PolyNode Parent => this.m_Parent;

        #endregion

        #region Public Methods and Operators

        public PolyNode GetNext()
        {
            if (this.m_Childs.Count > 0) return this.m_Childs[0];
            else return this.GetNextSiblingUp();
        }

        #endregion

        #region Methods

        internal void AddChild(PolyNode Child)
        {
            var cnt = this.m_Childs.Count;
            this.m_Childs.Add(Child);
            Child.m_Parent = this;
            Child.m_Index = cnt;
        }

        internal PolyNode GetNextSiblingUp()
        {
            if (this.m_Parent == null) return null;
            else if (this.m_Index == this.m_Parent.m_Childs.Count - 1) return this.m_Parent.GetNextSiblingUp();
            else return this.m_Parent.m_Childs[this.m_Index + 1];
        }

        private bool IsHoleNode()
        {
            var result = true;
            var node = this.m_Parent;
            while (node != null)
            {
                result = !result;
                node = node.m_Parent;
            }

            return result;
        }

        #endregion
    }
}
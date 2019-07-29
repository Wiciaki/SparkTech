namespace SparkTech.SDK.Clipper
{
    using System.Collections.Generic;

    public class PolyTree : PolyNode
    {
        #region Fields

        internal List<PolyNode> m_AllPolys = new List<PolyNode>();

        #endregion

        #region Public Properties

        public int Total
        {
            get
            {
                var result = this.m_AllPolys.Count;

                //with negative offsets, ignore the hidden outer polygon ...
                if (result > 0 && this.m_Childs[0] != this.m_AllPolys[0]) result--;
                return result;
            }
        }

        #endregion

        #region Public Methods and Operators

        //The GC probably handles this cleanup more efficiently ...
        //~PolyTree(){Clear();}

        public void Clear()
        {
            for (var i = 0; i < this.m_AllPolys.Count; i++) this.m_AllPolys[i] = null;

            this.m_AllPolys.Clear();
            this.m_Childs.Clear();
        }

        public PolyNode GetFirst()
        {
            if (this.m_Childs.Count > 0) return this.m_Childs[0];
            else return null;
        }

        #endregion
    }
}
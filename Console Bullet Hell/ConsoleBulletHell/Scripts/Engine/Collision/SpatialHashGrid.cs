using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class SpatialHashGrid
    {
        public Rect2D Bounds;
        public Vector2 CenterOffsetRaw;

        public float CellSize;

        public int girdX;
        public int girdY;

        public int gridXRes;
        public int gridYRes;

        public SpatialHashNode[] Nodes;

        private int _length;
        private List<int> _tempNodes = new List<int>(512);

        public SpatialHashGrid(Vector2Int _size, float cellSize)
        {
            CellSize = cellSize;
            gridXRes = _size.x;
            gridYRes = _size.y;

            girdX = Maths.CeilToInt(_size.x);
            girdY = Maths.CeilToInt(_size.y);

            float halfC = CellSize * 0.5f;
            CenterOffsetRaw = new Vector2((-(gridXRes * halfC)), (-(gridXRes * halfC)));

            Bounds = new Rect2D(Vector2.zero, girdX * CellSize, girdY * CellSize);

            _length = girdX * girdY;
            Nodes = new SpatialHashNode[_length];
            for (int i = 0; i < _length; i++)
            {
                Nodes[i] = new SpatialHashNode(i);
            }
        }

        public void ClearGrid()
        {
            for (int i = 0; i < _length; i++)
            {
                Nodes[i].Clear();
            }
        }

        public int GetIndex1D(int _x, int _y)
        {
            return (_x + girdX * _y);
        }

        public void Update(Collider shape)
        {
            ICollideable host = shape.Host;
            bool skip = PrepList(shape, host);

            if (skip) { return; }
            for (int i = 0; i < _tempNodes.Count; i++)
            {
                Nodes[_tempNodes[i]].Add(host);
            }
        }

        private bool PrepList(Collider shape, ICollideable host)
        {
            _tempNodes.Clear();

            host.SetOOB(!Bounds.Overlaps(shape.bounds));
            if (host.IsOOB())
            {
                return true;
            }

            AddToNode(shape.bounds.min - CenterOffsetRaw, shape.bounds.max - CenterOffsetRaw);
            return false;
        }

        private void AddToNode(Vector2 posMin, Vector2 posMax)
        {
            posMin /= CellSize;
            posMax /= CellSize;

            int maxX = Maths.Clamp((int)posMax.x, 0, girdX - 1);
            int maxY = Maths.Clamp((int)posMax.y, 0, girdY - 1);
            int minX = Maths.Clamp((int)posMin.x, 0, girdX - 1);
            int minY = Maths.Clamp((int)posMin.y, 0, girdY - 1);

            int aabbMax = GetIndex1D(maxX, maxY);
            int aabbMin = GetIndex1D(minX, minY);

            _tempNodes.Add(aabbMin);

            if (aabbMin != aabbMax)
            {
                _tempNodes.Add(aabbMax);
                int lenX = maxX - minX + 1;
                int lenY = maxY - minY + 1;
                for (int x = 0; x < lenX; x++)
                {
                    for (int y = 0; y < lenY; y++)
                    {
                        if ((x == 0 && y == 0) || (x == lenX - 1 && y == lenY - 1)) { continue; }
                        _tempNodes.Add(GetIndex1D(x, y) + aabbMin);
                    }
                }
            }
        }

        private SpatialHashNode _node;
        private ICollideable _temp;
        public int Retrieve(ref ICollideable[] shapes, ICollideable shape)
        {
            int ll = 0;
            Entity a = shape.GetEntity();
            Entity b;
            for (int i = 0; i < a.Nodes.Count; i++)
            {
                _node = Nodes[a.Nodes[i]];
                for (int j = 0; j < _node.Bodies.Count; j++)
                {
                    _temp = _node.Bodies[j];
                    b = _temp.GetEntity();
                    if (Phys2D.MaskHasLayer(a.Type, b.Type) && !shapes.ContainsID(b.ID, ll))
                    {
                        shapes[ll] = _temp;
                        ll++;

                        if (ll >= shapes.Length)
                        {
                            return ll;
                        }
                    }
                }
            }
            return ll;
        }
    }
}
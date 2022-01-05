using System.Linq;
using Tarodev_Pathfinding._Scripts.Grid;
using UnityEngine;

namespace _Scripts.Tiles {
    public class HexNode : NodeBase {
        public override void CacheNeighbors() {
            Neighbors = GridManager.Instance.Tiles.Where(t => Coords.GetDistance(t.Value.Coords) == 1).Select(t=>t.Value).ToList();
        }
    }
}

public struct HexCoords : ICoords {
    private readonly int _q;
    private readonly int _r;

    public HexCoords(int q, int r) {
        _q = q;
        _r = r;
        Pos = _q * new Vector2(Sqrt3, 0) + _r * new Vector2(Sqrt3 / 2, 1.5f);
    }

    public float GetDistance(ICoords other) => (this - (HexCoords)other).AxialLength();

    private static readonly float Sqrt3 = Mathf.Sqrt(3);

    public Vector2 Pos { get; set; }

    private int AxialLength() {
        if (_q == 0 && _r == 0) return 0;
        if (_q > 0 && _r >= 0) return _q + _r;
        if (_q <= 0 && _r > 0) return -_q < _r ? _r : -_q;
        if (_q < 0) return -_q - _r;
        return -_r > _q ? -_r : _q;
    }

    public static HexCoords operator -(HexCoords a, HexCoords b) {
        return new HexCoords(a._q - b._q, a._r - b._r);
    }
}
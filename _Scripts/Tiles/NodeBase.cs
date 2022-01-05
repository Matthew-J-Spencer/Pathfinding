using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Tiles {
    public abstract class NodeBase : MonoBehaviour {
        [Header("References")] [SerializeField]
        private Color _obstacleColor;

        [SerializeField] private Gradient _walkableColor;
        [SerializeField] protected SpriteRenderer _renderer;
     
        public ICoords Coords;
        public float GetDistance(NodeBase other) => Coords.GetDistance(other.Coords); // Helper to reduce noise in pathfinding
        public bool Walkable { get; private set; }
        private bool _selected;
        private Color _defaultColor;

        public virtual void Init(bool walkable, ICoords coords) {
            Walkable = walkable;

            _renderer.color = walkable ? _walkableColor.Evaluate(Random.Range(0f, 1f)) : _obstacleColor;
            _defaultColor = _renderer.color;

            OnHoverTile += OnOnHoverTile;

            Coords = coords;
            transform.position = Coords.Pos;
        }

        public static event Action<NodeBase> OnHoverTile;
        private void OnEnable() => OnHoverTile += OnOnHoverTile;
        private void OnDisable() => OnHoverTile -= OnOnHoverTile;
        private void OnOnHoverTile(NodeBase selected) => _selected = selected == this;

        protected virtual void OnMouseDown() {
            if (!Walkable) return;
            OnHoverTile?.Invoke(this);
        }

        #region Pathfinding

        [Header("Pathfinding")] [SerializeField]
        private TextMeshPro _fCostText;

        [SerializeField] private TextMeshPro _gCostText, _hCostText;
        public List<NodeBase> Neighbors { get; protected set; }
        public NodeBase Connection { get; private set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;

        public abstract void CacheNeighbors();

        public void SetConnection(NodeBase nodeBase) {
            Connection = nodeBase;
        }

        public void SetG(float g) {
            G = g;
            SetText();
        }

        public void SetH(float h) {
            H = h;
            SetText();
        }

        private void SetText() {
            if (_selected) return;
            _gCostText.text = G.ToString();
            _hCostText.text = H.ToString();
            _fCostText.text = F.ToString();
        }

        public void SetColor(Color color) => _renderer.color = color;

        public void RevertTile() {
            _renderer.color = _defaultColor;
            _gCostText.text = "";
            _hCostText.text = "";
            _fCostText.text = "";
        }

        #endregion
    }
}


public interface ICoords {
    public float GetDistance(ICoords other);
    public Vector2 Pos { get; set; }
}
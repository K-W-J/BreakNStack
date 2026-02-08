using System.Collections.Generic;
using Code.Modules;
using UnityEngine;

namespace Code.Blocks
{
    public class BlockColliderFixer : MonoBehaviour, IModule
    {
        private List<Vector2> _pathVertices;
        
        private PolygonCollider2D _polygonCollider;
        private BlockRenderer _blockRenderer;
        private Block _block;
        
        public void InitializeComponent(ModuleOwner owner)
        {
            _block = owner as Block;
            Debug.Assert(_block != null, "BlockColliderFixer of Block is Null");
            _blockRenderer = _block.GetModule<BlockRenderer>();
            
            _polygonCollider = GetComponent<PolygonCollider2D>();
            _pathVertices = new List<Vector2>();
        }
        
        public void UpdateColliderShape()
        {
            Sprite blockSprite = _block.BlockData.default_Sprite;
            
            _polygonCollider.pathCount = blockSprite.GetPhysicsShapeCount();
            
            for (int i = 0; i < _polygonCollider.pathCount; i++)
            {
                _pathVertices.Clear();
                
                blockSprite.GetPhysicsShape(i, _pathVertices);
                
                for (int j = 0; j < _pathVertices.Count; j++)
                {
                    Vector2 vertex = _pathVertices[j];

                    vertex *= _blockRenderer.transform.localScale;

                    if (_block.BlockData.isFlip)
                        vertex.x *= -1;

                    _pathVertices[j] = vertex;
                }
                
                _polygonCollider.SetPath(i, _pathVertices.ToArray());
            }
        }
    }
}
using Code.Modules;
using UnityEngine;

namespace Code.Blocks
{
    public class BlockRenderer : MonoBehaviour, IModule, IInitializeSpawn
    {
        private static readonly int Contrast = Shader.PropertyToID("_Contrast");
        
        private Block _block;
        private SpriteRenderer _spriteRenderer;
        private Material _grayMat;
        
        public void InitializeComponent(ModuleOwner owner)
        {
            _block = owner as Block;
            Debug.Assert(_block != null, "BlockRenderer should be attached to Block");
            
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _grayMat = _spriteRenderer.material;
        }

        public void InitializeSpawn()
        {
            _spriteRenderer.sprite = _block.BlockData.default_Sprite;
        }
        
        public void ChangeBreakSprite()
        {
            float healthRatio = ((float)_block.CurrentHealth / _block.BlockData.maxHealth) * 100;
            
            if (healthRatio > 75f)
                _spriteRenderer.sprite = _block.BlockData.default_Sprite;
            else if (healthRatio > 50f)
                _spriteRenderer.sprite = _block.BlockData.break_2_Sprite;
            else if (healthRatio > 25f)
                _spriteRenderer.sprite = _block.BlockData.break_3_Sprite;
            else
                _spriteRenderer.sprite = _block.BlockData.break_4_Sprite;
        }
        
        public void SetFlip(bool isFlip)
        {
            _spriteRenderer.flipX = isFlip;
            _block.BlockCollider.transform.localScale = Vector3.one;
            
            float scaleX = _block.BlockCollider.transform.localScale.x * (_block.BlockData.isFlip ? -1: 1);
            Vector2 flipScale = new Vector2(scaleX, _block.BlockCollider.transform.localScale.y);
            _block.BlockCollider.transform.localScale = flipScale;
        }

        public void SetGrayScale(float intensity)
        {
            _grayMat.SetFloat(Contrast, intensity);
        }
        
        public void SetAlpha(float alpha)
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r,_spriteRenderer.color.g,_spriteRenderer.color.b,alpha);
        }
    }
}
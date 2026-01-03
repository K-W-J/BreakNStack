using System.Collections.Generic;
using UnityEngine;

namespace Code.Effects
{
    [CreateAssetMenu(fileName = "EffectSpriteData", menuName = "SO/Effect/EffectSprite", order = 0)]
    public class EffectSpriteSO : ScriptableObject
    {
        public List<Sprite> effectSprites;
    }
}
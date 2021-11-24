using UnityEngine;

namespace LevelPack
{
    [CreateAssetMenu(fileName = "skinpackage", menuName = "Flow/Skin Pack", order = 1)]
    public class SkinPackage : ScriptableObject
    {
        [Tooltip("Colores a usar en este tema")]
        public Color[] colores = new Color[16];
    }
}

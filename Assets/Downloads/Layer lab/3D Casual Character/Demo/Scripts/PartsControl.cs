using UnityEngine;
using System.Collections.Generic;

namespace Layer_lab._3D_Casual_Character
{
    public class PartsControl : MonoBehaviour
    {
        [SerializeField] private ButtonParts button;
        [SerializeField] private Transform content;
        [SerializeField] private Sprite[] spriteIcons;
        private List<ButtonParts> _buttonParts = new();

        private void Start()
        {
            SpawnPartsButton(PartsType.Hair,CharacterControl.Instance.CharacterBase.PartsHair.ToArray(), $"{PartsType.Hair}", false);
            SpawnPartsButton(PartsType.Face,CharacterControl.Instance.CharacterBase.PartsFace.ToArray(), $"{PartsType.Face}", false);
            SpawnPartsButton(PartsType.Headgear,CharacterControl.Instance.CharacterBase.PartsHeadGear.ToArray(), $"{PartsType.Headgear}", true);
            SpawnPartsButton(PartsType.Top,CharacterControl.Instance.CharacterBase.PartsTop.ToArray(), $"{PartsType.Top}", false);
            SpawnPartsButton(PartsType.Glove,CharacterControl.Instance.CharacterBase.PartsGlove.ToArray(), $"{PartsType.Glove}", true);
            SpawnPartsButton(PartsType.Bottom,CharacterControl.Instance.CharacterBase.PartsBottom.ToArray(), $"{PartsType.Bottom}", false);
            SpawnPartsButton(PartsType.Shoes,CharacterControl.Instance.CharacterBase.PartsShoes.ToArray(), $"{PartsType.Shoes}", false);
            SpawnPartsButton(PartsType.Bag,CharacterControl.Instance.CharacterBase.PartsBag.ToArray(), $"{PartsType.Bag}", true);
            SpawnPartsButton(PartsType.Eyewear,CharacterControl.Instance.CharacterBase.PartsEyewear.ToArray(), $"{PartsType.Eyewear}", true);
            button.gameObject.SetActive(false);
        }

        private Sprite GetSprite(string name)
        {
            foreach (var sprite in spriteIcons)
            {
                if (sprite.name.Contains(name))
                {
                    return sprite;
                }
            }

            return null;
        }

        
        public void SpawnPartsButton(PartsType partsType, GameObject[] parts, string name, bool isEmpty)
        {
            ButtonParts buttonParts = Instantiate(button, content, false);
            buttonParts.SetButton(partsType, parts, GetSprite(name.ToLower()), isEmpty);
            _buttonParts.Add(buttonParts);
        }

        public void SetAllRandom()
        {
            for (int i = 0; i < _buttonParts.Count; i++)
            {
                _buttonParts[i].SetRandom();   
            }
        }
    }
}

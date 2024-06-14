using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Layer_lab._3D_Casual_Character
{
    public class ButtonParts : MonoBehaviour
    {
        private GameObject[] _parts;
        private int _index;
        [SerializeField] private TMP_Text textTitle;
        [field: SerializeField] private bool IsEmpty { get; set; }

        [SerializeField] private Image imageIcon;

        private PartsType CurrentPartType;
        public void SetButton(PartsType partsType, GameObject[] parts, Sprite icon, bool isNone)
        {
            CurrentPartType = partsType;
            IsEmpty = isNone;
            imageIcon.sprite = icon;
            imageIcon.SetNativeSize();
            _parts = parts;
            SetParts();
        }
    
        private void SetParts()
        {
            if (IsEmpty)
            {
                CharacterControl.Instance.CharacterBase.SetItem(CurrentPartType, -1);
                _index = -1;
            }
            else
            {
                CharacterControl.Instance.CharacterBase.SetItem(CurrentPartType, 0);
            }

            _SetTitle();
        }

  
    

        public void OnClick_Next()
        {
            _index++;
        
            if (IsEmpty)
            {
                if (_index >= _parts.Length) _index = -1;
            }
            else
            {
                if (_index >= _parts.Length) _index = 0;
            }
        
            _SetParts();
            _SetTitle();
        }

        public void OnClick_Previous()
        {
            _index--;

            if (IsEmpty)
            {
                if (_index < -1) _index = _parts.Length - 1;
            }
            else
            {
                if (_index < 0) _index = _parts.Length - 1;   
            }

            _SetParts();
            _SetTitle();
        }


        private void _SetParts()
        {
            CharacterControl.Instance.CharacterBase.SetItem(CurrentPartType, _index);
        }
    

        private void _SetTitle()
        {
            if (!IsEmpty && _index <= -1 || IsEmpty && _index <= -1)
            {
                textTitle.text = "--";
                textTitle.CrossFadeAlpha(0.3f, 0f, true);
            }
            else
            {
                string result = _parts[_index].name.Replace("Pack1_", "");
                result = result.Replace("_", "");
                textTitle.text = result;
                textTitle.CrossFadeAlpha(1f, 0f, true);
            }
        }

        public void SetRandom()
        {
            int random = 0;
            
            if (IsEmpty)
            {
                random = Random.Range(-_parts.Length, _parts.Length - 1);
                if (random < -1) random = -1;
            }
            else
            {
                random = Random.Range(0, _parts.Length - 1);
            }
            
            _index = random;
            CharacterControl.Instance.CharacterBase.SetItem(CurrentPartType, random);
            _SetTitle();
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Layer_lab._3D_Casual_Character
{
    public enum PartsType
    {
        Hair,
        Face,
        Headgear,
        Top,
        Bottom,
        Bag,
        Shoes,
        Glove,
        Eyewear,
        Body
    }
    
    public class CharacterBase : MonoBehaviour
    {
        public List<GameObject> PartsBody { get; set; } = new();
        public List<GameObject> PartsHair { get; set; } = new();
        public List<GameObject> PartsFace { get; set; } = new();
        public List<GameObject> PartsHeadGear { get; set; } = new();
        public List<GameObject> PartsTop { get; set; } = new();
        public List<GameObject> PartsBottom { get; set; } = new();
        public List<GameObject> PartsEyewear { get; set; } = new();
        public List<GameObject> PartsBag { get; set; } = new();
        public List<GameObject> PartsShoes { get; set; } = new();
        public List<GameObject> PartsGlove { get; set; } = new();

        public int Index { get; set; } = 0;
        
        private void Awake()
        {
            SetRoot();
        }


        public void SavePrefab()
        {
            #if UNITY_EDITOR
            string localPath = "Assets/Layer lab/3D Props Casual Character Pack1/Prefabs/"+ gameObject.name +".prefab";
            bool isPrefabSuccess;

            GameObject instanceObject = Instantiate(gameObject);
            instanceObject.transform.localPosition = Vector3.zero;
            instanceObject.transform.rotation = Quaternion.identity;
            instanceObject.transform.localScale = Vector3.one;
            
            PrefabUtility.SaveAsPrefabAsset(instanceObject, localPath, out isPrefabSuccess);
            if (isPrefabSuccess)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save" + isPrefabSuccess);
            
            Destroy(instanceObject);
            AssetDatabase.Refresh();
            #endif
        }
        
        public void SetRandom()
        {
            SetRoot();
            SetItem(PartsType.Hair, Random.Range(0, PartsHair.Count - 1));
            SetItem(PartsType.Face,Random.Range(0, PartsFace.Count - 1));
            SetItem(PartsType.Headgear, Random.Range(-5, PartsHeadGear.Count - 1));
            SetItem(PartsType.Top, Random.Range(0, PartsTop.Count - 1));
            SetItem(PartsType.Bottom, Random.Range(0, PartsBottom.Count - 1));
            SetItem(PartsType.Eyewear, Random.Range(-5, PartsEyewear.Count - 1));
            SetItem(PartsType.Bag, Random.Range(-5, PartsBag.Count - 1));
            SetItem(PartsType.Shoes, Random.Range(0, PartsShoes.Count - 1));
            SetItem(PartsType.Glove, Random.Range(-5, PartsGlove.Count - 1));
        }
        
        protected void SetRoot()
        {
            PartsHair.Clear();
            PartsFace.Clear();
            PartsHeadGear.Clear();
            PartsTop.Clear();
            PartsBottom.Clear();
            PartsEyewear.Clear();
            PartsBag.Clear();
            PartsShoes.Clear();
            PartsGlove.Clear();
            PartsBody.Clear();

            Transform parts = null;
            foreach (Transform t in transform)
            {
                if (t.name == "Parts") parts = t;

                if (t.name == "Body")
                {
                    foreach (Transform child in t.transform)
                    {
                        if (child.gameObject.name.Contains("Body_1") || child.gameObject.name.Contains("Body_2"))
                        {
                            child.gameObject.SetActive(false);
                            PartsBody.Add(child.gameObject);
                        }
                            
                    }
                }
            }
            
            
            

            foreach (Transform g in parts)
            {
                if (g.name.Contains($"{PartsType.Hair}")) foreach (Transform child in g.transform) PartsHair.Add(child.gameObject);
                if (g.name.Contains($"{PartsType.Face}")) foreach (Transform child in g.transform) PartsFace.Add(child.gameObject);
                if (g.name.Contains($"{PartsType.Headgear}")) foreach (Transform child in g.transform) PartsHeadGear.Add(child.gameObject);
                if (g.name.Contains($"{PartsType.Top}")) foreach (Transform child in g.transform) PartsTop.Add(child.gameObject);
                if (g.name.Contains($"{PartsType.Bottom}")) foreach (Transform child in g.transform) PartsBottom.Add(child.gameObject);
                if (g.name.Contains($"{PartsType.Eyewear}")) foreach (Transform child in g.transform) PartsEyewear.Add(child.gameObject);
                if (g.name.Contains($"{PartsType.Bag}")) foreach (Transform child in g.transform) PartsBag.Add(child.gameObject);
                if (g.name.Contains($"{PartsType.Shoes}")) foreach (Transform child in g.transform) PartsShoes.Add(child.gameObject);
                if (g.name.Contains($"{PartsType.Glove}")) foreach (Transform child in g.transform) PartsGlove.Add(child.gameObject);
             
            }
        }


        private bool IsEquipGlove { get; set; }
        public void CheckBody()
        {
            PartsBody[0].SetActive(IsEquipGlove);
            PartsBody[1].SetActive(!IsEquipGlove);
        }
        
        public void SetItem(PartsType partsType, int idx)
        {
            
            switch (partsType)
            {
                case PartsType.Hair:
                    for (int i = 0; i < PartsHair.Count; i++) PartsHair[i].gameObject.SetActive(i == idx);
                    break;
                case PartsType.Face:
                    for (int i = 0; i < PartsFace.Count; i++) PartsFace[i].gameObject.SetActive(i == idx);
                    break;
                case PartsType.Headgear:
                    for (int i = 0; i < PartsHeadGear.Count; i++) PartsHeadGear[i].gameObject.SetActive(i == idx);
                    break;
                case PartsType.Top:
                    for (int i = 0; i < PartsTop.Count; i++) PartsTop[i].gameObject.SetActive(i == idx);
                    break;
                case PartsType.Bottom:
                    for (int i = 0; i < PartsBottom.Count; i++) PartsBottom[i].gameObject.SetActive(i == idx);
                    break;
                case PartsType.Bag:
                    for (int i = 0; i < PartsBag.Count; i++) PartsBag[i].gameObject.SetActive(i == idx);
                    break;
                case PartsType.Shoes:
                    for (int i = 0; i < PartsShoes.Count; i++) PartsShoes[i].gameObject.SetActive(i == idx);
                    break;
                case PartsType.Glove:
                    IsEquipGlove = false;
                    
                    for (int i = 0; i < PartsGlove.Count; i++)
                    {
                        if (i == idx) IsEquipGlove = true; 
                        PartsGlove[i].gameObject.SetActive(i == idx);
                    }
                    break;
                case PartsType.Eyewear:
                    for (int i = 0; i < PartsEyewear.Count; i++) PartsEyewear[i].gameObject.SetActive(i == idx);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(partsType), partsType, null);
            }
            
            CheckBody();
        }

    }
}
        


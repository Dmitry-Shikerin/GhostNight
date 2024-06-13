using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    public class PreviewEntityModelTests
    {
        readonly IPreviewEntityModel _previewEntity = new PreviewEntityModel();
        GameObject _testPrefab;
        
        [SetUp]
        public void SetUp()
        {
            var newObject = new GameObject("test_object");
            _testPrefab = PrefabUtility.SaveAsPrefabAsset(newObject, "Assets/test_object.prefab");
            Object.DestroyImmediate(newObject);
        }

        [Test]
        public void Create_Instantiate_GivenPrefab()
        {
            _previewEntity.Create(_testPrefab);

            Assert.That(_previewEntity.Get().name, Is.EqualTo(_testPrefab.name+"(Clone)"));
        }

        [Test]
        public void Destroy_WhenObjectExist_ShouldDestroy()
        {
            _previewEntity.Create(_testPrefab);
            
            _previewEntity.Destroy();

            var result = _previewEntity.Get();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Notify_Invoke_Observers()
        {
            _previewEntity.Create(_testPrefab);
            var observer1 = new FakePreviewEntityObserver();
            _previewEntity.RegisterObserver(observer1);

            _previewEntity.NotifyObservers();

            Assert.That(observer1.UpdatedObject, Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            var path = AssetDatabase.GetAssetPath(_testPrefab);
            AssetDatabase.DeleteAsset(path);
        }
        
    }

    public class FakePreviewEntityObserver : IPreviewEntityObserver
    {
        public GameObject UpdatedObject;
        public void UpdatePreviewEntity(GameObject previewObject)
        {
            UpdatedObject = previewObject;
        }
    }
}

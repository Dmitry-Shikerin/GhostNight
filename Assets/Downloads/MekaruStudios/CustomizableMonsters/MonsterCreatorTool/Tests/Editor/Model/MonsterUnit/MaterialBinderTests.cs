using System;
using NUnit.Framework;
using UnityEngine;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    [TestFixture]
    public class MaterialBinderTests
    {
        IPreviewEntityModel _previewEntityModel;
        string _childTransformName;

        Material _materialToBind;

        [SetUp]
        public void Setup()
        {
            GetPreviewEntityModel();
            ConstructTargetObject();
            ConstructBindingMaterial();
        }

        [Test]
        public void Bind_When_TargetObjectNotExist_ThrowsNullReferenceException()
        {
            _previewEntityModel.Destroy();
            Assert.That(() => MaterialBinder.Bind(_materialToBind, _childTransformName),
                Throws.TypeOf<NullReferenceException>());
            ConstructTargetObject();
        }

        [Test]
        public void Bind_WithInvalidSlotName_ThrowsException()
        {
            Assert.That(() => MaterialBinder.Bind(_materialToBind, "invalid"),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Bind_SetMaterial_OnPreviewEntityModel()
        {
            var targetTransform = _previewEntityModel.Get().transform.Find(_childTransformName);

            MaterialBinder.Bind(_materialToBind, _childTransformName);

            var newMaterial = targetTransform.GetComponent<Renderer>().sharedMaterial;
            Assert.That(newMaterial, Is.SameAs(_materialToBind));
        }

        [TearDown]
        public void Teardown()
        {
            _previewEntityModel.Destroy();
        }

        void GetPreviewEntityModel()
        {
            _previewEntityModel = ServiceLocator.Instance.Resolve<IPreviewEntityModel>();
        }
        void ConstructTargetObject()
        {
            var targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var childObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _childTransformName = childObject.name;
            childObject.transform.SetParent(targetObject.transform);
            _previewEntityModel.Create(targetObject);
        }
        void ConstructBindingMaterial()
        {
            _materialToBind = new Material(Shader.Find("Hidden/Checkerboard"));
        }
    }
}

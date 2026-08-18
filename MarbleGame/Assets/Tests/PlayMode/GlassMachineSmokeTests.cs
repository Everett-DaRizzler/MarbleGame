using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using MarbleGame;
using System.IO;

namespace MarbleGame.Tests
{
    public class GlassMachineSmokeTests
    {
        private const string MarblePlayerSource = "Assets/Scripts/Runtime/MarblePlayer.cs";

        [Test]
        public void FirstPlayableSceneIsPresentInProject()
        {
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/GlassMachine_Prototype.unity");
            Assert.That(scene, Is.Not.Null);
            Assert.That(EditorBuildSettings.scenes, Has.Some.Matches<EditorBuildSettingsScene>(s => s.path == "Assets/Scenes/GlassMachine_Prototype.unity" && s.enabled));
        }

        [Test]
        public void MarblePlayer_UsesArrowOnlyLateralInput()
        {
            var source = File.ReadAllText(MarblePlayerSource);

            StringAssert.Contains("leftArrowKey", source);
            StringAssert.Contains("rightArrowKey", source);
            StringAssert.DoesNotContain("Keyboard.current.wKey", source);
            StringAssert.DoesNotContain("Keyboard.current.aKey", source);
            StringAssert.DoesNotContain("Keyboard.current.sKey", source);
            StringAssert.DoesNotContain("Keyboard.current.dKey", source);
        }

        [Test]
        public void MarblePlayer_DoesNotSeedOrApplyForwardVelocity()
        {
            var source = File.ReadAllText(MarblePlayerSource);

            StringAssert.DoesNotContain("Vector3.forward *", source);
            StringAssert.DoesNotContain("AddForce(travelDirection", source);
            StringAssert.DoesNotContain("linearVelocity = Vector3.forward", source);
            StringAssert.DoesNotContain("linearVelocity = Vector3.back", source);
        }

        [Test]
        public void ThirdPersonCamera_IgnoresTheTargetColliderDuringCollisionChecks()
        {
            var source = File.ReadAllText("Assets/Scripts/Runtime/ThirdPersonCamera.cs");

            StringAssert.Contains("hit.rigidbody == target.Body", source);
            StringAssert.Contains("SphereCastNonAlloc", source);
        }
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpectraEscapeEditModeTests
{
    [TearDown]
    public void TearDown()
    {
        DraggableMirror.activeMirror = null;

        foreach (GameObject obj in Object.FindObjectsByType<GameObject>())
        {
            if (obj.name.StartsWith("TEST_"))
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    [Test]
    public void FrameRateLimiter_SetsTargetFrameRateTo60()
    {
        int previousFrameRate = Application.targetFrameRate;
        int previousVSync = QualitySettings.vSyncCount;

        try
        {
            GameObject go = new GameObject("TEST_FrameRateLimiter");
            FrameRateLimiter limiter = go.AddComponent<FrameRateLimiter>();

            limiter.ApplyFrameRateSettings();

            Assert.AreEqual(60, Application.targetFrameRate);
            Assert.AreEqual(0, QualitySettings.vSyncCount);
        }
        finally
        {
            Application.targetFrameRate = previousFrameRate;
            QualitySettings.vSyncCount = previousVSync;
        }
    }

    [Test]
    public void Mirror_Rotates45Degrees_WhenEnabled()
    {
        GameObject go = new GameObject("TEST_Mirror");
        go.transform.position = new Vector3(10000, 10000, 10000);

        go.AddComponent<BoxCollider>();
        DraggableMirror mirror = go.AddComponent<DraggableMirror>();

        mirror.RotateMirror();

        Assert.That(
            Mathf.Abs(Mathf.DeltaAngle(go.transform.eulerAngles.y, 45f)),
            Is.LessThan(0.01f)
        );
    }

    [Test]
    public void Mirror_DoesNotRotate_WhenLevelFinished()
    {
        GameObject go = new GameObject("TEST_DisabledMirror");
        go.transform.position = new Vector3(10000, 10000, 10000);

        go.AddComponent<BoxCollider>();
        DraggableMirror mirror = go.AddComponent<DraggableMirror>();

        mirror.DisableMirrors(true);

        Quaternion before = go.transform.rotation;

        mirror.RotateMirror();

        Assert.That(
            Quaternion.Angle(before, go.transform.rotation),
            Is.LessThan(0.01f)
        );
    }
}

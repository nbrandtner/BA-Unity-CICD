using UnityEngine;

public class MirrorUIController : MonoBehaviour
{
    public void RotateActiveMirror()
    {
        if (DraggableMirror.activeMirror != null)
        {
            DraggableMirror.activeMirror.RotateMirror();
            Debug.Log("Rotatebutton pressed");
        }
    }
}

using UnityEngine;

public class Controls : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            gameObject.GetComponent<StreamingVideoTexture.StreamingVideoTexture>().PlayRequested = true;
        }
    }
}

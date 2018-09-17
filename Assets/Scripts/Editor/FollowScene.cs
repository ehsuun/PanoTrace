using UnityEngine;
using System.Collections;
using UnityEditor;

public class FollowScene : MonoBehaviour {
    public EditorWindow sceneWindow;
    public Transform sceneCamera;
    internal SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        if (!sceneWindow)
        {
            if (EditorWindow.focusedWindow.titleContent.text == "Scene")
            {
                sceneWindow = EditorWindow.focusedWindow;
            }
        }

        if (!sceneCamera)
        {
            if(EditorWindow.focusedWindow == sceneWindow)
            {
                sceneCamera = Camera.current.transform;
            }
        }else
        {
            transform.position = sceneCamera.position;
            transform.rotation = sceneCamera.rotation;
        }

        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)){
            string path = Application.dataPath.TrimEnd("Assets".ToCharArray()) + "Screenshots/";
            ScreenCapture.CaptureScreenshot(path + "Sceenshot" + Random.Range(0, 10000)+".png", 2);
        }
    }
}

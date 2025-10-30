using UnityEngine;

public class Split : MonoBehaviour
{
    public Camera mainCamera;   // caméra classique au départ
    public Camera camera1;      // paddle 1
    public Camera camera2;      // paddle 2

    private bool isSplit = false;
    [SerializeField] private AudioSource samba;

    void Start()
    {

        mainCamera.enabled = true;
        camera1.enabled = false;
        camera2.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isSplit = !isSplit;
            SetSplitScreen(isSplit);
        }
    }

    public void SetSplitScreen(bool split)
    {
        if (split)
        {
            // Split: désactive la caméra principale, active celles des paddles
            mainCamera.enabled = false;
            camera1.enabled = true;
            camera2.enabled = true;

            // haut / bas
            camera1.rect = new Rect(0, 0.5f, 1, 0.5f);
            camera2.rect = new Rect(0, 0, 1, 0.5f);

            samba.Play();
        }
        else
        {
            // Retour à la caméra classique
            mainCamera.enabled = true;
            camera1.enabled = false;
            camera2.enabled = false;
        }
    }
}

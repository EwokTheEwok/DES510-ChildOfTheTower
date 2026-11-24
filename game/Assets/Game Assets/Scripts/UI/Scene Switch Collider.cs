using UnityEngine;

public class SceneSwitchCollider : MonoBehaviour
{
    [SerializeField] private SceneSwitch sceneSwitch;

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            sceneSwitch.LoadScene("Ending Scene");
        }
    }
}

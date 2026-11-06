using UnityEngine;

public class pauseUI : MonoBehaviour
{
    [SerializeField] private GameObject container;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            container.SetActive(true);
        }
    }

}

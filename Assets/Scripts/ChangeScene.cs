using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName = "FirstLevel";
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private KeyCode sceneKey = KeyCode.Z;
    private bool changing;

    private void Update()
    {
        if (Input.GetKeyDown(sceneKey) && !changing)
        {
            StartCoroutine(Wait());
            animator.SetTrigger("End");
            changing = true;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}
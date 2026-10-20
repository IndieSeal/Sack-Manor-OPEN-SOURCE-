using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private KeyCode sceneKey = KeyCode.Z;
    [SerializeField] private Animator menu;
    private bool changing;

    private void Update()
    {
        if (Input.GetKeyDown(sceneKey) && !changing)
        {
            animator.SetTrigger("End");
            changing = true;

            StartCoroutine(wait());
        }
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
        menu.gameObject.SetActive(true);

        while (!Input.GetKeyDown(sceneKey)) { yield return null; }

        menu.SetTrigger("End");
    }
}
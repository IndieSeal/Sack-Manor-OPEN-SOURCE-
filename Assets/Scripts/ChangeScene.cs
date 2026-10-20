using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField, FoldoutGroup("General")] private string sceneName = "FirstLevel";
    [SerializeField, FoldoutGroup("General")] private Animator animator;

    [SerializeField, FoldoutGroup("Scene With Key")] private bool changeSceneWithKey = false;
    [SerializeField, FoldoutGroup("Scene With Key"), ShowIf("changeSceneWithKey")] private KeyCode sceneKey = KeyCode.Z;
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
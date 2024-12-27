using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAnimationManager : MonoBehaviour
{
    private List<Animator> animators = new List<Animator>();

    private void Start()
    {
        animators.AddRange(FindObjectsOfType<Animator>());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene")
        {
            foreach (Animator animator in animators)
            {
                if (animator != null)
                {
                    animator.Play(0, -1, 0f);
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

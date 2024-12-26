using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAnimationManager : MonoBehaviour
{
    private List<Animator> animators = new List<Animator>();

    private void Start()
    {
        // Find all Animator components in the current scene and add them to the list
        animators.AddRange(FindObjectsOfType<Animator>());

        // Subscribe to the SceneManager.sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene is the Menu scene
        if (scene.name == "MenuScene") // Replace "MenuScene" with the actual name of your scene
        {
            // Restart animations for all animators
            foreach (Animator animator in animators)
            {
                if (animator != null) // Ensure the animator still exists
                {
                    animator.Play(0, -1, 0f); // Reset and play animation
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

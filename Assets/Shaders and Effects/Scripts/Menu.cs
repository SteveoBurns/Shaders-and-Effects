using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadersAssessment
{
    /// <summary>
    /// This class handles menu functionality
    /// </summary>
    public class Menu : MonoBehaviour
    {

        /// <summary>
        /// Load the passed scene name
        /// </summary>
        /// <param name="_sceneName">Name of the scene to load</param>
        public void LoadScene(string _sceneName) => SceneManager.LoadScene(_sceneName);
        
        
        /// <summary>
        /// Quits from both the Play Mode in the Unity Editor and the Built Application.
        /// </summary>
        public void QuitGame()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }
    }
}
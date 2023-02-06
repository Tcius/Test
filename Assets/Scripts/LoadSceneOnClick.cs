using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Para incluir el SceneManager

public class LoadSceneOnClick : MonoBehaviour {

	public void LoadByIndex (int sceneIndex) {
		SceneManager.LoadScene(sceneIndex); // Índice que aparece en el formulario de Build Settings
	}
	
}

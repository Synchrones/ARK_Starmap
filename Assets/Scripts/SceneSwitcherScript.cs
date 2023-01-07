using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherScript : MonoBehaviour
{   
    public Animator animator;

    void Start() 
    {
        DontDestroyOnLoad(GameObject.Find("options"));    
    }

    public void loadARK()
    {
        animator.SetTrigger("FadeOut");
    }

    public void onFadeCompleted()
    {
        SceneManager.LoadScene("Main");
    }
}

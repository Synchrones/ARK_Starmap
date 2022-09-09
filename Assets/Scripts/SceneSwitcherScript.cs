using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherScript : MonoBehaviour
{   
    public Animator animator;
    void Update()
    {
        
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

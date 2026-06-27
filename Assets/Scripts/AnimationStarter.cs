using UnityEngine;

public class AnimationStarter : MonoBehaviour
{
    [SerializeField] string animationName;
    void Start()
    {
        GetComponent<Animator>().Play(animationName);
    }

}

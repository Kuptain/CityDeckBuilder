using UnityEngine;
using UnityEngine.UI;

public class IconAnimationData : MonoBehaviour
{
    public Image image;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve curve;
    [SerializeField] AnimationCurve yCurve;
    [SerializeField] float heigthMultiplier;
    Vector3 startPosition;
    Transform endTarget;
    float timer;

    public void SetUp(Sprite sprite,Vector3 start ,Transform target)
    {
        image.sprite = sprite;
        startPosition = start;
        endTarget = target;
    }

    public void Move()
    {
        timer += Time.deltaTime;
        timer = Mathf.Clamp(timer, 0, duration);
        float lerpPosition = curve.Evaluate(timer/duration);
        float targetY = yCurve.Evaluate(timer / duration) * heigthMultiplier;
        if (timer == duration || endTarget == null) 
        {
            Arrive();
            return;
        }
        Vector3 targetVector = Vector3.Lerp(startPosition, endTarget.position, lerpPosition);
        transform.position = new Vector3(targetVector.x, targetY, targetVector.z);

    }
    void Arrive()
    {
        Destroy(gameObject);
    }
}

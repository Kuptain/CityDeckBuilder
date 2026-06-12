using UnityEngine;

public class HandPositionManager : MonoBehaviour
{
    [SerializeField] float width;
    [SerializeField] float angleTotal;
    [SerializeField] float lerpSpeed;

    void Update()
    {
        MoveHand();
    }

    void MoveHand()
    {
        int count = CardManager.instance.hand.Count;
        for(int i =0;i< count;i++)
        {
            CharacterCard card = CardManager.instance.hand[i];
            Vector3 target = GetTargetPosition(i, count);
            card.transform.position = Vector3.Lerp(card.transform.position, target, lerpSpeed * Time.deltaTime);
        }
    }

    Vector3 GetTargetPosition(int index, int count)
    {
        Vector3 target = new Vector3(transform.position.x + CalculateRelativPosition(index, count), transform.position.y, transform.position.z);
        return target;
    }

    float CalculateRelativPosition(int index, int count)
    {
        float x = width / (count-1);
        return x * index - (width / 2);
    }

    float CalculateRelativeAngle(int index, int count)
    {
        float angle = angleTotal / count;
        return (index - count / 2) * angle;
    }

}

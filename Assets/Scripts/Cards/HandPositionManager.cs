using UnityEngine;
using System.Collections.Generic;

public class HandPositionManager : MonoBehaviour
{
    [SerializeField] handType type;
    [SerializeField] float width;
    [SerializeField] float angleTotal;
    [SerializeField] float lerpSpeed;

    void Update()
    {
        if (type == handType.main)
        {
            MoveHand();
        }
        else
        {
            MoveTempHand();
        }
    }

    void MoveHand()
    {
        int count = CardManager.instance.hand.Count;
        for (int i = 0; i < count; i++)
        {
            CharacterCard card = CardManager.instance.hand[i];
            Vector3 target = GetTargetPosition(i, count);
            card.transform.position = Vector3.Lerp(card.transform.position, target, lerpSpeed * Time.deltaTime);
        }
    }
    void MoveTempHand()
    {
        int count = CardManager.instance.temporaryHand.Count;
        for (int i = 0; i < count; i++)
        {
            ResourceCard card = CardManager.instance.temporaryHand[i];
            if (card.frameCounter < 2)
            {
                card.frameCounter += 1;
            }
            else
            {
                Vector3 target = GetTargetPosition(i, count);
                card.transform.position = Vector3.Lerp(card.transform.position, target, lerpSpeed * Time.deltaTime);
            }
        }
    }

    Vector3 GetTargetPosition(int index, int count)
    {
        Vector3 target = new Vector3(transform.position.x + CalculateRelativPosition(index, count), transform.position.y, transform.position.z);
        return target;
    }

    float CalculateRelativPosition(int index, int count)
    {
        if(count <= 1)
        {
            return 0;
        }
        float x = width / (count - 1);
        return x * index - (width / 2);
    }

    float CalculateRelativeAngle(int index, int count)
    {
        float angle = angleTotal / count;
        return (index - count / 2) * angle;
    }


    enum handType
    {
        main = 0,
        temp = 1
    }
}

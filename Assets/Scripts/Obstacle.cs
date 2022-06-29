using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private Vector3 _startPosLeft, _startPosRight;

    [SerializeField]
    private float _moveTime;

    private void Start()
    {
        transform.position = Random.Range(0, 1) > 0.5f ? _startPosLeft : _startPosRight;
    }

    public void MoveToPos(Vector3 targetPos)
    {
        StartCoroutine(IMoveToPos(targetPos));
    }

    private IEnumerator IMoveToPos(Vector3 targetPos)
    {
        float timeElapsed = 0f;
        while(timeElapsed < _moveTime)
        {
            timeElapsed += Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveTime);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        transform.position = targetPos;
    }
}

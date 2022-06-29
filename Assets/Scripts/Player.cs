using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _bgObject,_directionObject,_greenExplosionPrefab,_redExplosionPrefab;

    private Transform _directionTransform;

    private bool hasGameStarted;
    private bool canRotate;
    [SerializeField]
    private float _maxOffset, _timeForHalfCycle, _intialOffset;

    private bool canShoot;
    private float shootCounter, shootCounterSpeed;

    private bool canMove;
    [SerializeField] private float _moveSpeed, _startPosY;
    private Vector3 moveDirection;

    [SerializeField]
    private AudioClip _moveClip, _loseClip, _winClip;

    private bool canScore;

    private void Awake()
    {
        _bgObject.SetActive(true);
        _directionTransform = _directionObject.transform;
        _directionTransform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void Start()
    {
        hasGameStarted = false;
        canRotate = true;

        canShoot = true;
        shootCounter = 0.5f;
        shootCounterSpeed = 1 / _timeForHalfCycle;

        canMove = false;
        moveDirection = Vector3.zero;

        canScore = false;
    }

    private void Update()
    {
        if(canShoot && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        AudioManager.Instance.PlaySound(_moveClip);

        canRotate = false;
        hasGameStarted = true;
        canShoot = false;
        canMove = true;
        canScore = true;

        float angle = _maxOffset * (shootCounter - 0.5f) * 2f - _intialOffset;
        moveDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized;
        _directionObject.SetActive(false);
        _bgObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!hasGameStarted) return;

        if(canRotate)
        {
            shootCounter += Time.fixedDeltaTime * shootCounterSpeed;

            if(shootCounter > 1f || shootCounter < 0f)
            {
                shootCounterSpeed *= -1f;
            }

            float angle = _maxOffset * (shootCounter - 0.5f) * 2f;
            _directionTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if(canMove)
        {
            Vector3 targetPos = transform.position + moveDirection;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed *Time.fixedDeltaTime);

            if(transform.position.y < _startPosY)
            {
                canMove = false;
                canRotate = true;
                canShoot = true;

                _directionObject.SetActive(true);
                _bgObject.SetActive(true);

                Vector3 tempPos = transform.position;
                tempPos.y = _startPosY;
                transform.position = tempPos;
            }
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.DEATH))
        {
            AudioManager.Instance.PlaySound(_loseClip);
            Destroy(collision.gameObject);
            Instantiate(_redExplosionPrefab, transform.position, Quaternion.identity);
            GameManager.Instance.EndGame();
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag(Constants.Tags.GOAL))
        {
            if (!canScore) return;
            AudioManager.Instance.PlaySound(_winClip);
            Destroy(collision.gameObject);
            Instantiate(_greenExplosionPrefab, transform.position, Quaternion.identity);
            GameManager.Instance.UpdateScore();
            moveDirection *= -1f;
            return;
        }
    }


}

using UI;
using UnityEngine;

namespace Player
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private bool _canJump = false;
        
        private PlayerController _player;
        private PausePanel _pause;
        private Rotator _rotator;
        private Camera _cam;

        private void Awake()
        {
            _player = FindObjectOfType<PlayerController>();
            _pause = FindObjectOfType<PausePanel>();
            _rotator = FindObjectOfType<Rotator>();
            _cam = Camera.main;
        }

        private void Update()
        {
            Rotate();
            Jump();
            Shoot();
            Pause();
        }

        private void Rotate()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(horizontal) > 0f)
            {
                _rotator.Rotate(horizontal);
            }
        }

        private void Jump()
        {
            if (_canJump && Input.GetKeyDown(KeyCode.Space))
            {
                _player.Jump();
            }
        }

        private void Shoot()
        {
            Vector2 pos = transform.position;
            Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - pos).normalized;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _player.Shoot(direction);
            }
        }

        private void Pause()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _pause.Toggle();
            }
        }
    }
}

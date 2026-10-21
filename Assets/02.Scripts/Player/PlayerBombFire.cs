using System;
using UI;
using UnityEngine;
using Weapon;

namespace Player
{
    public class PlayerBombFire : MonoBehaviour
    {
        [SerializeField] private Transform _firePosition;
        [SerializeField] private BombPool _bombPool;
        [SerializeField] private BombAmountUI _bombAmountUI;
        [SerializeField] private float _baseThrowPower = 15f;
        [SerializeField] private int _maxBombs = 3;
        [SerializeField] private float _maxHoldTime = 2f;
        [SerializeField] private float _maxThrowForce = 30f;

        private Camera _camera;
        private int _currentBombs;
        private float _holdTime;
        private float _throwForce;

        private void Awake()
        {
            _camera = Camera.main;
            _currentBombs = _maxBombs;
        }

        private void Start()
        {
            _bombAmountUI.Refresh(_currentBombs);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _holdTime = 0f;
                _throwForce = _baseThrowPower;
            }

            if (Input.GetMouseButton(1))
            {
                _holdTime += Time.deltaTime;
                float holdTimeRate = MathF.Min(1f, _holdTime / _maxHoldTime);
                _throwForce = Mathf.Lerp(_baseThrowPower, _maxThrowForce, holdTimeRate);
            }

            if (Input.GetMouseButtonUp(1) && _currentBombs > 0)
            {
                Bomb bomb = _bombPool.GetBomb(_firePosition.position);
                Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
                bombRigidbody.AddForce(_camera.transform.forward * _throwForce, ForceMode.Impulse);
                _currentBombs--;
                _bombAmountUI.Refresh(_currentBombs);
            }
        }
    }
}

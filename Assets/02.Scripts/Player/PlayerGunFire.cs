using System;
using UnityEngine;

namespace Player
{
    public class PlayerGunFire : MonoBehaviour
    {
        private const string FireButton = "Fire1";

        [SerializeField] private float _range = 100f;
        [SerializeField] private float _damage = 10f;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetButtonDown(FireButton))
                Shoot();
        }

        private void Shoot()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, _range))
                return;
            // Debug.Log("Hit: " + hit.collider.name);
        }
    }
}
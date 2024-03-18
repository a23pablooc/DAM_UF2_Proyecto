using System;
using System.Collections.Generic;
using Bullet;
using UnitScripts.PlanetScripts;
using UnityEngine;

namespace UnitScripts.ShipScripts
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Unit unit;

        [SerializeField] private AudioSource shootSound;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform weaponTransform;
        [SerializeField] private float damage;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float shootDelay;
        [SerializeField] private float bulletLifeTime;

        [SerializeField] private string playerBulletLayer;
        [SerializeField] private string iaBulletLayer;

        private float _lastShoot;

        private readonly List<Collider> _shipsTargets = new();
        private readonly List<Collider> _planetTargets = new();

        private void Start()
        {
            if (!GameManager.Instance) throw new Exception("GameManager not found");

            GameManager.Instance.OnPlanetDestroyed += OnPlanetDestroyed;
            GameManager.Instance.OnShipDestroyed += OnShipDestroyed;
        }

        private void OnDestroy()
        {
            if (!GameManager.Instance) throw new Exception("GameManager not found");

            GameManager.Instance.OnPlanetDestroyed -= OnPlanetDestroyed;
            GameManager.Instance.OnShipDestroyed -= OnShipDestroyed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsTargetShooteable(other)) return;

            if (other.gameObject.TryGetComponent<PlanetUnit>(out _))
                _planetTargets.Add(other);
            else if (other.gameObject.TryGetComponent<ShipUnit>(out _))
                _shipsTargets.Add(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!HasTargets(out var target)) return;

            unit.transform.LookAt(target.transform.position);

            if (!(Time.time > _lastShoot + shootDelay)) return;

            _lastShoot = Time.time;
            ShootBullet();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsTargetShooteable(other)) return;

            if (other.gameObject.TryGetComponent<PlanetUnit>(out _) && !_planetTargets.Contains(other))
                _planetTargets.Add(other);
            else if (other.gameObject.TryGetComponent<ShipUnit>(out _) && !_shipsTargets.Contains(other))
                _shipsTargets.Add(other);
        }

        private void ShootBullet()
        {
            var bullet = Instantiate(bulletPrefab, weaponTransform.position, weaponTransform.rotation);
            shootSound.Play();

            var owner = unit.GetComponent<Unit>().Owner;
            var bulletLayer = owner switch
            {
                PlayerType.Player => playerBulletLayer,
                PlayerType.IA => iaBulletLayer,
                PlayerType.Neutral => throw new Exception($"Invalid ship owner: {owner}"),
                _ => throw new Exception($"Invalid ship owner: {owner}")
            };

            ChangeLayer(bullet, bulletLayer);

            bullet.TryGetComponent<BulletController>(out var bulletController);
            bulletController.Init(unit.gameObject, damage, bulletLifeTime, bulletSpeed);
        }

        private static void ChangeLayer(GameObject bullet, string layer)
        {
            bullet.layer = LayerMask.NameToLayer(layer);
            foreach (Transform child in bullet.transform)
            {
                ChangeLayer(child.gameObject, layer);
            }
        }

        private bool IsTargetShooteable(Collider other)
        {
            var isShooteable = other.TryGetComponent<IShooteable>(out _);
            if (!isShooteable) return false;

            if (!other.gameObject.TryGetComponent<Unit>(out var otherUnit)) return false;

            return otherUnit.Owner != unit.Owner;
        }

        private bool HasTargets(out Collider target)
        {
            if (_shipsTargets.Count > 0)
            {
                target = _shipsTargets[0];
                return true;
            }

            if (_planetTargets.Count > 0)
            {
                target = _planetTargets[0];
                return true;
            }

            target = null;
            return false;
        }

        private void OnPlanetDestroyed(PlanetUnit planet)
        {
            _planetTargets.Remove(planet.GetComponent<Collider>());
        }

        private void OnShipDestroyed(ShipUnit ship)
        {
            _shipsTargets.Remove(ship.GetComponent<Collider>());
        }
    }
}
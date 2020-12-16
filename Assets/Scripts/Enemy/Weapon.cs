using System;
using UnityEngine;

public enum WeaponType
{
    Pistol,
    AssaultRifle,
    ShotGun
}

public class Weapon : MonoBehaviour
{
    public event Action StartShootAnimation = delegate { };
    public event Action StopShootAnimation = delegate { };
    public event Action StartReloadAnimation = delegate { };
    public event Action EndReloadAnimation = delegate { };

    public event Action OnShoot = delegate { };
    public event Action OnReload = delegate { };

    public int Magazine { get => _magazine; }
    public int backPack { get => _backPack; }
    public int AmmoCapacity { get => _ammoCapacity; }

    public WeaponType WeaponType = WeaponType.Pistol;
    [SerializeField] GameObject _bulletPrefab = null;
    [SerializeField] Transform _bulletSpawnPoint = null;
    [SerializeField] int _magazine = 10;
    [SerializeField] int _backPack = 10;

    [Header("Stats")]
    [SerializeField] int _damage = 10;
    [SerializeField] int _ammoCapacity = 10;
    [SerializeField] float _fireRate = 1f;
    [SerializeField] float _reloadTime = 3f;
    [SerializeField] float _maxReloadTime = 3f;

    public bool InfiniteBullets = false;

    IFighter<HitData, HitResult> _owner;
    public bool canShoot { get => _magazine > 0; }
    public bool isReloading = false;
    public bool shootPhase = false;
    public bool lockFire = false;
    public bool isFiring = false;

    public void StartShooting()
    {
        shootPhase = true;
        StartShootAnimation();
    }
    public void StopShooting()
    {
        shootPhase = false;
        StopShootAnimation();
    }

    public void SetOwner(IFighter<HitData, HitResult> owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// Instanciamos la bala y reducimos la cantidad de balas en nuestra magazine. Al llegar a 0 recargamos.
    /// </summary>
    public void Shoot()
    {
        if (lockFire) return;
        if (_magazine > 0)
        {
            if (!InfiniteBullets) _magazine--;

            lockFire = true;
            Bullet bulletInstace = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.LookRotation(_bulletSpawnPoint.forward)).GetComponent<Bullet>();
            bulletInstace.Damage = _damage;
            bulletInstace.SetOwner(_owner);

            OnShoot();

            if (_magazine <= 0)
                OnStartReload();
        }
    }

    public void OnStartReload()
    {
        StopShootAnimation();
        OnReload();
        if (InfiniteBullets || _backPack > 0)
        {
            isReloading = true;
            StartReloadAnimation();
        }
    }
    public void OnEndReloading()
    {
        if (!InfiniteBullets)
        {
            int bulletsToAdd = 0;
            if (_backPack >= _ammoCapacity)
                bulletsToAdd = _ammoCapacity;
            else if (_backPack < _ammoCapacity)
                bulletsToAdd = _backPack;

            _backPack -= bulletsToAdd;
            _magazine += bulletsToAdd;
        }

        EndReloadAnimation();
        isReloading = false;
        print("====================== END RELOAD PHASE ============================");

        if (shootPhase) StartShootAnimation();
    }
    public void LoadBulletInfo(int bulletsInMagazine, int bulletsInBackPak, bool infiniteBullets = false)
    {
        _magazine = bulletsInMagazine;
        _backPack = bulletsInMagazine;
        InfiniteBullets = infiniteBullets;
    }
    public void IncreaseDamage(int ammount)
    {
        if (ammount > 0)
            _damage += ammount;
    }
    public void IncreaseAmmoCapacity(int ammount)
    {
        if (ammount > 0)
            _ammoCapacity += ammount;
    }
    public void ReduxReloadTime(float Percentage)
    {
        if (Percentage > 0)
            _reloadTime -= _maxReloadTime * Percentage;
    }
    public void ReduxFireRate(float ammount)
    {
        if (ammount > 0)
            _fireRate -= ammount;
    }
}

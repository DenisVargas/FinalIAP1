using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sonido")]
    [SerializeField] private AudioSource ManagerSounds;
    [SerializeField] private AudioClip PistolShootS;
    [SerializeField] private AudioClip PistolShootS2;
    [SerializeField] private AudioClip PistolShootS3;
    [SerializeField] private AudioClip RifleShootS;
    [SerializeField] private AudioClip SurvivourHurtS1;
    [SerializeField] private AudioClip SurvivourHurtS2;
    [SerializeField] private AudioClip ZombieAttackS;
    [SerializeField] private AudioClip ZombieDyingS;


    void PistolShootSound()
    {
       
        ManagerSounds.PlayOneShot(PistolShootS);
    }

    void PistolShootSound2()
    {
        ManagerSounds.PlayOneShot(PistolShootS2);

    }

    void PistolShootSound3()
    {
        ManagerSounds.PlayOneShot(PistolShootS3);

    }



    void RifleShootSound()
    {
        ManagerSounds.PlayOneShot(RifleShootS);
    }


    void SurvivourHurtSound1()
    {
        ManagerSounds.PlayOneShot(SurvivourHurtS1);

    }

    void SurvivourHurtSound2()
    {
        ManagerSounds.PlayOneShot(SurvivourHurtS2);

    }

    void ZombieAttackSound()
    {
        ManagerSounds.PlayOneShot(ZombieAttackS);

    }

    void ZombieDyingSound()
    {
        ManagerSounds.PlayOneShot(ZombieDyingS);


    }
}

using UnityEngine;
using UnityEngine.Events;

public class AnimEventReciever : MonoBehaviour
{
    [SerializeField] UnityEvent AttackStart = new UnityEvent();
    [SerializeField] UnityEvent AttackActive = new UnityEvent();
    [SerializeField] UnityEvent AttackRecovery = new UnityEvent();
    [SerializeField] UnityEvent AttackEnd = new UnityEvent();
    [SerializeField] UnityEvent AlertStart = new UnityEvent();
    [SerializeField] UnityEvent AlertEnd = new UnityEvent();

    void AE_StartUp()
    {
        //Debug.Log("AttackState::AnimationEvent::Startup");
        AttackStart.Invoke();
    }

    void AE_ActiveStart()
    {
        //Debug.Log("AnimEventReciever::AnimationEvent::Active");
        AttackActive.Invoke();
    }

    void AE_Recovery()
    {
        //Debug.Log("AnimEventReciever::AnimationEvent::Recovery");
        AttackRecovery.Invoke();
    }

    void AE_AttackEnd()
    {
        //Debug.Log("AnimEventReciever::START");
        AttackEnd.Invoke();
    }

    void AE_AlertStart()
    {
        AlertStart.Invoke();
    }

    void AE_AlertEnd()
    {
        AlertEnd.Invoke();
    }
}

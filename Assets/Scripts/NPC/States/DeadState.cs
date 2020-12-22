using UnityEngine;
using IA.FSM;
using System.Collections;

public class DeadState : State
{
    
    [SerializeField] Collider[] collisions = new Collider[0];
    [SerializeField] bool multipleDeads = false;

    public override void Begin()
    {
        int deadAnim = Random.Range(0, 2);
        //print("Entre a muerte");

        if (multipleDeads)
            switch (deadAnim)
            {
                case 0:
                    _anims.Play("Dead 1"); //Seteo la animacion.
                    break;

                case 1:
                    _anims.Play("Dead 2"); //Seteo la animacion.
                    break;

                default:
                    break;
            }
        else
            _anims.Play("Dead");

        foreach (var item in collisions)
            item.enabled = false;

        StartCoroutine(delayedDestroy());
    }

    IEnumerator delayedDestroy()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}

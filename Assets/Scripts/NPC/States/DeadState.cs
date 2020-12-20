using UnityEngine;
using IA.FSM;

public class DeadState : State
{
    [SerializeField] bool multipleDeads = false;

    public override void Begin()
    {
        int deadAnim = Random.Range(0, 2);
        print("Entre a muerte");

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
    }
}

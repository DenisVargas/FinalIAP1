using System;
using System.Linq;
using UnityEngine;
using IA.LineOfSight;
using IA.PathFinding;
using IA.FSM;

[RequireComponent(typeof(Animator))]
public class Police : MonoBehaviour, IDamageable<Damage, HitResult>
{
    [SerializeField] int health = 100;
    [SerializeField] float AttackRange = 1.5f;
    [SerializeField] LineOfSightComponent sight;
    [SerializeField] Animator anims;
    [SerializeField] FiniteStateMachine<CommonState> _states;
   

    Action<IDamageable<Damage, HitResult>> setAttackTarget = delegate { };

    public bool IsAlive => health > 0;

    // Start is called before the first frame update
    void Awake()
    {
        anims = GetComponent<Animator>();
   

        #region StateMachine

        _states = new FiniteStateMachine<CommonState>();

        var idle = GetComponent<IdleState>().SetAnimator(anims).AttachTo(_states, true);

        //Attack
       /* PoliceShootState shoot = GetComponent<PoliceShootState>();
        shoot.OnAttackEnded += AttackCompleted;
        setAttackTarget += shoot.SetTarget;
        shoot.AttachTo(_states);*/

        //Move
        MoveToState mv = GetComponent<MoveToState>();
        mv.findTarget = FindCloserTarget;
        mv.SetAnimator(anims)
          .AttachTo(_states);
       

        var dead = GetComponent<DeadState>().SetAnimator(anims).AttachTo(_states);

        idle.AddTransition(mv, (cs) => { print("Transitioning!"); })
            //.AddTransition(shoot)
            .AddTransition(dead, (cs) => { print("Transitioning to Dead from Idle"); });

        mv.AddTransition(idle, (cs) => { print("Transitioning!"); })
         // .AddTransition(shoot)
          .AddTransition(dead, (cs) => { });

        dead.AddTransition(dead, (cs) => { print("Transitioning"); })
            .AddTransition(idle, (cs) => { });

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
     
        _states.Update();
    }

 

    public CommonState getCurrentStateType()
    {
        return _states.getCurrentStateType();
    }
    /// <summary>
    /// Encuentra y Evalúa si un Enemigo esta dentro de la línea de visión.
    /// </summary>
    public void FindCloserTarget()
    {
        var posibleTargets = Physics.OverlapSphere(transform.position, sight.range, sight.visibles);
        var closerTarget = posibleTargets
                           .Where(x => x.transform.CompareTag("Zombie"))
                           .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                           .Select(x => GetComponent<IDamageable<Damage, HitResult>>())
                           .FirstOrDefault();

        if (closerTarget != null && sight.IsInSight(closerTarget.transform))
        {
            print("Got you bitch");
            setAttackTarget(closerTarget);
            _states.Feed(CommonState.alert);
        }
    }


    /// <summary>
    /// Esta unidad sufre daño.
    /// </summary>
    /// <param name="inputDamage">Estadísticas del daño recibido.</param>
    /// <returns>El resultado del ataque.</returns>
    public HitResult getHit(Damage inputDamage)
    {
        health -= inputDamage.damageAmmount;

        if (health <= 0)
        {
            health = 0;
            _states.Feed(CommonState.dead);
        }

        return new HitResult();
    }
    /// <summary>
    /// Callback. Se llama cuando esta unidad realiza un ataque sobre otra entidad.
    /// </summary>
    /// <param name="hitResult">El resultado del ataque.</param>
    public void onHit(HitResult hitResult)
    {
        //Si mi target actual está muerto, cambio de estado.
    }
}

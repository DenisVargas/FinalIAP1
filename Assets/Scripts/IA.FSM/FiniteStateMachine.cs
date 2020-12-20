using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM
{
    [Serializable]
    public class FiniteStateMachine<T>
    {
        public IState<T> currentState;
        [SerializeField] Dictionary<T, IState<T>> States = new Dictionary<T, IState<T>>();

        public FiniteStateMachine()
        {
            currentState = null;
            States = new Dictionary<T, IState<T>>();
        }
        public void Update()
        {
            if (currentState != null)
                currentState.Execute();
            else
                Debug.LogWarning("El estado no ha sido asignado");
        }
        public void Feed(T input)
        {
            if (currentState != null)
                currentState = currentState.transitionTo(input);
        }
        public void Feed(T input, T fallback)
        {
            currentState = currentState.transitionTo(input);
            if (currentState == null)
                SetState(fallback);
        }
        public T getCurrentStateType()
        {
            return currentState.getStateType;
        }
        public void SetState(T input)
        {
            if(States.ContainsKey(input))
                currentState = States[input];
        }
        public void AddState(IState<T> state)
        {
            var stateType = state.getStateType;
            if (States.ContainsKey(stateType))
            {
                States[state.getStateType] = state;
                //Debug.Log("El estado existe");
            }
            else
            {
                States.Add(state.getStateType, state);
                //Debug.Log($"El estado no existe en la SM. Añadiendo {state.getStateType}");
            }
        }
    }
}

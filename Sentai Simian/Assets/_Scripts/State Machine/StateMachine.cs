using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS.StateMachine
{
    public class StateMachine<T>
    {
        private T owner;

        private State<T> currentState;
        private State<T> previousState;
        private State<T> globalState;

        public State<T> CurrentState => currentState;
        public State<T> PreviousState => previousState;
        public State<T> GlobalState => globalState;
        public void SetGlobalState(State<T> newState) => globalState = newState;

        public StateMachine(T _owner)
        {
            owner = _owner;
        }

        public void Update()
        {
            if (globalState != null) globalState.Execute(owner);

            if (currentState != null) currentState.Execute(owner);
        }

        public void SwitchState(State<T> newState)
        {
            if (newState == null)
                return;

            if (currentState != null)
            {
                previousState = currentState;
                currentState.Exit(owner);
            }

            currentState = newState;
            currentState.Enter(owner);
        }

        public void RevertToPreviousState()
        {
            if (previousState != null)
                SwitchState(previousState);
        }

        public void HandleMessage(Telegram telegram)
        {
            if (currentState != null && currentState.HandleMessage(owner, telegram))
            {
                return;
            }

            if (globalState != null)
            {
                globalState.HandleMessage(owner, telegram);
            }
        }
    }
}

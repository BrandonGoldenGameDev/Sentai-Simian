namespace SS.StateMachine
{
    [System.Serializable]
    public class State<T>
    {
        public virtual void Enter(T owner) { }
        public virtual void Execute(T owner) { }
        public virtual void Exit(T owner) { }
        public virtual bool HandleMessage(T owner, Telegram telegram) { return false; }
    }
}
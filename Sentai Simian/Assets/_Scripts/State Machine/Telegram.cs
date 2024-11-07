using UnityEngine;

namespace SS.StateMachine
{
    public enum Message
    {
        MSG_DAMAGED,
        MSG_DAMAGED_LASER,
        MSG_STAGGERED,
        MSG_COLLISION,
        MSG_DEAD
    }

    public class Telegram
    {
        public Transform sender;
        public Message message;

        public Telegram(Transform sender, Message message)
        {
            this.sender = sender;
            this.message = message;
        }
    }
}
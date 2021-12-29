
using System;
using System.Collections.Generic;

namespace AnilTools
{
    public static class Messenger
    {
        /*  
            EXAMPLES
            
            Messenger.SendMessage("on golem killed", golemKilledVoid());
            Messenger.SendMessage(onjumpedHash, () => doaction);
            
            Messenger.GetMessage("on golem killed"); // returns has message
            Messenger.GetMessage("onjumpedHash");    // returns has message
            
            Messenger.AddListener("OnPlayerRoll", roll());
            Messenger.Listen("OnPlayerRoll");  // Invokes Actions
        */

        /// <summary>
        /// key object might be int string and so on
        /// </summary>
        private static readonly List<object> MessageList = new List<object>();
        /// <summary>
        /// key object might be int string and so on
        /// </summary>
        private static readonly Dictionary<object, List<Action>> Events = new Dictionary<object, List<Action>>();

        /// <returns>has message</returns>
        public static bool GetMessage(object hash)
        {
            if (!MessageList.Contains(hash))
            {
                //Debug2.LogWarning("messageCouldnt Find");
                return false;
            }
            return true;
        }

        public static void SendMessage(object hash)
        {
            MessageList.Add(hash);
        }

        public static void RemoveMessage(object hash)
        {
            if (!MessageList.Remove(hash))
                Debug2.LogWarning("message coundnt be read");
        }

        public static void AddListener(object hash, params Action[] messageEvents)
        {
            if (!Events.ContainsKey(hash))
            Events.Add(hash, new List<Action>());

            Events[hash].AddRange(messageEvents);
        }

        public static void RemoveListener(object hash)
        {
            Events.Remove(hash);
        }

        /// <summary>
        /// Invokes Actions
        /// </summary>
        public static void Listen(object hash)
        {
            if (Events.TryGetValue(hash, out List<Action> message))
            {
                message.ForEach(x => x.Invoke());
            }
            else
            {
                Debug2.LogError("event listener doesnt found");
            }
        }

    }
}

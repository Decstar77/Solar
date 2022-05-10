using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    using EventCallback = Func<EventType, object, bool>;

    public enum EventType
    {
        INVALID = 0,
        RENDER_START,
        RENDER_END,

        ON_SAVE,
        ON_COMPILE,

    }

    public static class EventSystem
    {

        private static List<EventCallback>[] events;
        private static List<object>[] listeners;

        internal static bool Initialize()
        {
            int count = Enum.GetValues(typeof(EventType)).Length;
            events = new List<EventCallback>[count];
            listeners = new List<object>[count];

            for (int i = 0; i < count; i++)
            {
                events[i] = new List<EventCallback>();
                listeners[i] = new List<object>();
            }
            
            return true;
        }

        public static void AddListener(EventType type, EventCallback callback, object listener)
        {
            int index = (int)type;
            events[index].Add(callback);
            listeners[index].Add(listener);
        }

        public static void RemoveListner(EventType type, object listener) {
            int index = (int)type;
            int removeIndex = 0;

            for (int i = 0; i < events.Length; i++)
            {
                if (listeners[index][i] ==  listener)
                {
                    removeIndex = i;
                    break;
                }
            }

            listeners[index].RemoveAt(removeIndex);
            events[index].RemoveAt(removeIndex);
        }

        public static bool Fire(EventType type, object context)
        {
            int index = (int)type;
            foreach (EventCallback callback in events[index])
            {
                if (callback.Invoke(type, context))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

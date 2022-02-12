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
    }

    public static class EventSystem
    {                
        private static List<EventCallback>[] events;

        internal static bool Initialize()
        {
            int count = Enum.GetValues(typeof(EventType)).Length;
            events = new List<EventCallback>[count];

            for (int i = 0; i < count; i++)
            {
                events[i] = new List<EventCallback>();
            }
            
            return true;
        }

        public static void Listen(EventType type, EventCallback callback)
        {
            int index = (int)type;
            events[index].Add(callback);
        }

        public static void Fire(EventType type, object context)
        {
            int index = (int)type;
            foreach (EventCallback callback in events[index])
            {
                if (callback.Invoke(type, context))
                {
                    break;
                }
            }
        }
    }
}

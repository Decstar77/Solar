using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public abstract class Pin
    {
        private static int IdCounter = 10000;
        public int Id { get { return id; } }
        private int id = -1;
        protected Pin connectedTo = null;

        public string Name { get; set; }
        public bool IsInputPin { get; }

        public Pin(string name, bool isInputPin)
        {
            id = IdCounter++;
            Name = name;
            IsInputPin = isInputPin;
        }

        public abstract void DrawUI();
        public abstract bool CanConnect(Pin pin);

        public void Connect(Pin pin)
        {
            if (CanConnect(pin)) {
                Disconnect();
                pin.Disconnect();
                connectedTo = pin;
                pin.connectedTo = this;
            }
        }

        public void Disconnect()
        {
            if (connectedTo != null)
                connectedTo.connectedTo = null;
            connectedTo = null;
        }

        public bool IsConnected() => connectedTo != null;
        public Pin GetConnectedPin() => connectedTo;
    }
}

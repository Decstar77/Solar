using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public class Pin
    {
        private static int IdCounter = 10000;
        public int Id { get { return id; } }
        private int id = -1;
        private Pin connectedTo = null;

        public string Name { get; set; }

        public Pin(string name)
        {
            id = IdCounter++;
            Name = name;
        }

        public void Connect(Pin pin)
        {
            Disconnect();
            pin.Disconnect();
            connectedTo = pin;
            pin.connectedTo = this;
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

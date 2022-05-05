using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public enum PinInputType
    {
        INPUT = 0x1,
        OUTPUT = 0x2,
    }

    public abstract class Pin
    {
        private static int IdCounter = 10000;

        public int Id { get { return id; } }

        private int id = -1;

        public Node Node { get { return node; } }        
        protected Node node = null;

        public string Name { get; set; }
        
        public PinInputType PinType { get; set; }


        protected Pin connectedTo = null;

        public Pin(string name, Node node, PinInputType pinType)
        {
            id = IdCounter++;
            Name = name;
            this.node = node;
            PinType = pinType;

            if (pinType == PinInputType.INPUT)
                node.InputPins.Add(this);
            if (pinType == PinInputType.OUTPUT)
                node.OutputPins.Add(this);
        }

        public abstract void DrawUI();
        public abstract bool CanConnect(Pin pin);

        protected void DrawBasicPins()
        {
            if (PinType == PinInputType.INPUT)
            {
                ImNodes.BeginInputAttribute(Id, ImNodesPinShape.CircleFilled);
                ImGui.Text(Name + " ");
                ImNodes.EndInputAttribute();
            }
            else if (PinType == PinInputType.OUTPUT)
            {
                ImNodes.BeginOutputAttribute(Id, ImNodesPinShape.CircleFilled);
                ImGui.Text(Name + " ");
                ImNodes.EndOutputAttribute();
            }
        }

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

    public abstract class ValuePin<T> : Pin
    {
        protected T value = default(T);

        protected ValuePin(string name, Node node, PinInputType pinType) : base(name, node, pinType)
        {
        }

        public void SetValue(T t)
        {
            value = t;
        }

        public T GetValue()
        {
            if (IsConnected() && PinType == PinInputType.INPUT)
            {
                return ((ValuePin<T>)connectedTo).GetValue();
            }

            return value;
        }
    }


}

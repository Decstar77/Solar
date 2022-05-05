using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public struct SerNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> OuputPinIds { get; set; }
        public List<int> InputPinIds { get; set; }
        public int InFlowPin { get; set; }
        public int OutFlowPin { get; set; }
        public object Data { get; set; }

        public SerNode()
        {
            Id = -1;
            Name = string.Empty;
            OuputPinIds = new List<int>();
            InputPinIds = new List<int>();
            OutFlowPin = 0;
            InFlowPin = 0;
            Data = null;
        }
    }




    public abstract class Node
    {
        private static int IdCounter = 0;
        public int Id { get { return id; } }
        private int id = -1;

        public List<Pin> OutputPins { get; set; }
        public List<Pin> InputPins { get; set; }

        public string Name { get; set; }
        public FlowPin inFlowPin = null;
        public FlowPin outFlowPin = null;

        public Node(string name)
        {
            id = IdCounter++;
            Name = name;
            OutputPins = new List<Pin>();
            InputPins = new List<Pin>();
        }

        public SerNode CreateSerNode()
        {
            SerNode serNode = new SerNode();
            serNode.Id = id;
            serNode.Name = Name;
            serNode.InFlowPin = inFlowPin == null ? -1 : inFlowPin.Id;
            serNode.OutFlowPin = outFlowPin == null ? -1 : outFlowPin.Id;
            serNode.OuputPinIds.AddRange(OutputPins.Select(x => x.Id));
            serNode.InputPinIds.AddRange(InputPins.Select(x => x.Id));
            serNode.Data = new DepthStencilDesc();

            return serNode;
        }

        public abstract void DrawUI();
        public abstract bool CreateResources(RenderGraph renderGraph);
        public abstract Node Run(RenderGraph graph, Context context);

        protected void AddFlowPins() {
            inFlowPin = new FlowPin("In", this, PinInputType.INPUT);
            outFlowPin = new FlowPin("Out", this, PinInputType.OUTPUT);
        }

        protected void DrawFlowPins() {
            ImNodes.BeginInputAttribute(inFlowPin.Id, ImNodesPinShape.CircleFilled);
            ImGui.Text(inFlowPin.Name);
            ImNodes.EndInputAttribute();
            ImGui.SameLine();
            ImNodes.BeginOutputAttribute(outFlowPin.Id, ImNodesPinShape.CircleFilled);
            ImGui.Text(outFlowPin.Name);
            ImNodes.EndOutputAttribute();
        }

        public Node SetPositionScreenSpace(Vector2 pos)
        {
            ImNodes.SetNodeScreenSpacePos(id, pos.x, pos.y);
            return this;
        }

        protected T DrawStruct<T>(object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(bool))
                {
                    bool v = (bool)field.GetValue(obj);
                    ImGui.CheckBox(field.Name, ref v);
                    field.SetValue(obj, v);
                }
                else if (field.FieldType.IsEnum)
                {
                    string[] names = field.FieldType.GetEnumNames();
                    string curName = field.FieldType.GetEnumName(field.GetValue(obj));
                    Array values = field.FieldType.GetEnumValues();

                    int index = Array.IndexOf(names, curName);

                    ImGui.PushItemWidth(100);
                    ImGui.Combo(field.Name, ref index, names);
                    ImGui.PopItemWidth();

                    field.SetValue(obj, Enum.ToObject(field.FieldType, values.GetValue(index)));
                }
            }

            return (T)obj;
        }
    }
}

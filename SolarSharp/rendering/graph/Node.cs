using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public abstract class Node
    {
        private static int IdCounter = 0;
        public int Id { get { return id; } }
        private int id = -1;

        public List<Pin> OutputPins { get; set; }
        public List<Pin> InputPins { get; set; }

        public string Name { get; set; }

        protected FlowPin inPin;
        protected FlowPin outPin;

        public Node(string name)
        {
            id = IdCounter++;
            Name = name;
            OutputPins = new List<Pin>();
            InputPins = new List<Pin>();
        }

        public abstract void DrawUI();
        public abstract bool CreateResources(RenderGraph renderGraph);
        public abstract void Run(RenderGraph graph, Context context);

        protected void AddFlowPins() {
            inPin = new FlowPin("In", this, PinInputType.INPUT);
            outPin = new FlowPin("Out", this, PinInputType.OUTPUT);
        }

        protected void DrawFlowPins() {
            ImNodes.BeginInputAttribute(inPin.Id, ImNodesPinShape.CircleFilled);
            ImGui.Text(inPin.Name);
            ImNodes.EndInputAttribute();
            ImGui.SameLine();
            ImNodes.BeginOutputAttribute(outPin.Id, ImNodesPinShape.CircleFilled);
            ImGui.Text(outPin.Name);
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

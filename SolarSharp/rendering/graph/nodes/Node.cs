using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolarSharp.Rendering.Graph
{
    public struct SerializationNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, object> Pins { get; set; }
        public string ClassName { get; set; }
        public IDictionary<string, object> ClassData { get; set; }
        public float PositionEditorSpaceX { get; set; }
        public float PositionEditorSpaceY { get; set; }
    }

    public class RenderGraphSerializableData : System.Attribute
    {    
    }

    public abstract class Node
    {
        public static int IdCounter = 0;
        public int Id { get { return id; } }
        private int id = -1;
        public string Name { get; set; }
        public Vector2 PositionEditorSpace { get; set; }
        public List<Pin> OutputPins { get; set; }
        public List<Pin> InputPins { get; set; }
        public FlowPin inFlowPin { get; set; }
        public FlowPin outFlowPin { get; set; }

        public Node(string name)
        {
            id = IdCounter++;
            Name = name;
            OutputPins = new List<Pin>();
            InputPins = new List<Pin>();
        }

        public abstract void DrawUI();
        public abstract bool CreateResources(RenderGraph renderGraph);
        public abstract Node Run(RenderGraph graph, Context context);

        public SerializationNode CreateSerNode()
        {
            SerializationNode serNode = new SerializationNode();
            serNode.Id = id;
            serNode.Name = Name;
            serNode.PositionEditorSpaceX = PositionEditorSpace.x;
            serNode.PositionEditorSpaceY = PositionEditorSpace.y;

            Type type = GetType();
            serNode.ClassName = type.FullName;
            
            serNode.ClassData = type.GetProperties().
                Where(x => Attribute.IsDefined(x, typeof(RenderGraphSerializableData))).
                ToDictionary(x => x.Name, x => x.GetValue(this, null));

            serNode.Pins = type.GetProperties().
                Where(x => x.PropertyType.IsAssignableTo(typeof(Pin))).
                ToDictionary(x => x.Name, x => x.GetValue(this, null));

            return serNode;
        }

        public static Node CreateFromSerNode(SerializationNode serNode) 
        {
            Type t = Type.GetType(serNode.ClassName);
            Node node = (Node)Activator.CreateInstance(t);

            node.id = serNode.Id;
            node.Name = serNode.Name;
            node.PositionEditorSpace = new Vector2(serNode.PositionEditorSpaceX, serNode.PositionEditorSpaceY);

            node.OutputPins.Clear();
            node.InputPins.Clear();

            foreach (var v in serNode.Pins) {
                PropertyInfo f = t.GetProperty(v.Key);
                if (v.Value != null) {
                    Pin pin = (Pin)JsonSerializer.Deserialize((JsonElement)v.Value, f.PropertyType);
                    pin.Node = node;
                    f.SetValue(node, pin);

                    if (pin.PinType == PinInputType.INPUT) node.InputPins.Add(pin);
                    if (pin.PinType == PinInputType.OUTPUT) node.OutputPins.Add(pin);
                }
            }

            foreach (var v in serNode.ClassData) {
                PropertyInfo f = t.GetProperty(v.Key);
                object a = JsonSerializer.Deserialize((JsonElement)v.Value, f.PropertyType);
                f.SetValue(node, a);
            }           

            return node;
        }

        public void SerializeConnections(RenderGraph renderGraph)
        {
            OutputPins.ForEach(x => x.SerializeConnections(renderGraph));
            InputPins.ForEach(x => x.SerializeConnections(renderGraph));
        }

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

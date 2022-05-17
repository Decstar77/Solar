using SolarSharp;
using SolarSharp.core;
using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    public class LRule
    { 
        public char Letter { get; set; }
        private string[] possibleResults;
        private Random random;

        public LRule(char letter, params string[] possibleResults)
        {
            Letter = letter;
            this.possibleResults = possibleResults;
            random = new Random();
        }

        public string GetResult()
        {
            return possibleResults[random.Next(possibleResults.Length)];            
        }
    }

    public struct LParams
    {
        public Vector3 position;
        public Vector3 direction;
        public int length;
    }

    public enum LevelLetterEncoding
    {
        UNKOWN = '1',
        SAVE = '[',
        LOAD = ']',
        FORWARD = 'F',
        TURN_RIGHT = '+',
        TURN_LEFT = '-',
    }

    public class LevelGenerator
    {
        public LRule[] rules;        
        public int iterationLimit = 1;

        public LevelGenerator(int interationLimit, params LRule[] rules)
        {
            this.iterationLimit = interationLimit; ;
            this.rules = rules;
        }

        public string GenerateSequence(string word )
        { 
            return GrowRecursive(word);
        }

        public static void DebugDraw_(string word, int length)
        {
            Stack<LParams> lParams = new Stack<LParams>();

            Vector3 direction = Vector3.UnitZ;
            Vector3 currentPos = Vector3.Zero;

            foreach (var c in word)
            {                
                LevelLetterEncoding encoding = (LevelLetterEncoding)c;
                switch (encoding)
                {
                    case LevelLetterEncoding.UNKOWN:
                        Debug.Assert(false);
                        break;
                    case LevelLetterEncoding.SAVE:
                        lParams.Push(new LParams
                        {
                            direction = direction,
                            position = currentPos,
                            length = length
                        });
                        break;
                    case LevelLetterEncoding.LOAD: {
                            LParams param = lParams.Pop();
                            direction = param.direction;
                            currentPos = param.position;
                            length = param.length;
                        }
                        break;

                    case LevelLetterEncoding.FORWARD: {
                            Vector3 temp = currentPos;
                            currentPos += direction * length;
                            length -= 4;
                            DebugDraw.Line(temp + Vector3.UnitY * 2, currentPos + Vector3.UnitY * 2);
                        }                       
                        break;
                    case LevelLetterEncoding.TURN_RIGHT:
                        direction = Quaternion.RotatePoint(direction, Quaternion.RotateLocalY(Quaternion.Identity, Util.DegToRad(90)));
                        break;
                    case LevelLetterEncoding.TURN_LEFT:
                        direction = Quaternion.RotatePoint(direction, Quaternion.RotateLocalY(Quaternion.Identity, Util.DegToRad(-90)));
                        break;
                }

            }

        }

        private string GrowRecursive(string word, int iterationIndex = 0)
        {
            if (iterationIndex >= iterationLimit)
            {
                return word;
            }

            StringBuilder newWord = new StringBuilder();
            foreach (var c in word) {
                newWord.Append(c);
                foreach (var r in rules) {
                    if (r.Letter == c) {
                        newWord.Append(GrowRecursive(r.GetResult(), iterationIndex + 1));
                    }                     
                }
            }

            return newWord.ToString();
        }
    }



    internal class LevelGeneratorWindow : EditorWindow
    {
        string result = "";
        int iterations = 2;
        int length = 8;
        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Level generator", ref show))
            {
                ImGui.InputInt("Iterations", ref iterations);
                ImGui.InputInt("Length", ref length);
                if (ImGui.Button("Generate"))
                {
                    LRule rule = new LRule('F', "[+F][-F]", "[+F]F[-F]", "[-F]F[+F]");
                    LevelGenerator levelGenerator = new LevelGenerator(iterations, rule);
                    result = levelGenerator.GenerateSequence("[F]--F");
                }

                
                if (!string.IsNullOrEmpty(result))
                {
                    ImGui.Text(result);
                    LevelGenerator.DebugDraw_(result, length);
                }
            }

            ImGui.End();
        }

        public override void Shutdown()
        {
        }

        public override void Start()
        {
        }
    }
}

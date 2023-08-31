using System.Text;
using System.Text.Json;

namespace MIIDIToLFO.Lib.Convert.LFO
{
    // Class represents Vital's LFO JSON structure for direct conversion.
    public class VitalLFOShape : LFOShape
    {
        public const uint maxNumPointsConst = 136;
        public string author { get; set; } = "MIIDIToLFO";
        public string name { get; set; } = "";
        public uint num_points { get; set; } = 0;
        public List<double> points { get; set; } = new List<double>();
        public List<double> powers { get; set; } = new List<double>();
        public bool smooth { get; set; } = false;

        public VitalLFOShape(string name = "") 
        {
            this.name = name;
            maxNumPoints = maxNumPointsConst;
        }
        public override void AddPoint(LFOPoint point)
        {
            if (num_points < maxNumPoints)
            {
                points.Add(point.x);
                points.Add(point.y);
                powers.Add(Utils.Map0To1(point.f, -10.0, 10.0));
                num_points++;
                index = num_points;
            }
        }

        public override byte[] ToBinary()
        {
            var json = JsonSerializer.Serialize(this);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}

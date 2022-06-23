using System.Windows.Forms;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace BlightHelper
{
    public class Settings : ISettings
    {


        public ToggleNode Enable { get; set; } = new ToggleNode(false);

        public ToggleNode Debug2 { get; set; } = new ToggleNode(false);

        public ButtonNode RefreshFile { get; set; } = new ButtonNode();        
        public RangeNode<int> FrameThickness { get; set; } = new RangeNode<int>(2, 2, 5);
        public ColorNode BaseColor { get; set; } = new ColorNode(SharpDX.Color.DeepPink);





    }
}

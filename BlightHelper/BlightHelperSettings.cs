using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace BlightHelper
{
    public class BlightHelperSettings : ISettings
    {
        //Mandatory setting to allow enabling/disabling your plugin
        public ToggleNode Enable { get; set; } = new ToggleNode(false);

        public ToggleNode Debug2 { get; set; } = new ToggleNode(false);

        public ButtonNode RefreshFile { get; set; } = new ButtonNode();
        public RangeNode<int> FrameThickness { get; set; } = new RangeNode<int>(2, 2, 5);
        public ColorNode BaseColor { get; set; } = new ColorNode(Color.DeepPink);

        public FilterList FilterList { get; set; } = new FilterList();
    }
    [Submenu]
    public class FilterList
    {

        public ListNode ListFilter { get; set; } = new ListNode();

    }
}
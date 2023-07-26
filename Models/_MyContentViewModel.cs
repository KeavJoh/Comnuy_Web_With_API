namespace ComnuyWebWithAPI.Models
{
    public class _ContentDeliveryModel
    {
        public List<Tool> Tools { get; set; }
        public Tool Tool { get; set; }
        public List<Tool> RandomTools { get; set; }
        public List<Tool> ToolsOrderByDate { get; set; }
        public List<ToolGroup> ToolGroups { get; set; }
        public ToolGroup ToolGroup { get; set; }
        public int SelectedToolGroupId { get; set; }
    }
}

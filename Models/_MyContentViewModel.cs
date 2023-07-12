namespace ComnuyWebWithAPI.Models
{
    public class _MyContentViewModel
    {
        public List<Tool> Tools { get; set; }
        public List<Tool> RandomTools { get; set; }
        public List<Tool> ToolsOrderByDate { get; set; }
        public List<ToolGroup> ToolGroups { get; set; }
        public int SelectedToolGroupId { get; set; }
    }
}

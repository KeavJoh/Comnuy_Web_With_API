﻿namespace ComnuyWebWithAPI.Models
{
    public class Tool
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? WebAddress { get; set; }
        public string? Picture_1 { get; set; }
        public string? Picture_2 { get; set; }
        public string? Picture_3 { get; set; }
        public string? Picture_4 { get; set; }
        public string? Picture_5 { get; set; }
        public string? Owner { get; set; }
        public string? LastChangeUser { get; set; }
        public DateTime LastChangesDate { get; set; }
        public int ToolGroup { get; set; }
    }
}

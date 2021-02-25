using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class NodeDto
    {
        public string Id { get; set; }
        public string Shape { get; set; }
        public string Label { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public string Question { get; set; }
        public string Func { get; set; }
        public string Key { get; set; }
        public string Color => GetColor();
        public string Size => GetSize();
        public bool IsEnd { get; set; } = false;

        private string GetColor()
        {
            switch (Shape)
            {
                case "flow-circle":
                    return "#FA8C16";
                case "flow-rhombus":
                    return "#13C2C2";
                case "flow-rect":
                    return "#1890FF";
                default:
                    return string.Empty;
            }
        }

        private string GetSize()
        {
            switch (Shape)
            {
                case "flow-circle":
                    return "72*72";
                case "flow-rhombus":
                case "flow-rect":
                default:
                    return string.Empty;
            }
        }
    }
}

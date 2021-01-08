using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityTable : SanityObject
    {
        public SanityTable()
        {
            SanityType = "tableItem";
        }

        public Boolean Headers { get; set; }
        public Boolean Bootstrap { get; set; }
        public Boolean Bordered { get; set; }
        public Boolean Borderless { get; set; }
        public Boolean Condesed { get; set; }
        public Boolean Hover { get; set; }
        public Boolean Responsive { get; set; }
        public Boolean Striped { get; set; }
        public TableItem Table { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public string Class { get; set; }
    }

    public class TableItem
    {
        public List<TableRow> Rows { get; set; }
    }

    public class TableRow : SanityObject
    {
        public TableRow()
        {
            SanityType = "_column";
        }
        public List<string> Cells { get; set; }
    }
}

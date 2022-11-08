using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCLICK.Entityes
{
    public class Tag
    {
        public int id { get; set; }
        public string name { get; set; }
        public string long_name { get; set; }
        public string name_prep_case { get; set; }
        public bool is_name_prep_active { get; set; }
        public bool is_active { get; set; }
        public int order { get; set; }
        public bool show_in_details { get; set; }
        public bool show_in_filter_card { get; set; }
        public bool hide_in_filter { get; set; }
        public bool is_uncompleted { get; set; }
        public bool is_forced_show { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public bool show_icon_in_filter { get; set; }
        public bool show_icon_in_commercial_filter { get; set; }
        public object name_tab { get; set; }
        public string detailed_description { get; set; }
        public bool from_1c { get; set; }
        public object link { get; set; }
        public object link_title { get; set; }
        public List<object> flat_sizes { get; set; }
        public bool display_icon { get; set; }
        public bool red_tag { get; set; }
        public bool red_tag_in_filter { get; set; }
        public bool show_description_on_filter { get; set; }
        public int type { get; set; }
        public List<int> cities { get; set; }
    }
}

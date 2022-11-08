using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCLICK.Entityes
{
    public class Result
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string page_url { get; set; }
        public string complex { get; set; }
        public string building { get; set; }
        public string completion_date { get; set; }
        public string section { get; set; }
        public string layout { get; set; }
        public int number { get; set; }
        public string short_name { get; set; }
        public string full_name { get; set; }
        public string type { get; set; }
        public string plan { get; set; }
        public double square { get; set; }
        public double living_square { get; set; }
        public double kitchen_square { get; set; }
        public int floor { get; set; }
        public int max_floor { get; set; }
        public int rooms { get; set; }
        public string owner { get; set; }
        public string agreement_type { get; set; }
        public string price_package { get; set; }
        public int price { get; set; }
        public int price_promo { get; set; }
        public object promo_date { get; set; }
        public string description { get; set; }
        public int number_on_floor { get; set; }
        public int loggias_count { get; set; }
        public int balconies_count { get; set; }
        public int separate_wcs_count { get; set; }
        public int combined_wcs_count { get; set; }
        public bool furnished { get; set; }
        public string window_view { get; set; }
        public string ref_id { get; set; }
        public bool is_active { get; set; }
        public DateTime created_date { get; set; }
        public DateTime modified_date { get; set; }
        public string status { get; set; }
        public string url_feed { get; set; }
        public bool feed_yandex2 { get; set; }
        public bool feed_adwords2 { get; set; }
        public bool popular { get; set; }
        public string code { get; set; }
        public object preview { get; set; }
        public object preview_medium { get; set; }
        public string main_flat_image { get; set; }
        public string flat_image_hash { get; set; }
        public string flat1_image_hash { get; set; }
        public object flat2_image_file { get; set; }
        public string flat2_image_hash { get; set; }
        public object flat3_image_file { get; set; }
        public string flat3_image_hash { get; set; }
        public object flat4_image_file { get; set; }
        public string flat4_image_hash { get; set; }
        public string floor_image_file { get; set; }
        public string floor_image_file_png { get; set; }
        public string flat_image_file_png { get; set; }
        public string floor_image_hash { get; set; }
        public object section_image_file { get; set; }
        public string section_image_hash { get; set; }
        public object rose_wind_file { get; set; }
        public string rose_wind_hash { get; set; }
        public List<Tag> tags { get; set; }
        public string tagsOnString
        {
            get
            {
                string _tagsOnString = "";
                foreach (var tag in tags)
                {
                    _tagsOnString += $"{tag.name} ";
                }
                return _tagsOnString;
            }
        }
        public bool favorite { get; set; }
        public object price_increase_date { get; set; }
        public string delivery_title { get; set; }
        public bool has_complex_offer { get; set; }
    }
}

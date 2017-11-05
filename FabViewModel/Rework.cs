using System;

namespace FabViewModel {
    public class Rework {
        public int id { get; set; }
        public string wo_number { get; set; }
        public int route_id { get; set; }
        public string route_ver { get; set; }
        public string from_process { get; set; }
        public string to_process { get; set; }
        public string issue_process { get; set; }
        public string receiver { get; set; }
        public string rwk_user { get; set; }
        public string reason_cate { get; set; }
        public string reason_sub_cate { get; set; }
        public string branch_type { get; set; }
        public int rwk_route_id { get; set; }
        public string rwk_route_ver { get; set; }
        public int rwk_out_route_id { get; set; }
        public string rwk_out_route_ver { get; set; }
        public string rwk_out_process { get; set; }
        public string rwk_note { get; set; }
        public DateTime creation_date { get; set; }
        public string created_by { get; set; }
        public DateTime last_updated_date { get; set; }
        public string last_updated_by { get; set; }
    }
}

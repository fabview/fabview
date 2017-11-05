using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FabView.Utility;

namespace ZGZY.Common {
    public class Log {
        public string wo_number { get; set; }
        public string operation { get; set; }
        public string old_value { get; set; }
        public string new_value { get; set; }
        public string created_by { get; set; }
        public string app_name { get; set; }

        public void LogOperation(Log log) {
            string sql = "insert into hlogOperation value ('{0}','{1}','{2}','{3}',getdate(),'{4}','{5}');";
            sql = string.Format(log.wo_number, log.operation, log.old_value, log.new_value, log.created_by, log.app_name);

            DbHelper db = new DbHelper();
            if (db.ExecuteNonQuery(sql) == -1) {

            } else {

            }
        }

        public void LogOrder(Log log) {
            string sql = "insert into hlogOrder select '{0}',getdate(),'{1}','{2}', o.* from horder o where sub_order='{3}';";
            sql = string.Format(sql, log.operation, log.created_by, log.app_name, log.wo_number);

            DbHelper db = new DbHelper();
            if (db.ExecuteNonQuery(sql) == -1) {

            } else {

            }
        }
    }
}

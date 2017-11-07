using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using System;

namespace ZGZY.WebUI.fabview.ashx.OrderManager {
    /// <summary>
    /// ScheduleHandler 的摘要说明
    /// </summary>
    public class ScheduleHandler : IHttpHandler {
        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "list": {
                        ListOrder(context);
                        break;
                    }
                case "schedule": {
                        Schedule(context);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 查询待排产的订单
        /// </summary>
        /// <param name="context"></param>
        void ListOrder(HttpContext context) {
            var orderNumber = context.Request["order_number"];
            var subOrder = context.Request["sub_order"];
            var EPDBegin = context.Request["estimate_pack_date_begin"];
            var EPDEnd = context.Request["estimate_pack_date_end"];
            string sql = @"select * from horder where 1=1 ";

            if (!string.IsNullOrEmpty(orderNumber)) {
                sql += " and order_number like '%" + orderNumber + "%'";
            }
            if (!string.IsNullOrEmpty(EPDBegin)) {
                sql += " and estimate_pack_date>='" + EPDBegin + "'";
            }
            if (!string.IsNullOrEmpty(EPDEnd)) {
                sql += " and estimate_pack_date<='" + EPDEnd + "'";
            }
            if (!string.IsNullOrEmpty(subOrder)) {
                sql += " and order_number='" + subOrder + "'";
            }

            sql += " order by sub_order";

            DbHelper db = new DbHelper();
            DataTable dt = db.ExecuteDataTable(sql);
            string strJson = JsonConvert.SerializeObject(dt);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        /// <summary>
        /// 排产操作
        /// </summary>
        /// <param name="context"></param>
        void Schedule(HttpContext context) {
            string _service = "ScheduleHandler/Schedule \r\n \r\n";
            Result rs = new Result();
            DbHelper db = new DbHelper();
            string wo_numbers = context.Request["sub_order"];
            string beginPrdDate = context.Request["begin_production_date"];

            //1.更新订单状态为“SCHEDULED”（已排产）。符合排产的订单条件：订单状态为已分配工艺路线、待排产
            string sql_sch = @"update horder set status='WAIT', schedule_date=getdate(), begin_production_date='{0}', 
                                    next_process = t.process_name 
                                    from (select process_name,wo_number from hprdroute where wo_number in ({1}) and sequence=1) t
                                where sub_order in ({1}) and horder.sub_order = t.wo_number;";
            sql_sch = string.Format(sql_sch, beginPrdDate, wo_numbers);
            db.ExecuteNonQuery(sql_sch);

            //2.生成各工序的操作任务（制造指令）

            //2.1 生成条码采集任务（TASK_TYPE = SCAN）
            //xujiyuan：条码按需采集，不需要事先设定
            //string sql_task = @"insert into htasks
            //                               (wo_number
            //                               ,parts_no
            //                               ,process_name
            //                               ,process_step
            //                               ,task_type
            //                               ,task
            //                               ,perform_type
            //                               ,creation_date
            //                               ,created_by)
            //                         select
            //                               {0},
            //                               code_value,
            //                               code2,
            //                               '',
            //                               'BARCODE',
            //                               code_value,
            //                               'MANUAL',
            //                               getdate(),
            //                               'ScheduleHandler/Schedule'
            //                            from hcode_line where code_cate='BARCODE'
            //                                and code2 in (select process_name from hprdroute where wo_number = {0});";

            //2.2 生成工序检查项目任务（TASK_TYPE = CHECK）
            string sql_check = @"insert into htasks
                                           (wo_number
                                           ,parts_no
                                           ,process_name
                                           ,process_step
                                           ,task_type
                                           ,task
                                           ,perform_type
                                           ,creation_date
                                           ,created_by)
                                     select
                                           {0},
                                           '-1',
                                           process,
                                           '',
                                           'CHECK',
                                           check_item,
                                           'MANUAL',
                                           getdate(),
                                           'ScheduleHandler/Schedule'
                                        from hchecklist where isnull(enabled,'Y') <> 'N' 
                                            and process in (select process_name from hprdroute where wo_number = {0});";
            foreach (string wo_number in wo_numbers.Split(',')) {
                //string sql_task_t = string.Format(sql_task, wo_number);
                string sql_check_t = string.Format(sql_check, wo_number);


                //if (db.ExecuteNonQuery(sql_task_t, rs) == -1) {
                //    rs.status = 0;
                //    rs.msg = _service + rs.msg + sql_task_t;
                //    rs.item = "{\"wo_number\":[{\"wo_number\":\"" + wo_number.Trim('\'') + "\"}]}";

                //    goto json;
                //} else {
                //    rs.status = 1;
                //    rs.msg = "排产成功。";
                //    rs.item = "[]";
                //}

                if (db.ExecuteNonQuery(sql_check_t, rs) == -1) {
                    rs.status = 0;
                    rs.msg = _service + rs.msg + sql_check_t;
                    rs.item = "{\"wo_number\":[{\"wo_number\":\"" + wo_number.Trim('\'') + "\"}]}";

                    goto json;
                } else {
                    rs.status = 1;
                    rs.msg = "排产成功。";
                    rs.item = "[]";
                }
            }

            json:
            {
                string strJson = JsonConvert.SerializeObject(rs);
                context.Response.Clear();
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentType = "application/json";
                context.Response.Write(strJson);
                context.Response.Flush();
                context.Response.End();
            }
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}
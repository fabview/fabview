using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using FabViewModel;
using System.IO;
using ZGZY.Common;

namespace ZGZY.WebUI.ashx {
    /// <summary>
    /// ProcessHandler 的摘要说明
    /// </summary>
    public class ProcessHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "track_out": {
                        TrackOut(context);
                        break;
                    }
                case "track_in": {
                        TrackIn(context);
                        break;
                    }
                case "process_adjust": {
                        ProcessAdjustion(context);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 产品进站
        /// 更新表HPRDROUTE的进站操作人、日期。如果进站日期不为空，则不重复赋值。
        /// </summary>
        /// <param name="context"></param>
        void TrackIn(HttpContext context) {
            string _service = "ProcessHandler/TrackIn \r\n \r\n";
            Result rs = new Result();
            DbHelper db = new DbHelper();

            string strSubOrder = HttpContext.Current.Request["sub_order"];
            string strUserName = HttpContext.Current.Request["user_name"];
            string strProcessName = HttpContext.Current.Request["process_name"];

            //查看订单状态，如果是“IN-PROCESS”，表示已经进trackin，直接返回
            DataTable dt = db.ExecuteDataTable("select status from horder where sub_order='" + strSubOrder + "'");
            if (dt.Rows[0]["status"].ToString() == "IN-PROCESS")
                return;

            string sql = @"UPDATE HPRDROUTE 
                            SET    TRACKIN_DATE= 
		                            CASE 
				                            WHEN TRACKIN_DATE IS NULL THEN GETDATE() 
				                            ELSE TRACKIN_DATE 
		                            END, 
		                            TRACKIN_BY = 
		                            CASE 
				                            WHEN TRACKIN_BY IS NULL THEN '{0}' 
				                            ELSE TRACKIN_BY 
		                            END, 
		                            LAST_UPDATED_DATE=GETDATE(),
		                            LAST_UPDATED_BY='{0}' 
                            WHERE  WO_NUMBER = '{1}' AND PROCESS_NAME = '{2}';";

            string sql_update_order = "update horder set cur_process='{0}',next_process=null,status='{1}' where sub_order='{2}'";
            sql_update_order = string.Format(sql_update_order, strProcessName, "IN-PROCESS", strSubOrder);
            db.ExecuteNonQuery(sql_update_order);

            sql = string.Format(sql, strUserName, strSubOrder, strProcessName);
            if (db.ExecuteNonQuery(sql) == -1) {
                rs.status = 0;
                rs.msg = _service + rs.msg + sql;
                rs.item = "{\"wo_number\":[{\"wo_number\":\"" + strSubOrder + "\"}]}";
            } else {
                rs.status = 1;
                rs.msg = "";
            }

            string strJson = JsonConvert.SerializeObject(rs);
            strJson = JsonConvert.SerializeObject(rs);
            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        /// <summary>
        /// 产品出站
        /// </summary>
        /// <param name="context"></param>
        void TrackOut(HttpContext context) {
            /*****************************************
             * 1. 检查前工序是否有指令没有执行。（如果有任务没有执行，则不允许出站）
             * 2. 更新HPRDROUTE表（工艺路线表）的完工日期、操作员
             * 3. 更新HTASKS表（工序任务表）的执行日期
             * 4. 更新HORDER表（订单表）的当前工序、下工序、订单状态
             * 5. 写入HTASKS表：生产备注
             * 
             *****************************************/
            string _service = "ProcessHandler/TrackOut \r\n \r\n";
            Result rs = new Result();
            DbHelper db = new DbHelper();
            DataTable dt_tasks = null;

            string strNextProcess, strOrderStatus;
            string strSubOrder = HttpContext.Current.Request["sub_order"];
            string strUserName = HttpContext.Current.Request["user_name"];
            string strProcessName = HttpContext.Current.Request["process_name"];
            string strCheckResult = HttpContext.Current.Request["check_result"];
            string strPrdNote = HttpContext.Current.Request["production_note"].Trim();

            /****************1. 检查前工序是否有指令没有执行。（如果有任务没有执行，则不允许出站）*****************/
            string sql_check_tasks = @"select count(1) from htasks where performed_by is null and process_name in 
                                            (select process_name from hprdroute where sequence<(select sequence from hprdroute 
                                                where process_name='{0}' and wo_number='{1}'))
                                        and wo_number='{1}'";
            sql_check_tasks = string.Format(sql_check_tasks, strProcessName, strSubOrder);
            int task_count = db.ExecuteScalar(sql_check_tasks);
            if (task_count > 0) {
                string sql_query_tasks = @"select process_name from htasks where performed_by is null and process_name in 
                                            (select process_name from hprdroute where sequence<(select sequence from hprdroute 
                                                where process_name='{0}' and wo_number='{1}'))
                                        and wo_number='{1}'";
                sql_query_tasks = string.Format(sql_query_tasks, strProcessName, strSubOrder);
                dt_tasks = db.ExecuteDataTable(sql_query_tasks);
                foreach (DataRow dr in dt_tasks.Rows) {
                    rs.msg += dr["process_name"] + "，";
                }

                rs.status = 0;
                rs.msg += "有未完成的任务。";
                rs.msg = _service + rs.msg + sql_check_tasks;
                rs.item = "{\"wo_number\":[{\"wo_number\":\"" + strSubOrder + "\"}]}";

                goto json;
            }

            /****************2. 更新HPRDROUTE表（工艺路线表）的完工日期、操作员*****************/
            string sql = @"UPDATE HPRDROUTE 
                            SET    TRACKOUT_DATE= 
		                            CASE 
				                            WHEN TRACKOUT_DATE IS NULL THEN GETDATE() 
				                            ELSE TRACKOUT_DATE 
		                            END, 
		                            TRACKOUT_BY = 
		                            CASE 
				                            WHEN TRACKOUT_BY IS NULL THEN '{0}' 
				                            ELSE TRACKOUT_BY 
		                            END, 
		                            LAST_UPDATED_DATE=GETDATE(),
		                            LAST_UPDATED_BY='{0}' 
                            WHERE  WO_NUMBER = '{1}' AND PROCESS_NAME = '{2}';";
            sql = string.Format(sql, strUserName, strSubOrder, strProcessName, strSubOrder);

            if (db.ExecuteNonQuery(sql) == -1) {
                rs.status = 0;
                rs.msg = _service + rs.msg + sql;
                rs.item = "{\"wo_number\":[{\"wo_number\":\"" + strSubOrder + "\"}]}";

                goto json;
            } else {
                rs.status = 1;
                rs.msg = "";
            }

            /****************3. 更新HTASKS表（工序任务表）的执行日期*****************/
            if (task_count > 0) {
                string sql_tasks = "update htasks set performed_by='" + strUserName + "',performed_date=getdate(), result='{0}', performed_note='{1}'" +
                                        " where process_name='" + strProcessName + "' and wo_number='" + strSubOrder + "' and task='{2}' and task_type='CHECK';";

                string _sql = string.Empty;
                //循环处理点检项目，strCheckResult格式：1:温度:Pass:备注-结果OK;2:压力:Pass:备注-压力不稳定;......
                foreach (string result in strCheckResult.Split(';')) {
                    string[] r = result.Split(':');//result格式：1:温度:Pass:结果OK;
                    _sql += string.Format(sql_tasks, r[2], r[3], r[1]) + "\r\n";
                }

                if (db.ExecuteNonQuery(_sql) == -1) {
                    rs.status = 0;
                    rs.msg = _service + rs.msg + _sql;
                    rs.item = "{\"wo_number\":[{\"wo_number\":\"" + strSubOrder + "\"}]}";

                    goto json;
                } else {
                    rs.status = 1;
                    rs.msg = "";
                }
            }
            /****************4. 更新HORDER表（订单表）的当前工序、下工序、订单状态*****************/
            string sql_next_process = @"select process_name from hprdroute 
                                            where sequence=(select top 1 t2.sequence from hprdroute t1,hprdroute t2 where t1.process_name='{0}' and t1.wo_number='{1}'
and t1.wo_number=t2.wo_number and t2.sequence>t1.sequence order by t2.sequence asc)
                                            and wo_number='{1}'";
            sql_next_process = string.Format(sql_next_process, strProcessName, strSubOrder);
            DataTable dt = db.ExecuteDataTable(sql_next_process);
            if (dt.Rows.Count == 0) { //表示当前工序已经是最后一个工序
                strNextProcess = "";
                strOrderStatus = "FINISHED";
            } else {
                strNextProcess = dt.Rows[0]["process_name"].ToString();
                strOrderStatus = "WAIT";
            }

            //出站时，更新HORDER表的订单状态、下工序名称
            string sql_order = @"update horder set status='{0}', next_process='{1}' where sub_order='{2}';";
            sql_order = string.Format(sql_order, strOrderStatus, strNextProcess, strSubOrder);
            if (db.ExecuteNonQuery(sql_order) == -1) {
                rs.status = 0;
                rs.msg = _service + rs.msg + sql_next_process;
                rs.item = "{\"wo_number\":[{\"wo_number\":\"" + strSubOrder + "\"}]}";

                goto json;
            } else {
                rs.status = 1;
                rs.msg = "";
            }

            //HTASKS表：写入生产备注信息
            //if(strPrdNote != "") {
            //    string sql_insert_tasks = @"insert into htasks(wo_number,parts_no,process_name,process_step,task_type,task,perform_type) 
            //                                    select "
            //}

            string sql_insert_tasks = @"insert into htasks
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
                                           '{0}',
                                           '-1',
                                           '{1}',
                                           '',
                                           'NOTE',
                                           '{2}',
                                           'MANUAL',
                                           getdate(),
                                           'ProcessHandler/TrackOut'
                                        from htasks where wo_number='{0}' and process_name in (
                                            select process_name from hprdroute where wo_number='{0}' and sequence>=(
                                              select sequence from  hprdroute where wo_number='{0}' and process_name='{2}')
                                            );";

            sql_insert_tasks = string.Format(sql_insert_tasks, strSubOrder, strNextProcess, strPrdNote, strProcessName);
            db.ExecuteNonQuery(sql_insert_tasks);

            json:
            {
                string strJson = JsonConvert.SerializeObject(rs);
                strJson = JsonConvert.SerializeObject(rs);
                context.Response.Clear();
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentType = "application/json";
                context.Response.Write(strJson);
                context.Response.Flush();
                context.Response.End();
            }
        }

        /// <summary>
        /// 工序调整（返工）
        /// </summary>
        /// <param name="context"></param>
        void ProcessAdjustion(HttpContext context) {
            string _service = "ProcessHandler/OperationAdjustion";
            Result rs = new Result();
            ResultHelper rh = new ResultHelper();
            DbHelper db = new DbHelper();
            Log log = new Log();

            string strUserName = HttpContext.Current.Request["user_name"];
            string strSubOrders = HttpContext.Current.Request["sub_orders"];
            string strReworkNote = HttpContext.Current.Request["rwk_note"];

            string strReceiver = HttpContext.Current.Request["receiver"];
            string strReworkUser = HttpContext.Current.Request["rwk_user"];
            string strIssProcess = HttpContext.Current.Request["issue_process"];
            string strToProcess = HttpContext.Current.Request["to_process"];
            string strReasonCate = HttpContext.Current.Request["reason_cate"];

            var json = HttpContext.Current.Request["json"];
            var jsetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            Rework rework = (Rework)JsonConvert.DeserializeObject<Rework>(json, jsetting);//反序列化  

            string sql_insert_rework = @"insert into hrework
                                   (wo_number,
                                   from_process,
                                   to_process,
                                   issue_process,
                                   receiver,
                                   rwk_user,
                                   reason_cate,
                                   reason_sub_cate,
                                   branch_type,
                                   rwk_note,
                                   creation_date,
                                   created_by)
                             values(
			                        '#wo_number',
			                        (select cur_process from horder where sub_order='#wo_number'),
			                        '{0}',
			                        '{1}',
			                        '{2}',
			                        '{3}',
			                        '{4}',
			                        '{5}',
			                        '{6}',
			                        '{7}',
			                        getdate(),
			                        '{8}');";
            sql_insert_rework = string.Format(sql_insert_rework,
                                rework.to_process,
                                rework.issue_process,
                                rework.receiver,
                                rework.rwk_user,
                                rework.reason_cate,
                                rework.reason_sub_cate,
                                rework.branch_type,
                                rework.rwk_note,
                                strUserName);
            sql_insert_rework = sql_insert_rework.Replace("#wo_number", "{0}");

            //string sql_update_order = @"updade horder 
            //                                set next_process=(select cur_process from horder where sub_order='{0}') 
            //                                where sub_order='{0}';";
            string sql_update_order = @"update horder set cur_process='',next_process=(select process_name from hprocess where process_id={0}),
                                            status='WAIT' where sub_order='{1}';";
            string sql_insert_prdroute = @"insert into hprdroute
                                              (wo_number,
                                               process_id,
                                               process_name,
                                               sequence,
                                               creation_date,
                                               created_by)
                                              select '{0}',
                                                     process_id,
                                                     process_name,
                                                     (select sequence + 0.1
                                                        from dbo.hprdroute
                                                       where wo_number = '{0}'
                                                         and process_name = (select case cur_process when null then next_process when '' then next_process else cur_process end
                                                                               from horder
                                                                              where sub_order = '{0}')),
                                                     Getdate(),
                                                     'ProcessHandler/ProcessAdjustion'
                                                from hprocess
                                               where process_id = '{1}';";
            //而然要求按需作业（条码采集、检验等），暂时注释代码
            //string sql_insert_task = @"insert into htasks
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
            //                               '-1',
            //                               '{1}',
            //                               '',
            //                               'NOTE',
            //                               '{2}',
            //                               'MANUAL',
            //                               getdate(),
            //                               'ProcessHandler/OperationAdjustion'
            //                            from htasks where wo_number='{0}' and process_name in (
            //                                select process_name from hprdroute where wo_number='{0}' and sequence>=(
            //                                  select sequence from  hprdroute where wo_number='{0}' and process_name='{1}')
            //                                );";


            StringReader sr = new StringReader(strSubOrders);

            string strSubOrder = string.Empty;
            string sql_insert_rework_t = string.Empty;
            string sql_update_order_t = string.Empty;
            string sql_insert_task_t = string.Empty;
            string sql_insert_prdroute_t = string.Empty;

            while ((strSubOrder = sr.ReadLine()) != null) {
                //查看目标工序是否在订单的工艺路线中
                DataTable dt = db.ExecuteDataTable("select 1 from hprdroute where wo_number='" + strSubOrder + "' and process_id=" + rework.to_process + ";");
                if (dt.Rows.Count == 0) {
                    sql_insert_prdroute_t += string.Format(sql_insert_prdroute, strSubOrder, rework.to_process);
                    db.ExecuteNonQuery(sql_insert_prdroute_t);
                }

                sql_insert_rework_t += string.Format(sql_insert_rework, strSubOrder) + "\r\n";
                sql_update_order_t += string.Format(sql_update_order, rework.to_process, strSubOrder) + "\r\n";
                //sql_insert_task_t += string.Format(sql_insert_task, strSubOrder, rework.to_process, strReworkNote);
            }

            //if (db.ExecuteNonQuery(sql_insert_prdroute_t) == -1) {
            //    rs.status = 0;
            //    rs.msg = rh.GenMsg(_service, rs.msg, sql_insert_rework_t);
            //    rs.item = rh.GenItem(strSubOrder);

            //    //rh.Response(context, rs);
            //} else 
            if (db.ExecuteNonQuery(sql_update_order_t) == -1) {
                rs.status = 0;
                rs.msg = rh.GenMsg(_service, rs.msg, sql_update_order_t);
                rs.item = rh.GenItem(strSubOrder);

                //rh.Response(context, rs);
            } else if (db.ExecuteNonQuery(sql_insert_rework_t) == -1) {
                rs.status = 0;
                rs.msg = rh.GenMsg(_service, rs.msg, sql_insert_task_t);
                rs.item = rh.GenItem(strSubOrder);

                //rh.Response(context, rs);
            } else {
                rs.status = 1;
                rs.msg = "";

                rh.Response(context, rs);
            }


            string strJson = JsonConvert.SerializeObject(rs);
            strJson = JsonConvert.SerializeObject(rs);
            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}
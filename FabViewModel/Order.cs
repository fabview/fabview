using System;

namespace FabViewModel
{
    public class Order
    {
        public float ORDERID { get; set; }
        public string ORDER_NUMBER { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUB_ORDER { get; set; }
        public string SUB_ORDER_DESC { get; set; }
        public int QTY { get; set; }
        public string PARTS_NO { get; set; }
        public string STATUS { get; set; }
        public string SALES_ORDER { get; set; }
        public string CONTRACT { get; set; }
        public string DISTRIBUTOR { get; set; }
        public string DT_CONTACT { get; set; }
        public string END_CUSTOMER { get; set; }
        public string END_CUST_SEX { get; set; }
        public int END_CUST_AGE { get; set; }
        public string SALES { get; set; }
        public int PROCESS_DAYS { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public DateTime ESTIMATE_PACK_DATE { get; set; }
        public DateTime ACT_PACK_DATE { get; set; }
        public DateTime ACT_SHIP_DATE { get; set; }
        public string NOTE { get; set; }
        public string PICTURE { get; set; }
        public string PROD_CATE { get; set; }
        public string PROD_NAME { get; set; }
        public string PROCESS_TYPE { get; set; }
        public string PROD_COLOR { get; set; }
        public string PROD_POSITION { get; set; }
        public string ATTACHMENT { get; set; }
    }
}

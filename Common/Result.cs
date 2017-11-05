/********************************************
 * Description: 
 * Created by xujiyuan 2017
********************************************/
namespace FabView.Utility
{
    public class Result
    {
        private int _status;
        private string _msg;
        private object _item;

        /// <summary>
        /// 
        /// </summary>
        public int status
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string msg
        {
            set { _msg = value; }
            get { return _msg; }
        }
        /// <summary>
        /// 
        /// </summary>
        public object item
        {
            set { _item = value; }
            get { return _item; }
        }
    }
}

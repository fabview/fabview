using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using log4net;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ZGZY.SQLServerDALTest
{
    /// <summary>
    /// LoginInfo测试类
    /// </summary>
    [TestFixture]
    public class LoginLogTest
    {
        //关于nunit：
        //1.测试方法必须是public类型的，无参无返回值；
        //2.测试的类上需要加标签：[NUnit.Framework.TestFixture]
        //3.类里的测试方法需要加标签：[NUnit.Framework.Test]
        //4.关于nunit工具的使用请看项目libs目录下的记事本文件：Nuit 快速入门.txt

        //关于log4net：
        //1.使用的项目里必须初始化，可以在当前类库的AssemblyInfo.cs类里初始化、类名上初始化、SetUp方法里初始化；
        //2.配置文件得配置好；

        //log4net快速入门教程：
        //www.cnblogs.com/dragon/archive/2005/03/24/124254.html
        //blog.csdn.net/zhoufoxcn/article/details/2220533
        //blog.csdn.net/zhoufoxcn/article/details/6029021

        //log4net另一种初始化方法：在Nunit中初始测试的方法中
        //[SetUp]
        //public void SetUp()
        //{
        //    log4net.Config.BasicConfigurator.Configure();    //初始化Log4net
        //}
        //如果是网站应用程序，同时有Global.asax文件，那么在里面的Application_Start文件里写上log4net.Config.BasicConfigurator.Configure(Watch = true); 即可完成初始化，以后不用每次使用log4net都初始化

        [Test] //测试方法必须无参无返回值
        public void GetTotalCountIsZeroTest()
        {
            ZGZY.IDAL.ILoginLog loginInfoDal = ZGZY.DALFactory.Factory.GetLoginInfoDAL();  //数据库访问对象
            int count = loginInfoDal.GetTotalCount("1=1");

            //得到日志器
            ILog log = log4net.LogManager.GetLogger(typeof(LoginLogTest));
            //记录日志
            log.InfoFormat("获取的个数是：{0}", count);   //Info级别

            Assert.AreEqual(count, 0);   //断言结果等于0
        }

        [Test]
        public void GetTotalCountGreatThanZeroTest()
        {
            ZGZY.IDAL.ILoginLog loginInfoDal = ZGZY.DALFactory.Factory.GetLoginInfoDAL();
            int count = loginInfoDal.GetTotalCount("1=1");

            Assert.Greater(count, 0);   //断言结果大于0
        }

        [Test]
        public void GetPagerTest()
        {
            ZGZY.BLL.LoginLog bll = new BLL.LoginLog();
            string count = bll.GetPager("1=1", "LoginDate", "desc", 1, 20);

            Assert.AreEqual(count, "{\"total\": 0,\"rows\":[]}");    //没有记录
        }

        [Test]
        public void CheckLoginTest()
        {
            ZGZY.BLL.LoginLog bll = new BLL.LoginLog();
            DateTime? lastLoginTime;

            Assert.AreEqual(bll.CheckLogin("192.168.1.1", out lastLoginTime), null); //断言“192.168.1.1”这个ip半个小时内没有5次错误登陆记录
        }

        [Test]
        public void WriteLoginLogTest()
        {
            ZGZY.BLL.LoginLog bll = new BLL.LoginLog();
            ZGZY.Model.LoginLog loginInfo = new Model.LoginLog();
            loginInfo.UserName = "wangjie";
            loginInfo.UserIp = "117.78.0.138";
            loginInfo.City = "北京";
            loginInfo.Success = false;
            bool result = bll.WriteLoginLog(loginInfo);

            Assert.AreEqual(result, true);   //断言登录成功
        }

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Redirect
{
    public class RedirectConfigItem
    {
        public static string DefaultExpiredInfo = "该域名已经停止服务，请联系管理员获知新的域名";

        #region 配置文件读来的字段
        public string OldDomain { get; private set; }
        public string NewDomain { get; private set; }

        //该天的最后一秒过期
        //到期之后不提供跳转，直接显示ExpiredInfo的内容
        public DateTime ExpiredDate { get; private set; }

        //默认
        public string ExpiredInfo { get; private set; }

        //到期前的提示文案，如果提示文案为空，直接跳转
        //可以使用日期变量[ExpiredDate]，如“域名将在[ExpiredDate]停止提供跳转，请记录新域名”
        public string WarningInfo { get; private set; }

        //跳转的时候是否保持url中域名之后的部分
        //默认为true
        public bool IsKeepPath { get; private set; }
        #endregion

        #region 快捷字段
        public bool IsExpired
        {
            get { return DateTime.Now > ExpiredDate; }
        }

        public bool IsNeedWarning
        {
            get { return !string.IsNullOrEmpty(WarningInfo); }
        }
        #endregion

        public RedirectConfigItem(RedirectConfigElement conf)
        {
            OldDomain = conf.OldDomain;
            NewDomain = conf.NewDomain;
            IsKeepPath = conf.IsKeepPath == 1;
            ExpiredDate = DateTime.Parse(conf.ExpiredDate).AddDays(1).AddMilliseconds(-1);
            if (string.IsNullOrEmpty(conf.ExpiredInfo))
                ExpiredInfo = DefaultExpiredInfo;
            else
                ExpiredInfo = conf.ExpiredInfo;

            if (!string.IsNullOrEmpty(conf.WarningInfo))
                WarningInfo = conf.WarningInfo.Replace("[ExpiredDate]", conf.ExpiredDate);
        }
    }

    #region 定义在配置文件中的格式
    class RedirectConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public RedirectConfigElementCollection ConfigCollection
        {
            get { return (RedirectConfigElementCollection)this[""]; }
        }

        [ConfigurationProperty("DefaultExpiredInfo", IsRequired = true)]
        public string DefaultExpiredInfo
        {
            get { return (string)this["DefaultExpiredInfo"]; }
            set { this["DefaultExpiredInfo"] = value; }
        }
    }

    [ConfigurationCollection(typeof(RedirectConfigElement), AddItemName = "redirection")]
    class RedirectConfigElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RedirectConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RedirectConfigElement)element).OldDomain;
        }
    }

    public class RedirectConfigElement : ConfigurationElement
    {
        //历史域名，访问到的域名
        //必要,key
        [ConfigurationProperty("OldDomain", IsRequired = true, IsKey = true)]
        public string OldDomain
        {
            get { return (string)this["OldDomain"]; }
            set { this["OldDomain"] = value; }
        }

        //新域名
        //必要
        [ConfigurationProperty("NewDomain", IsRequired = true, IsKey = false)]
        public string NewDomain
        {
            get { return (string)this["NewDomain"]; }
            set { this["NewDomain"] = value; }
        }

        //跳转的时候是否保持url中域名之后的部分，为1是表示跳转带上path的部分
        //必要
        [ConfigurationProperty("IsKeepPath", DefaultValue = 1, IsRequired = true)]
        [IntegerValidator(MaxValue = 1, MinValue = 0, ExcludeRange = false)]
        public int IsKeepPath
        {
            get { return (int)this["IsKeepPath"]; }
            set { this["IsKeepPath"] = value; }
        }

        //过期日期，具体时间到该天的最后一秒
        //必要
        [ConfigurationProperty("ExpiredDate", IsRequired = true)]
        public string ExpiredDate
        {
            get { return (string)this["ExpiredDate"]; }
            set { this["ExpiredDate"] = value; }
        }

        //过期后显示的文案，有默认文案
        //非必要
        [ConfigurationProperty("ExpiredInfo", IsRequired = false)]
        public string ExpiredInfo
        {
            get { return (string)this["ExpiredInfo"]; }
            set { this["ExpiredInfo"] = value; }
        }

        //到期前的提示文案，如果提示文案为空，直接跳转
        //可以使用日期变量[ExpiredDate]，如“域名将在[ExpiredDate]停止提供跳转，请记录新域名”
        //非必要
        [ConfigurationProperty("WarningInfo", IsRequired = false)]
        public string WarningInfo
        {
            get { return (string)this["WarningInfo"]; }
            set { this["WarningInfo"] = value; }
        }
    }
    #endregion
}
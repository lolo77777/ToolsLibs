using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToolsHelper
{
    /// <summary>
    /// 快速配置读取设置信息
    /// </summary>
    public class ConfigHelper : IConfigHelper
    {
        private string _basePath;
        private string _defaultFileName = "appsettings.json";

        /// <summary>
        /// IConfiguration
        /// </summary>
        public IConfiguration ConfigBuilder { get; set; }

        public ConfigHelper()
        {
            _basePath = Directory.GetCurrentDirectory();
        }

        private bool SaveJson(Dictionary<string, object> sectionInfo, string configFilePath, string configFileName = "appsettings.json")
        {
            if (sectionInfo.Count == 0)
                return false;

            try
            {
                var filePath = Path.Combine(configFilePath, configFileName);
                JObject jsonObject;

                if (File.Exists(filePath))
                {
                    using (StreamReader file = new StreamReader(filePath))
                    {
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            jsonObject = (JObject)JToken.ReadFrom(reader);
                        }
                    }
                }
                else
                {
                    jsonObject = new JObject();
                }

                foreach (var key in sectionInfo.Keys)
                {
                    jsonObject[key] = JObject.FromObject(sectionInfo[key]);
                }

                using (var writer = new StreamWriter(filePath))
                using (JsonTextWriter jsonwriter = new JsonTextWriter(writer)
                {
                    Formatting = Formatting.Indented,//格式化缩进
                    Indentation = 4,  //缩进四个字符
                    IndentChar = ' '  //缩进的字符是空格
                })
                {
                    jsonObject.WriteTo(jsonwriter);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 读取配置信息
        /// </summary>
        /// <typeparam name="T">配置对应的实体类型</typeparam>
        /// <param name="obj">配置对应的实体类实例</param>
        /// <returns>是否成功</returns>
        public bool GetSection<T>(ref T obj)
        {
            var typename = obj.GetType().Name;
            if (ConfigBuilder.GetSection(typename).Exists())
            {
                obj = Activator.CreateInstance<T>();
                ConfigBuilder.GetSection(typename).Bind(obj);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 加载配置信息，请注意：文件不能为空！！！至少需要{}；加载成功请使用ConfigBuilder进行读取
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>是否加载成功</returns>
        public bool LoadConfig(string filename = "appsettings.json")
        {
            _defaultFileName = filename;
            var full_path = Path.Combine(_basePath, _defaultFileName);
            if (File.Exists(full_path))
            {
                try
                {
                    var builder = new ConfigurationBuilder()
                   .SetBasePath(_basePath)
                   .AddJsonFile(_defaultFileName);

                    ConfigBuilder = builder.Build();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool SaveConfig(Dictionary<string, object> sectionsInfo)
        {
            return SaveJson(sectionsInfo, _basePath);
        }

        /// <summary>
        /// 保存配置信息，只会更新参数对应的部分
        /// </summary>
        /// <param name="objs">参数实体类实例</param>
        /// <returns>是否成功</returns>
        public bool SaveConfig(params object[] objs)
        {
            var sectionsInfo = new Dictionary<string, object>();
            objs.ToList().ForEach(obj => sectionsInfo[obj.GetType().Name] = obj);
            return SaveJson(sectionsInfo, _basePath);
        }
    }
}
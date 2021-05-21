using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;

namespace ToolsHelper
{
    public interface IConfigHelper
    {
        IConfiguration ConfigBuilder { get; set; }

        bool GetSection<T>(ref T obj1);

        bool LoadConfig(string filename = "appsettings.json");

        bool SaveConfig(Dictionary<string, object> sectionsInfo);

        bool SaveConfig(params object[] objs);
    }
}
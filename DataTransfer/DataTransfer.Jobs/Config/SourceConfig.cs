using System.Collections.Generic;

namespace DataTransfer.Jobs.Config
{
    public class SourceConfig
    {
        public AppConfig appConfig { get; set; }
        public List<TableConfig> TableConfigList { get; set; }
        /// <summary>
        /// 文件夹名
        /// </summary>
        public string FolderName { get; set; }

    }
}

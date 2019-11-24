namespace Plugin
{
    using IniFormatter;
    class Main:IMain
    {
        public string Name() => "INI格式化工具";
        public string Version() => "1.000α";
        public string Summary() => "INI格式化工具可以对INI文件进行格式化, 通常是将Ra2规则文件注册信息的序号重拍..";
        public string Inventors() => "舰队的偶像-岛风酱,小星星";
        public string Copyright() => "Copyright © 2019 舰队的偶像-岛风酱 , All Rights Reserved.";
        public string[] Librarys() => new string[]{
            "System.Json, Version=2.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
            "PluginCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
        };
        public void Start()
        {
            var w = new MainWindow();
            w.Show();
        }
        public string Info()// 这个是旧版Manager兼容
        {
            return "星星专属 INI格式化工具";
        }
    }
}

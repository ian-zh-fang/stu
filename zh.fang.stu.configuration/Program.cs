namespace zh.fang.stu.configuration
{
    using System;
    using System.Configuration;    

    class Program
    {
        static void Main(string[] args)
        {
            // 读取非内置文件
            var file = new ExeConfigurationFileMap();
            file.ExeConfigFilename = $"{AppDomain.CurrentDomain.BaseDirectory}vs.config";
            var cfg = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            var cfgsec = cfg.GetSection("securityProvider");

            var config = ConfigurationManager.GetSection("securityProvider");

            var section = (MyConfigSection)ConfigurationManager.GetSection("testProvider/testItems");
            Console.WriteLine($"{section.Name} \t {section.Type}");
            for (int i = 0; i < section.Items.Count; i++)
            {
                var item = section.Items[i];
                Console.WriteLine($"{item.Name} \t {item.Type}");
            }

            // error
            var group = (MyConfigSectionGroup)ConfigurationManager.GetSection("testProvider");

            // ok
            group = (MyConfigSectionGroup)ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSectionGroup("testProvider");
            Console.WriteLine($"{group.Name} \t {group.Type}");

            Console.WriteLine();
            Console.WriteLine("************ static .actor test ***************");
            new StaticClass();
            new StaticClass();

            Console.ReadKey();

        }
    }

    class StaticClass
    {
        static StaticClass()
        {
            Console.WriteLine($"static .actor");
        }

        public StaticClass()
        {
            Console.WriteLine($".actor");
        }
    }
}

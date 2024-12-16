using Pika.Base.Scripting;
using Pika.Base.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Pika.Base.Scripting
{
    public class ScriptManager
    {
        private static ScriptManager scriptManager;

        private IHandlerLoader handlerLoader;


        // 脚本源文件夹
        private List<string> sourceDirs = new List<string>();

        // 输出文件夹
        private string outDir;
        // 附加的jar包地址
        private string jarsDir = "";
        /**
         * 脚本{"接口名"：{"类名":"类对象"}}
         */
        ConcurrentDictionary<string, ConcurrentDictionary<string, IScript>> tmpScriptInstances = new ConcurrentDictionary<string, ConcurrentDictionary<string, IScript>>();
        ConcurrentDictionary<string, ConcurrentDictionary<string, IScript>> scriptInstances = new ConcurrentDictionary<string, ConcurrentDictionary<string, IScript>>();
        /**
         * 脚本{"类名":"类对象"}
         */
        ConcurrentDictionary<string, IScript> tmpNameScriptMap = new ConcurrentDictionary<string, IScript>();
        ConcurrentDictionary<string, IScript> nameScriptMap = new ConcurrentDictionary<string, IScript>();

        private ScriptManager()
        {
            //String dir = System.getProperty("user.dir");
            //String path = dir + "-scripts" + File.separator + "src" + File.separator + "main" + File.separator + "java"
            //        + File.separator;
            //String outpath = dir + File.separator + "target" + File.separator + "scriptsbin" + File.separator;
            //String jarsDir = dir + File.separator + "target" + File.separator;
            //setSource(path, outpath, jarsDir);
            //LOGGER.info("项目路径：{} 脚本路径：{} 输出路径：{} jar路径：{}", dir, path, outpath, jarsDir);
        }

        public static ScriptManager getInstance()
        {
            if (scriptManager == null)
            {
                if (scriptManager == null)
                {
                    scriptManager = new ScriptManager();
                }
            }
            return scriptManager;
        }

        public void ConsumerScript<T>(string scriptName, Action<T> action) where T : IScript
        {

            if (nameScriptMap.TryGetValue(scriptName, out IScript temp) && action != null)
            {
                try
                {
                    action((T)temp);
                }
                catch (Exception e)
                {
                    PikaLogger.Error(e, "");
                }
            }
            else
            {
                PikaLogger.Error("");
            }
        }

        public bool PredicateScript<T>(string scriptName, Func<T, bool> condition) where T : IScript
        {

            if (nameScriptMap.TryGetValue(scriptName, out IScript temp) && condition != null)
            {
                try
                {
                    return condition((T)temp);
                }
                catch (Exception e)
                {
                    PikaLogger.Error(e, "");
                }
            }
            else
            {
                PikaLogger.Error("");
            }
            return false;
        }
    }
}

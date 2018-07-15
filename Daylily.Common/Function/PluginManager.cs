﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Daylily.Common.Assist;
using Daylily.Common.Function.Application;
using Daylily.Common.Function.Application.Command;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Function
{
    public static class PluginManager
    {
        public static ConcurrentDictionary<string, CommandApp> CommandMap { get; } =
            new ConcurrentDictionary<string, CommandApp>();
        public static ConcurrentDictionary<string, Type> CommandMapTest { get; } =
            new ConcurrentDictionary<string, Type>();

        public static List<ServiceApp> ServiceList { get; } = new List<ServiceApp>();
        public static List<ApplicationApp> ApplicationList { get; } = new List<ApplicationApp>();

        public static ConcurrentDictionary<string, Assembly> AssemblyList { get; } =
            new ConcurrentDictionary<string, Assembly>();

        private static readonly string PluginDir = Path.Combine(Domain.CurrentDirectory, "plugins");

        public static void LoadAllPlugins(string[] args)
        {
            Type[] iType =
            {
                typeof(CheckCqAt),
                //typeof(DragonDetectorAlpha),
                typeof(PandaDetector),
                typeof(PornDetector),
                typeof(Repeat),
                typeof(GroupQuiet),
                typeof(KeywordTrigger),

                typeof(MyGraveyard),
                typeof(Kudosu),
                typeof(Panda),
                typeof(Rcon),
                typeof(Send),
                typeof(Shutdown),
                typeof(Plugin),
            };

            foreach (var item in iType)
            {
                InsertPlugin(item, args);
            }

            foreach (var item in Directory.GetFiles(PluginDir, "*.dll"))
            {
                bool isValid = false;
                FileInfo fi = new FileInfo(item);
                try
                {
                    Logger.Info("已发现" + fi.Name);

                    //Assembly asm = Assembly.LoadFile(fi.FullName);
                    Assembly asm = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(fi.FullName);
                    // 目前无Unload

                    Type[] t = asm.GetExportedTypes();
                    foreach (Type type in t)
                    {
                        string typeName = "";
                        try
                        {
                            if (type.BaseType.BaseType != typeof(AppConstruct)) continue;
                            if (type.BaseType == typeof(CommandApp))
                                if (type.IsDefined(typeof(CommandAttribute), false) == false)
                                {
                                    Logger.Error("bad implementation");
                                    continue;
                                }
                            typeName = type.Name ?? "";
                            InsertPlugin(type, args);

                            isValid = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(typeName + " occurred an unexpected error.");
                            Logger.Exception(ex);
                        }
                    }

                    if (isValid)
                        AssemblyList.GetOrAdd(fi.Name, asm);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

                if (!isValid)
                    Logger.Warn($"\"{fi.Name}\" 不是合法的插件扩展。");

            }

            //throw new NotImplementedException();
        }

        public static void RemovePlugin<T>()
        {
            foreach (var item in CommandMap)
            {
                if (typeof(T) != item.Value.GetType()) continue;
                CommandMap.Remove(item.Key, out _);
            }

            foreach (var item in ServiceList)
            {
                if (typeof(T) != item.GetType()) continue;
                ServiceList.Remove(item);
            }

            foreach (var item in ApplicationList)
            {
                if (typeof(T) != item.GetType()) continue;
                ApplicationList.Remove(item);
            }
        }

        public static void AddPlugin<T>(string[] args)
        {
            Type type = typeof(T);
            AppConstruct plugin = Activator.CreateInstance(type) as AppConstruct;
            InsertPlugin(plugin, args);
        }

        private static void InsertPlugin(Type type, string[] args)
        {
            AppConstruct plugin = Activator.CreateInstance(type) as AppConstruct;
            //if (plugin.AppType != AppType.Command)
            {
                InsertPlugin(plugin, args);
            }
        }

        private static void InsertPlugin(AppConstruct plugin, string[] args)
        {
            plugin.OnLoad(args);
            switch (plugin.AppType)
            {
                case AppType.Command:
                    CommandApp cmdPlugin = (CommandApp)plugin;
                    if (cmdPlugin.Command == null)
                        Logger.Warn($"\"{plugin.Name}\" 没有设置命令！！");
                    else
                    {
                        string[] cmds = cmdPlugin.Command.Split(',');
                        foreach (var cmd in cmds)
                            CommandMap.GetOrAdd(cmd, (CommandApp)plugin);
                    }

                    Logger.Origin($"命令 \"{plugin.Name}\" ({cmdPlugin.Command}) 已经加载完毕。");
                    break;
                case AppType.Application:
                    ApplicationList.Add((ApplicationApp)plugin);
                    Logger.Origin($"应用 \"{plugin.Name}\" 已经加载完毕。");
                    break;
                default:
                    ServiceList.Add((ServiceApp)plugin);
                    Logger.Origin($"服务 \"{plugin.Name}\" 已经加载完毕。");
                    break;
            }
        }
    }
}

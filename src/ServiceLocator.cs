using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using Castle.Core;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.Windsor;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss
{
    /// <summary>
    /// use this class to access all service
    /// </summary>
    public sealed class ServiceLocator
    {
        private IWindsorContainer _container;
        private IWindsorContainer container
        {
            get
            {
                if (_container == null)
                    throw new KissException("ServiceLocator is not initialized! please call ServiceLocator.Instance.Init() method when app startup.");

                return _container;
            }
        }

        private readonly SingleEntryGate gate = new SingleEntryGate();

        static ServiceLocator()
        {
            Singleton<ServiceLocator>.Instance = new ServiceLocator();
        }

        private ServiceLocator()
        {
        }

        /// <summary>
        /// init, must called to setup windsor container
        /// </summary>
        public void Init()
        {
            Init(null, false);
        }

        /// <summary>
        /// init, must called to setup windsor container
        /// </summary>
        /// <param name="action"></param>
        public void Init(Action action)
        {
            Init(action, true);
        }

        /// <summary>
        /// init, must called to setup windsor container
        /// </summary>
        public void Init(Action action, bool enablePlugins)
        {
            if (!gate.TryEnter())
                return;

            try
            {
                if (!CastleConfigFileExist)
                    _container = new WindsorContainer();
                else
                    _container = new WindsorContainer(CastleConfigFile);

                if (action != null)
                    action.Invoke();

                StartComponents();

                if (enablePlugins)
                {
                    PluginBootstrapper pluginBootstrapper = new PluginBootstrapper();
                    AddComponentInstance(pluginBootstrapper);

                    pluginBootstrapper.InitializePlugins(pluginBootstrapper.GetPluginDefinitions());
                }
            }
            catch (Exception ex)
            {
                throw new KissException("ServiceLocator init failed!", ex);
            }
        }

        /// <summary>Finds types assignable from of a certain type</summary>
        /// <param name="requestedType">The type to find</param>
        /// <param name="excludeSelf">If or not exclude requestedType</param>
        /// <returns>A list of types found</returns>
        public IList<Type> Find(Type requestedType, bool excludeSelf)
        {
            IList<Type> list = Resolve<ITypeFinder>().Find(requestedType);

            if (excludeSelf)
                list.Remove(requestedType);

            return list;
        }

        private void StartComponents()
        {
            StartComponents(container.Kernel);
            container.Kernel.ComponentCreated += KernelComponentCreated;
        }

        private void StartComponents(IKernel kernel)
        {
            var naming = (INamingSubSystem)kernel.GetSubSystem(SubSystemConstants.NamingKey);
            foreach (GraphNode node in kernel.GraphNodes)
            {
                var model = node as ComponentModel;
                if (model != null)
                {
                    if (typeof(IStartable).IsAssignableFrom(model.Implementation) ||
                        typeof(IAutoStart).IsAssignableFrom(model.Implementation))
                    {
                        IHandler h = naming.GetHandler(model.Name);
                        if (h.CurrentState == HandlerState.Valid)
                        {
                            object component = kernel[model.Name];
                            if (component is IStartable)
                                (component as IStartable).Start();
                            else if (component is IAutoStart)
                                (component as IAutoStart).Start();
                        }
                    }
                }
            }
            container.AddFacility<StartableFacility>();
        }

        private static void KernelComponentCreated(ComponentModel model, object instance)
        {
            if (instance is IStartable)
                return;

            var startable = instance as IAutoStart;
            if (startable != null)
            {
                startable.Start();
            }
        }

        /// <summary>
        /// instance
        /// </summary>
        public static ServiceLocator Instance { get { return Singleton<ServiceLocator>.Instance; } }

        #region Container Methods

        /// <summary>Resolves a service configured in the factory.</summary>
        public T Resolve<T>() where T : class
        {
            return container.Resolve<T>();
        }

        public object Resolve(Type serviceType)
        {
            return container.Resolve(serviceType);
        }

        /// <summary>Resolves a named service configured in the factory.</summary>
        /// <param name="key">The name of the service to resolve.</param>
        /// <returns>An instance of the resolved service.</returns>        
        public object Resolve(string key)
        {
            return container.Resolve(key);
        }

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="classType">The type of component to register.</param>
        public void AddComponent(string key, Type classType)
        {
            container.AddComponent(key, classType);
        }

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="classType">The type of component to register.</param>
        public void AddComponent(string key, Type serviceType, Type classType)
        {
            container.AddComponent(key, serviceType, classType);
        }

        public void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyleType)
        {
            container.AddComponentLifeStyle(key, serviceType, classType, lifestyleType);
        }

        /// <summary>Adds a component instance to the container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="instance">The service instance to add.</param>
        public void AddComponentInstance(string key, Type serviceType, object instance)
        {
            container.Kernel.AddComponentInstance(key, serviceType, instance);
        }

        /// <summary>Adds a component instance to the container.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T AddComponentInstance<T>(T instance) where T : class
        {
            if (instance != null)
            {
                AddComponentInstance(typeof(T).Name, typeof(T), instance);
            }
            return instance as T;
        }

        #endregion

        private string castleConfigFile;
        private string CastleConfigFile
        {
            get
            {
                if (castleConfigFile == null)
                {
                    string filename = ConfigurationManager.AppSettings["castle"];
                    if (StringUtil.IsNullOrEmpty(filename))
                        filename = HttpContext.Current == null ? "castle.xml" : "~/App_Data/castle.xml";

                    castleConfigFile = ServerUtil.MapPath(filename);
                }

                return castleConfigFile;
            }
        }

        private bool CastleConfigFileExist
        {
            get
            {
                return File.Exists(CastleConfigFile);
            }
        }
    }
}

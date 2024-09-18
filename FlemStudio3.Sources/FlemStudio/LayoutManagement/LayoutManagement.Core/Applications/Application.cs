namespace FlemStudio.LayoutManagement.Core.Applications
{
    public abstract class Application
    {
        public Action? OnStateUpdated;

        public abstract void Dispose();
    }
    public abstract class Application<TApplicationState> : Application
    {
        protected TApplicationState State { get; }

        protected Application(TApplicationState state)
        {
            State = state;
        }
        public TApplicationState GetState()
        {
            return State;
        }

    }

    public class LoadedApplication : IDisposable
    {
        protected ApplicationService ApplicationService;
        public Guid Guid { get; }
        public string? Tag { get; }
        public ApplicationType ApplicationType { get; }
        public Application Application { get; }

        protected List<ApplicationUser> Users = new();
        protected Cooldown? SaveCooldown = null;

        public LoadedApplication(ApplicationService applicationService, Guid guid, string? tag, ApplicationType applicationType, Application application)
        {
            ApplicationService = applicationService;
            Guid = guid;
            Tag = tag;
            ApplicationType = applicationType;
            Application = application;

            Application.OnStateUpdated += OnStateUpdated;
        }


        internal ApplicationUser CreateUser()
        {
            ApplicationUser user = ApplicationType.CreateUser(this);
            Users.Add(user);
            return user;
        }

        internal void OnUserDisposed(ApplicationUser user)
        {
            Users.Remove(user);
            if (Users.Count == 0)
            {
                ApplicationService.OnApplicationUnused(this);
            }
        }

        private void OnStateUpdated()
        {
            if (SaveCooldown != null)
            {
                SaveCooldown.Reset();
            }
            else
            {
                SaveCooldown = new Cooldown(5.0f);
            }
        }

        public void Update(float deltaTime)
        {
            if (SaveCooldown != null)
            {
                if (SaveCooldown.Update(deltaTime))
                {
                    SaveCooldown = null;
                    Save();

                }
            }
        }

        public void Dispose()
        {
            if (SaveCooldown != null)
            {
                Save();

            }
            Application.OnStateUpdated -= OnStateUpdated;
            Application.Dispose();
        }

        public void Save()
        {
            ApplicationService.SaveApplication(this);
        }

    }

    /*

    public class LoadedApplication<TApplication> : LoadedApplication where TApplication : Application
    {
        
        
        public new ApplicationType<TApplication> ApplicationType { get; }
        public new TApplication Application { get; }


        public LoadedApplication(ApplicationService applicationService, Guid guid, ApplicationType<TApplication> applicationType, TApplication application) : base(applicationService, guid, applicationType, application)
        {
            ApplicationType = applicationType;
            Application = application;

            
        }

        public override void Save()
        {
            ApplicationType.WriteApplication(Application, ApplicationService.GetStateFilePath(this));
        }
    }
    */

    public abstract class ApplicationUser : IDisposable
    {
        internal LoadedApplication LoadedApplication;
        public Guid ApplicationGuid => LoadedApplication.Guid;
        public string ApplicationType => LoadedApplication.ApplicationType.Name;



        protected ApplicationUser(LoadedApplication loadedApplication)
        {
            LoadedApplication = loadedApplication;
        }

        public virtual void Dispose()
        {
            LoadedApplication.OnUserDisposed(this);
        }
    }

    public abstract class ApplicationUser<TApplication> : ApplicationUser, IDisposable where TApplication : Application
    {

        protected TApplication Application;
        protected ApplicationUser(LoadedApplication loadedApplication) : base(loadedApplication)
        {
            Application = (TApplication)loadedApplication.Application;
        }
    }

    public abstract class ApplicationType
    {
        public string Name { get; }
        public Type Type { get; }
        public Type UserType { get; }

        public abstract string FileExtension { get; }

        protected ApplicationType(string name, Type type, Type userType)
        {
            Name = name;
            Type = type;
            UserType = userType;
        }

        public abstract Application LoadApplication(string path);

        public abstract Application CreateApplication(string path);

        public abstract void SaveApplication(Application application, string path);

        public abstract ApplicationUser CreateUser(LoadedApplication loadedApplication);
    }

    public abstract class ApplicationType<TApplication, TApplicationUser> : ApplicationType where TApplication : Application where TApplicationUser : ApplicationUser
    {
        public ApplicationType(string name) : base(name, typeof(TApplication), typeof(TApplicationUser))
        {

        }

        public override Application LoadApplication(string path)
        {
            return DoReadApplication(path);
        }

        public override Application CreateApplication(string path)
        {
            TApplication application = DoCreateApplication();
            DoWriteApplication(application, path);
            return application;
        }

        public override void SaveApplication(Application application, string path)
        {
            DoWriteApplication((TApplication)application, path);
        }

        public override ApplicationUser CreateUser(LoadedApplication loadedApplication)
        {
            return DoCreateApplicationUser(loadedApplication);
        }

        public abstract TApplication DoReadApplication(string path);
        public abstract void DoWriteApplication(TApplication application, string path);
        public abstract TApplication DoCreateApplication();
        public abstract TApplicationUser DoCreateApplicationUser(LoadedApplication loadedApplication);


    }




}

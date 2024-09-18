using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FlemStudio.LayoutManagement.Core.Applications
{
    public interface ITestApplication
    {
        public Action<string, string>? OnInputUpdated { get; set; }
        public string Input { get; set; }
    }
    public class TestApplicationState
    {
        public string Input { get; set; } = "Bonjour";
    }

    public class TestApplication : Application<TestApplicationState>, ITestApplication
    {
        public TestApplication(TestApplicationState state) : base(state)
        {

        }

        public Action<string, string>? OnInputUpdated { get; set; }
        public string Input
        {
            get
            {
                return State.Input;
            }
            set
            {
                if (State.Input != value)
                {
                    string oldValue = State.Input;
                    State.Input = value;
                    OnStateUpdated?.Invoke();
                    OnInputUpdated?.Invoke(oldValue, value);
                }
            }
        }

        public override void Dispose()
        {

        }
    }

    public class TestApplicationUser : ApplicationUser<TestApplication>, ITestApplication
    {

        public TestApplicationUser(LoadedApplication loadedApplication) : base(loadedApplication)
        {
            Application.OnInputUpdated += NotifyInputUpdated;
        }

        public override void Dispose()
        {
            Application.OnInputUpdated -= NotifyInputUpdated;
            base.Dispose();
        }

        public void NotifyInputUpdated(string oldValue, string newValue)
        {
            OnInputUpdated?.Invoke(oldValue, newValue);
        }
        public Action<string, string>? OnInputUpdated { get; set; }
        public string Input
        {
            get => Application.Input;
            set => Application.Input = value;
        }
    }


    public class TestApplicationType : ApplicationType<TestApplication, TestApplicationUser>
    {
        protected ISerializer Serializer;
        protected IDeserializer Deserializer;

        public TestApplicationType() : base("TestApplication")
        {
            Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        }

        public override string FileExtension => Name + ".yaml";

        public override TestApplication DoCreateApplication()
        {
            return new TestApplication(new TestApplicationState());
        }

        public override TestApplicationUser DoCreateApplicationUser(LoadedApplication loadedApplication)
        {
            return new TestApplicationUser(loadedApplication);
        }

        public override TestApplication DoReadApplication(string path)
        {
            using (TextReader reader = new StreamReader(path))
            {
                return new TestApplication(Deserializer.Deserialize<TestApplicationState>(reader));
            }
        }

        public override void DoWriteApplication(TestApplication application, string path)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                Serializer.Serialize(writer, application.GetState());
            }
            Debug.WriteLine("Test application saved: " + path);

        }


    }
}

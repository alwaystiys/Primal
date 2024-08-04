using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace PrimalEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".primal";
        [DataMember]
        public string Name { get; private set;  }
        [DataMember]
        public string Path { get; private set;  }

        public string FullPath => $"{Path}{Name}{Extension}";
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scene = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes
        {
            get;
        }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            _scene.Add(new Scene(this, "Default Scene"));
        }






    }
}

using PrimalEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

namespace PrimalEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".primal";
        [DataMember]
        public string Name { get; private set; } = "New Project";
        [DataMember]
        public string Path { get; private set; }

        public string FullPath => $@"{Path}{Name}\{Name}{Extension}";
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes
        {
            get; private set;
        }

        private Scene _activeScene;
        //[DataMember] 
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }


        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        public ICommand UndoCommand { private set; get; }
        public ICommand RedoCommand { private set; get; }

        public ICommand AddSceneCommand { private set; get; }
        public ICommand RemoveSceneCommand { private set; get; }
        public ICommand SaveCommand { private set; get; }


        private void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveScene(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.fromFile<Project>(file);
        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullPath);
        }

        public void Unload()
        {

        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            AddSceneCommand = new RelayComand<object>(x =>
            {
                AddScene($"New Scenes {_scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"
                ));
            });

            RemoveSceneCommand = new RelayComand<Scene>(x =>
            {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveScene(x);
                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveScene(x),
                    $"Remove {x.Name}"
                ));
            }, x => !x.IsActive);

            UndoCommand = new RelayComand<object>(x => UndoRedo.Undo());
            RedoCommand = new RelayComand<object>(x => UndoRedo.Redo());
            SaveCommand = new RelayComand<object>(x => Save(this));
        }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            //_scenes.Add(new Scene(this, "Default Scene"));
            OnDeserialized(new StreamingContext());
        }






    }
}

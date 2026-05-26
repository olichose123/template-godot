using System.Threading.Tasks;
using Chickensoft.Log;
using Chickensoft.Log.Godot;
using Godot;
using {{CODENAME}}.Core;
using {{CODENAME}}.Core.Steps;

namespace {{CODENAME}}
{
    public partial class Main : Node
    {
        ILog Log;
        PipelineRunner _pipeline;

        public override void _Ready()
        {
            base._Ready();
            // prepare logging
            LogFormatter.DefaultMessagePrefix = "INFO";
            LogFormatter.DefaultWarningPrefix = "WARN";
            LogFormatter.DefaultErrorPrefix = "ERROR";
            GDFileWriter.DefaultFileName = "res://output.log";

            Log = new Log(nameof(Main), new GDWriter(), GDFileWriter.Instance());

            // produce initial log message
            var time = System.DateTime.Now;
            Log.Print($"Game started at {time}");

            // create root node that will contain everything
            Node root = new Node();
            AddChild(root);
            Game.Initialize(root);

            // Initialize Pipeline
            _pipeline = new PipelineRunner();
            _pipeline.AddStep(new LoadSettingsStep());
            _pipeline.AddStep(new ModDiscoveryStep());
            _pipeline.AddStep(new ModActivationStep());
            _pipeline.AddStep(new AssetLoadingStep());
            _pipeline.AddStep(new DefinitionProcessingStep());

            _pipeline.ProgressChanged += (s, progress) => Game.UpdateLoadingScreen(progress);
            _pipeline.Finished += OnInitializationFinished;

            // Start Pipeline
            _ = _pipeline.Run();
        }

        private void OnInitializationFinished(object sender, InitializationContext context)
        {
            if (context.IsCriticalFailure)
            {
                Log.Err("Critical failure during initialization. Showing error screen.");
                // Game.ShowErrorScreen(context.Errors); 
                return;
            }

            // Transfer context to global Game state
            Game.AssetManager = context.AssetManager;
            Game.ModManager = context.ModManager;
            Game.Settings = context.Settings;

            Log.Print("Initialization complete. Starting main menu...");
            // Game.StartMainMenu();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            Game.Update((float)delta);
        }
    }
}

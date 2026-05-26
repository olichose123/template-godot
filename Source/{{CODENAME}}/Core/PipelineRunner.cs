using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chickensoft.Log;
using Chickensoft.Log.Godot;

namespace {{CODENAME}}.Core
{
    public class PipelineRunner
    {
        private readonly List<IInitStep> _steps = new List<IInitStep>();
        private readonly InitializationContext _context = new InitializationContext();
        private readonly ILog _log = new Log(nameof(PipelineRunner), new GDWriter(), GDFileWriter.Instance());

        public event EventHandler<float> ProgressChanged;
        public event EventHandler<InitializationContext> Finished;

        public void AddStep(IInitStep step) => _steps.Add(step);

        public async Task Run()
        {
            _log.Print("Starting Initialization Pipeline...");
            
            for (int i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];
                float progress = (float)i / _steps.Count;
                ProgressChanged?.Invoke(this, progress);

                _log.Print($"Step [{i + 1}/{_steps.Count}]: {step.Description}");
                
                try
                {
                    await step.Execute(_context);
                }
                catch (Exception ex)
                {
                    _log.Err($"Step '{step.Description}' failed: {ex.Message}");
                    _context.ReportError(ex.Message, true);
                }

                if (_context.IsCriticalFailure)
                {
                    _log.Err("Critical failure detected. Aborting pipeline.");
                    break;
                }
            }

            ProgressChanged?.Invoke(this, 1.0f);
            _log.Print("Pipeline Finished.");
            Finished?.Invoke(this, _context);
        }
    }
}

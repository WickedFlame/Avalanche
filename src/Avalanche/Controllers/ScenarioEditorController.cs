using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.Runner;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers
{
    public class ScenarioEditorController : Controller
    {
        public IActionResult Index(string scenario)
        {
            var path = PathMapper.GetScenarioPath();

            var scenarios = Directory.GetFiles(path, "*.yml");

            var tmp = LoadFile(scenario);

            var model = new ScenarioEditorModel
            {
                Scenarios = scenarios.Select(s => Path.GetFileName(s).Replace(".yml", "")),
                Name = tmp.Name,
                RawContent = tmp.RawContent
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveScenario([FromForm]RawScenarioModel model)
        {
            try
            {
                YamlMap.Serializer.Deserialize<Scenario>(model.RawContent);
            }
            catch
            {
                throw new InvalidDataException();
            }


            var file = $"{PathMapper.GetScenarioPath()}/{model.Name}.yml";
            System.IO.File.WriteAllText(file, model.RawContent);
            return RedirectToAction("Index", new { Scenario = model.Name });
        }

        private static RawScenarioModel LoadFile(string scenario)
        {
            if(string.IsNullOrEmpty(scenario))
            {
                return new RawScenarioModel
                {
                    Name = string.Empty,
                    RawContent = string.Empty
                };
            }

            var file = PathMapper.GetScenarioFile(scenario);
            return new RawScenarioModel
            {
                Name = scenario,
                RawContent = System.IO.File.ReadAllText(file),
            };
        }
    }
}

using EnsureThat;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Validation;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public class AnonymizerEngine : IAnonymizerEngine
    {
        private readonly AnonymizerConfigurationManager _configurationManger;
        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<AnonymizerEngine>();
        private readonly FhirJsonDeserializer _parser = new FhirJsonDeserializer();
        private readonly Dictionary<string, IAnonymizerProcessor> _processors;
        private readonly AnonymizationFhirPathRule[] _rules;
        private readonly ResourceValidator _validator = new ResourceValidator();

        public AnonymizerEngine(string configFilePath)
            : this(AnonymizerConfigurationManager.CreateFromConfigurationFile(configFilePath)) { }

        public AnonymizerEngine(AnonymizerConfigurationManager configurationManager)
        {
            _configurationManger = configurationManager;
            _processors = new Dictionary<string, IAnonymizerProcessor>();

            InitializeProcessors(_configurationManger);

            _rules = _configurationManger.FhirPathRules;

            _logger.LogDebug("AnonymizerEngine initialized successfully");
        }

        public async Task<Resource> AnonymizeResourceAsync(
            Resource resource,
            AnonymizerSettings settings = null
        )
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            ValidateInput(settings, resource);

            // CreateRootNode() wraps the live resource directly (root.Poco == resource), so the
            // visitor/processor pipeline below mutates `resource` itself in place - unlike the old
            // ElementNode-based pipeline, there's no separate tree to convert back via
            // ToPoco<Resource>() afterwards.
            var root = PocoNodeExtension.CreateRootNode(resource);
            await root.AnonymizeAsync(_rules, _processors, settings);
            ValidateOutput(settings, resource);

            return resource;
        }

        public static void InitializeFhirPathExtensionSymbols()
        {
            FhirPathCompiler.DefaultSymbolTable.AddExtensionSymbols();
        }

        public static AnonymizerEngine CreateWithFileContext(
            string configFilePath,
            string fileName,
            string inputFolderName
        )
        {
            var configurationManager = AnonymizerConfigurationManager.CreateFromConfigurationFile(
                configFilePath
            );
            var dateShiftScope = configurationManager.GetParameterConfiguration().DateShiftScope;
            var dateShiftKeyPrefix = string.Empty;
            if (dateShiftScope == DateShiftScope.File)
            {
                dateShiftKeyPrefix = Path.GetFileName(fileName);
            }
            else if (dateShiftScope == DateShiftScope.Folder)
            {
                dateShiftKeyPrefix = Path.GetFileName(inputFolderName.TrimEnd('\\', '/'));
            }

            configurationManager.SetDateShiftKeyPrefix(dateShiftKeyPrefix);
            return new AnonymizerEngine(configurationManager);
        }

        public async Task<ITypedElement> AnonymizeElementAsync(
            ITypedElement element,
            AnonymizerSettings settings = null
        )
        {
            EnsureArg.IsNotNull(element, nameof(element));

            // Reuse the element in place if it's already a live PocoNode (e.g. from
            // PocoNodeOrList.Root()); otherwise fall back to converting it, which may build a
            // disconnected copy if `element` isn't itself backed by a real POCO.
            var resourceNode = element as PocoNode ?? element.ToPocoNode();
            return await resourceNode.AnonymizeAsync(_rules, _processors, settings);
        }

        public async Task<string> AnonymizeJsonAsync(
            string json,
            AnonymizerSettings settings = null
        )
        {
            EnsureArg.IsNotNullOrEmpty(json, nameof(json));

            var resource = _parser.Deserialize<Resource>(json);
            var anonymizedResource = await AnonymizeResourceAsync(resource, settings);

            return anonymizedResource.ToJson(settings != null && settings.IsPrettyOutput);
        }

        private void ValidateInput(AnonymizerSettings settings, Resource resource)
        {
            if (settings != null && settings.ValidateInput)
            {
                _validator.ValidateInput(resource);
            }
        }

        private void ValidateOutput(AnonymizerSettings settings, Resource anonymizedNode)
        {
            if (settings != null && settings.ValidateOutput)
            {
                _validator.ValidateOutput(anonymizedNode);
            }
        }

        private void InitializeProcessors(AnonymizerConfigurationManager configurationManager)
        {
            _processors[AnonymizerMethod.DateShift.ToString().ToUpperInvariant()] =
                DateShiftProcessor.Create(configurationManager);
            _processors[AnonymizerMethod.Redact.ToString().ToUpperInvariant()] =
                RedactProcessor.Create(configurationManager);
            _processors[AnonymizerMethod.CryptoHash.ToString().ToUpperInvariant()] =
                new CryptoHashProcessor(
                    configurationManager.GetParameterConfiguration().CryptoHashKey,
                    configurationManager.GetParameterConfiguration().CryptoHashAlgorithm
                );
            _processors[AnonymizerMethod.Encrypt.ToString().ToUpperInvariant()] =
                new EncryptProcessor(configurationManager.GetEncryptKeyBytes());
            _processors[AnonymizerMethod.Substitute.ToString().ToUpperInvariant()] =
                new SubstituteProcessor();
            _processors[AnonymizerMethod.Perturb.ToString().ToUpperInvariant()] =
                new PerturbProcessor();
            _processors[AnonymizerMethod.Keep.ToString().ToUpperInvariant()] = new KeepProcessor();
            _processors[AnonymizerMethod.Generalize.ToString().ToUpperInvariant()] =
                new GeneralizeProcessor();
            _processors[AnonymizerMethod.Remove.ToString().ToUpperInvariant()] =
                new RemoveProcessor();
        }

        public void ClearProcessors()
        {
            _processors.Clear();
        }

        public void AddProcessor(string key, IAnonymizerProcessor processor)
        {
            _processors[key.ToUpperInvariant()] = processor;
        }
    }

    public class DePseudonymizerEngine : AnonymizerEngine, IDePseudonymizerEngine
    {
        public DePseudonymizerEngine(AnonymizerConfigurationManager configurationManager)
            : base(configurationManager)
        {
            ClearProcessors();
        }

        public Task<Resource> DePseudonymizeResourceAsync(
            Resource resource,
            AnonymizerSettings settings = null
        )
        {
            return AnonymizeResourceAsync(resource, settings);
        }
    }
}

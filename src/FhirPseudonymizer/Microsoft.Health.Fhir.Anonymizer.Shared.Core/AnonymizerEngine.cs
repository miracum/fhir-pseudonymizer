using System.Collections.Generic;
using System.IO;
using EnsureThat;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Validation;

namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public class AnonymizerEngine : IAnonymizerEngine
    {
        private readonly AnonymizerConfigurationManager _configurationManger;
        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<AnonymizerEngine>();
        private readonly FhirJsonParser _parser = new FhirJsonParser();
        private readonly Dictionary<string, IAnonymizerProcessor> _processors;
        private readonly AnonymizationFhirPathRule[] _rules;
        private readonly ResourceValidator _validator = new ResourceValidator();

        public AnonymizerEngine(string configFilePath) : this(
            AnonymizerConfigurationManager.CreateFromConfigurationFile(configFilePath))
        {
        }

        public AnonymizerEngine(AnonymizerConfigurationManager configurationManager)
        {
            _configurationManger = configurationManager;
            _processors = new Dictionary<string, IAnonymizerProcessor>();

            InitializeProcessors(_configurationManger);

            _rules = _configurationManger.FhirPathRules;

            _logger.LogDebug("AnonymizerEngine initialized successfully");
        }

        public Resource AnonymizeResource(Resource resource, AnonymizerSettings settings = null)
        {
            EnsureArg.IsNotNull(resource, nameof(resource));

            ValidateInput(settings, resource);
            var anonymizedResource = AnonymizeElement(resource.ToTypedElement()).ToPoco<Resource>();
            ValidateOutput(settings, anonymizedResource);

            return anonymizedResource;
        }

        public static void InitializeFhirPathExtensionSymbols()
        {
            FhirPathCompiler.DefaultSymbolTable.AddExtensionSymbols();
        }

        public static AnonymizerEngine CreateWithFileContext(string configFilePath, string fileName,
            string inputFolderName)
        {
            var configurationManager = AnonymizerConfigurationManager.CreateFromConfigurationFile(configFilePath);
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

        public ITypedElement AnonymizeElement(ITypedElement element, AnonymizerSettings settings = null)
        {
            EnsureArg.IsNotNull(element, nameof(element));

            var resourceNode = ElementNode.FromElement(element);
            return resourceNode.Anonymize(_rules, _processors);
        }

        public string AnonymizeJson(string json, AnonymizerSettings settings = null)
        {
            EnsureArg.IsNotNullOrEmpty(json, nameof(json));

            var resource = _parser.Parse<Resource>(json);
            var anonymizedResource = AnonymizeResource(resource, settings);

            var serializationSettings = new FhirJsonSerializationSettings
            {
                Pretty = settings != null && settings.IsPrettyOutput
            };
            return anonymizedResource.ToJson(serializationSettings);
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
                new CryptoHashProcessor(configurationManager.GetParameterConfiguration().CryptoHashKey);
            _processors[AnonymizerMethod.Encrypt.ToString().ToUpperInvariant()] =
                new EncryptProcessor(configurationManager.GetParameterConfiguration().EncryptKey);
            _processors[AnonymizerMethod.Substitute.ToString().ToUpperInvariant()] = new SubstituteProcessor();
            _processors[AnonymizerMethod.Perturb.ToString().ToUpperInvariant()] = new PerturbProcessor();
            _processors[AnonymizerMethod.Keep.ToString().ToUpperInvariant()] = new KeepProcessor();
            _processors[AnonymizerMethod.Generalize.ToString().ToUpperInvariant()] = new GeneralizeProcessor();
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
        public DePseudonymizerEngine(AnonymizerConfigurationManager configurationManager) : base(configurationManager)
        {
            ClearProcessors();
        }

        public Resource DePseudonymizeResource(Resource resource, AnonymizerSettings settings = null)
        {
            return AnonymizeResource(resource, settings);
        }
    }
}

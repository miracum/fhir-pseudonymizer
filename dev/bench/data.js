window.BENCHMARK_DATA = {
  "lastUpdate": 1788368283115,
  "repoUrl": "https://github.com/miracum/fhir-pseudonymizer",
  "entries": {
    "Benchmark": [
      {
        "commit": {
          "author": {
            "email": "chgl@users.noreply.github.com",
            "name": "chgl",
            "username": "chgl"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a6167238f4dfef499f315ac4a7a031415288de2c",
          "message": "ci: added benchmarks (#374)\n\n* feat: wip on dynamic rules per request\n\n* v2->v3\n\n* feat: experimental controller with config + resource as parameters input\n\n* v3\n\n* drop just parsing tests\n\n* Potential fix for pull request finding 'CodeQL / Missing Dispose call on local IDisposable'\n\nCo-authored-by: Copilot Autofix powered by AI <62310815+github-advanced-security[bot]@users.noreply.github.com>\n\n* Potential fix for pull request finding 'CodeQL / Missing Dispose call on local IDisposable'\n\nCo-authored-by: Copilot Autofix powered by AI <62310815+github-advanced-security[bot]@users.noreply.github.com>\n\n* test: added test for dynamic config  + key derivation\n\n* lockfile\n\n* feat: cache engines\n\n* disposable\n\n* ci: added k6 and micro benchmarks\n\n* ci: finalize k6 ci\n\n* ci\n\n* logs\n\n---------\n\nCo-authored-by: Copilot Autofix powered by AI <62310815+github-advanced-security[bot]@users.noreply.github.com>",
          "timestamp": "2026-08-03T15:54:41+02:00",
          "tree_id": "f697f18c3d48b816a02f292cb9d6a8f01d6b1de4",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/a6167238f4dfef499f315ac4a7a031415288de2c"
        },
        "date": 1785765382739,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 381094870.5714286,
            "unit": "ns",
            "range": "± 5539018.534130166"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1753315.5895833333,
            "unit": "ns",
            "range": "± 25433.723366728806"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21616775.202083334,
            "unit": "ns",
            "range": "± 308209.44291527756"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "chgl@users.noreply.github.com",
            "name": "chgl",
            "username": "chgl"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "8837ff0e46618b42ef942c6fe5f30e4a3a03af80",
          "message": "refactor: stresstest code (#376)\n\n* refactor: stresstest code\n\npartially to get rid of nbomber\n\n* ci: fix",
          "timestamp": "2026-08-03T16:28:06+02:00",
          "tree_id": "404c90ed18f59b1375c7bb3559535f86a4cc16ca",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/8837ff0e46618b42ef942c6fe5f30e4a3a03af80"
        },
        "date": 1785767393775,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 381052823.0833333,
            "unit": "ns",
            "range": "± 5582864.9493435705"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1847496.1869791667,
            "unit": "ns",
            "range": "± 22460.135707732705"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22663851.020089287,
            "unit": "ns",
            "range": "± 367151.92850147217"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "chgl@users.noreply.github.com",
            "name": "chgl",
            "username": "chgl"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "86b47389506ccd09781cc4afe69602ce804761c0",
          "message": "ci: fix pages (#377)",
          "timestamp": "2026-08-03T16:41:43+02:00",
          "tree_id": "6435df92ceac8ac427a6de9e25a9b77e4ebcc3d7",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/86b47389506ccd09781cc4afe69602ce804761c0"
        },
        "date": 1785768202628,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 380448874.93333334,
            "unit": "ns",
            "range": "± 6272912.8810179625"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1908781.353236607,
            "unit": "ns",
            "range": "± 13825.438196162204"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22116974.104166668,
            "unit": "ns",
            "range": "± 343464.9522658193"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "64022198+miracum-bot@users.noreply.github.com",
            "name": "miracum-bot",
            "username": "miracum-bot"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "f8486ead02bda5a78e794d943e76c26f984cc460",
          "message": "chore(master): release 2.31.0 (#375)",
          "timestamp": "2026-08-03T14:46:40Z",
          "tree_id": "80059d703c8775f3756972d16a0538ab0c6bf905",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/f8486ead02bda5a78e794d943e76c26f984cc460"
        },
        "date": 1785768899227,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 366775191.25,
            "unit": "ns",
            "range": "± 8360814.902711681"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1723554.4197916666,
            "unit": "ns",
            "range": "± 32104.2131968776"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21204276.358333334,
            "unit": "ns",
            "range": "± 391967.8259352298"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "chgl@users.noreply.github.com",
            "name": "chgl",
            "username": "chgl"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "2ff83323854446466786a3939d3214314cb52a6e",
          "message": "feat: support for blake3 as the crypto hash algorithm (#378)\n\n* feat: support for blake3 as the crypto hash algorithm\n\n* feat: optimize\n\n* test: added snapshot tests for blake3",
          "timestamp": "2026-08-04T09:44:57+02:00",
          "tree_id": "caac410bc00d05228bde3c97b8f41e69f5324dd9",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/2ff83323854446466786a3939d3214314cb52a6e"
        },
        "date": 1785829693131,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 393838188,
            "unit": "ns",
            "range": "± 5166678.795089307"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1963504.131640625,
            "unit": "ns",
            "range": "± 45026.07799463958"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2321.2836382939267,
            "unit": "ns",
            "range": "± 8.800555804352923"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23710823.029166665,
            "unit": "ns",
            "range": "± 281871.8615358944"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 335.5558487892151,
            "unit": "ns",
            "range": "± 2.4709126635315446"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "29139614+renovate[bot]@users.noreply.github.com",
            "name": "renovate[bot]",
            "username": "renovate[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": false,
          "id": "9822737ea61662752715d2e4d25303468c4dc7b8",
          "message": "chore(deps): update docker.io/mockserver/mockserver docker tag to v7 (#383)\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-08-04T08:13:31Z",
          "tree_id": "1eaac3be5c368156224b3a5b72f77bc69a6de12e",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/9822737ea61662752715d2e4d25303468c4dc7b8"
        },
        "date": 1785831578242,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 348574110.6,
            "unit": "ns",
            "range": "± 3205135.9501907867"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1786518.6942708334,
            "unit": "ns",
            "range": "± 20946.79260055906"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2487.5526485443115,
            "unit": "ns",
            "range": "± 4.055687858144363"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20580364.089583334,
            "unit": "ns",
            "range": "± 101643.06177879247"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 327.1780522419856,
            "unit": "ns",
            "range": "± 0.2770573828323635"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "29139614+renovate[bot]@users.noreply.github.com",
            "name": "renovate[bot]",
            "username": "renovate[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "8ae31a71d23b1b4cc2e329ff3aeb8b6d0bbb5a63",
          "message": "chore(deps): update github-actions (#379)\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-08-04T08:14:06Z",
          "tree_id": "d6401bdaac7eae4eacfdfb09a231e2462bce9b9d",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/8ae31a71d23b1b4cc2e329ff3aeb8b6d0bbb5a63"
        },
        "date": 1785831836003,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 209212047.94117647,
            "unit": "ns",
            "range": "± 3609825.3265224705"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1027245.927734375,
            "unit": "ns",
            "range": "± 15501.948610358611"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1653.6894076211113,
            "unit": "ns",
            "range": "± 23.31144509346468"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 13840514.547008548,
            "unit": "ns",
            "range": "± 478504.0572581747"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 214.15370668683732,
            "unit": "ns",
            "range": "± 3.032120655021108"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "64022198+miracum-bot@users.noreply.github.com",
            "name": "miracum-bot",
            "username": "miracum-bot"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "8350165eb948926e61ffcdd14bc0a5f38fe6bb28",
          "message": "chore(master): release 2.32.0 (#381)",
          "timestamp": "2026-08-07T20:24:00Z",
          "tree_id": "9302397caa70d828e37803e027ba6eff07d9ceae",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/8350165eb948926e61ffcdd14bc0a5f38fe6bb28"
        },
        "date": 1786134628861,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 357981863.35714287,
            "unit": "ns",
            "range": "± 5147692.449973836"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1730131.33125,
            "unit": "ns",
            "range": "± 14452.10335227847"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2473.389110837664,
            "unit": "ns",
            "range": "± 17.61574437086041"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21576643.859375,
            "unit": "ns",
            "range": "± 55631.74820222193"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 315.6329500198364,
            "unit": "ns",
            "range": "± 2.1483103132295596"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "eicherj@users.noreply.github.com",
            "name": "Johanna Eicher",
            "username": "eicherj"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "f7f1c73d559c62b19d45835207403879cdd02d00",
          "message": "fix: wire AnonymizerLogging to the app's configured ILoggerFactory (#385)\n\n* fix: wire AnonymizerLogging to the app's configured ILoggerFactory\n\nAnonymizerEngine, AnonymizationVisitor, CryptoHashProcessor and other\nengine-internal classes create their loggers via the static\nAnonymizerLogging.CreateLogger<T>(), which defaulted to a bare\nLoggerFactory() with no providers attached. Since nothing ever set\nAnonymizerLogging.LoggerFactory to the app's real, DI-configured\nfactory, Logging__LogLevel__Default/appsettings logging config had no\neffect inside the anonymization engine, and all its debug/trace log\noutput (e.g. per-node hash logging in CryptoHashProcessor) was\nsilently discarded.\n\n* fix: fall back to NullLogger when the anonymizer logger factory is disposed\n\nAnonymizerLogging.LoggerFactory is static, so once it's wired to the\napp's real ILoggerFactory, a disposed host (e.g. between test runs\nusing WebApplicationFactory) leaves it pointing at a disposed factory.\nCreateLogger<T>() now catches ObjectDisposedException and falls back\nto NullLogger<T>.Instance instead of throwing.",
          "timestamp": "2026-08-31T16:33:34Z",
          "tree_id": "6e64f510d6209df836fea544a766a87feb8d1ba8",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/f7f1c73d559c62b19d45835207403879cdd02d00"
        },
        "date": 1788194426181,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 359768901.5,
            "unit": "ns",
            "range": "± 4228508.112454934"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1662209.7223958333,
            "unit": "ns",
            "range": "± 20985.76603570262"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2232.4946705744815,
            "unit": "ns",
            "range": "± 7.838974074657582"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20941234.64955357,
            "unit": "ns",
            "range": "± 187142.19449920653"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 320.83965342385426,
            "unit": "ns",
            "range": "± 0.905912713722833"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "64022198+miracum-bot@users.noreply.github.com",
            "name": "miracum-bot",
            "username": "miracum-bot"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "c756846adcfd74cbbe99aeb3cde13694681e2f7d",
          "message": "chore(master): release 2.32.1 (#386)",
          "timestamp": "2026-08-31T16:43:46Z",
          "tree_id": "51826beaf61ed698fa84158122ce6675eda9b89c",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/c756846adcfd74cbbe99aeb3cde13694681e2f7d"
        },
        "date": 1788195003707,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 379364654.8,
            "unit": "ns",
            "range": "± 6453452.046016138"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1749700.1015625,
            "unit": "ns",
            "range": "± 17716.022739625245"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2230.273554665702,
            "unit": "ns",
            "range": "± 5.687283322123026"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20304819.20089286,
            "unit": "ns",
            "range": "± 151037.67835587656"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 332.06242039998375,
            "unit": "ns",
            "range": "± 1.2052419371742569"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "daniel.hahne@uni-leipzig.de",
            "name": "Daniel Hahne",
            "username": "trobanga"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3ac9106ce9769641639cbf8f4c4017dd99195125",
          "message": "feat: conform MiiFhirClient to the MII Pseudonymization IG 2026.1.0 (#384)\n\n* feat!: conform MiiFhirClient to the MII Pseudonymization IG 2026.1.0\n\nThe IG at version 2026.1.0 defines $pseudonymize and $de-pseudonymize with\ndifferent parameter names and different data types than the client sent:\n\n| Sent before             | IG 2026.1.0             |\n| ----------------------- | ----------------------- |\n| `target` as FhirString  | `context` as Identifier |\n| `original` as FhirString| `original` as Identifier|\n| `allowCreate`           | not defined             |\n| `pseudonym` as FhirString| `pseudonym` as Identifier|\n\nThe mapping of the Parameters resource is now code-generated. This replaces\nthe hand-written walking of the parts of the `original` output. It needs\nFhirParametersGenerator 0.7.5, which generates both directions and supports\nlists of complex objects.\n\nThe example instances of the IG are the test fixtures.\n\n`appsettings.json` had no `Mii` section, so `Mii__RequestRetryCount` was `0`\nand a transient failure was never retried, unlike gPAS and entici. The new\nsection sets the same default of 3 retries.\n\nBREAKING CHANGE: the client sends and reads `valueIdentifier` only. The new\n`Mii__System` setting gives the system of these identifiers and is required\nif `PseudonymizationService` is `Mii`.\n\nSigned-off-by: Daniel Hahne <daniel.hahne@uni-leipzig.de>\n\n* docs: document the retry count setting of every pseudonymization service\n\n`gPAS__RequestRetryCount` and `entici__RequestRetryCount` have always worked\nand default to 3 in `appsettings.json`, but neither was in the README. Only\n`Mii__RequestRetryCount` was listed, which read as if Mii were the one\nservice that can retry.\n\nVfps has no row because it talks gRPC and does not use this policy.\n\nSigned-off-by: Daniel Hahne <daniel.hahne@uni-leipzig.de>\n\n---------\n\nSigned-off-by: Daniel Hahne <daniel.hahne@uni-leipzig.de>",
          "timestamp": "2026-09-02T09:38:42Z",
          "tree_id": "7bdb127f573acaf5f9250107d1756c4d3e5860ab",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/3ac9106ce9769641639cbf8f4c4017dd99195125"
        },
        "date": 1788342377169,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 376493491.0851064,
            "unit": "ns",
            "range": "± 14486528.505508956"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1818667.2921875,
            "unit": "ns",
            "range": "± 25140.681639123766"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2285.045470101493,
            "unit": "ns",
            "range": "± 11.432703935171862"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22066080.307692308,
            "unit": "ns",
            "range": "± 150420.8691709526"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 319.48694154194425,
            "unit": "ns",
            "range": "± 0.8870246249199143"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "29139614+renovate[bot]@users.noreply.github.com",
            "name": "renovate[bot]",
            "username": "renovate[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "b5076362c6bbed315ee18565ebc132c26d3b7c01",
          "message": "chore(deps): update github-actions (#388)\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>\n\nRelease-As: 2.33.0",
          "timestamp": "2026-09-02T18:47:30+02:00",
          "tree_id": "d34ced7d6716c544db05ca73af3daaf7ee930f3e",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/b5076362c6bbed315ee18565ebc132c26d3b7c01"
        },
        "date": 1788367779833,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 292265872.95238096,
            "unit": "ns",
            "range": "± 6851063.350317375"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1505533.4224330357,
            "unit": "ns",
            "range": "± 16468.5884965039"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2386.431838226318,
            "unit": "ns",
            "range": "± 12.345395818512872"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 19218602.35714286,
            "unit": "ns",
            "range": "± 285379.39028396417"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 303.37571723120556,
            "unit": "ns",
            "range": "± 2.091714802712201"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "64022198+miracum-bot@users.noreply.github.com",
            "name": "miracum-bot",
            "username": "miracum-bot"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "f89035813a6064bc9081a0f2e0c154fca2610a5a",
          "message": "chore(master): release 2.33.0 (#387)",
          "timestamp": "2026-09-02T16:51:20Z",
          "tree_id": "79548b3454f4dc4888c61eb6db91b1332b38e6d9",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/f89035813a6064bc9081a0f2e0c154fca2610a5a"
        },
        "date": 1788368282390,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 368165433.4285714,
            "unit": "ns",
            "range": "± 8435369.206740573"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1670088.28515625,
            "unit": "ns",
            "range": "± 6590.5090963359535"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2247.088912691389,
            "unit": "ns",
            "range": "± 7.231039759409961"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21159297.15625,
            "unit": "ns",
            "range": "± 172174.8736872685"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 319.2328632061298,
            "unit": "ns",
            "range": "± 1.1050877941236033"
          }
        ]
      }
    ]
  }
}
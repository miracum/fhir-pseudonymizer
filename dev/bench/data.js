window.BENCHMARK_DATA = {
  "lastUpdate": 1788949109532,
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
          "id": "1093f87d6dadcb9ca5b34f131bb337220bd74134",
          "message": "feat: integrate the experimental dynamic config endpoint into the stable $de-identify operation (#392)",
          "timestamp": "2026-09-07T14:05:27+02:00",
          "tree_id": "bf328f188b61246d894bf2683c97cb7ddddc0b0a",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/1093f87d6dadcb9ca5b34f131bb337220bd74134"
        },
        "date": 1788782866843,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 362530321.8,
            "unit": "ns",
            "range": "± 4556720.484838476"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1756373.7640625,
            "unit": "ns",
            "range": "± 12022.743768717983"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2452.0677391052245,
            "unit": "ns",
            "range": "± 19.087640249897117"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22658429.42857143,
            "unit": "ns",
            "range": "± 277530.21108410927"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 326.42325534820554,
            "unit": "ns",
            "range": "± 2.7922532648126115"
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
          "id": "de71506617feffac17f8e052c9b6fb3a9c4b1531",
          "message": "fix: simplified kafka config to enabled/disabled via a flag vs. topic names (#394)",
          "timestamp": "2026-09-07T14:29:07+02:00",
          "tree_id": "14f1301fef52ea1de5b47c535f2b935d9a4d8263",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/de71506617feffac17f8e052c9b6fb3a9c4b1531"
        },
        "date": 1788784277685,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 354345741.38461536,
            "unit": "ns",
            "range": "± 4759451.041789721"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1695287.815625,
            "unit": "ns",
            "range": "± 22906.132213151115"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2225.7998006184894,
            "unit": "ns",
            "range": "± 16.655326915337405"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21284221.354910713,
            "unit": "ns",
            "range": "± 322965.8744728568"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 319.39668277899426,
            "unit": "ns",
            "range": "± 0.29225065885127377"
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
          "id": "f0eefaaf999829e713e188e752d1de0430b99c7c",
          "message": "docs: added OpenSSF best practices badge (#395)",
          "timestamp": "2026-09-07T14:53:05+02:00",
          "tree_id": "8ed497085b275f7a8de849ef84a2a2482db9aef1",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/f0eefaaf999829e713e188e752d1de0430b99c7c"
        },
        "date": 1788785722613,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 379646216.26666665,
            "unit": "ns",
            "range": "± 5149940.693766864"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1827630.5362723214,
            "unit": "ns",
            "range": "± 16760.572517036675"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2490.9508417569673,
            "unit": "ns",
            "range": "± 2.8955071723131325"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23090469.485576924,
            "unit": "ns",
            "range": "± 128238.91785161663"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 313.91192657606945,
            "unit": "ns",
            "range": "± 0.6920609023235929"
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
          "id": "3fc558228ccab44cc7b4db6137286d2303211fe2",
          "message": "chore(master): release 2.34.0 (#393)",
          "timestamp": "2026-09-08T09:59:18Z",
          "tree_id": "05b77e11ef5173275f96df2d5c313648645dda4c",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/3fc558228ccab44cc7b4db6137286d2303211fe2"
        },
        "date": 1788861884907,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 357440295.93333334,
            "unit": "ns",
            "range": "± 4540887.706325847"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1632329.8270833334,
            "unit": "ns",
            "range": "± 21241.62886508729"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2238.9938703683706,
            "unit": "ns",
            "range": "± 5.087226822557856"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20413857.191964287,
            "unit": "ns",
            "range": "± 108752.56595484978"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 318.57121324539185,
            "unit": "ns",
            "range": "± 0.6729861639652275"
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
          "id": "4c6f84cbd79de6ff37483ecdf48b3f78ea1a8ce9",
          "message": "docs: added more docs on cotrnibuting and pr template (#398)",
          "timestamp": "2026-09-08T20:59:12+02:00",
          "tree_id": "c4b21f3c5685502db71ebc3fe53eab8ebde7f679",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/4c6f84cbd79de6ff37483ecdf48b3f78ea1a8ce9"
        },
        "date": 1788894084017,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 360755090.4444444,
            "unit": "ns",
            "range": "± 7706438.609668673"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1611977.9078125,
            "unit": "ns",
            "range": "± 13856.157740962504"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2373.2459264119466,
            "unit": "ns",
            "range": "± 12.018932045409475"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20164060.770833332,
            "unit": "ns",
            "range": "± 311046.448929262"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 313.87888738087247,
            "unit": "ns",
            "range": "± 1.0755958929268696"
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
          "id": "7416cc95ddb95b109daf7ab80ed03fdb71f16d68",
          "message": "feat: added Kestrel__MaxRequestBodySize config option (#400)",
          "timestamp": "2026-09-08T22:19:21+02:00",
          "tree_id": "244923979e045b0179b37c4b534cefe15f24da7b",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/7416cc95ddb95b109daf7ab80ed03fdb71f16d68"
        },
        "date": 1788898891197,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 355459927.9230769,
            "unit": "ns",
            "range": "± 3955775.0054172375"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1743730.91796875,
            "unit": "ns",
            "range": "± 28576.57513564216"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2252.138335418701,
            "unit": "ns",
            "range": "± 14.349154326885733"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20320464.51339286,
            "unit": "ns",
            "range": "± 201446.43709143056"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 328.83271830422535,
            "unit": "ns",
            "range": "± 2.007327714635824"
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
          "id": "6c094ad4bef9bb50201f57481ec3f01a90100b26",
          "message": "chore(master): release 2.35.0 (#399)",
          "timestamp": "2026-09-08T20:55:38Z",
          "tree_id": "78198108969ac7997a051dcfc059264b23fa2bcc",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/6c094ad4bef9bb50201f57481ec3f01a90100b26"
        },
        "date": 1788901348773,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 386530350.2,
            "unit": "ns",
            "range": "± 6286653.455304899"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1795388.0340401786,
            "unit": "ns",
            "range": "± 14409.927896507008"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2504.0124776022776,
            "unit": "ns",
            "range": "± 4.905998908609104"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23053752.80580357,
            "unit": "ns",
            "range": "± 283523.14478432084"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 314.29993503434315,
            "unit": "ns",
            "range": "± 0.5986278524950753"
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
          "id": "185cf083828311bd48a1ada5b1fd3e803f2ee10c",
          "message": "perf: guard the cryptoHash debug log to avoid quadratic node.Location cost (#401)\n\nSigned-off-by: Daniel Hahne <daniel.hahne@uni-leipzig.de>",
          "timestamp": "2026-09-09T07:27:08Z",
          "tree_id": "9270fa62bfc0a58d7aa3a066a69f8d44876f8867",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/185cf083828311bd48a1ada5b1fd3e803f2ee10c"
        },
        "date": 1788939506139,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 359745745.8666667,
            "unit": "ns",
            "range": "± 10549131.391471835"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1767064.7235576923,
            "unit": "ns",
            "range": "± 8226.858866962551"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 2349.290902064397,
            "unit": "ns",
            "range": "± 13.324610829350714"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22755398.25892857,
            "unit": "ns",
            "range": "± 220324.88255498072"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 317.71379330952965,
            "unit": "ns",
            "range": "± 3.1468368469496064"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 6301722711.333333,
            "unit": "ns",
            "range": "± 64671470.89602655"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 11762891142.333334,
            "unit": "ns",
            "range": "± 231125368.7920002"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5905908354.666667,
            "unit": "ns",
            "range": "± 39632533.47930564"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10997954323,
            "unit": "ns",
            "range": "± 166099114.15839106"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5711552390.666667,
            "unit": "ns",
            "range": "± 47510016.67151615"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7864670791.333333,
            "unit": "ns",
            "range": "± 38641707.294899"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 917909974.3333334,
            "unit": "ns",
            "range": "± 85853716.40318362"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1428671882.3333333,
            "unit": "ns",
            "range": "± 84196442.83617607"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 896162564.3333334,
            "unit": "ns",
            "range": "± 43878190.87446585"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1449931719.3333333,
            "unit": "ns",
            "range": "± 51707571.57936772"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 904385882.3333334,
            "unit": "ns",
            "range": "± 100542270.39566617"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1199386916.6666667,
            "unit": "ns",
            "range": "± 94279124.78921154"
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
          "id": "1df9413d213849e5426857e1e8d6e1ee408bf35b",
          "message": "perf: ai-assisted micro-optimizations (#403)",
          "timestamp": "2026-09-09T10:34:57+02:00",
          "tree_id": "a18ac391935cecb5a754667b0403b3864d12c466",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/1df9413d213849e5426857e1e8d6e1ee408bf35b"
        },
        "date": 1788943294163,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 351287383.85714287,
            "unit": "ns",
            "range": "± 5766480.342482351"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1713625.8587239583,
            "unit": "ns",
            "range": "± 36243.88480013012"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1686.8356948852538,
            "unit": "ns",
            "range": "± 22.862696516082305"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21465795.445833333,
            "unit": "ns",
            "range": "± 196185.1056065663"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 314.48767035802206,
            "unit": "ns",
            "range": "± 2.9649003545262675"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 4872228160,
            "unit": "ns",
            "range": "± 24772014.134285547"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9260838303.666666,
            "unit": "ns",
            "range": "± 63221871.85337762"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 4831742875.333333,
            "unit": "ns",
            "range": "± 24709074.502308205"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 9632002286.666666,
            "unit": "ns",
            "range": "± 449863909.20802116"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 4876524591,
            "unit": "ns",
            "range": "± 4536414.877211629"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 6789261152.666667,
            "unit": "ns",
            "range": "± 14929117.913437799"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 813656569.3333334,
            "unit": "ns",
            "range": "± 35285930.809476376"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1290372049,
            "unit": "ns",
            "range": "± 152452874.50748023"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 809762636,
            "unit": "ns",
            "range": "± 20723591.623640023"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1290485931,
            "unit": "ns",
            "range": "± 127464398.38297771"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 805993538.3333334,
            "unit": "ns",
            "range": "± 26720394.346356384"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1071485348.6666666,
            "unit": "ns",
            "range": "± 49843628.633578844"
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
          "id": "a2d4d10c7ea6867c50e1ed9b3590ebb564b9b4b0",
          "message": "fix: clear pool (#404)",
          "timestamp": "2026-09-09T10:56:56+02:00",
          "tree_id": "683dceb2aeb19b9e8df3db349d23dd1d1f7f0a84",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/a2d4d10c7ea6867c50e1ed9b3590ebb564b9b4b0"
        },
        "date": 1788944663930,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 382760814.8,
            "unit": "ns",
            "range": "± 4864784.649668621"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1970267.7706473214,
            "unit": "ns",
            "range": "± 23699.533341649443"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1585.529609298706,
            "unit": "ns",
            "range": "± 5.092703653129501"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23076955.352083333,
            "unit": "ns",
            "range": "± 177675.96181367867"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 311.6326630796705,
            "unit": "ns",
            "range": "± 1.0555556566538364"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5646631074,
            "unit": "ns",
            "range": "± 21270153.501025114"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10617401946.333334,
            "unit": "ns",
            "range": "± 627317405.503708"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5936205673.333333,
            "unit": "ns",
            "range": "± 92664098.34448397"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 11587247143.333334,
            "unit": "ns",
            "range": "± 492127719.49567217"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5710457807,
            "unit": "ns",
            "range": "± 124714173.58220966"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7904400037.333333,
            "unit": "ns",
            "range": "± 261076776.64564082"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 892707958.6666666,
            "unit": "ns",
            "range": "± 33871467.20580215"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1301865842.6666667,
            "unit": "ns",
            "range": "± 46841153.26705237"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 878957018.6666666,
            "unit": "ns",
            "range": "± 130089125.01069045"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1431537627,
            "unit": "ns",
            "range": "± 153637458.6930274"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 913966818.3333334,
            "unit": "ns",
            "range": "± 97871950.16700175"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1180044833,
            "unit": "ns",
            "range": "± 73231642.63034955"
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
          "id": "eb0f2d567383118560f5a3c5f7e92fae30e3a422",
          "message": "chore(master): release 2.35.1 (#402)",
          "timestamp": "2026-09-09T10:07:33Z",
          "tree_id": "48ee639d8142bafe807cd98bee8c5349f32209d9",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/eb0f2d567383118560f5a3c5f7e92fae30e3a422"
        },
        "date": 1788949108721,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 372331297.7368421,
            "unit": "ns",
            "range": "± 8141509.156899316"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1858021.9958333333,
            "unit": "ns",
            "range": "± 31791.570233872655"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1621.2144700686138,
            "unit": "ns",
            "range": "± 3.3610077516311"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22158790.125,
            "unit": "ns",
            "range": "± 223641.88628778528"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 313.42059561184476,
            "unit": "ns",
            "range": "± 1.8609943297155684"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5406957235.666667,
            "unit": "ns",
            "range": "± 115370549.71572292"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10241355384.333334,
            "unit": "ns",
            "range": "± 455580389.27431524"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5447662064.333333,
            "unit": "ns",
            "range": "± 156573777.2498016"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10050865353.333334,
            "unit": "ns",
            "range": "± 96726990.13822304"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5572183895,
            "unit": "ns",
            "range": "± 76021438.00137088"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7826857413,
            "unit": "ns",
            "range": "± 159250318.53828213"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 869957383,
            "unit": "ns",
            "range": "± 44024118.61681305"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1397646255,
            "unit": "ns",
            "range": "± 87017053.83663866"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 848016430.3333334,
            "unit": "ns",
            "range": "± 123625394.90764281"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1364070134.6666667,
            "unit": "ns",
            "range": "± 97022018.02695073"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 849302796.6666666,
            "unit": "ns",
            "range": "± 40142335.91423574"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1095319439,
            "unit": "ns",
            "range": "± 41544537.58248966"
          }
        ]
      }
    ]
  }
}
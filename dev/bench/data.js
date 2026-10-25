window.BENCHMARK_DATA = {
  "lastUpdate": 1792941479306,
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
          "id": "75e778be228ceb938f6a380073d142449a6c1cfc",
          "message": "docs: added CODEOWNERS file (#405)",
          "timestamp": "2026-09-10T00:05:18+02:00",
          "tree_id": "bd25c7afa4f6b869c47ed907a89d0523e20790ea",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/75e778be228ceb938f6a380073d142449a6c1cfc"
        },
        "date": 1788992076460,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 225782434.95918366,
            "unit": "ns",
            "range": "± 8984310.369080532"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1181029.570888832,
            "unit": "ns",
            "range": "± 52532.89492619211"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1096.496313241812,
            "unit": "ns",
            "range": "± 1.9360934010685136"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 15651319.701766305,
            "unit": "ns",
            "range": "± 746877.9594855221"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 223.13565177267247,
            "unit": "ns",
            "range": "± 5.5325570013650385"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5940929819.666667,
            "unit": "ns",
            "range": "± 72270054.89376402"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 12051652662.333334,
            "unit": "ns",
            "range": "± 263225184.37926728"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5516754276.666667,
            "unit": "ns",
            "range": "± 170417562.56429935"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 11173077883,
            "unit": "ns",
            "range": "± 155631368.85798946"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5627224145,
            "unit": "ns",
            "range": "± 69848201.97201002"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7568542533.333333,
            "unit": "ns",
            "range": "± 171781947.43010157"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 768331028.3333334,
            "unit": "ns",
            "range": "± 109329127.24055764"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1091148829.6666667,
            "unit": "ns",
            "range": "± 104850672.15615676"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 837420690.6666666,
            "unit": "ns",
            "range": "± 99189362.48756051"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1315831913.3333333,
            "unit": "ns",
            "range": "± 113722657.32674614"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 798831558.6666666,
            "unit": "ns",
            "range": "± 109724420.34927109"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1145883538,
            "unit": "ns",
            "range": "± 132844324.54511689"
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
          "id": "e35d6f0d10cda3f5a6a9c528e2e9beefb48754db",
          "message": "chore(renovate): enable renovate automerge (#407)",
          "timestamp": "2026-09-10T01:13:02+02:00",
          "tree_id": "fc21432886f6020ccc56979294fdffdc12e43845",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/e35d6f0d10cda3f5a6a9c528e2e9beefb48754db"
        },
        "date": 1788996013489,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 370872367.06666666,
            "unit": "ns",
            "range": "± 6706052.015258408"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1816585.388392857,
            "unit": "ns",
            "range": "± 25525.608768292637"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1611.5544829368591,
            "unit": "ns",
            "range": "± 0.879683238803636"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23127519.33639706,
            "unit": "ns",
            "range": "± 464889.7529370049"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 375.4183803876241,
            "unit": "ns",
            "range": "± 2.2615624923680544"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5315426394.666667,
            "unit": "ns",
            "range": "± 31044977.962349888"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9780451769,
            "unit": "ns",
            "range": "± 45011037.81368434"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5415833925,
            "unit": "ns",
            "range": "± 44697954.22467714"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10791163912,
            "unit": "ns",
            "range": "± 103163740.12603162"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5417690177.666667,
            "unit": "ns",
            "range": "± 86155754.35786518"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7333167788.333333,
            "unit": "ns",
            "range": "± 91140306.81232457"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 870008353,
            "unit": "ns",
            "range": "± 35834205.51689995"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1292080442.6666667,
            "unit": "ns",
            "range": "± 67388835.77032197"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 853757204,
            "unit": "ns",
            "range": "± 42443973.64297635"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1333128236,
            "unit": "ns",
            "range": "± 99749654.26023138"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 838463812.6666666,
            "unit": "ns",
            "range": "± 79788060.21198887"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1152972632.6666667,
            "unit": "ns",
            "range": "± 81985167.71854328"
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
          "id": "15c388c43e6eab24cb2554c920007d2170d03152",
          "message": "chore(deps): update github/codeql-action action to v4.37.9 (#409)\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-09-10T14:39:46Z",
          "tree_id": "0ca5eef6838ef3f74c38b4e5c876594c4d5420bf",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/15c388c43e6eab24cb2554c920007d2170d03152"
        },
        "date": 1789051867787,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 342934651.64285713,
            "unit": "ns",
            "range": "± 1741747.3624929567"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1642696.439732143,
            "unit": "ns",
            "range": "± 25499.293075809335"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1565.4827026954065,
            "unit": "ns",
            "range": "± 3.286505218470122"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20177565.389583334,
            "unit": "ns",
            "range": "± 330143.4371230211"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 316.5576912879944,
            "unit": "ns",
            "range": "± 1.4458648705796773"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5236448842,
            "unit": "ns",
            "range": "± 42308407.99422461"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10190965640,
            "unit": "ns",
            "range": "± 27580285.853779852"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5260946264.666667,
            "unit": "ns",
            "range": "± 61121182.34352411"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10914496382,
            "unit": "ns",
            "range": "± 323027020.8513979"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5448454694,
            "unit": "ns",
            "range": "± 110700300.73703887"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 8410421493,
            "unit": "ns",
            "range": "± 101401003.62635842"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 866419925.3333334,
            "unit": "ns",
            "range": "± 151554540.2369399"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1383220142.6666667,
            "unit": "ns",
            "range": "± 98601317.82426813"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 856882903.6666666,
            "unit": "ns",
            "range": "± 46372727.58437557"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1418545045.3333333,
            "unit": "ns",
            "range": "± 111780094.10335009"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 854661194.3333334,
            "unit": "ns",
            "range": "± 22702469.18271106"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1181876047.6666667,
            "unit": "ns",
            "range": "± 73666730.74813624"
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
          "id": "0bd6cdb52f8c992ae7cb9bc423019b372b1e622c",
          "message": "chore(deps): update all non-major dependencies (#410)\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>\nCo-authored-by: chgl <chgl@users.noreply.github.com>",
          "timestamp": "2026-09-12T14:30:08+02:00",
          "tree_id": "97caa9f81c805603cae0244790366e64467abafd",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/0bd6cdb52f8c992ae7cb9bc423019b372b1e622c"
        },
        "date": 1789216625151,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 352825370.9285714,
            "unit": "ns",
            "range": "± 3813284.7828272968"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1753835.256138393,
            "unit": "ns",
            "range": "± 8922.57070559749"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1603.0077932675679,
            "unit": "ns",
            "range": "± 1.6649930793184737"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21287486.73214286,
            "unit": "ns",
            "range": "± 74505.13535371087"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 325.1774839083354,
            "unit": "ns",
            "range": "± 0.7768630081475432"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5154402467,
            "unit": "ns",
            "range": "± 37668315.665589735"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9681414159.666666,
            "unit": "ns",
            "range": "± 118284681.92968899"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5116542043,
            "unit": "ns",
            "range": "± 31582416.58711238"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 9661197134.333334,
            "unit": "ns",
            "range": "± 60506323.80105896"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5111381780.666667,
            "unit": "ns",
            "range": "± 13273618.691198807"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 6987878955.666667,
            "unit": "ns",
            "range": "± 23989797.439893324"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 820951649.6666666,
            "unit": "ns",
            "range": "± 16307456.107526284"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1304264466,
            "unit": "ns",
            "range": "± 167405975.19333416"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 852576752.3333334,
            "unit": "ns",
            "range": "± 50274902.41142407"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1370403124.3333333,
            "unit": "ns",
            "range": "± 155619235.57784393"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 840411018.3333334,
            "unit": "ns",
            "range": "± 12158186.903428378"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1111121152,
            "unit": "ns",
            "range": "± 44309231.5217977"
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
          "id": "d2f2bad1b3067b8ddd602e86ae2bb0e7a3d82c79",
          "message": "feat: oauth support for vfps clients (#412)",
          "timestamp": "2026-09-17T22:52:29+02:00",
          "tree_id": "a57464a44976205c85f5007127f5c8616730ed9f",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/d2f2bad1b3067b8ddd602e86ae2bb0e7a3d82c79"
        },
        "date": 1789678767468,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 346626955.9285714,
            "unit": "ns",
            "range": "± 5994394.798340202"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1634663.0302083334,
            "unit": "ns",
            "range": "± 28751.680922599964"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1584.3074656266433,
            "unit": "ns",
            "range": "± 5.702082296883626"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20588668.977083333,
            "unit": "ns",
            "range": "± 328969.2105337218"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 315.39790512965277,
            "unit": "ns",
            "range": "± 0.6067691589414702"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5140361958,
            "unit": "ns",
            "range": "± 7880472.805370309"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10202240922.666666,
            "unit": "ns",
            "range": "± 36396787.82479328"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5295087544,
            "unit": "ns",
            "range": "± 123994383.80157666"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10346665095.666666,
            "unit": "ns",
            "range": "± 29732065.549418095"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5116378582.666667,
            "unit": "ns",
            "range": "± 23348370.225967173"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7604016902.666667,
            "unit": "ns",
            "range": "± 11143922.183977252"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 775621536,
            "unit": "ns",
            "range": "± 19341145.02764591"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1277674586.6666667,
            "unit": "ns",
            "range": "± 158821725.33595586"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 783912226.3333334,
            "unit": "ns",
            "range": "± 39921477.71718943"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1386192181.3333333,
            "unit": "ns",
            "range": "± 101368011.23159224"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 811765919.3333334,
            "unit": "ns",
            "range": "± 26539598.30291616"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1138429811.3333333,
            "unit": "ns",
            "range": "± 88646112.1660523"
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
          "id": "246c4ed92ad5219b9d9afd726f34d5a6fe8ba341",
          "message": "chore(master): release 2.36.0 (#406)",
          "timestamp": "2026-09-18T13:49:54+02:00",
          "tree_id": "0366313999bafc61a2b8a165e27b857cdfe7887e",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/246c4ed92ad5219b9d9afd726f34d5a6fe8ba341"
        },
        "date": 1789732528859,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 272739409.0714286,
            "unit": "ns",
            "range": "± 4318889.293418167"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1254879.765625,
            "unit": "ns",
            "range": "± 25079.46431940821"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1201.5664558410645,
            "unit": "ns",
            "range": "± 2.0475159296425414"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 15318838.475961538,
            "unit": "ns",
            "range": "± 62288.322527448036"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 258.8360243865422,
            "unit": "ns",
            "range": "± 0.24989829754735"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 4248277206,
            "unit": "ns",
            "range": "± 30486175.459641818"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 7839068495.666667,
            "unit": "ns",
            "range": "± 347321652.39689845"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 4282554605,
            "unit": "ns",
            "range": "± 23278790.07206326"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 7765146339,
            "unit": "ns",
            "range": "± 22552150.62215218"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 4293163809.3333335,
            "unit": "ns",
            "range": "± 34516809.79264625"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5768523846.666667,
            "unit": "ns",
            "range": "± 16312793.097281542"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 685033816.6666666,
            "unit": "ns",
            "range": "± 102392138.65884835"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1072053570.6666666,
            "unit": "ns",
            "range": "± 184144993.1901015"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 676232009.3333334,
            "unit": "ns",
            "range": "± 91107111.51023032"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1156241633.3333333,
            "unit": "ns",
            "range": "± 203700989.56621838"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 679131320.6666666,
            "unit": "ns",
            "range": "± 86980231.95991223"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 963937433.3333334,
            "unit": "ns",
            "range": "± 151381005.8550252"
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
          "id": "37476f0dab43dedfaa26a8d2a109eba1d974de9a",
          "message": "fix: cancel requests mid-processing if the client disconnects (#413)",
          "timestamp": "2026-09-18T20:21:07+02:00",
          "tree_id": "96fad0e1c9b33ff3c74e30d4b2d9fdcd560e48d7",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/37476f0dab43dedfaa26a8d2a109eba1d974de9a"
        },
        "date": 1789756104997,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 298583183.35714287,
            "unit": "ns",
            "range": "± 3364636.170417164"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1573856.834263393,
            "unit": "ns",
            "range": "± 11081.32020134387"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1621.8316384633383,
            "unit": "ns",
            "range": "± 6.46263761731635"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20228022.36875,
            "unit": "ns",
            "range": "± 58269.58773049485"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 301.77563667297363,
            "unit": "ns",
            "range": "± 1.4098554192232278"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5572711100.666667,
            "unit": "ns",
            "range": "± 57594726.96384646"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 11267336550.666666,
            "unit": "ns",
            "range": "± 136947863.70437494"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5438427593,
            "unit": "ns",
            "range": "± 33025275.82414191"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10907632243.666666,
            "unit": "ns",
            "range": "± 556428008.7760612"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5344201134.333333,
            "unit": "ns",
            "range": "± 86182254.32521276"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7516346233,
            "unit": "ns",
            "range": "± 85454229.04385269"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 744460114.3333334,
            "unit": "ns",
            "range": "± 48029976.09347689"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1204096501,
            "unit": "ns",
            "range": "± 51915464.7996934"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 730252888.3333334,
            "unit": "ns",
            "range": "± 41173751.364014626"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1310248117,
            "unit": "ns",
            "range": "± 61617251.06985818"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 720742895.3333334,
            "unit": "ns",
            "range": "± 27491508.837407127"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1159053053.3333333,
            "unit": "ns",
            "range": "± 77903487.36201754"
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
          "id": "f4b6c293b5f4d957df8ab95c0c362e03221208c1",
          "message": "feat: support for access token auth (#415)",
          "timestamp": "2026-09-19T14:28:08+02:00",
          "tree_id": "474589e59e49cd0b62155e8217040dd3d6b11890",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/f4b6c293b5f4d957df8ab95c0c362e03221208c1"
        },
        "date": 1789821310831,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 351136759.38461536,
            "unit": "ns",
            "range": "± 3811249.8017415176"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1778512.0791666666,
            "unit": "ns",
            "range": "± 14761.951290124858"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1630.8040295918784,
            "unit": "ns",
            "range": "± 3.743635113062947"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21276801.984375,
            "unit": "ns",
            "range": "± 125869.52051047693"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 312.29887504577636,
            "unit": "ns",
            "range": "± 0.7845521507285356"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5122000009,
            "unit": "ns",
            "range": "± 25158592.524376318"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9588257378.333334,
            "unit": "ns",
            "range": "± 32550771.630745366"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5248015147,
            "unit": "ns",
            "range": "± 139365513.53920475"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10024321019.333334,
            "unit": "ns",
            "range": "± 142264543.8440821"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5399867212,
            "unit": "ns",
            "range": "± 243225027.92772022"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7572401721.666667,
            "unit": "ns",
            "range": "± 56354949.505697116"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 869054901.6666666,
            "unit": "ns",
            "range": "± 19282364.276760884"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1345642837.3333333,
            "unit": "ns",
            "range": "± 37304437.53211717"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 861352698,
            "unit": "ns",
            "range": "± 79394374.40144014"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1489607839,
            "unit": "ns",
            "range": "± 63078026.42756899"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 942738646.3333334,
            "unit": "ns",
            "range": "± 64049546.25786033"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1315468954.3333333,
            "unit": "ns",
            "range": "± 63268369.18987478"
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
          "id": "9387da04fc52f96bdf217063950561f40acb0949",
          "message": "refactor: use StatusCode constants instead of raw codes (#416)",
          "timestamp": "2026-09-19T14:53:25+02:00",
          "tree_id": "494fcadc0c416244294accd8b465dca2564e90a4",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/9387da04fc52f96bdf217063950561f40acb0949"
        },
        "date": 1789822824523,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 358415374.21428573,
            "unit": "ns",
            "range": "± 4330818.359368943"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1798779.37109375,
            "unit": "ns",
            "range": "± 7038.469707260837"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1614.5451412200928,
            "unit": "ns",
            "range": "± 2.3346378633877376"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22174725.011160713,
            "unit": "ns",
            "range": "± 257511.2471170534"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 327.7685483296712,
            "unit": "ns",
            "range": "± 0.825364908612648"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5296588507.666667,
            "unit": "ns",
            "range": "± 13319712.182019562"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9752773829.333334,
            "unit": "ns",
            "range": "± 22281610.069551535"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5282797998,
            "unit": "ns",
            "range": "± 38101150.3456816"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 9994378753.333334,
            "unit": "ns",
            "range": "± 39061360.63861084"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5235945665.666667,
            "unit": "ns",
            "range": "± 49727818.07968704"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7143178457,
            "unit": "ns",
            "range": "± 45724509.34280108"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 828296356.6666666,
            "unit": "ns",
            "range": "± 111187200.5006707"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1352481773,
            "unit": "ns",
            "range": "± 175001550.54306746"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 882109446.6666666,
            "unit": "ns",
            "range": "± 31722409.627357695"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1328217389,
            "unit": "ns",
            "range": "± 59867626.13972633"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 842873804,
            "unit": "ns",
            "range": "± 81968476.12121433"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1119266292.3333333,
            "unit": "ns",
            "range": "± 48650831.52707912"
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
          "id": "45e65b39f1d001ed8b3a66d328391fed27ac620f",
          "message": "chore(master): release 2.37.0 (#414)\n\nCo-authored-by: chgl <chgl@users.noreply.github.com>",
          "timestamp": "2026-09-19T15:56:04+02:00",
          "tree_id": "aa1e300854255ab4e50ce462d0abe97f389325a9",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/45e65b39f1d001ed8b3a66d328391fed27ac620f"
        },
        "date": 1789826571887,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 349703786.2,
            "unit": "ns",
            "range": "± 4207082.940657444"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1768203.99609375,
            "unit": "ns",
            "range": "± 33441.25103161983"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1592.636705716451,
            "unit": "ns",
            "range": "± 1.1057242542933603"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21613917.826923076,
            "unit": "ns",
            "range": "± 55738.376551428235"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 326.11712302480424,
            "unit": "ns",
            "range": "± 0.5554902479661333"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5174547173,
            "unit": "ns",
            "range": "± 48362254.950292766"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9629777172.666666,
            "unit": "ns",
            "range": "± 49672845.80240179"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5143888060,
            "unit": "ns",
            "range": "± 37644690.17691503"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 9918551540,
            "unit": "ns",
            "range": "± 22104319.405155253"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5231925978,
            "unit": "ns",
            "range": "± 31506457.537328232"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7142077977.666667,
            "unit": "ns",
            "range": "± 15464692.129026894"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 823930241,
            "unit": "ns",
            "range": "± 94471678.24165086"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1286162955.6666667,
            "unit": "ns",
            "range": "± 83341023.40856728"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 866348073.6666666,
            "unit": "ns",
            "range": "± 46141237.35166591"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1363732451,
            "unit": "ns",
            "range": "± 154080498.3835207"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 834930740,
            "unit": "ns",
            "range": "± 69163605.70397453"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1126326150,
            "unit": "ns",
            "range": "± 54115218.358361386"
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
          "id": "1f21664780c690b65e5ade077cb3c1c2beb87073",
          "message": "chore(deps): update dependency verify.xunitv3 to v32 (#389)\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-09-19T16:58:21+02:00",
          "tree_id": "d1cd889e2d9f84709929384c94779eefa81ddce7",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/1f21664780c690b65e5ade077cb3c1c2beb87073"
        },
        "date": 1789830322247,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 351994918.8666667,
            "unit": "ns",
            "range": "± 5943141.46171485"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1797409.99609375,
            "unit": "ns",
            "range": "± 13737.201008248125"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1592.8430968693324,
            "unit": "ns",
            "range": "± 2.8933910490011443"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22091839.675,
            "unit": "ns",
            "range": "± 301669.8771353832"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 330.86610136032107,
            "unit": "ns",
            "range": "± 2.6943278628323113"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5304730167.333333,
            "unit": "ns",
            "range": "± 144910370.53884614"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9554948429,
            "unit": "ns",
            "range": "± 118631655.8506082"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5143590485,
            "unit": "ns",
            "range": "± 27537707.737606827"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10266264225,
            "unit": "ns",
            "range": "± 441891141.8502624"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5349500111,
            "unit": "ns",
            "range": "± 182649611.318109"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7450928333,
            "unit": "ns",
            "range": "± 69919353.26968774"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 858843627.6666666,
            "unit": "ns",
            "range": "± 83338258.86128755"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1328361393.6666667,
            "unit": "ns",
            "range": "± 92758233.05355023"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 874958749,
            "unit": "ns",
            "range": "± 78718600.4130183"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1431213218,
            "unit": "ns",
            "range": "± 74764576.32573324"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 902123419.3333334,
            "unit": "ns",
            "range": "± 55270380.431697816"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1226317118,
            "unit": "ns",
            "range": "± 65381687.626598656"
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
          "id": "d3e1f81a24ada47fa599d198eb63ca69c9937156",
          "message": "chore(master): release 2.37.1 (#417)",
          "timestamp": "2026-09-19T17:37:26+02:00",
          "tree_id": "aa1ab5a4ae99f4b6ebdfbfecedd71ddbf9c7ba4d",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/d3e1f81a24ada47fa599d198eb63ca69c9937156"
        },
        "date": 1789832667825,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 349122284.14285713,
            "unit": "ns",
            "range": "± 4872968.908661138"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1621282.9296875,
            "unit": "ns",
            "range": "± 19411.579184853053"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1613.694119317191,
            "unit": "ns",
            "range": "± 3.930974590780951"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20570641.489583332,
            "unit": "ns",
            "range": "± 143806.79542846844"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 314.4486234982808,
            "unit": "ns",
            "range": "± 0.5813764145837617"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5355977943.333333,
            "unit": "ns",
            "range": "± 102456270.67976664"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10313646073,
            "unit": "ns",
            "range": "± 23416474.173065145"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5328798408,
            "unit": "ns",
            "range": "± 71074649.09792656"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10603889707,
            "unit": "ns",
            "range": "± 36969210.62511591"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5273040317.333333,
            "unit": "ns",
            "range": "± 43289908.17199416"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7766352547,
            "unit": "ns",
            "range": "± 37775821.718287304"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 802353628.6666666,
            "unit": "ns",
            "range": "± 30788320.89530575"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1312074935,
            "unit": "ns",
            "range": "± 158385467.1904401"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 809481642,
            "unit": "ns",
            "range": "± 44236076.36101004"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1375944719.3333333,
            "unit": "ns",
            "range": "± 133965494.2946195"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 809931014.3333334,
            "unit": "ns",
            "range": "± 43466025.26304852"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1140949005.3333333,
            "unit": "ns",
            "range": "± 74543935.17867"
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
          "id": "f416806a6705bd5a0037c1057235cd75e84b4eb3",
          "message": "fix: store every exception during processing in the DLQ (#421)",
          "timestamp": "2026-09-22T18:58:21+02:00",
          "tree_id": "330446bd3a6ce319e04ce02ba6e407f49d23ff77",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/f416806a6705bd5a0037c1057235cd75e84b4eb3"
        },
        "date": 1790096739940,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 372122541.5,
            "unit": "ns",
            "range": "± 6379868.720808419"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1860410.2979166666,
            "unit": "ns",
            "range": "± 20738.95228333572"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1602.2272457122804,
            "unit": "ns",
            "range": "± 3.793206877750702"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22934267.983333334,
            "unit": "ns",
            "range": "± 421986.29271701106"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 324.55621836980185,
            "unit": "ns",
            "range": "± 2.2922031522104716"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5678631573.333333,
            "unit": "ns",
            "range": "± 12951266.718257112"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10360859353.333334,
            "unit": "ns",
            "range": "± 243867594.48927498"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5562657320.333333,
            "unit": "ns",
            "range": "± 36532255.67941219"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 10427843089.666666,
            "unit": "ns",
            "range": "± 162313610.5504517"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5686521059.666667,
            "unit": "ns",
            "range": "± 91193437.11937247"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7717975835.333333,
            "unit": "ns",
            "range": "± 64825243.80345373"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 916171436.3333334,
            "unit": "ns",
            "range": "± 52309490.124070086"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1361043534,
            "unit": "ns",
            "range": "± 71565418.75844686"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 888130575,
            "unit": "ns",
            "range": "± 90565264.50878829"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1434955074.6666667,
            "unit": "ns",
            "range": "± 82147928.73660211"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 937352393,
            "unit": "ns",
            "range": "± 82817314.46516687"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1264850415,
            "unit": "ns",
            "range": "± 86964014.43438603"
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
          "id": "0ed4aaa4da133ea70b6b0ccd4462d4bba72c6d90",
          "message": "chore(master): release 2.37.2 (#422)",
          "timestamp": "2026-09-22T19:00:32+02:00",
          "tree_id": "98c67e14a19fec7aaa59fd288a8e7998d6e29efb",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/0ed4aaa4da133ea70b6b0ccd4462d4bba72c6d90"
        },
        "date": 1790096799545,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 303670692.9375,
            "unit": "ns",
            "range": "± 5682106.581915413"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1343521.638950893,
            "unit": "ns",
            "range": "± 20974.571095074054"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1272.821893964495,
            "unit": "ns",
            "range": "± 3.2596170327243654"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 17045525.31473214,
            "unit": "ns",
            "range": "± 183815.17488429937"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 248.18285522460937,
            "unit": "ns",
            "range": "± 1.2753696528847718"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 4924042840,
            "unit": "ns",
            "range": "± 141364507.8066862"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 8971367233,
            "unit": "ns",
            "range": "± 916818156.2484126"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 4738328360.333333,
            "unit": "ns",
            "range": "± 31947241.734929264"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 8741014541.666666,
            "unit": "ns",
            "range": "± 603480366.4816948"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 4418357998.333333,
            "unit": "ns",
            "range": "± 73448444.35622911"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 6002539622.333333,
            "unit": "ns",
            "range": "± 110479456.90402788"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 680431935.6666666,
            "unit": "ns",
            "range": "± 95531916.8693777"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1223486743.3333333,
            "unit": "ns",
            "range": "± 224194372.01186115"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 703295811.6666666,
            "unit": "ns",
            "range": "± 97522307.1093804"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1152964960.3333333,
            "unit": "ns",
            "range": "± 178046175.09855372"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 702841316,
            "unit": "ns",
            "range": "± 86452000.78532927"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1007074603,
            "unit": "ns",
            "range": "± 170750967.56041747"
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
          "id": "cb6a6a0bcd273a118886d7f94243dd4e7d215f54",
          "message": "refactor: migrate to otel metrics (#418)\n\nCo-authored-by: Copilot Autofix powered by AI <62310815+github-advanced-security[bot]@users.noreply.github.com>",
          "timestamp": "2026-09-22T19:19:36+02:00",
          "tree_id": "e89fdd883b4361dbebf82c5a8337e37ddb40a303",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/cb6a6a0bcd273a118886d7f94243dd4e7d215f54"
        },
        "date": 1790097963106,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 346327134.8666667,
            "unit": "ns",
            "range": "± 4118944.47088188"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1738325.1901041667,
            "unit": "ns",
            "range": "± 10401.877688488623"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1541.0767397880554,
            "unit": "ns",
            "range": "± 1.741319668642487"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21942718.511111107,
            "unit": "ns",
            "range": "± 199683.58482014696"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 315.47464955647786,
            "unit": "ns",
            "range": "± 0.9953754829705456"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5000420230.666667,
            "unit": "ns",
            "range": "± 21399268.880479686"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9456157801.666666,
            "unit": "ns",
            "range": "± 19130373.295465965"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5048557041.666667,
            "unit": "ns",
            "range": "± 23388139.07010956"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 9986973240.666666,
            "unit": "ns",
            "range": "± 451151526.50715536"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5026413147.333333,
            "unit": "ns",
            "range": "± 32838785.70860336"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7048246049.333333,
            "unit": "ns",
            "range": "± 12270187.897462057"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 814726276.6666666,
            "unit": "ns",
            "range": "± 42708164.63898542"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1315689759.3333333,
            "unit": "ns",
            "range": "± 164700668.05183247"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 834053743.6666666,
            "unit": "ns",
            "range": "± 31104403.55849005"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1320833128.3333333,
            "unit": "ns",
            "range": "± 85952299.47090596"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 847201226,
            "unit": "ns",
            "range": "± 17087933.776224583"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1134713681.6666667,
            "unit": "ns",
            "range": "± 58959402.78880469"
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
          "id": "bbbe71990e9b5f6ec56443b7dc1207c1d0a5ae1e",
          "message": "fix: retry indefenitely on transient errors when processing via kafka (#424)",
          "timestamp": "2026-09-23T19:48:11+02:00",
          "tree_id": "ff390b211d14a3d49ec23fe30db816ddb01bf033",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/bbbe71990e9b5f6ec56443b7dc1207c1d0a5ae1e"
        },
        "date": 1790186166261,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 425180789,
            "unit": "ns",
            "range": "± 9316922.501991685"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1882454.6890625,
            "unit": "ns",
            "range": "± 18877.71557876357"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1551.4764565687913,
            "unit": "ns",
            "range": "± 3.7510130407062694"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20864065.098958332,
            "unit": "ns",
            "range": "± 101084.10191554211"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 327.63474838550275,
            "unit": "ns",
            "range": "± 0.9586766519912391"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 6717018992,
            "unit": "ns",
            "range": "± 110275046.50854732"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 11943789234,
            "unit": "ns",
            "range": "± 868760573.9819818"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 6150630468.666667,
            "unit": "ns",
            "range": "± 48313061.999964625"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 12148505890,
            "unit": "ns",
            "range": "± 141850657.0421012"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5839925600,
            "unit": "ns",
            "range": "± 70620966.28577048"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 8675211141,
            "unit": "ns",
            "range": "± 61372212.437383085"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 963926900.6666666,
            "unit": "ns",
            "range": "± 73090002.3005724"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1498472064.3333333,
            "unit": "ns",
            "range": "± 94166040.83208394"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1009791115,
            "unit": "ns",
            "range": "± 139435103.10223502"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1509672015.6666667,
            "unit": "ns",
            "range": "± 62193447.05577587"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 972407785.6666666,
            "unit": "ns",
            "range": "± 27959458.617055967"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1318409902.6666667,
            "unit": "ns",
            "range": "± 158828149.0603936"
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
          "id": "4d7a2eda84c230e038012d06f5b4b23692634dd7",
          "message": "chore(master): release 2.37.3 (#425)",
          "timestamp": "2026-09-23T22:53:57+02:00",
          "tree_id": "0f2ebc0aa1444683d124d1c9d15e54946a288920",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/4d7a2eda84c230e038012d06f5b4b23692634dd7"
        },
        "date": 1790197281397,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 362896666.5,
            "unit": "ns",
            "range": "± 4475771.318267462"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1751600.0204326923,
            "unit": "ns",
            "range": "± 12666.39699884574"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1520.624764887492,
            "unit": "ns",
            "range": "± 2.5270064765409797"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 20936160.7515625,
            "unit": "ns",
            "range": "± 475464.6363234496"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 323.0181872504098,
            "unit": "ns",
            "range": "± 0.523915103511741"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5540272222.666667,
            "unit": "ns",
            "range": "± 59238433.80680419"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10515482153,
            "unit": "ns",
            "range": "± 245272136.58170137"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5422115205.666667,
            "unit": "ns",
            "range": "± 193478963.15392414"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 11565450200.333334,
            "unit": "ns",
            "range": "± 85896076.99338372"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5637942920.333333,
            "unit": "ns",
            "range": "± 144369186.58451906"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 8235518381.333333,
            "unit": "ns",
            "range": "± 111246707.27005142"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 872184140,
            "unit": "ns",
            "range": "± 43409604.31215656"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1367898858.6666667,
            "unit": "ns",
            "range": "± 58908124.44701169"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 852184164.6666666,
            "unit": "ns",
            "range": "± 107027576.2049567"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1428626060,
            "unit": "ns",
            "range": "± 127563847.30902342"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 821590251.6666666,
            "unit": "ns",
            "range": "± 23985260.314384256"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1160799794.3333333,
            "unit": "ns",
            "range": "± 71248397.3083598"
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
          "id": "b77420a868738df569063c720c486470f2768cd9",
          "message": "feat: refactor kafka processing for improved throughput (#427)",
          "timestamp": "2026-09-25T00:45:42+02:00",
          "tree_id": "f3851cb131f73b5ddbb5ddab8d20aa548e7ddc92",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/b77420a868738df569063c720c486470f2768cd9"
        },
        "date": 1790290336613,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 333260402.8666667,
            "unit": "ns",
            "range": "± 5444608.79521638"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1614989.2611607143,
            "unit": "ns",
            "range": "± 25099.140773096544"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1699.3912952423095,
            "unit": "ns",
            "range": "± 19.26863228845342"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21244018.129166666,
            "unit": "ns",
            "range": "± 274070.0459526869"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 305.30984287261964,
            "unit": "ns",
            "range": "± 3.2935717331223393"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 4942465537.666667,
            "unit": "ns",
            "range": "± 32006740.318134777"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 9329539902,
            "unit": "ns",
            "range": "± 43461003.15100645"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 4923211786.333333,
            "unit": "ns",
            "range": "± 25506647.505515583"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 9321979116,
            "unit": "ns",
            "range": "± 61609089.75960795"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 4853473943.666667,
            "unit": "ns",
            "range": "± 55253008.151038855"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 6647940789.333333,
            "unit": "ns",
            "range": "± 61957113.95102427"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 792694330.6666666,
            "unit": "ns",
            "range": "± 10644071.539000493"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1255773962.3333333,
            "unit": "ns",
            "range": "± 169783788.0917085"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 820094610.3333334,
            "unit": "ns",
            "range": "± 24127260.639932796"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1335691031.6666667,
            "unit": "ns",
            "range": "± 130390061.03771247"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 831112441.6666666,
            "unit": "ns",
            "range": "± 32611613.161052033"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1100412164,
            "unit": "ns",
            "range": "± 72953802.28000544"
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
          "id": "be82f77afb338857fee5e0b17abb3330bb68db0d",
          "message": "feat: additional instrumentation for kafka processing (#426)",
          "timestamp": "2026-09-25T01:30:31+02:00",
          "tree_id": "5c4a6dcec122fd02d81632cecdf2fe8cd3568005",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/be82f77afb338857fee5e0b17abb3330bb68db0d"
        },
        "date": 1790293076555,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 372953181.1666667,
            "unit": "ns",
            "range": "± 12122050.17665295"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1774067.8916666666,
            "unit": "ns",
            "range": "± 20392.070801367034"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1606.2844755990166,
            "unit": "ns",
            "range": "± 3.6631742076985683"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23349969.48125,
            "unit": "ns",
            "range": "± 374589.901933661"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 327.4198266665141,
            "unit": "ns",
            "range": "± 1.8780225232039776"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 5465834945.666667,
            "unit": "ns",
            "range": "± 102473296.10230792"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10408314744.666666,
            "unit": "ns",
            "range": "± 317783025.0679123"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5865656875.666667,
            "unit": "ns",
            "range": "± 47003349.510461494"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 11321230857.666666,
            "unit": "ns",
            "range": "± 725132199.147859"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 6067968907.666667,
            "unit": "ns",
            "range": "± 150794785.1158439"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7884016542.333333,
            "unit": "ns",
            "range": "± 233844863.1291221"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 921314525.3333334,
            "unit": "ns",
            "range": "± 31978076.593737062"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1390312879.3333333,
            "unit": "ns",
            "range": "± 91288933.66739546"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 893163973.3333334,
            "unit": "ns",
            "range": "± 44304382.870702095"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1513444559,
            "unit": "ns",
            "range": "± 78395532.62165487"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 941131730.3333334,
            "unit": "ns",
            "range": "± 95149303.36627233"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1286691281,
            "unit": "ns",
            "range": "± 105230304.89193988"
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
          "id": "bf243af4bdbe8d7b394e0a70b5728cf6b46b1ec7",
          "message": "chore(master): release 2.38.0 (#428)",
          "timestamp": "2026-09-25T09:45:13+02:00",
          "tree_id": "86b26ab449b0d9e3753dfa99a169a32c2f302b97",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/bf243af4bdbe8d7b394e0a70b5728cf6b46b1ec7"
        },
        "date": 1790322771930,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 405474779.5625,
            "unit": "ns",
            "range": "± 7871976.8193046255"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1908117.746875,
            "unit": "ns",
            "range": "± 15472.796171622098"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1615.1874997065618,
            "unit": "ns",
            "range": "± 6.467520243965154"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 24145584.335416667,
            "unit": "ns",
            "range": "± 274671.33214687754"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 332.0217709541321,
            "unit": "ns",
            "range": "± 1.8998078903543898"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 6214536937,
            "unit": "ns",
            "range": "± 39671470.56227682"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 10766470286,
            "unit": "ns",
            "range": "± 105587051.36867878"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 5860658180.333333,
            "unit": "ns",
            "range": "± 92144699.72473964"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 11257800506.333334,
            "unit": "ns",
            "range": "± 684036086.5099238"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 5956163804,
            "unit": "ns",
            "range": "± 276811553.0696465"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 7989488613.666667,
            "unit": "ns",
            "range": "± 170120075.06389737"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 900211775.3333334,
            "unit": "ns",
            "range": "± 77727643.59972245"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 1393613681,
            "unit": "ns",
            "range": "± 80943668.28648981"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 898153028,
            "unit": "ns",
            "range": "± 112515931.11553079"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1499367417.3333333,
            "unit": "ns",
            "range": "± 80916617.67050298"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 890954061.6666666,
            "unit": "ns",
            "range": "± 54045152.89941855"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 1278693873,
            "unit": "ns",
            "range": "± 83205076.77441955"
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
          "id": "36e9d6f28127ed6b060835a6e76cac28554caa52",
          "message": "chore(release): prepare v3 release (#434)",
          "timestamp": "2026-10-15T22:41:11+02:00",
          "tree_id": "64ba0e0c0926654f393159ecada96392bb79781c",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/36e9d6f28127ed6b060835a6e76cac28554caa52"
        },
        "date": 1792097080518,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 111048954.78571428,
            "unit": "ns",
            "range": "± 771839.2000533211"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1536825.7229166667,
            "unit": "ns",
            "range": "± 5982.250650214871"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1638.6356999533516,
            "unit": "ns",
            "range": "± 3.2392485200586436"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 19709775.76875,
            "unit": "ns",
            "range": "± 73527.0891322974"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 306.2714276313782,
            "unit": "ns",
            "range": "± 0.6498194697259789"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 854228426.6666666,
            "unit": "ns",
            "range": "± 11296293.712636828"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 2039337139.3333333,
            "unit": "ns",
            "range": "± 8945481.507360816"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 864793867,
            "unit": "ns",
            "range": "± 8470207.376359742"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 2141959339,
            "unit": "ns",
            "range": "± 11929061.657972641"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 856832873,
            "unit": "ns",
            "range": "± 4906237.420119108"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1488783402.6666667,
            "unit": "ns",
            "range": "± 7485845.819707038"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 277692079,
            "unit": "ns",
            "range": "± 89984365.61941299"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 635377255,
            "unit": "ns",
            "range": "± 285096017.4396835"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 275852242,
            "unit": "ns",
            "range": "± 92823936.95653166"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 649011972,
            "unit": "ns",
            "range": "± 197890031.40667745"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 279232838.3333333,
            "unit": "ns",
            "range": "± 84895382.01833262"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 473419662.3333333,
            "unit": "ns",
            "range": "± 230843842.99323553"
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
          "id": "bcad147536ee08e4bb32b694ce4b7ad07ac5e7c0",
          "message": "chore(master): release 3.0.0 (#453)\n\n:robot: I have created a release *beep* *boop*\n---\n\n\n##\n[3.0.0](https://github.com/miracum/fhir-pseudonymizer/compare/v2.38.0...v3.0.0)\n(2026-10-15)\n\n\n### ⚠ BREAKING CHANGES\n\n* auth against re-identify via api keys is no longer possible. A JWT has\nto be fetched from an IdP first instead.\n* the application fails to start without a specified anonymization\nconfig. This breaks deployments relying on the default\nanonymization.yaml.\n* **kafka:** required to be set by default if kafka processing is\nenabled. Changes message keys and downstream routing.\n* Any *__RequestRetryCount settings are now ignored\n* smaller length keys will now fail validation\n* previous MetricsEnabled and MetricsPort config now ignored.\n* pseudonymization service is now unset by default, anyone relying on\ngPAS will now encounter an error. If using gPAS, at leat version 2023 is\nrequired.\n* this serializer may be less lenient than the pervious default\n* URIs that look like FHIR server URLs and contain ~ or - are now\nprocessed identical to URIs without those values. E.g. the system\nhttps://example-test/fhir/Patient/123 was previously hashed to by\nreplacing the entire value with a hash, now, just the 123 is hashed.\nThis is a fix towards the more correct behavior.\n* The v6 libs may be more strict when it comes to parsing resources.\nAdditionally, all timestamps are now searialized with sub-second\nprecision.\n\n### Features\n\n* added more metrics and ready-to-import dashboard\n([aaaabd8](https://github.com/miracum/fhir-pseudonymizer/commit/aaaabd85b36be1f435a59c4dc10b423cec063795))\n* correctly encrypt/hash uri systems that contain ~, -\n([#429](https://github.com/miracum/fhir-pseudonymizer/issues/429))\n([17c3fc1](https://github.com/miracum/fhir-pseudonymizer/commit/17c3fc12851a162ca1fc540c543f1ab87291e508))\n* default to PseudonymizationService: None instead of gPAS and remove\ngPAS version config\n([#431](https://github.com/miracum/fhir-pseudonymizer/issues/431))\n([18b8307](https://github.com/miracum/fhir-pseudonymizer/commit/18b83071ed78d2295d5d7cadfbad32abdc409363))\n* default to using the System.Text.Json-based FHIR serializer\n([#430](https://github.com/miracum/fhir-pseudonymizer/issues/430))\n([2056c17](https://github.com/miracum/fhir-pseudonymizer/commit/2056c1727ea4d54832639f3ce801166a056a14d1))\n* **kafka:** setting for crypto-hashing message keys\n([#439](https://github.com/miracum/fhir-pseudonymizer/issues/439))\n([7f86690](https://github.com/miracum/fhir-pseudonymizer/commit/7f86690df2fbc947c4e8643b0b54a455455b61b2))\n* moved chart from miracum/charts to this repo\n([#451](https://github.com/miracum/fhir-pseudonymizer/issues/451))\n([5bf6add](https://github.com/miracum/fhir-pseudonymizer/commit/5bf6add530cc2070f665f81127d4ebcf5dcc3fee))\n* moved metrics config inside a dedicated Metrics key\n([#432](https://github.com/miracum/fhir-pseudonymizer/issues/432))\n([746c042](https://github.com/miracum/fhir-pseudonymizer/commit/746c0423e5e75dc3136fb3f60752f3e8de035e83))\n* moved retry config into a dedicated config object\n([#435](https://github.com/miracum/fhir-pseudonymizer/issues/435))\n([3b1d60d](https://github.com/miracum/fhir-pseudonymizer/commit/3b1d60dc0ea1ad75d8c968a7b4e44b6be8520ae9))\n* removed default anonymization config\n([#448](https://github.com/miracum/fhir-pseudonymizer/issues/448))\n([b2c2e0c](https://github.com/miracum/fhir-pseudonymizer/commit/b2c2e0c6efc5c728d4f5def5e48951bbf9f399af))\n* replaced api key auth with JWT tokens\n([#449](https://github.com/miracum/fhir-pseudonymizer/issues/449))\n([ffa9c0a](https://github.com/miracum/fhir-pseudonymizer/commit/ffa9c0aa969133a21db5168adac376d7429330ec))\n* require at least 32-byte long keys\n([#433](https://github.com/miracum/fhir-pseudonymizer/issues/433))\n([46ea1a8](https://github.com/miracum/fhir-pseudonymizer/commit/46ea1a893420056675e749cc19c08c49191b279d))\n* updated to FHIR.NET v6\n([#373](https://github.com/miracum/fhir-pseudonymizer/issues/373))\n([7cf89e8](https://github.com/miracum/fhir-pseudonymizer/commit/7cf89e821dd53f32b914fe17fbdd33b4fd202aa6))\n\n\n### Bug Fixes\n\n* bubble up non-transient Vfps pseudonymization errors to the caller\n([4733fb5](https://github.com/miracum/fhir-pseudonymizer/commit/4733fb59fd90125b1cf0f7ac8ef652790594a286))\n* client-side gRPC load-balancing with more aggressive timeouts\n([#442](https://github.com/miracum/fhir-pseudonymizer/issues/442))\n([af1d47e](https://github.com/miracum/fhir-pseudonymizer/commit/af1d47eaa9b06463618b9827c58dde35b96e40d3))\n* dont write in-flight messages to the DLQ on shutdown exceptions\n([#437](https://github.com/miracum/fhir-pseudonymizer/issues/437))\n([6570162](https://github.com/miracum/fhir-pseudonymizer/commit/65701627506c2bcdcbf0922524a56913cd36c748))\n* **kafka:** same commit guarantees for provenance as for output\nmessages\n([#440](https://github.com/miracum/fhir-pseudonymizer/issues/440))\n([10852b2](https://github.com/miracum/fhir-pseudonymizer/commit/10852b245d9bbf95e0f794683f3a844273d55cc3))\n* support dynamic settings and config in $re-identify\n([#450](https://github.com/miracum/fhir-pseudonymizer/issues/450))\n([ec3d996](https://github.com/miracum/fhir-pseudonymizer/commit/ec3d9960787511782666a53dfdd69fc0ac5edb9b))\n\n\n### Performance Improvements\n\n* disable mvc model validation\n([#436](https://github.com/miracum/fhir-pseudonymizer/issues/436))\n([8274e8d](https://github.com/miracum/fhir-pseudonymizer/commit/8274e8d6ef2e574eefda2cc6d0560f7acb06dbfd))\n\n\n### Documentation\n\n* added a dedicated docs site\n([#446](https://github.com/miracum/fhir-pseudonymizer/issues/446))\n([655da66](https://github.com/miracum/fhir-pseudonymizer/commit/655da66b9d61c9ee1479bc4edc8b83ac2cf17433))\n* fixed outdated cryptohash documentation and re-prettier'd everything\n([#441](https://github.com/miracum/fhir-pseudonymizer/issues/441))\n([15d2cc6](https://github.com/miracum/fhir-pseudonymizer/commit/15d2cc678551c555aba52077cd69cd90284512eb))\n* minor docs updates\n([#447](https://github.com/miracum/fhir-pseudonymizer/issues/447))\n([4cbb4ea](https://github.com/miracum/fhir-pseudonymizer/commit/4cbb4ea4eae988bd451bb88a811bef4aa0ac57ce))\n* updated README benchmark numbers\n([ed0625f](https://github.com/miracum/fhir-pseudonymizer/commit/ed0625f7dbb23ef0d9b20bcbab02c38424bc73df))\n\n\n### Miscellaneous Chores\n\n* **release:** prepare v3 release\n([#434](https://github.com/miracum/fhir-pseudonymizer/issues/434))\n([36e9d6f](https://github.com/miracum/fhir-pseudonymizer/commit/36e9d6f28127ed6b060835a6e76cac28554caa52))\n\n\n### CI/CD\n\n* refactor vfps/argo workflow chart\n([#452](https://github.com/miracum/fhir-pseudonymizer/issues/452))\n([e64f081](https://github.com/miracum/fhir-pseudonymizer/commit/e64f081053b9b7bb0f18f4e342a5cc745a6a28b8))\n* replace deprecated slsa-verifier with gh cli\n([#443](https://github.com/miracum/fhir-pseudonymizer/issues/443))\n([c78a688](https://github.com/miracum/fhir-pseudonymizer/commit/c78a688bca65cd382ba264a37588851476600d07))\n\n---\nThis PR was generated with [Release\nPlease](https://github.com/googleapis/release-please). See\n[documentation](https://github.com/googleapis/release-please#release-please).",
          "timestamp": "2026-10-15T22:56:12+02:00",
          "tree_id": "711eae8c95d61714f32e37de02b3a40819bcad9a",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/bcad147536ee08e4bb32b694ce4b7ad07ac5e7c0"
        },
        "date": 1792097990272,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 119411217.22580644,
            "unit": "ns",
            "range": "± 3591673.100240766"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1904621.5229166667,
            "unit": "ns",
            "range": "± 16688.09798943521"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1699.9495233389048,
            "unit": "ns",
            "range": "± 2.4340439096692985"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23758724.05078125,
            "unit": "ns",
            "range": "± 463636.7045787903"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 351.81581881841026,
            "unit": "ns",
            "range": "± 1.8484826699055272"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 829763045,
            "unit": "ns",
            "range": "± 5561633.246431214"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1948688165,
            "unit": "ns",
            "range": "± 22190918.601519044"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 844293992.3333334,
            "unit": "ns",
            "range": "± 9482794.109561399"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 2043816180,
            "unit": "ns",
            "range": "± 7430733.566577865"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 844483006.6666666,
            "unit": "ns",
            "range": "± 9989009.42737899"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1529610167,
            "unit": "ns",
            "range": "± 8399786.278724238"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 368333229.6666667,
            "unit": "ns",
            "range": "± 103314509.24072778"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 558176076.3333334,
            "unit": "ns",
            "range": "± 238074397.00235394"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 393460854.6666667,
            "unit": "ns",
            "range": "± 76532748.73387767"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 602240218.3333334,
            "unit": "ns",
            "range": "± 214792586.24877274"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 374414267,
            "unit": "ns",
            "range": "± 107019666.03796908"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 512149425.3333333,
            "unit": "ns",
            "range": "± 267677170.26881394"
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
          "id": "1df0698d0150caf7f9d14d05b010e842138e80f4",
          "message": "chore(deps): update all digest updates (#455)\n\nThis PR contains the following updates:\n\n| Package | Type | Update | Change |\n|---|---|---|---|\n|\n[mcr.microsoft.com/dotnet/aspnet](https://redirect.github.com/dotnet/aspnetcore)\n| stage | digest | `99a0fe6` → `6512136` |\n| [mcr.microsoft.com/dotnet/sdk](https://redirect.github.com/dotnet/sdk)\n| final | digest | `d5355a3` → `d8051d8` |\n|\n[quay.io/keycloak/keycloak](https://redirect.github.com/keycloak-rel/keycloak-rel)\n| | digest | `ff4257d` → `29be725` |\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n👻 **Immortal**: This PR will be recreated if closed unmerged. Get\n[config\nhelp](https://redirect.github.com/renovatebot/renovate/discussions) if\nthat's undesired.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xNjQuMSIsInVwZGF0ZWRJblZlciI6IjQ0LjE2NC4xIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-15T21:14:57Z",
          "tree_id": "24eb9861d43e9913fd315fdfceebfe7a4b03aa1f",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/1df0698d0150caf7f9d14d05b010e842138e80f4"
        },
        "date": 1792099398757,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 87165822.47058824,
            "unit": "ns",
            "range": "± 1685099.199057789"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1404217.4291666667,
            "unit": "ns",
            "range": "± 25971.90208745699"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1249.9544792175293,
            "unit": "ns",
            "range": "± 1.7490761100000076"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 17368369.72906404,
            "unit": "ns",
            "range": "± 499146.9991416208"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 248.99589582851954,
            "unit": "ns",
            "range": "± 1.0246308766319931"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 646095624,
            "unit": "ns",
            "range": "± 15275598.343778322"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1513604251.3333333,
            "unit": "ns",
            "range": "± 8685677.093015624"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 642920794,
            "unit": "ns",
            "range": "± 16355333.499956643"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1581501920.3333333,
            "unit": "ns",
            "range": "± 12305701.0258163"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 659179722.3333334,
            "unit": "ns",
            "range": "± 2339440.7832142995"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1169485546.3333333,
            "unit": "ns",
            "range": "± 32215112.1820694"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 288444636,
            "unit": "ns",
            "range": "± 19096156.211817157"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 528005827.6666667,
            "unit": "ns",
            "range": "± 122760468.77058232"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 302007728.6666667,
            "unit": "ns",
            "range": "± 22717487.46728056"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 559203171.6666666,
            "unit": "ns",
            "range": "± 169456062.3890648"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 291617985.3333333,
            "unit": "ns",
            "range": "± 13350760.771214213"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 521894044,
            "unit": "ns",
            "range": "± 138457428.2015203"
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
          "id": "60ac2cc214954d20956ad30d35e6d68c3b3de856",
          "message": "chore(deps): update docker.io/mockserver/mockserver docker tag to v8 (#454)\n\nThis PR contains the following updates:\n\n| Package | Update | Change |\n|---|---|---|\n| docker.io/mockserver/mockserver | major | `7.6.0` → `8.0.0` |\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n🔕 **Ignore**: Close this PR and you won't be reminded about this update\nagain.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xNjQuMSIsInVwZGF0ZWRJblZlciI6IjQ0LjE2NC4xIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-15T21:15:26Z",
          "tree_id": "ebbb79c5f6330fad8c9c8fc52e1c97e952d9dcd7",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/60ac2cc214954d20956ad30d35e6d68c3b3de856"
        },
        "date": 1792099605300,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 105256845.13953489,
            "unit": "ns",
            "range": "± 3220361.6990193417"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1679148.8010416667,
            "unit": "ns",
            "range": "± 31043.649330769123"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1634.5242268488958,
            "unit": "ns",
            "range": "± 2.9279585489857842"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21219127.0875,
            "unit": "ns",
            "range": "± 620781.6703109411"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 315.56139625821794,
            "unit": "ns",
            "range": "± 0.316186071249959"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 747947045.3333334,
            "unit": "ns",
            "range": "± 1494254.9944672538"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1778368460,
            "unit": "ns",
            "range": "± 6191267.899370451"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 759587314,
            "unit": "ns",
            "range": "± 6018671.94841362"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1842848170,
            "unit": "ns",
            "range": "± 8402593.363133254"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 768591814.6666666,
            "unit": "ns",
            "range": "± 4696031.616256041"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1333885767.6666667,
            "unit": "ns",
            "range": "± 594236.3079923115"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 309194353.3333333,
            "unit": "ns",
            "range": "± 113863942.06482543"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 583242943,
            "unit": "ns",
            "range": "± 368168823.0606499"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 266996694.66666666,
            "unit": "ns",
            "range": "± 125739832.32049291"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 546766517.6666666,
            "unit": "ns",
            "range": "± 166698537.7897937"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 309166638.3333333,
            "unit": "ns",
            "range": "± 89216245.14786223"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 429330757,
            "unit": "ns",
            "range": "± 209834352.97739166"
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
          "id": "743dc66b94bca50a6c98582b2c956cf81a137903",
          "message": "chore(deps): update all non-major dependencies (#456)\n\n> [!NOTE]\n> This PR body was truncated due to platform limits.\n\nThis PR contains the following updates:\n\n| Package | Change |\n[Age](https://docs.renovatebot.com/merge-confidence/) |\n[Confidence](https://docs.renovatebot.com/merge-confidence/) | Type |\nUpdate | Pending |\n|---|---|---|---|---|---|---|\n|\n[Confluent.Kafka](https://redirect.github.com/confluentinc/confluent-kafka-dotnet)\n| `2.15.0` → `2.15.1` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Confluent.Kafka/2.15.1?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Confluent.Kafka/2.15.0/2.15.1?slim=true)\n| nuget | patch | `2.16.0` |\n|\n[Google.Protobuf](https://redirect.github.com/protocolbuffers/protobuf)\n| `3.36.0` → `3.36.2` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Google.Protobuf/3.36.2?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Google.Protobuf/3.36.0/3.36.2?slim=true)\n| nuget | patch | |\n| [Grpc.Net.Client](https://redirect.github.com/grpc/grpc-dotnet) |\n`2.83.0` → `2.84.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Grpc.Net.Client/2.84.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Grpc.Net.Client/2.83.0/2.84.0?slim=true)\n| nuget | minor | |\n| [Grpc.Net.ClientFactory](https://redirect.github.com/grpc/grpc-dotnet)\n| `2.83.0` → `2.84.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Grpc.Net.ClientFactory/2.84.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Grpc.Net.ClientFactory/2.83.0/2.84.0?slim=true)\n| nuget | minor | |\n| [Grpc.Tools](https://redirect.github.com/grpc/grpc) | `2.83.0` →\n`2.84.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Grpc.Tools/2.84.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Grpc.Tools/2.83.0/2.84.0?slim=true)\n| nuget | minor | |\n| [Hl7.Fhir.Base](https://redirect.github.com/FirelyTeam/firely-net-sdk)\n| `6.5.0` → `6.6.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Hl7.Fhir.Base/6.6.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Hl7.Fhir.Base/6.5.0/6.6.0?slim=true)\n| nuget | minor | `6.7.0` |\n| [Hl7.Fhir.R4](https://redirect.github.com/FirelyTeam/firely-net-sdk) |\n`6.5.0` → `6.6.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Hl7.Fhir.R4/6.6.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Hl7.Fhir.R4/6.5.0/6.6.0?slim=true)\n| nuget | minor | `6.7.0` |\n| [Microsoft.AspNetCore.Authentication.JwtBearer](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.11` →\n`10.0.12` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.AspNetCore.Authentication.JwtBearer/10.0.12?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.AspNetCore.Authentication.JwtBearer/10.0.11/10.0.12?slim=true)\n| nuget | patch | `10.0.13` |\n| [Microsoft.AspNetCore.Components.WebAssembly](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.11` →\n`10.0.12` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.AspNetCore.Components.WebAssembly/10.0.12?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.AspNetCore.Components.WebAssembly/10.0.11/10.0.12?slim=true)\n| nuget | patch | `10.0.13` |\n|\n[Microsoft.AspNetCore.Components.WebAssembly.DevServer](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.11` →\n`10.0.12` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.AspNetCore.Components.WebAssembly.DevServer/10.0.12?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.AspNetCore.Components.WebAssembly.DevServer/10.0.11/10.0.12?slim=true)\n| nuget | patch | `10.0.13` |\n| [Microsoft.AspNetCore.Mvc.Testing](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.11` →\n`10.0.12` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.AspNetCore.Mvc.Testing/10.0.12?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.AspNetCore.Mvc.Testing/10.0.11/10.0.12?slim=true)\n| nuget | patch | `10.0.13` |\n| [Microsoft.Extensions.Http.Polly](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.11` →\n`10.0.12` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.Extensions.Http.Polly/10.0.12?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.Extensions.Http.Polly/10.0.11/10.0.12?slim=true)\n| nuget | patch | `10.0.13` |\n| [Microsoft.NET.Test.Sdk](https://redirect.github.com/microsoft/vstest)\n| `18.9.0` → `18.10.1` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.NET.Test.Sdk/18.10.1?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.NET.Test.Sdk/18.9.0/18.10.1?slim=true)\n| nuget | minor | `18.11.0` |\n|\n[Microsoft.Testing.Extensions.CodeCoverage](https://redirect.github.com/microsoft/codecoverage)\n| `18.10.0` → `18.11.2` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.Testing.Extensions.CodeCoverage/18.11.2?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.Testing.Extensions.CodeCoverage/18.10.0/18.11.2?slim=true)\n| nuget | minor | `18.12.0` |\n| [OpenTelemetry](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet))\n| `1.18.0` → `1.19.1` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry/1.19.1?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry/1.18.0/1.19.1?slim=true)\n| nuget | minor | `1.20.0` |\n|\n[OpenTelemetry.Exporter.OpenTelemetryProtocol](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet))\n| `1.18.0` → `1.19.1` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Exporter.OpenTelemetryProtocol/1.19.1?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Exporter.OpenTelemetryProtocol/1.18.0/1.19.1?slim=true)\n| nuget | minor | `1.20.0` |\n| [OpenTelemetry.Extensions.Hosting](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet))\n| `1.18.0` → `1.19.1` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Extensions.Hosting/1.19.1?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Extensions.Hosting/1.18.0/1.19.1?slim=true)\n| nuget | minor | `1.20.0` |\n| [OpenTelemetry.Instrumentation.AspNetCore](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet-contrib))\n| `1.18.0` → `1.19.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Instrumentation.AspNetCore/1.19.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Instrumentation.AspNetCore/1.18.0/1.19.0?slim=true)\n| nuget | minor | `1.20.0` |\n| [OpenTelemetry.Instrumentation.Http](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet-contrib))\n| `1.18.0` → `1.19.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Instrumentation.Http/1.19.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Instrumentation.Http/1.18.0/1.19.0?slim=true)\n| nuget | minor | `1.20.0` |\n| [OpenTelemetry.Instrumentation.Runtime](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet-contrib))\n| `1.18.0` → `1.19.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Instrumentation.Runtime/1.19.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Instrumentation.Runtime/1.18.0/1.19.0?slim=true)\n| nuget | minor | `1.20.0` |\n| [Verify.XunitV3](https://redirect.github.com/VerifyTests/Verify) |\n`32.0.0` → `32.0.1` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Verify.XunitV3/32.0.1?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Verify.XunitV3/32.0.0/32.0.1?slim=true)\n| nuget | patch | |\n| docker.io/confluentinc/cp-kcat | `8.2.3` → `8.2.4` |\n![age](https://developer.mend.io/api/mc/badges/age/docker/docker.io%2fconfluentinc%2fcp-kcat/8.2.4?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/docker/docker.io%2fconfluentinc%2fcp-kcat/8.2.3/8.2.4?slim=true)\n| | patch | |\n| [dotnet-sdk](https://redirect.github.com/dotnet/sdk) | `10.0.400` →\n`10.0.401` |\n![age](https://developer.mend.io/api/mc/badges/age/dotnet-version/dotnet-sdk/10.0.401?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/dotnet-version/dotnet-sdk/10.0.400/10.0.401?slim=true)\n| dotnet-sdk | patch | `10.0.402` |\n| [ghcr.io/miracum/vfps](https://redirect.github.com/miracum/vfps) |\n`v1.13.2` → `v1.25.0` |\n![age](https://developer.mend.io/api/mc/badges/age/docker/ghcr.io%2fmiracum%2fvfps/v1.25.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/docker/ghcr.io%2fmiracum%2fvfps/v1.13.2/v1.25.0?slim=true)\n| | minor | |\n|\n[mcr.microsoft.com/dotnet/aspnet](https://redirect.github.com/dotnet/aspnetcore)\n| `10.0.12-resolute-chiseled` → `10.0.13-resolute-chiseled` |\n![age](https://developer.mend.io/api/mc/badges/age/docker/mcr.microsoft.com%2fdotnet%2faspnet/10.0.13?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/docker/mcr.microsoft.com%2fdotnet%2faspnet/10.0.12/10.0.13?slim=true)\n| stage | patch | |\n| [mcr.microsoft.com/dotnet/sdk](https://redirect.github.com/dotnet/sdk)\n| `10.0.401-resolute` → `10.0.402-resolute` |\n![age](https://developer.mend.io/api/mc/badges/age/docker/mcr.microsoft.com%2fdotnet%2fsdk/10.0.402?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/docker/mcr.microsoft.com%2fdotnet%2fsdk/10.0.401/10.0.402?slim=true)\n| final | patch | |\n|\n[quay.io/keycloak/keycloak](https://redirect.github.com/keycloak-rel/keycloak-rel)\n| `26.7.3` → `26.8.1` |\n![age](https://developer.mend.io/api/mc/badges/age/docker/quay.io%2fkeycloak%2fkeycloak/26.8.1?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/docker/quay.io%2fkeycloak%2fkeycloak/26.7.3/26.8.1?slim=true)\n| | minor | |\n|\n[verify.tool](https://redirect.github.com/patriksvensson/verify.terminal)\n| `0.9.1` → `0.9.2` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/verify.tool/0.9.2?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/verify.tool/0.9.1/0.9.2?slim=true)\n| nuget | patch | |\n| [xunit.v3.mtp-v2](https://redirect.github.com/xunit/xunit) | `4.0.0` →\n`4.0.1` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/xunit.v3.mtp-v2/4.0.1?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/xunit.v3.mtp-v2/4.0.0/4.0.1?slim=true)\n| nuget | patch | `4.0.2` |\n\n---\n\n### Release Notes\n\n<details>\n<summary>grpc/grpc-dotnet (Grpc.Net.Client)</summary>\n\n###\n[`v2.84.0`](https://redirect.github.com/grpc/grpc-dotnet/releases/tag/v2.84.0)\n\n##### What's Changed\n\n- Document complete release version updates by\n[@&#8203;JamesNK](https://redirect.github.com/JamesNK) in\n[#&#8203;2748](https://redirect.github.com/grpc/grpc-dotnet/pull/2748)\n- Bump axios from 1.16.0 to 1.18.1 in\n/testassets/InteropTestsGrpcWebWebsite/Tests by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2745](https://redirect.github.com/grpc/grpc-dotnet/pull/2745)\n- Bump fast-uri from 3.1.2 to 3.1.4 in /examples/Browser/Server/wwwroot\nby [@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2749](https://redirect.github.com/grpc/grpc-dotnet/pull/2749)\n- Avoid blocking SemaphoreSlim.Wait(0) on single-threaded browser WASM\nby [@&#8203;pavelsavara](https://redirect.github.com/pavelsavara) in\n[#&#8203;2756](https://redirect.github.com/grpc/grpc-dotnet/pull/2756)\n- Update package dependencies and examples by\n[@&#8203;JamesNK](https://redirect.github.com/JamesNK) in\n[#&#8203;2759](https://redirect.github.com/grpc/grpc-dotnet/pull/2759)\n- Bump fast-uri from 3.1.4 to 3.1.5 in /examples/Browser/Server/wwwroot\nby [@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2754](https://redirect.github.com/grpc/grpc-dotnet/pull/2754)\n- Bump brace-expansion in /testassets/InteropTestsGrpcWebWebsite/Tests\nby [@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2755](https://redirect.github.com/grpc/grpc-dotnet/pull/2755)\n- Bump ip-address from 10.2.0 to 10.5.0 in\n/testassets/InteropTestsGrpcWebWebsite/Tests by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2757](https://redirect.github.com/grpc/grpc-dotnet/pull/2757)\n- Stabilize unobserved exception retry test by\n[@&#8203;JamesNK](https://redirect.github.com/JamesNK) in\n[#&#8203;2758](https://redirect.github.com/grpc/grpc-dotnet/pull/2758)\n- Fix corrupted bytes sent after GOAWAY by\n[@&#8203;DeagleGross](https://redirect.github.com/DeagleGross) in\n[#&#8203;2766](https://redirect.github.com/grpc/grpc-dotnet/pull/2766)\n- Bump fast-uri from 3.1.5 to 3.1.7 in /examples/Browser/Server/wwwroot\nby [@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2761](https://redirect.github.com/grpc/grpc-dotnet/pull/2761)\n- Bump js-yaml in /testassets/InteropTestsGrpcWebWebsite/Tests by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2765](https://redirect.github.com/grpc/grpc-dotnet/pull/2765)\n- Bump joi from 17.13.4 to 17.13.7 in\n/testassets/InteropTestsGrpcWebWebsite/Tests by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2764](https://redirect.github.com/grpc/grpc-dotnet/pull/2764)\n- Bump baseline-browser-mapping from 2.9.19 to 2.11.21 in\n/examples/Browser/Server/wwwroot by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2763](https://redirect.github.com/grpc/grpc-dotnet/pull/2763)\n- Bump browserslist from 4.25.1 to 4.28.8 in\n/testassets/InteropTestsGrpcWebWebsite/Tests by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2762](https://redirect.github.com/grpc/grpc-dotnet/pull/2762)\n- Bump [@&#8203;babel/core](https://redirect.github.com/babel/core) from\n7.28.0 to 7.29.7 in /testassets/InteropTestsGrpcWebWebsite/Tests by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;2767](https://redirect.github.com/grpc/grpc-dotnet/pull/2767)\n- Update the versions for pre release. by\n[@&#8203;asheshvidyut](https://redirect.github.com/asheshvidyut) in\n[#&#8203;2768](https://redirect.github.com/grpc/grpc-dotnet/pull/2768)\n- Update version for 2.84.x-pre1 by\n[@&#8203;asheshvidyut](https://redirect.github.com/asheshvidyut) in\n[#&#8203;2769](https://redirect.github.com/grpc/grpc-dotnet/pull/2769)\n- Remove pre suffix for 2.84.0 stable release by\n[@&#8203;asheshvidyut](https://redirect.github.com/asheshvidyut) in\n[#&#8203;2770](https://redirect.github.com/grpc/grpc-dotnet/pull/2770)\n\n##### New Contributors\n\n- [@&#8203;pavelsavara](https://redirect.github.com/pavelsavara) made\ntheir first contribution in\n[#&#8203;2756](https://redirect.github.com/grpc/grpc-dotnet/pull/2756)\n- [@&#8203;DeagleGross](https://redirect.github.com/DeagleGross) made\ntheir first contribution in\n[#&#8203;2766](https://redirect.github.com/grpc/grpc-dotnet/pull/2766)\n\n**Full Changelog**:\n<https://github.com/grpc/grpc-dotnet/compare/v2.83.0...v2.84.0>\n\n</details>\n\n<details>\n<summary>FirelyTeam/firely-net-sdk (Hl7.Fhir.Base)</summary>\n\n###\n[`v6.6.0`](https://redirect.github.com/FirelyTeam/firely-net-sdk/releases/tag/v6.6.0):\n6.6.0\n\n#### Intro:\n\nThis release adds `net10.0` to the frameworks the SDK targets, next to\n`net8.0` and\n`netstandard2.1`. It also lets `FhirClient` send a resource directly as\nthe body of an operation\nrequest, extends the FHIRPath `htmlChecks()` function to string input,\nand makes the\n`SnapshotGenerator` report differential elements that are out of order.\nThere is one binary\nbreaking change: `ElementNavFhirExtensions.HtmlChecks(PocoNode)` now\nreturns `bool?` instead of\n`bool`. Code that calls it needs to be recompiled. See the FHIRPath\nnotes below.\n\n**Platform support**\n\n- The SDK now also targets `net10.0`, next to `net8.0` and\n`netstandard2.1`. This is additive: .NET 8\nand .NET 9 applications keep resolving to the `net8.0` assets. On .NET\n10, class mapping lookups\nby name through `ModelInspector.FindClassMapping(ReadOnlySpan<char>)` no\nlonger allocate.\n- `net8.0` remains supported until the next major version (SDK 7), even\nafter Microsoft ends support\n  for .NET 8 in November 2026.\n- **Upcoming: `netstandard2.1` will be removed** in a later minor\nrelease, once Unity 6.8 - which\nreplaces Mono with CoreCLR and .NET 10 - has shipped. Its only remaining\naudience is Unity, and\nUnity 6.8 can consume the `net10.0` assets. If you depend on the\n`netstandard2.1` assets for\n  another platform, please let us know.\n\n**FhirClient**\n\n- Operations with exactly one resource input can now send that resource\ndirectly as the POST body,\ninstead of wrapping it in a `Parameters` resource.\n`WholeSystemOperationAsync`,\n`TypeOperationAsync`, `InstanceOperationAsync` and `OperationAsync` have\nnew overloads that take a\n`Resource`, and `TransactionBuilder` has matching overloads for its\noperation entries. The\nexisting `Parameters` overloads are unchanged. See issue\n[#&#8203;3599](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3599).\n\n**FHIRPath**\n\n- `htmlChecks()` can now be invoked on a `string`. Its contents are\nvalidated as the content of a\nnarrative `div`, following the current FHIRPath build\n([FHIR-56303](https://jira.hl7.org/browse/FHIR-56303)).\nInvoked on anything other than `xhtml` or a `string`, it now returns\nempty instead of `false`. To\nsupport this, `ElementNavFhirExtensions.HtmlChecks(PocoNode)` returns\n`bool?` instead of `bool`,\nwhich is a binary breaking change for code that calls it directly. See\nissue\n[#&#8203;3604](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3604).\n\n**Snapshot generation**\n\n- The `SnapshotGenerator` now reports a specific issue\n(`PROFILE_ELEMENTDEF_INVALID_ELEMENT_ORDER`,\ncode 10020) when a differential element is out of order, i.e. when it\nconstrains a base element\nthat precedes a base element already matched by an earlier differential\nelement. The spec\nrequires `differential.element` and `snapshot.element` to follow the\norder of the base\ndefinition. Such elements could previously not be matched and were\nsilently treated as new\nelements, which surfaced downstream as a confusing error. The generator\ndoes not reorder the\ndifferential. See issue\n[#&#8203;3600](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3600).\n\n**Dependencies**\n\n- The `System.Reflection.Emit.Lightweight` and `System.Buffers` package\ndependencies were removed;\n  both are part of every framework the SDK targets.\n- Updated Microsoft.SourceLink.GitHub to 10.0.401. MSTest (4.4.1) and\nVerify.MSTest (33.1.5) were\n  updated too, but are test-only and not part of the shipped packages.\n\n#### Changes:\n\n-\n[#&#8203;3612](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3612):\nVONK-10067: Add net10.0 target framework and convert to slnx\n-\n[#&#8203;3614](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3614):\nBump the runtime-dependencies-minor group with 1 update\n-\n[#&#8203;3609](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3609):\nBump the runtime-dependencies-minor group with 2 updates\n-\n[#&#8203;3601](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3601):\nReport a clear issue for out-of-order differential elements in\nSnapshotGenerator\n-\n[#&#8203;3602](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3602):\nSupport resource-body operation overloads\n-\n[#&#8203;3605](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3605):\nSupport FHIRPath htmlChecks() on string input\n-\n[#&#8203;3608](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3608):\nBump Microsoft.SourceLink.GitHub from 10.0.400 to 10.0.401\n-\n[#&#8203;3607](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3607):\nBump MSTest.TestFramework and Verify.MSTest\n-\n[#&#8203;3603](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3603):\nRelease 6.5.0\n-\n[#&#8203;3600](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3600):\nSnapshotGenerator doesn't sort out-of-order differential/snapshot\nelements (unlike HAPI's Java validator)\n\nThis list of changes was [auto\ngenerated](https://dev.azure.com/firely/firely-net-sdk/_build/results?buildId=81699\\&view=logs).\n\n</details>\n\n<details>\n<summary>dotnet/dotnet\n(Microsoft.AspNetCore.Authentication.JwtBearer)</summary>\n\n###\n[`v10.0.12`](https://redirect.github.com/dotnet/dotnet/compare/v10.0.11...v10.0.12)\n\n</details>\n\n<details>\n<summary>microsoft/vstest (Microsoft.NET.Test.Sdk)</summary>\n\n###\n[`v18.10.1`](https://redirect.github.com/microsoft/vstest/releases/tag/v18.10.1)\n\n#### What's Changed\n\n- Isolate code coverage files in published output by\n[@&#8203;azat-msft](https://redirect.github.com/azat-msft) in\n[#&#8203;16476](https://redirect.github.com/microsoft/vstest/pull/16476)\n\n**Full Changelog**:\n<https://github.com/microsoft/vstest/compare/v18.10.0...v18.10.1>\n\n###\n[`v18.10.0`](https://redirect.github.com/microsoft/vstest/releases/tag/v18.10.0)\n\n#### What's Changed\n\n- Drop Mono fallback, run .NET Framework tests on Windows only by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16158](https://redirect.github.com/microsoft/vstest/pull/16158)\n- Run Microsoft.Testing.Platform test apps under `vstest.console` and\ndatacollector by [@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16201](https://redirect.github.com/microsoft/vstest/pull/16201)\n- Fix test output eaten by MSBuild terminal logger by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16223](https://redirect.github.com/microsoft/vstest/pull/16223)\n- Remove the experimental test session feature by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16231](https://redirect.github.com/microsoft/vstest/pull/16231)\n- Skip a single bad executor instead of failing all executor loading by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16239](https://redirect.github.com/microsoft/vstest/pull/16239)\n- Assert apartment state instead of using Clipboard in UI tests by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16270](https://redirect.github.com/microsoft/vstest/pull/16270)\n- Surface test host crashes during protocol negotiation by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16285](https://redirect.github.com/microsoft/vstest/pull/16285)\n- Pass the inferred target platform to the host in run settings by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16271](https://redirect.github.com/microsoft/vstest/pull/16271)\n- Report raw invalid IsTargetPlatformInferred value, cover host x64\nforcing by [@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16295](https://redirect.github.com/microsoft/vstest/pull/16295)\n- Disable the MTP testhost by default\n([#&#8203;16337](https://redirect.github.com/microsoft/vstest/issues/16337))\nby [@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16341](https://redirect.github.com/microsoft/vstest/pull/16341)\n\n**Full Changelog**:\n<https://github.com/microsoft/vstest/compare/v18.9.0...v18.10.0>\n\n</details>\n\n<details>\n<summary>open-telemetry/opentelemetry-dotnet (OpenTelemetry)</summary>\n\n###\n[`v1.19.1`](https://redirect.github.com/open-telemetry/opentelemetry-dotnet/blob/HEAD/RELEASENOTES.md#1191)\n\nRelease details:\n[1.19.1](https://redirect.github.com/open-telemetry/opentelemetry-dotnet/releases/tag/core-1.19.1)\n\n- Fixed `NotSupportedException` thrown when building a `TracerProvider`\nor\n`MeterProvider` with a large number of wildcard source/meter patterns on\n`net8.0`, and a related `OutOfMemoryException` that could occur when\nmany\n  such providers were built over the lifetime of a process.\n\n###\n[`v1.19.0`](https://redirect.github.com/open-telemetry/opentelemetry-dotnet/blob/HEAD/RELEASENOTES.md#1190)\n\nRelease details:\n[1.19.0](https://redirect.github.com/open-telemetry/opentelemetry-dotnet/releases/tag/core-1.19.0)\n\n- Added Schema URL to internally created `Resource` instances.\n- Added `AlwaysRecordSampler`.\n- Added an `AddOpenTelemetry` extension method for\n`IHostApplicationBuilder`. It\nregisters the OpenTelemetry SDK services and additionally seeds\n`service.name`\nfrom `IHostEnvironment.ApplicationName` and\n`deployment.environment.name` from\n`IHostEnvironment.EnvironmentName` as low-priority resource defaults. It\nalso\nregisters the host's live configuration into DI so that extensions\nreceiving\nonly `IServiceCollection` can contribute configuration sources during\nsetup.\n- Extended key/value list attribute handling to cover additional\ndictionary\nshapes (`IEnumerable<KeyValuePair<string, string?>>` and `IDictionary`).\nThese\nattributes will be serialized as JSON objects or `kvlist`, as\nappropriate.\n- If an exception is thrown when serializing an attribute, the attribute\nwill now\n  be dropped.\n- Disable HttpClientFactory integration on browser WebAssembly (e.g.\nBlazor)\nenvironments to avoid stalled export requests when async HTTP handlers\nare used.\n- Fixed CA certificate loading (e.g. using\n`OTEL_EXPORTER_OTLP_CERTIFICATE`) for\n  PEM-encoded certificates that only contain a public key.\n- Fixed `CircularBufferBuckets` so the first delta histogram insertion\nafter a\n  reset does not result in an unnecessary scale reduction.\n- Fixed lazy logger provider builds after a failure from reusing\npartially\n  initialized provider state.\n- Fixed parsing of an inbound `tracestate` header whose member value\ntrimmed to\nan empty value that previously threw an `IndexOutOfRangeException`\ninternally\n  and could silently truncate the tracestate.\n- Prevented oversized metric metadata from being cached when it cannot\nfit within\n  the configured request size.\n- Reduced allocations when formatting self-diagnostics events with up to\nthree parameters.\n- Improved wildcard source/meter name matching to avoid excessive\nmatching time\n  at runtime.\n- Reduced allocations when creating log record attributes from an array.\n- Reduced allocations when setting baggage through the `params`\noverload.\n- Reduced allocations when constructing `SpanAttributes` from an array.\n\n</details>\n\n<details>\n<summary>dotnet/sdk (dotnet-sdk)</summary>\n\n###\n[`v10.0.401`](https://redirect.github.com/dotnet/sdk/releases/tag/v10.0.401):\n.NET 10.0.12\n\n[Compare\nSource](https://redirect.github.com/dotnet/sdk/compare/v10.0.400...v10.0.401)\n\n[Release](https://redirect.github.com/dotnet/core/releases/tag/v10.0.12)\n\n#### What's Changed\n\n- Fix 2xx builds by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51118](https://redirect.github.com/dotnet/sdk/pull/51118)\n- Handle hardlinks with file-based app csc optimization by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51135](https://redirect.github.com/dotnet/sdk/pull/51135)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51187](https://redirect.github.com/dotnet/sdk/pull/51187)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51217](https://redirect.github.com/dotnet/sdk/pull/51217)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51225](https://redirect.github.com/dotnet/sdk/pull/51225)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51231](https://redirect.github.com/dotnet/sdk/pull/51231)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51239](https://redirect.github.com/dotnet/sdk/pull/51239)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51262](https://redirect.github.com/dotnet/sdk/pull/51262)\n- Enable CI builds for 2xx branch by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;50985](https://redirect.github.com/dotnet/sdk/pull/50985)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51312](https://redirect.github.com/dotnet/sdk/pull/51312)\n- Improve progress output, rebuild projects serially by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51227](https://redirect.github.com/dotnet/sdk/pull/51227)\n- Clean up browser communication protocol by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51226](https://redirect.github.com/dotnet/sdk/pull/51226)\n- Convert Middleware and Agent.Host packages to source packages by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51335](https://redirect.github.com/dotnet/sdk/pull/51335)\n- Fix `dotnet new` to suggest `@` instead of deprecated `::` separator\nin update notifications by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51153](https://redirect.github.com/dotnet/sdk/pull/51153)\n- Temporarily revert \"Clean up browser communication protocol\n([#&#8203;51226](https://redirect.github.com/dotnet/sdk/issues/51226))\"\nby [@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51344](https://redirect.github.com/dotnet/sdk/pull/51344)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51330](https://redirect.github.com/dotnet/sdk/pull/51330)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51387](https://redirect.github.com/dotnet/sdk/pull/51387)\n- Update --project/--solution descriptions for dotnet test (MTP) by\n[@&#8203;Youssef1313](https://redirect.github.com/Youssef1313) in\n[#&#8203;51371](https://redirect.github.com/dotnet/sdk/pull/51371)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51403](https://redirect.github.com/dotnet/sdk/pull/51403)\n- Enable file-based user-secrets tests by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51180](https://redirect.github.com/dotnet/sdk/pull/51180)\n- Remove no longer necessary Roslyn lexer experimental suppression by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51410](https://redirect.github.com/dotnet/sdk/pull/51410)\n- Update implicit versions for October by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;51341](https://redirect.github.com/dotnet/sdk/pull/51341)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51418](https://redirect.github.com/dotnet/sdk/pull/51418)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51426](https://redirect.github.com/dotnet/sdk/pull/51426)\n- \\[release/10.0.2xx] Fix dotnet run to accept Windows path separators\non Linux by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51436](https://redirect.github.com/dotnet/sdk/pull/51436)\n- Exclude PublishDir from default items to prevent recursive artifacts\nin file-based apps by\n[@&#8203;baronfel](https://redirect.github.com/baronfel) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51417](https://redirect.github.com/dotnet/sdk/pull/51417)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51440](https://redirect.github.com/dotnet/sdk/pull/51440)\n- Update versions .props based on the RC2 build by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;51414](https://redirect.github.com/dotnet/sdk/pull/51414)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51479](https://redirect.github.com/dotnet/sdk/pull/51479)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51490](https://redirect.github.com/dotnet/sdk/pull/51490)\n- \\[release/10.0.2xx] Fix CA1869 false positive in top-level statements\nby\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51483](https://redirect.github.com/dotnet/sdk/pull/51483)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51503](https://redirect.github.com/dotnet/sdk/pull/51503)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51517](https://redirect.github.com/dotnet/sdk/pull/51517)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51523](https://redirect.github.com/dotnet/sdk/pull/51523)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51539](https://redirect.github.com/dotnet/sdk/pull/51539)\n- \\[release/10.0.2xx] Manually set source tag to a 10.0.2xx build by\n[@&#8203;dkurepa](https://redirect.github.com/dkurepa) in\n[#&#8203;51541](https://redirect.github.com/dotnet/sdk/pull/51541)\n- \\[release/10.0.2xx] Remove legacy console template comment from .NET\n10 console template by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51537](https://redirect.github.com/dotnet/sdk/pull/51537)\n- Update RulesMissingDocumentation.md by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51543](https://redirect.github.com/dotnet/sdk/pull/51543)\n- Remove analyzer redirecting DLL by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51511](https://redirect.github.com/dotnet/sdk/pull/51511)\n- Add source package for file-based programs support by\n[@&#8203;RikkiGibson](https://redirect.github.com/RikkiGibson) in\n[#&#8203;51373](https://redirect.github.com/dotnet/sdk/pull/51373)\n- Accept both projects and solutions in 'dotnet test --project' (MTP) by\n[@&#8203;Youssef1313](https://redirect.github.com/Youssef1313) in\n[#&#8203;51493](https://redirect.github.com/dotnet/sdk/pull/51493)\n- Add IsShipping property to FileBasedPrograms project file by\n[@&#8203;RikkiGibson](https://redirect.github.com/RikkiGibson) in\n[#&#8203;51581](https://redirect.github.com/dotnet/sdk/pull/51581)\n- \\[release/10.0.2xx] Remove razor_tooling from format example ci test\nby\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51573](https://redirect.github.com/dotnet/sdk/pull/51573)\n- Include FileBasedPrograms package project in the solution by\n[@&#8203;RikkiGibson](https://redirect.github.com/RikkiGibson) in\n[#&#8203;51587](https://redirect.github.com/dotnet/sdk/pull/51587)\n- Set EnableDefaultEmbeddedResourceItems and EnableDefaultNoneItems to\nfalse for file-based apps using default SDK by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51542](https://redirect.github.com/dotnet/sdk/pull/51542)\n- Include EmbeddedResources in Microsoft.DotNet.HotReload.Web.Middleware\npackage by [@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51595](https://redirect.github.com/dotnet/sdk/pull/51595)\n- \\[release/10.0.2xx] Make S.CL builder DSLs more transferable\n([#&#8203;50685](https://redirect.github.com/dotnet/sdk/issues/50685))\nby [@&#8203;baronfel](https://redirect.github.com/baronfel) in\n[#&#8203;51622](https://redirect.github.com/dotnet/sdk/pull/51622)\n- Ensure that `--nologo` adheres to DOTNET_NOLOGO if present by\n[@&#8203;baronfel](https://redirect.github.com/baronfel) in\n[#&#8203;51462](https://redirect.github.com/dotnet/sdk/pull/51462)\n- Revert \"Temporarily revert \"Clean up browser communication protocol\n([#&#8203;51226](https://redirect.github.com/dotnet/sdk/issues/51226))\"\"\nby [@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51571](https://redirect.github.com/dotnet/sdk/pull/51571)\n- Reuse ProjectCollection across DTBs by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51620](https://redirect.github.com/dotnet/sdk/pull/51620)\n- \\[release/10.0.2xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51612](https://redirect.github.com/dotnet/sdk/pull/51612)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51567](https://redirect.github.com/dotnet/sdk/pull/51567)\n- Improve clarity of 'dotnet project convert' output directory prompt by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51443](https://redirect.github.com/dotnet/sdk/pull/51443)\n- Refactor dotnet-watch into a package consumable from Aspire.CLI by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51223](https://redirect.github.com/dotnet/sdk/pull/51223)\n- Remove more analyzer redirecting VSIX related stuff by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51692](https://redirect.github.com/dotnet/sdk/pull/51692)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51709](https://redirect.github.com/dotnet/sdk/pull/51709)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51442](https://redirect.github.com/dotnet/sdk/pull/51442)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51732](https://redirect.github.com/dotnet/sdk/pull/51732)\n- Support variables in `#:project` directives by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51108](https://redirect.github.com/dotnet/sdk/pull/51108)\n- \\[release/10.0.2xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51733](https://redirect.github.com/dotnet/sdk/pull/51733)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51758](https://redirect.github.com/dotnet/sdk/pull/51758)\n- \\[release/10.0.2xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51760](https://redirect.github.com/dotnet/sdk/pull/51760)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51724](https://redirect.github.com/dotnet/sdk/pull/51724)\n- Fix solution parsing in dotnet test for Microsoft.Testing.Platform by\n[@&#8203;Youssef1313](https://redirect.github.com/Youssef1313) in\n[#&#8203;51411](https://redirect.github.com/dotnet/sdk/pull/51411)\n- Update MSTest templates to version 4.0.2 by\n[@&#8203;Youssef1313](https://redirect.github.com/Youssef1313) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51762](https://redirect.github.com/dotnet/sdk/pull/51762)\n- Separate 'test' command definitions from implementations by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51781](https://redirect.github.com/dotnet/sdk/pull/51781)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51798](https://redirect.github.com/dotnet/sdk/pull/51798)\n- Add deprecation warning NETSDK1234 for RoslynCompilerType=Framework by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51723](https://redirect.github.com/dotnet/sdk/pull/51723)\n- Fix test duration display to not show \"(0ms)\" when duration is\nunavailable by\n[@&#8203;Youssef1313](https://redirect.github.com/Youssef1313) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51792](https://redirect.github.com/dotnet/sdk/pull/51792)\n- \\[release/10.0.2xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51801](https://redirect.github.com/dotnet/sdk/pull/51801)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51812](https://redirect.github.com/dotnet/sdk/pull/51812)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51849](https://redirect.github.com/dotnet/sdk/pull/51849)\n- Hot Reload: Do not add empty div when diagnostics are empty by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51836](https://redirect.github.com/dotnet/sdk/pull/51836)\n- Update implicit versions for the November release by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;51652](https://redirect.github.com/dotnet/sdk/pull/51652)\n- Template Engine CLI commands: separate definitions from\nimplementations by [@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51747](https://redirect.github.com/dotnet/sdk/pull/51747)\n- Update to the RTM build of runtime. by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;51660](https://redirect.github.com/dotnet/sdk/pull/51660)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51817](https://redirect.github.com/dotnet/sdk/pull/51817)\n- Fix CS2002 when EnableDefaultCompileItems=true in file-based apps by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51790](https://redirect.github.com/dotnet/sdk/pull/51790)\n- \\[release/10.0.2xx] \\[dotnet-cli] prompt for target framework using\n`Spectre.Console` by\n[@&#8203;jonathanpeppers](https://redirect.github.com/jonathanpeppers)\nin [#&#8203;51859](https://redirect.github.com/dotnet/sdk/pull/51859)\n- Fix slnf test assets to use `\\\\` by\n[@&#8203;Youssef1313](https://redirect.github.com/Youssef1313) in\n[#&#8203;51791](https://redirect.github.com/dotnet/sdk/pull/51791)\n- Mention deprecation of RoslynCompilerType=Framework in the docs by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51870](https://redirect.github.com/dotnet/sdk/pull/51870)\n- Add Microsoft.DotNet.ProjectTools project by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51788](https://redirect.github.com/dotnet/sdk/pull/51788)\n- Cleanup test command by\n[@&#8203;Youssef1313](https://redirect.github.com/Youssef1313) in\n[#&#8203;51883](https://redirect.github.com/dotnet/sdk/pull/51883)\n- Upgrade Spectre.Console to version 0.54.0 by\n[@&#8203;zivkan](https://redirect.github.com/zivkan) in\n[#&#8203;51933](https://redirect.github.com/dotnet/sdk/pull/51933)\n- Fix working directory of simple file-based apps by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51828](https://redirect.github.com/dotnet/sdk/pull/51828)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51858](https://redirect.github.com/dotnet/sdk/pull/51858)\n- Handle empty string returned from `Environment.GetFolderPath` by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51854](https://redirect.github.com/dotnet/sdk/pull/51854)\n- \\[dotnet test]\\[MTP] Avoid surfacing `MissingTestSessionEnd` when the\nexit code is already non-success by\n[@&#8203;Youssef1313](https://redirect.github.com/Youssef1313) in\n[#&#8203;51857](https://redirect.github.com/dotnet/sdk/pull/51857)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;51989](https://redirect.github.com/dotnet/sdk/pull/51989)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52001](https://redirect.github.com/dotnet/sdk/pull/52001)\n- Fix process cleanup timeout causing port binding errors in\nno-hot-reload mode by [@&#8203;tmat](https://redirect.github.com/tmat)\nwith [@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51632](https://redirect.github.com/dotnet/sdk/pull/51632)\n- Move Microsoft.DotNet.FileBasedPrograms source package reference to\nProjectTools by [@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51995](https://redirect.github.com/dotnet/sdk/pull/51995)\n- \\[Backport]\\[10.0.2xx] Add suppressor support for `dotnet format` by\n[@&#8203;JoeRobich](https://redirect.github.com/JoeRobich) in\n[#&#8203;51697](https://redirect.github.com/dotnet/sdk/pull/51697)\n- Separate command parsing from implementation to reduce dotnet-watch\ndependencies by [@&#8203;tmat](https://redirect.github.com/tmat) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51624](https://redirect.github.com/dotnet/sdk/pull/51624)\n- Add missing details to run-file spec by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52085](https://redirect.github.com/dotnet/sdk/pull/52085)\n- \\[dotnet-watch] Include AddExplicitInterfaceImplementation capability\nby [@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;52069](https://redirect.github.com/dotnet/sdk/pull/52069)\n- Avoid discarding file-like arguments in `dotnet file.cs` invocations\nby [@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52110](https://redirect.github.com/dotnet/sdk/pull/52110)\n- Fix dotnet-run-file up-to-date checks across symlinks by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52064](https://redirect.github.com/dotnet/sdk/pull/52064)\n- Skip up-to-date optimization in dotnet-run-file when there are\n`#:project` directives by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52062](https://redirect.github.com/dotnet/sdk/pull/52062)\n- create a tasks-focused solution filter, and make repo tests more\nusable in VSCode by\n[@&#8203;baronfel](https://redirect.github.com/baronfel) in\n[#&#8203;52072](https://redirect.github.com/dotnet/sdk/pull/52072)\n- Add Executable launch profile support to dotnet run and dotnet watch\nby [@&#8203;baronfel](https://redirect.github.com/baronfel) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51727](https://redirect.github.com/dotnet/sdk/pull/51727)\n- Virtual project builder by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;52030](https://redirect.github.com/dotnet/sdk/pull/52030)\n- Clarify some aspects of `dotnet run -` by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52132](https://redirect.github.com/dotnet/sdk/pull/52132)\n- \\[release/10.0.2xx] Preserve file encoding in file-based app source\nfiles by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52159](https://redirect.github.com/dotnet/sdk/pull/52159)\n- \\[release/10.0.2xx] Switch onelocbuild to target release/10.0.2xx\nbranch by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52165](https://redirect.github.com/dotnet/sdk/pull/52165)\n- \\[dotnet-watch] Simplify logging by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;51307](https://redirect.github.com/dotnet/sdk/pull/51307)\n- Update to the 10.0.1 dotnet/dotnet build by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;52141](https://redirect.github.com/dotnet/sdk/pull/52141)\n- Update minimum MSBuild version to 18.0.0 by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;52219](https://redirect.github.com/dotnet/sdk/pull/52219)\n- \\[template engine cli] Remove dependency on filter definitions from\ncommand definitions by [@&#8203;tmat](https://redirect.github.com/tmat)\nin [#&#8203;52124](https://redirect.github.com/dotnet/sdk/pull/52124)\n- Completion by [@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;52137](https://redirect.github.com/dotnet/sdk/pull/52137)\n- Recursively add SetAction to dotnet nuget subcommands by\n[@&#8203;martinrrm](https://redirect.github.com/martinrrm) in\n[#&#8203;52270](https://redirect.github.com/dotnet/sdk/pull/52270)\n- Remove remaining usage of StyleCop by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;52255](https://redirect.github.com/dotnet/sdk/pull/52255)\n- Refactor Tool and Workload command subcommands - separate parsing and\naction logic by [@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;52162](https://redirect.github.com/dotnet/sdk/pull/52162)\n- Update osx arm64 helix images by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52327](https://redirect.github.com/dotnet/sdk/pull/52327)\n- Fix interaction of \"csc only after msbuild\" optimization and implicit\nbuild files by [@&#8203;jjonescz](https://redirect.github.com/jjonescz)\nin [#&#8203;52133](https://redirect.github.com/dotnet/sdk/pull/52133)\n- Skip obj→bin copy on build failure in CSharpCompilerCommand by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51614](https://redirect.github.com/dotnet/sdk/pull/51614)\n- MSBuild-forwarding commands refactoring by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;52315](https://redirect.github.com/dotnet/sdk/pull/52315)\n- \\[release/10.0.2xx] Update ManagedWinRTComponentCanBeReferenced test\nto Windows SDK 10.0.26100.0 by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52357](https://redirect.github.com/dotnet/sdk/pull/52357)\n- Fix CS0006 errors when NuGet cache cleared during file-based app\nrecompilation by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;51609](https://redirect.github.com/dotnet/sdk/pull/51609)\n- Improve build task tests by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;51192](https://redirect.github.com/dotnet/sdk/pull/51192)\n- \\[release/10.0.2xx] \\[dotnet-watch] avoid\n`PlatformNotSupportedException` on various platforms by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52355](https://redirect.github.com/dotnet/sdk/pull/52355)\n- Fix build of multi-targeted test projects by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;52254](https://redirect.github.com/dotnet/sdk/pull/52254)\n- Localized file check-in by OneLocBuild Task: Build definition ID 140:\nBuild ID\n[`2873125`](https://redirect.github.com/dotnet/sdk/commit/2873125) by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;52171](https://redirect.github.com/dotnet/sdk/pull/52171)\n- Remaining commands by [@&#8203;tmat](https://redirect.github.com/tmat)\nin [#&#8203;52352](https://redirect.github.com/dotnet/sdk/pull/52352)\n- \\[release/10.0.2xx] Fix CA1859 improperly handling default interface\nimplementations by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52320](https://redirect.github.com/dotnet/sdk/pull/52320)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52022](https://redirect.github.com/dotnet/sdk/pull/52022)\n- Include all file paths in analyzer redirecting metadata file by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52326](https://redirect.github.com/dotnet/sdk/pull/52326)\n- \\[release/10.0.2xx] Add support for IfDifferent by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52275](https://redirect.github.com/dotnet/sdk/pull/52275)\n- \\[HotReloadAgent] fix possible `NullReferenceException` by\n[@&#8203;jonathanpeppers](https://redirect.github.com/jonathanpeppers)\nin [#&#8203;52367](https://redirect.github.com/dotnet/sdk/pull/52367)\n- \\[dotnet-watch ] Improves option forwarding by\n[@&#8203;tmat](https://redirect.github.com/tmat) in\n[#&#8203;52387](https://redirect.github.com/dotnet/sdk/pull/52387)\n- release/10.0.2xx - Ensure that Trimming and AOT scenarios trigger\ndownloading of necessary runtime packs by\n[@&#8203;dsplaisted](https://redirect.github.com/dsplaisted) in\n[#&#8203;52404](https://redirect.github.com/dotnet/sdk/pull/52404)\n- Re-enable compiler toolset tests by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52401](https://redirect.github.com/dotnet/sdk/pull/52401)\n- \\[release/10.0.2xx] Fix a file-based app package test by\n[@&#8203;jjonescz](https://redirect.github.com/jjonescz) in\n[#&#8203;52451](https://redirect.github.com/dotnet/sdk/pull/52451)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;51895](https://redirect.github.com/dotnet/sdk/pull/51895)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52377](https://redirect.github.com/dotnet/sdk/pull/52377)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52491](https://redirect.github.com/dotnet/sdk/pull/52491)\n- Localized file check-in by OneLocBuild Task: Build definition ID 140:\nBuild ID\n[`2879817`](https://redirect.github.com/dotnet/sdk/commit/2879817) by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;52479](https://redirect.github.com/dotnet/sdk/pull/52479)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;52501](https://redirect.github.com/dotnet/sdk/pull/52501)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.2xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52534](https://redirect.github.com/dotnet/sdk/pull/52534)\n- \\[release/10.0.2xx] Update dependencies from microsoft/testfx by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;52522](https://redirect.github.com/dotnet/sdk/pull/52522)\n- \\[release/10.0.2xx] Adjust MTP exit code logic by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52543](https://redirect.github.com/dotnet/sdk/pull/52543)\n- \\[release/10.0.2xx] Handle assemblies gracefully by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52508](https://redirect.github.com/dotnet/sdk/pull/52508)\n- \\[release/10.0.2xx] Refactor ANSI handling for better readability by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;52548](https://redirect.github.com/dotnet/sdk/pull/52548)\n- \\[release/10.0.2xx] \\[dotnet test MTP]: Only use test progress when\nANSI terminal by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin\n\n> [!IMPORTANT]\n> ✂ PR body was truncated to here.\n\n\n</details>\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n👻 **Immortal**: This PR will be recreated if closed unmerged. Get\n[config\nhelp](https://redirect.github.com/renovatebot/renovate/discussions) if\nthat's undesired.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xNjQuMSIsInVwZGF0ZWRJblZlciI6IjQ0LjE2NC4xIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-15T21:26:30Z",
          "tree_id": "d302ff68e77db09f469289cd8d0b5f31bc82b741",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/743dc66b94bca50a6c98582b2c956cf81a137903"
        },
        "date": 1792100051190,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 57374669.33333333,
            "unit": "ns",
            "range": "± 834520.038087732"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 788012.30234375,
            "unit": "ns",
            "range": "± 8376.869611221658"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 983.714548928397,
            "unit": "ns",
            "range": "± 13.603620549217924"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 10202675.465909092,
            "unit": "ns",
            "range": "± 240777.91020754262"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 211.26027512550354,
            "unit": "ns",
            "range": "± 1.529796733287617"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 477531603.3333333,
            "unit": "ns",
            "range": "± 83700344.66975315"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1032121125.6666666,
            "unit": "ns",
            "range": "± 49859916.47358949"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 475050724.6666667,
            "unit": "ns",
            "range": "± 84817684.01594514"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1102733714,
            "unit": "ns",
            "range": "± 14958870.136301639"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 478166265.6666667,
            "unit": "ns",
            "range": "± 77899674.09403707"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 756224764.6666666,
            "unit": "ns",
            "range": "± 67928104.66912328"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 197596799.33333334,
            "unit": "ns",
            "range": "± 48349389.4935504"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 444579476,
            "unit": "ns",
            "range": "± 122488835.26212619"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 196100719,
            "unit": "ns",
            "range": "± 45469327.18460443"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 456535872,
            "unit": "ns",
            "range": "± 113381289.5011967"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 170975418.66666666,
            "unit": "ns",
            "range": "± 39913843.30989521"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 277830344.3333333,
            "unit": "ns",
            "range": "± 1400040.753412676"
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
          "id": "b223f6b88e96c504b7876c8eb2e62291a1d6bcbe",
          "message": "chore: set restrictive pod security context and fix markdown linting (#458)",
          "timestamp": "2026-10-15T23:31:59+02:00",
          "tree_id": "f253a3621b46ac9cc5c18e4a93bcef5e9c73278d",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/b223f6b88e96c504b7876c8eb2e62291a1d6bcbe"
        },
        "date": 1792100127629,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 66529539.166666664,
            "unit": "ns",
            "range": "± 781608.258561555"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 885494.728515625,
            "unit": "ns",
            "range": "± 10639.023348780593"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 980.2233255931309,
            "unit": "ns",
            "range": "± 13.805406058454185"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 11301865.763091216,
            "unit": "ns",
            "range": "± 373783.6264349644"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 224.6905997310366,
            "unit": "ns",
            "range": "± 2.256538125850015"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 488275318,
            "unit": "ns",
            "range": "± 70710038.09786972"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1026452267,
            "unit": "ns",
            "range": "± 65102995.99152659"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 595262105,
            "unit": "ns",
            "range": "± 64155802.84390689"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1131134996,
            "unit": "ns",
            "range": "± 38155027.66208132"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 525668396.6666667,
            "unit": "ns",
            "range": "± 59791986.3330923"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 871166404.3333334,
            "unit": "ns",
            "range": "± 73165190.92669389"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 200807080,
            "unit": "ns",
            "range": "± 46324316.99647388"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 541909917.3333334,
            "unit": "ns",
            "range": "± 180970788.39663723"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 183541877.33333334,
            "unit": "ns",
            "range": "± 19876995.06967231"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 594978919.6666666,
            "unit": "ns",
            "range": "± 117620298.88119827"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 209373439.66666666,
            "unit": "ns",
            "range": "± 43541028.22111514"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 298373520.3333333,
            "unit": "ns",
            "range": "± 3750682.1919163363"
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
          "id": "77778a9fc8a84ce51cc81f646a4c998637658dac",
          "message": "chore(deps): update github-actions (#459)\n\nThis PR contains the following updates:\n\n| Package | Type | Update | Change | Pending |\n|---|---|---|---|---|\n|\n[actions/deploy-pages](https://redirect.github.com/actions/deploy-pages)\n| action | patch | `v5.0.0` → `v5.0.1` | |\n|\n[benchmark-action/github-action-benchmark](https://redirect.github.com/benchmark-action/github-action-benchmark)\n| action | patch | `v1.22.1` → `v1.22.2` | |\n|\n[docker/setup-buildx-action](https://redirect.github.com/docker/setup-buildx-action)\n| action | minor | `v4.3.0` → `v4.4.1` | |\n|\n[github/codeql-action](https://redirect.github.com/github/codeql-action)\n| action | minor | `v4.37.9` → `v4.38.2` | `v4.39.0` (+1) |\n| [helm/kind-action](https://redirect.github.com/helm/kind-action) |\naction | minor | `v1.14.0` → `v1.15.0` | `v1.15.1` |\n\n---\n\n### Release Notes\n\n<details>\n<summary>actions/deploy-pages</summary>\n\n###\n[`v5.0.1`](https://redirect.github.com/actions/deploy-pages/releases/tag/v5.0.1)\n\n[Compare\nSource](https://redirect.github.com/actions/deploy-pages/compare/v5.0.0...v5.0.1)\n\n### Changelog\n\n- Add backoff and jitter to deployment polling\n[@&#8203;yoannchaudet](https://redirect.github.com/yoannchaudet)\n([#&#8203;444](https://redirect.github.com/actions/deploy-pages/issues/444))\n- Improve deployment request test coverage\n[@&#8203;adwitiyagoyal](https://redirect.github.com/adwitiyagoyal)\n([#&#8203;440](https://redirect.github.com/actions/deploy-pages/issues/440))\n\n***\n\nSee details of [all code\nchanges](https://redirect.github.com/actions/deploy-pages/compare/v5.0.0...v5.0.1)\nsince previous release.\n\n:warning: For use with products other than GitHub.com, such as GitHub\nEnterprise Server, please consult the [compatibility\ntable](https://redirect.github.com/actions/deploy-pages/#compatibility).\n\n</details>\n\n<details>\n<summary>benchmark-action/github-action-benchmark</summary>\n\n###\n[`v1.22.2`](https://redirect.github.com/benchmark-action/github-action-benchmark/blob/HEAD/CHANGELOG.md#v1222---15-Sep-2026)\n\n[Compare\nSource](https://redirect.github.com/benchmark-action/github-action-benchmark/compare/v1.22.1...v1.22.2)\n\n- **perf** shallow-clone the target branch only\n([#&#8203;364](https://redirect.github.com/benchmark-action/github-action-benchmark/issues/364))\n\n</details>\n\n<details>\n<summary>docker/setup-buildx-action</summary>\n\n###\n[`v4.4.1`](https://redirect.github.com/docker/setup-buildx-action/releases/tag/v4.4.1)\n\n[Compare\nSource](https://redirect.github.com/docker/setup-buildx-action/compare/v4.4.0...v4.4.1)\n\n- Skip BuildKit image pre-pulls for explicit endpoints by\n[@&#8203;crazy-max](https://redirect.github.com/crazy-max) in\n[#&#8203;624](https://redirect.github.com/docker/setup-buildx-action/pull/624)\n\n**Full Changelog**:\n<https://github.com/docker/setup-buildx-action/compare/v4.4.0...v4.4.1>\n\n###\n[`v4.4.0`](https://redirect.github.com/docker/setup-buildx-action/releases/tag/v4.4.0)\n\n[Compare\nSource](https://redirect.github.com/docker/setup-buildx-action/compare/v4.3.0...v4.4.0)\n\n- Use official Buildx releases for cloud driver by\n[@&#8203;crazy-max](https://redirect.github.com/crazy-max) in\n[#&#8203;606](https://redirect.github.com/docker/setup-buildx-action/pull/606)\n- Pull BuildKit image before builder creation by\n[@&#8203;crazy-max](https://redirect.github.com/crazy-max) in\n[#&#8203;609](https://redirect.github.com/docker/setup-buildx-action/pull/609)\n- Use shared error helpers for Buildx and Docker commands by\n[@&#8203;crazy-max](https://redirect.github.com/crazy-max) in\n[#&#8203;620](https://redirect.github.com/docker/setup-buildx-action/pull/620)\n- Bump\n[@&#8203;docker/actions-toolkit](https://redirect.github.com/docker/actions-toolkit)\nfrom 0.95.0 to 0.100.0 in\n[#&#8203;610](https://redirect.github.com/docker/setup-buildx-action/pull/610)\n[#&#8203;618](https://redirect.github.com/docker/setup-buildx-action/pull/618)\n[#&#8203;619](https://redirect.github.com/docker/setup-buildx-action/pull/619)\n- Bump [@&#8203;humanfs/node](https://redirect.github.com/humanfs/node)\nfrom 0.16.7 to 0.16.8 in\n[#&#8203;614](https://redirect.github.com/docker/setup-buildx-action/pull/614)\n- Bump js-yaml from 5.3.0 to 5.4.2 in\n[#&#8203;608](https://redirect.github.com/docker/setup-buildx-action/pull/608)\n[#&#8203;617](https://redirect.github.com/docker/setup-buildx-action/pull/617)\n- Bump postcss-selector-parser from 7.1.1 to 7.1.5 in\n[#&#8203;611](https://redirect.github.com/docker/setup-buildx-action/pull/611)\n\n**Full Changelog**:\n<https://github.com/docker/setup-buildx-action/compare/v4.3.0...v4.4.0>\n\n</details>\n\n<details>\n<summary>github/codeql-action</summary>\n\n###\n[`v4.38.2`](https://redirect.github.com/github/codeql-action/releases/tag/v4.38.2)\n\n[Compare\nSource](https://redirect.github.com/github/codeql-action/compare/v4.38.1...v4.38.2)\n\n- Update default CodeQL bundle version to\n[2.27.1](https://redirect.github.com/github/codeql-action/releases/tag/codeql-bundle-v2.27.1).\n[#&#8203;4160](https://redirect.github.com/github/codeql-action/pull/4160)\n\n###\n[`v4.38.1`](https://redirect.github.com/github/codeql-action/releases/tag/v4.38.1)\n\n[Compare\nSource](https://redirect.github.com/github/codeql-action/compare/v4.38.0...v4.38.1)\n\n- The CodeQL Action now has experimental support for CodeQL releases for\nwhich per-language bundles are available. Per-language bundles support\nanalysis for a single language and are therefore smaller than the\ncombined bundles that allow analysis for all supported languages. As a\nresult, per-language bundles take up less space on disk and are faster\nto download. We expect to roll this change out to everyone in the coming\nweeks.\n[#&#8203;4146](https://redirect.github.com/github/codeql-action/pull/4146)\n\n###\n[`v4.38.0`](https://redirect.github.com/github/codeql-action/releases/tag/v4.38.0)\n\n[Compare\nSource](https://redirect.github.com/github/codeql-action/compare/v4.37.9...v4.38.0)\n\n- On GitHub-hosted runners, the CodeQL Action now deletes unused CodeQL\nbundles from the toolcache before downloading a different bundle, which\nfrees up disk space for the analysis. We expect to roll this change out\nto everyone in September.\n[#&#8203;4124](https://redirect.github.com/github/codeql-action/pull/4124)\n- The CodeQL Action now supports CodeQL releases that are compatible\nwith Linux Arm64 and downloads the native `linux-arm64` CodeQL bundle\nwhen available.\n[#&#8203;4072](https://redirect.github.com/github/codeql-action/pull/4072)\n- Update default CodeQL bundle version to\n[2.27.0](https://redirect.github.com/github/codeql-action/releases/tag/codeql-bundle-v2.27.0).\n[#&#8203;4129](https://redirect.github.com/github/codeql-action/pull/4129)\n\n</details>\n\n<details>\n<summary>helm/kind-action</summary>\n\n###\n[`v1.15.0`](https://redirect.github.com/helm/kind-action/releases/tag/v1.15.0)\n\n[Compare\nSource](https://redirect.github.com/helm/kind-action/compare/v1.14.0...v1.15.0)\n\n#### What's Changed\n\n- clean up ci by [@&#8203;cpanato](https://redirect.github.com/cpanato)\nin [#&#8203;158](https://redirect.github.com/helm/kind-action/pull/158)\n- Bump actions/checkout from 6.0.2 to 6.0.3 in the actions group by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;161](https://redirect.github.com/helm/kind-action/pull/161)\n- fix: propagate registry_port to container and containerd by\n[@&#8203;Bisman-Singh](https://redirect.github.com/Bisman-Singh) in\n[#&#8203;160](https://redirect.github.com/helm/kind-action/pull/160)\n- fix: add curl retry flags to download calls by\n[@&#8203;kaovilai](https://redirect.github.com/kaovilai) in\n[#&#8203;165](https://redirect.github.com/helm/kind-action/pull/165)\n- Bump actions/checkout from 6.0.3 to 7.0.1 by\n[@&#8203;dependabot](https://redirect.github.com/dependabot)\\[bot] in\n[#&#8203;166](https://redirect.github.com/helm/kind-action/pull/166)\n- chore: bump default kind and kubectl by\n[@&#8203;yurnov](https://redirect.github.com/yurnov) in\n[#&#8203;169](https://redirect.github.com/helm/kind-action/pull/169)\n- Report url that could not be downloaded by\n[@&#8203;jsoref](https://redirect.github.com/jsoref) in\n[#&#8203;167](https://redirect.github.com/helm/kind-action/pull/167)\n\n#### New Contributors\n\n- [@&#8203;Bisman-Singh](https://redirect.github.com/Bisman-Singh) made\ntheir first contribution in\n[#&#8203;160](https://redirect.github.com/helm/kind-action/pull/160)\n- [@&#8203;kaovilai](https://redirect.github.com/kaovilai) made their\nfirst contribution in\n[#&#8203;165](https://redirect.github.com/helm/kind-action/pull/165)\n- [@&#8203;yurnov](https://redirect.github.com/yurnov) made their first\ncontribution in\n[#&#8203;169](https://redirect.github.com/helm/kind-action/pull/169)\n- [@&#8203;jsoref](https://redirect.github.com/jsoref) made their first\ncontribution in\n[#&#8203;167](https://redirect.github.com/helm/kind-action/pull/167)\n\n**Full Changelog**:\n<https://github.com/helm/kind-action/compare/v1.14.0...v1.15.0>\n\n</details>\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n👻 **Immortal**: This PR will be recreated if closed unmerged. Get\n[config\nhelp](https://redirect.github.com/renovatebot/renovate/discussions) if\nthat's undesired.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xNjQuMSIsInVwZGF0ZWRJblZlciI6IjQ0LjE2NC4xIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-15T21:38:20Z",
          "tree_id": "b2956e4d6920c75956d12d5a39c4ef3b4ae2b5ed",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/77778a9fc8a84ce51cc81f646a4c998637658dac"
        },
        "date": 1792100687150,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 62763288.196078435,
            "unit": "ns",
            "range": "± 1234030.1121942163"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 822676.5700520833,
            "unit": "ns",
            "range": "± 13961.953984011641"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 967.2749536037445,
            "unit": "ns",
            "range": "± 18.095698561763847"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 10648459.886458334,
            "unit": "ns",
            "range": "± 154102.94220539648"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 215.3786834081014,
            "unit": "ns",
            "range": "± 0.5309386784419352"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 511538256.3333333,
            "unit": "ns",
            "range": "± 63168436.14061825"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1136567267,
            "unit": "ns",
            "range": "± 42125285.05919614"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 536265155.6666667,
            "unit": "ns",
            "range": "± 95676829.2682303"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1257038556.3333333,
            "unit": "ns",
            "range": "± 45386383.774088636"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 592379477.3333334,
            "unit": "ns",
            "range": "± 27232713.50843104"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 882036882.3333334,
            "unit": "ns",
            "range": "± 39977684.93138416"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 186031825.33333334,
            "unit": "ns",
            "range": "± 30959057.02838012"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 542637908.6666666,
            "unit": "ns",
            "range": "± 153815343.89886466"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 179419163.66666666,
            "unit": "ns",
            "range": "± 33212854.62770215"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 486878183.6666667,
            "unit": "ns",
            "range": "± 168524045.15690628"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 189174573,
            "unit": "ns",
            "range": "± 24341889.70670718"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 338932811.6666667,
            "unit": "ns",
            "range": "± 60470385.83853862"
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
          "id": "5050a6955e4514bc5388b932ceabb77b2a9edcf6",
          "message": "chore(master): release 3.0.1 (#457)\n\n:robot: I have created a release *beep* *boop*\n---\n\n\n##\n[3.0.1](https://github.com/miracum/fhir-pseudonymizer/compare/v3.0.0...v3.0.1)\n(2026-10-15)\n\n\n### Miscellaneous Chores\n\n* **deps:** update all digest updates\n([#455](https://github.com/miracum/fhir-pseudonymizer/issues/455))\n([1df0698](https://github.com/miracum/fhir-pseudonymizer/commit/1df0698d0150caf7f9d14d05b010e842138e80f4))\n* **deps:** update all non-major dependencies\n([#456](https://github.com/miracum/fhir-pseudonymizer/issues/456))\n([743dc66](https://github.com/miracum/fhir-pseudonymizer/commit/743dc66b94bca50a6c98582b2c956cf81a137903))\n* **deps:** update docker.io/mockserver/mockserver docker tag to v8\n([#454](https://github.com/miracum/fhir-pseudonymizer/issues/454))\n([60ac2cc](https://github.com/miracum/fhir-pseudonymizer/commit/60ac2cc214954d20956ad30d35e6d68c3b3de856))\n* **deps:** update github-actions\n([#459](https://github.com/miracum/fhir-pseudonymizer/issues/459))\n([77778a9](https://github.com/miracum/fhir-pseudonymizer/commit/77778a9fc8a84ce51cc81f646a4c998637658dac))\n* set restrictive pod security context and fix markdown linting\n([#458](https://github.com/miracum/fhir-pseudonymizer/issues/458))\n([b223f6b](https://github.com/miracum/fhir-pseudonymizer/commit/b223f6b88e96c504b7876c8eb2e62291a1d6bcbe))\n\n---\nThis PR was generated with [Release\nPlease](https://github.com/googleapis/release-please). See\n[documentation](https://github.com/googleapis/release-please#release-please).",
          "timestamp": "2026-10-15T23:52:20+02:00",
          "tree_id": "7f903cf9b49eb50d19e34003029f33acd89127ce",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/5050a6955e4514bc5388b932ceabb77b2a9edcf6"
        },
        "date": 1792101344273,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 120394068.68181819,
            "unit": "ns",
            "range": "± 2930305.7589423824"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1756238.546875,
            "unit": "ns",
            "range": "± 10596.459441524723"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1768.7859840393066,
            "unit": "ns",
            "range": "± 10.536573754866264"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23819576.01538461,
            "unit": "ns",
            "range": "± 375874.35417353245"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 329.14083833694457,
            "unit": "ns",
            "range": "± 1.9321245429854021"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 941650777.6666666,
            "unit": "ns",
            "range": "± 13541381.893219331"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 2167274749.6666665,
            "unit": "ns",
            "range": "± 14105971.009064151"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 965893435,
            "unit": "ns",
            "range": "± 11108036.330419118"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 2309793037.3333335,
            "unit": "ns",
            "range": "± 48326487.70814867"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 927506739.3333334,
            "unit": "ns",
            "range": "± 2155544.7520342357"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1579675983.6666667,
            "unit": "ns",
            "range": "± 28360203.38130769"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 367070444.3333333,
            "unit": "ns",
            "range": "± 158471714.5604581"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 585555958,
            "unit": "ns",
            "range": "± 292806415.3500657"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 362937659.6666667,
            "unit": "ns",
            "range": "± 142077834.07046658"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 672656525.6666666,
            "unit": "ns",
            "range": "± 238968342.77843034"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 443005637.6666667,
            "unit": "ns",
            "range": "± 81529048.38084581"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 602097911.3333334,
            "unit": "ns",
            "range": "± 258223059.07205102"
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
          "id": "118907164f5ea5cf571246c06670d44bb8fbff42",
          "message": "feat: added option to fully hash and pseudonymize FHIR-Server-URL-lookalikes instead of just their Id part (#460)\n\nCloses #444",
          "timestamp": "2026-10-16T13:13:13+02:00",
          "tree_id": "0dcf8da3905c47e8cc537cbe87f4d48cd28bb022",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/118907164f5ea5cf571246c06670d44bb8fbff42"
        },
        "date": 1792149375762,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 81854426.4137931,
            "unit": "ns",
            "range": "± 2317639.837225161"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1235378.8807444852,
            "unit": "ns",
            "range": "± 25179.834217300962"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1276.3485388074603,
            "unit": "ns",
            "range": "± 4.206130806958995"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 16149859.362903226,
            "unit": "ns",
            "range": "± 484760.3920777429"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 244.2090174992879,
            "unit": "ns",
            "range": "± 1.657353591185264"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 588547512.6666666,
            "unit": "ns",
            "range": "± 4745162.377085566"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1389941442.6666667,
            "unit": "ns",
            "range": "± 11476412.474354925"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 595680529.6666666,
            "unit": "ns",
            "range": "± 4196455.416678978"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1454083710.3333333,
            "unit": "ns",
            "range": "± 8360352.516696489"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 590168431.6666666,
            "unit": "ns",
            "range": "± 8444614.965849321"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1055404840.3333334,
            "unit": "ns",
            "range": "± 44750284.77755936"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 303711759.6666667,
            "unit": "ns",
            "range": "± 36002974.569499254"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 501981627.6666667,
            "unit": "ns",
            "range": "± 318607081.20933056"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 278277501.3333333,
            "unit": "ns",
            "range": "± 32151421.293153748"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 589838718.3333334,
            "unit": "ns",
            "range": "± 275180877.19956744"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 288539072.3333333,
            "unit": "ns",
            "range": "± 41772927.74870636"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 513657466,
            "unit": "ns",
            "range": "± 137679145.81995255"
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
          "id": "f34ce3dd4068c27268de3172a1360c6ce3d802da",
          "message": "chore(master): release 3.1.0 (#461)\n\n:robot: I have created a release *beep* *boop*\n---\n\n\n##\n[3.1.0](https://github.com/miracum/fhir-pseudonymizer/compare/v3.0.1...v3.1.0)\n(2026-10-16)\n\n\n### Features\n\n* added option to fully hash and pseudonymize FHIR-Server-URL-lookalikes\ninstead of just their Id part\n([#460](https://github.com/miracum/fhir-pseudonymizer/issues/460))\n([1189071](https://github.com/miracum/fhir-pseudonymizer/commit/118907164f5ea5cf571246c06670d44bb8fbff42)),\ncloses [#444](https://github.com/miracum/fhir-pseudonymizer/issues/444)\n\n---\nThis PR was generated with [Release\nPlease](https://github.com/googleapis/release-please). See\n[documentation](https://github.com/googleapis/release-please#release-please).",
          "timestamp": "2026-10-16T13:34:11+02:00",
          "tree_id": "c1d0b6808ee59a186cb89943e28f17573cb8b0b2",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/f34ce3dd4068c27268de3172a1360c6ce3d802da"
        },
        "date": 1792150673542,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 114184215.92857143,
            "unit": "ns",
            "range": "± 2991637.408308031"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1750159.4682291667,
            "unit": "ns",
            "range": "± 14861.568127491264"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1859.9747923442296,
            "unit": "ns",
            "range": "± 4.286179322825339"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21598880.4375,
            "unit": "ns",
            "range": "± 55879.4762785588"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 309.53304062570845,
            "unit": "ns",
            "range": "± 0.5076609324532325"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 868603919.3333334,
            "unit": "ns",
            "range": "± 9109316.08588841"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1962523845.6666667,
            "unit": "ns",
            "range": "± 4376064.464475968"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 862632498.6666666,
            "unit": "ns",
            "range": "± 8298000.732431658"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 2156429287,
            "unit": "ns",
            "range": "± 12951605.751111057"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 883780982.3333334,
            "unit": "ns",
            "range": "± 5584181.850445268"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1441232090.6666667,
            "unit": "ns",
            "range": "± 15397571.023162592"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 321196945.3333333,
            "unit": "ns",
            "range": "± 105303156.27277656"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 562111880.6666666,
            "unit": "ns",
            "range": "± 162864232.0101169"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 381956794.6666667,
            "unit": "ns",
            "range": "± 7419405.02901522"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 627204673.6666666,
            "unit": "ns",
            "range": "± 145287380.7911802"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 345621918,
            "unit": "ns",
            "range": "± 98634374.43981932"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 494582501.3333333,
            "unit": "ns",
            "range": "± 206119781.45381942"
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
          "id": "6db737f4a0aed606f9785ec74ddb2cffae7efd09",
          "message": "chore(deps): update github-actions (#463)\n\nThis PR contains the following updates:\n\n| Package | Type | Update | Change |\n|---|---|---|---|\n|\n[actions/download-artifact](https://redirect.github.com/actions/download-artifact)\n| action | patch | `v8.0.1` → `v8.0.2` |\n|\n[actions/upload-artifact](https://redirect.github.com/actions/upload-artifact)\n| action | patch | `v7.0.1` → `v7.0.2` |\n| [astral-sh/setup-uv](https://redirect.github.com/astral-sh/setup-uv) |\naction | minor | `v10.2.0` → `v10.3.0` |\n|\n[github/codeql-action](https://redirect.github.com/github/codeql-action)\n| action | minor | `v4.38.2` → `v4.39.0` |\n| [helm/kind-action](https://redirect.github.com/helm/kind-action) |\naction | patch | `v1.15.0` → `v1.15.1` |\n| [miracum/.github](https://redirect.github.com/miracum/.github) |\nworkflow | minor | `v1.29.2` → `v1.30.0` |\n| [miracum/.github](https://redirect.github.com/miracum/.github) |\nworkflow | minor | `v1.29.3` → `v1.30.0` |\n|\n[ossf/scorecard-action](https://redirect.github.com/ossf/scorecard-action)\n| action | minor | `v2.4.4` → `v2.5.0` |\n\n---\n\n### Release Notes\n\n<details>\n<summary>actions/download-artifact</summary>\n\n###\n[`v8.0.2`](https://redirect.github.com/actions/download-artifact/releases/tag/v8.0.2)\n\n[Compare\nSource](https://redirect.github.com/actions/download-artifact/compare/v8.0.1...v8.0.2)\n\n#### What's Changed\n\n- Update the readme by\n[@&#8203;danwkennedy](https://redirect.github.com/danwkennedy) in\n[#&#8203;473](https://redirect.github.com/actions/download-artifact/pull/473)\n- Improves artifact download retries when the service returns HTTP 429\n(rate limiting), including honoring valid Retry-After headers.\n- Update\n[@&#8203;actions/artifact](https://redirect.github.com/actions/artifact)\nto 6.3.1 by [@&#8203;tunc-d](https://redirect.github.com/tunc-d) in\n[#&#8203;492](https://redirect.github.com/actions/download-artifact/pull/492)\n- Prepare download-artifact v8.0.2 by\n[@&#8203;tunc-d](https://redirect.github.com/tunc-d) in\n[#&#8203;493](https://redirect.github.com/actions/download-artifact/pull/493)\n\n#### New Contributors\n\n- [@&#8203;tunc-d](https://redirect.github.com/tunc-d) made their first\ncontribution in\n[#&#8203;492](https://redirect.github.com/actions/download-artifact/pull/492)\n\n**Full Changelog**:\n<https://github.com/actions/download-artifact/compare/v8.0.1...v8.0.2>\n\n</details>\n\n<details>\n<summary>actions/upload-artifact</summary>\n\n###\n[`v7.0.2`](https://redirect.github.com/actions/upload-artifact/releases/tag/v7.0.2)\n\n[Compare\nSource](https://redirect.github.com/actions/upload-artifact/compare/v7.0.1...v7.0.2)\n\n#### What's Changed\n\n- Improves artifact download retries when the service returns HTTP 429\n(rate limiting), including honoring valid Retry-After headers.\n- Updates\n[@&#8203;actions/artifact](https://redirect.github.com/actions/artifact)\nto v6.3.1.\n\n#### New Contributors\n\n- [@&#8203;tunc-d](https://redirect.github.com/tunc-d) made their first\ncontribution in\n[#&#8203;821](https://redirect.github.com/actions/upload-artifact/pull/821)\n\n**Full Changelog**:\n<https://github.com/actions/upload-artifact/compare/v7.0.1...v7.0.2>\n\n</details>\n\n<details>\n<summary>astral-sh/setup-uv</summary>\n\n###\n[`v10.3.0`](https://redirect.github.com/astral-sh/setup-uv/releases/tag/v10.3.0):\n🌈 Add Python architecture selection\n\n[Compare\nSource](https://redirect.github.com/astral-sh/setup-uv/compare/v10.2.0...v10.3.0)\n\n##### Changes\n\nThis release adds the `python-arch` input to select a Python\narchitecture independently of the Python version. With `uv` 0.13.0 or\nlater, set `python-arch: x86_64` to use x64 Python on Windows ARM64\nrunners.\n\nIt also includes embedded checksums for `uv` releases through 0.13.0.\n\n- Use macOS 15 runners for tests\n[@&#8203;charliermarsh](https://redirect.github.com/charliermarsh)\n([#&#8203;1081](https://redirect.github.com/astral-sh/setup-uv/issues/1081))\n\n##### 🚀 Enhancements\n\n- Add `python-arch` support\n[@&#8203;zanieb](https://redirect.github.com/zanieb)\n([#&#8203;1079](https://redirect.github.com/astral-sh/setup-uv/issues/1079))\n\n##### 🧰 Maintenance\n\n- chore: update known checksums for 0.13.0\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1089](https://redirect.github.com/astral-sh/setup-uv/issues/1089))\n- chore: update known checksums for 0.12.24\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1084](https://redirect.github.com/astral-sh/setup-uv/issues/1084))\n- chore: update known checksums for 0.12.23\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1082](https://redirect.github.com/astral-sh/setup-uv/issues/1082))\n- chore: update known checksums for 0.12.22\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1080](https://redirect.github.com/astral-sh/setup-uv/issues/1080))\n- chore: update known checksums for 0.12.21\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1078](https://redirect.github.com/astral-sh/setup-uv/issues/1078))\n- chore: update known checksums for 0.12.20\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1076](https://redirect.github.com/astral-sh/setup-uv/issues/1076))\n- chore: update known checksums for 0.12.19\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1068](https://redirect.github.com/astral-sh/setup-uv/issues/1068))\n- chore: update known checksums for 0.12.18\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1065](https://redirect.github.com/astral-sh/setup-uv/issues/1065))\n\n##### ⬆️ Dependency updates\n\n- chore(deps): bump zizmorcore/zizmor-action from 0.6.2 to 0.6.3\n@&#8203;[dependabot\\[bot\\]](https://redirect.github.com/apps/dependabot)\n([#&#8203;1077](https://redirect.github.com/astral-sh/setup-uv/issues/1077))\n\n</details>\n\n<details>\n<summary>github/codeql-action</summary>\n\n###\n[`v4.39.0`](https://redirect.github.com/github/codeql-action/releases/tag/v4.39.0)\n\n[Compare\nSource](https://redirect.github.com/github/codeql-action/compare/v4.38.3...v4.39.0)\n\n- *Breaking change*: Bump the minimum required CodeQL bundle version to\n2.20.7.\n[#&#8203;4205](https://redirect.github.com/github/codeql-action/pull/4205)\n- The `codeql-action/analyze` step now validates that a correct argument\nfor the `checkout_path` input has been provided. The `checkout_path`\ninput must be set to the path where the repository that is being\nanalyzed is checked out at on the runner, if that path is something\nother than the workspace root. If the path does not point to a\nrepository checkout or is different from the path that was used by\n`codeql-action/init`, then the CodeQL Action now produces a warning.\n[#&#8203;4148](https://redirect.github.com/github/codeql-action/pull/4148)\n- Resolved an issue with third-party caching implementations that\nprevented caches from being restored correctly.\n[#&#8203;4206](https://redirect.github.com/github/codeql-action/pull/4206)\n- The `codeql-action/start-proxy` step that is used in Code Scanning\nDefault Setup now finds CodeQL releases using the same logic as\n`codeql-action/init`. This primarily benefits customers who are using\nGitHub instances other than `github.com` where the\n`codeql-action/start-proxy` step may previously have been unable to\nlocate recent releases and ended up using outdated versions of the\nprivate registry authentication proxy as a result.\n[#&#8203;4198](https://redirect.github.com/github/codeql-action/pull/4198)\n\n###\n[`v4.38.3`](https://redirect.github.com/github/codeql-action/releases/tag/v4.38.3)\n\n[Compare\nSource](https://redirect.github.com/github/codeql-action/compare/v4.38.2...v4.38.3)\n\n- *Upcoming breaking change*: CodeQL version 2.21.2 and earlier were\ndiscontinued on 24 September 2026 alongside GitHub Enterprise Server\n3.17, and will be unsupported by the next minor release of the CodeQL\nAction. Added a deprecation warning for customers using these versions\nof CodeQL.\n[#&#8203;4188](https://redirect.github.com/github/codeql-action/pull/4188)\n- Update default CodeQL bundle version to\n[2.27.2](https://redirect.github.com/github/codeql-action/releases/tag/codeql-bundle-v2.27.2).\n[#&#8203;4203](https://redirect.github.com/github/codeql-action/pull/4203)\n- Fixed a bug where the decision of whether to use a per-language bundle\ndid not account for custom configurations that reference queries outside\nof compiled CodeQL packs. This issue was caught during internal testing\nand did not affect any customer repositories. We will resume the roll\nout of per-language bundles in the coming weeks.\n[#&#8203;4184](https://redirect.github.com/github/codeql-action/pull/4184)\n\n</details>\n\n<details>\n<summary>helm/kind-action</summary>\n\n###\n[`v1.15.1`](https://redirect.github.com/helm/kind-action/releases/tag/v1.15.1)\n\n[Compare\nSource](https://redirect.github.com/helm/kind-action/compare/v1.15.0...v1.15.1)\n\n#### What's Changed\n\n- feat: add version_file input to read the kind version from a file by\n[@&#8203;somaz94](https://redirect.github.com/somaz94) in\n[#&#8203;162](https://redirect.github.com/helm/kind-action/pull/162)\n- chore: bump kubectl to v1.37.1 and cloud-provider-kind to v0.12.0 by\n[@&#8203;cpanato](https://redirect.github.com/cpanato) in\n[#&#8203;170](https://redirect.github.com/helm/kind-action/pull/170)\n\n#### New Contributors\n\n- [@&#8203;somaz94](https://redirect.github.com/somaz94) made their\nfirst contribution in\n[#&#8203;162](https://redirect.github.com/helm/kind-action/pull/162)\n\n**Full Changelog**:\n<https://github.com/helm/kind-action/compare/v1.15.0...v1.15.1>\n\n</details>\n\n<details>\n<summary>miracum/.github</summary>\n\n###\n[`v1.30.0`](https://redirect.github.com/miracum/.github/releases/tag/v1.30.0)\n\n[Compare\nSource](https://redirect.github.com/miracum/.github/compare/v1.29.3...v1.30.0)\n\n##### Features\n\n- added chart docs lint\n([#&#8203;224](https://redirect.github.com/miracum/.github/issues/224))\n([7040470](https://redirect.github.com/miracum/.github/commit/704047074ac99fc4c0990f8b8377086069323116))\n\n###\n[`v1.29.3`](https://redirect.github.com/miracum/.github/releases/tag/v1.29.3)\n\n[Compare\nSource](https://redirect.github.com/miracum/.github/compare/v1.29.2...v1.29.3)\n\n##### Bug Fixes\n\n- signing race condition\n([#&#8203;223](https://redirect.github.com/miracum/.github/issues/223))\n([43926b6](https://redirect.github.com/miracum/.github/commit/43926b6aa69b5a63e15378200f698ce8cfeefc88))\n\n</details>\n\n<details>\n<summary>ossf/scorecard-action</summary>\n\n###\n[`v2.5.0`](https://redirect.github.com/ossf/scorecard-action/releases/tag/v2.5.0)\n\n[Compare\nSource](https://redirect.github.com/ossf/scorecard-action/compare/v2.4.4...v2.5.0)\n\n#### What's Changed\n\nThis release moves the action to Scorecard v5.6.0, the first Scorecard\nrelease since its hosted\nservices moved to new infrastructure.\n\nFor changes to checks and results, and what the infrastructure move\nmeans for you, see the\n[Scorecard v5.6.0 release\nnotes](https://redirect.github.com/ossf/scorecard/releases/tag/v5.6.0).\n\n- The action's container image is built with Go 1.27.2 on a refreshed\ndistroless base. by\n[@&#8203;justaugustus](https://redirect.github.com/justaugustus) in\n[#&#8203;1716](https://redirect.github.com/ossf/scorecard-action/pull/1716)\n- The installer (`cmd/installer`) now creates\n`.github/workflows/scorecard.yml`, and still\nrecognizes existing `scorecards.yml` and `scorecards-analysis.yml`\nworkflows. by\n[@&#8203;justaugustus](https://redirect.github.com/justaugustus) in\n[#&#8203;1716](https://redirect.github.com/ossf/scorecard-action/pull/1716)\n- The Scorecard version in results now always matches the Scorecard\nrelease the action is built\nwith. by\n[@&#8203;justaugustus](https://redirect.github.com/justaugustus) in\n[#&#8203;1718](https://redirect.github.com/ossf/scorecard-action/pull/1718)\n- The action no longer prints the GitHub token variable while validating\nits options. by\n[@&#8203;arpitjain099](https://redirect.github.com/arpitjain099) in\n[#&#8203;1709](https://redirect.github.com/ossf/scorecard-action/pull/1709)\n- `results_format` accepts `intoto`, which wraps the JSON results in an\nin-toto attestation\nstatement. by [@&#8203;puerco](https://redirect.github.com/puerco) in\n[#&#8203;1702](https://redirect.github.com/ossf/scorecard-action/pull/1702)\n\n##### Other\n\n- Add a daily canary against Scorecard main by\n[@&#8203;justaugustus](https://redirect.github.com/justaugustus) in\n[#&#8203;1711](https://redirect.github.com/ossf/scorecard-action/pull/1711)\n- Bump Scorecard to the v5.6.0 candidate and fix the canary toolchain by\n[@&#8203;justaugustus](https://redirect.github.com/justaugustus) in\n[#&#8203;1714](https://redirect.github.com/ossf/scorecard-action/pull/1714)\n- Run Scorecard on every merge, add a zizmor presubmit, analyze\nworkflows with CodeQL, and update\nRELEASE.md by\n[@&#8203;justaugustus](https://redirect.github.com/justaugustus) in\n[#&#8203;1716](https://redirect.github.com/ossf/scorecard-action/pull/1716)\n- Bump Scorecard to v5.6.0 by\n[@&#8203;justaugustus](https://redirect.github.com/justaugustus) in\n[#&#8203;1719](https://redirect.github.com/ossf/scorecard-action/pull/1719)\n\n#### New Contributors\n\n- [@&#8203;arpitjain099](https://redirect.github.com/arpitjain099) made\ntheir first contribution in\n[#&#8203;1709](https://redirect.github.com/ossf/scorecard-action/pull/1709)\n- [@&#8203;puerco](https://redirect.github.com/puerco) made their first\ncontribution in\n[#&#8203;1702](https://redirect.github.com/ossf/scorecard-action/pull/1702)\n\n**Full Changelog**:\n<https://github.com/ossf/scorecard-action/compare/v2.4.4...v2.5.0>\n\n</details>\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n👻 **Immortal**: This PR will be recreated if closed unmerged. Get\n[config\nhelp](https://redirect.github.com/renovatebot/renovate/discussions) if\nthat's undesired.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xNjQuMSIsInVwZGF0ZWRJblZlciI6IjQ0LjE2NC4xIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-17T12:59:20Z",
          "tree_id": "afdff40a05876008afed87a4c3be371b7a94b08a",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/6db737f4a0aed606f9785ec74ddb2cffae7efd09"
        },
        "date": 1792242440002,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 89354896.8028169,
            "unit": "ns",
            "range": "± 4354935.400248159"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1187137.6690848214,
            "unit": "ns",
            "range": "± 20588.606221654703"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1409.84990134606,
            "unit": "ns",
            "range": "± 10.381055263412865"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 14924628.55859375,
            "unit": "ns",
            "range": "± 277593.8144741078"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 264.30161625146866,
            "unit": "ns",
            "range": "± 5.247423121523834"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 711766806.6666666,
            "unit": "ns",
            "range": "± 24002618.273612782"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1745757454,
            "unit": "ns",
            "range": "± 29137027.935531054"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 746863271.3333334,
            "unit": "ns",
            "range": "± 27822092.860881068"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1845136026.6666667,
            "unit": "ns",
            "range": "± 35547859.03240533"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 748655269.3333334,
            "unit": "ns",
            "range": "± 16080177.513728334"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1313547361,
            "unit": "ns",
            "range": "± 40737984.71501948"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 342930519.6666667,
            "unit": "ns",
            "range": "± 32780491.919748705"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 516855628,
            "unit": "ns",
            "range": "± 138643985.1305515"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 325911422,
            "unit": "ns",
            "range": "± 47152399.272449404"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 604434104,
            "unit": "ns",
            "range": "± 259464142.99446434"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 289552110.3333333,
            "unit": "ns",
            "range": "± 85742536.49029157"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 560866496,
            "unit": "ns",
            "range": "± 166185428.70884618"
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
          "id": "e995f5241802323ff5efaed665fb99cc0d27dd04",
          "message": "chore(deps): update docker.io/mockserver/mockserver docker tag to v9 (#465)\n\nThis PR contains the following updates:\n\n| Package | Update | Change |\n|---|---|---|\n| docker.io/mockserver/mockserver | major | `8.0.0` → `9.0.0` |\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n🔕 **Ignore**: Close this PR and you won't be reminded about this update\nagain.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xNjQuMSIsInVwZGF0ZWRJblZlciI6IjQ0LjE2NC4xIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-17T12:59:57Z",
          "tree_id": "cbbd9cfac7f093bc17e19fe7be9801145645b004",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/e995f5241802323ff5efaed665fb99cc0d27dd04"
        },
        "date": 1792242537385,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 113029215.7586207,
            "unit": "ns",
            "range": "± 3029846.354974641"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1778741.9464285714,
            "unit": "ns",
            "range": "± 20258.77015528957"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1649.5018692016602,
            "unit": "ns",
            "range": "± 2.4657340378892476"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22494654.84375,
            "unit": "ns",
            "range": "± 261564.2481627566"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 332.0673149313246,
            "unit": "ns",
            "range": "± 1.4739822596746324"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 814530821.6666666,
            "unit": "ns",
            "range": "± 3075364.628222861"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1859480809.3333333,
            "unit": "ns",
            "range": "± 3714792.4567062063"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 805383641.6666666,
            "unit": "ns",
            "range": "± 2746317.643290618"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1985201975.6666667,
            "unit": "ns",
            "range": "± 8352842.618414065"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 790079867,
            "unit": "ns",
            "range": "± 6819147.645595892"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1386991782.6666667,
            "unit": "ns",
            "range": "± 2519313.383102692"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 344898641,
            "unit": "ns",
            "range": "± 110731143.25169621"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 590152222,
            "unit": "ns",
            "range": "± 365570893.4769577"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 321672514.3333333,
            "unit": "ns",
            "range": "± 126369111.69086732"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 627132280.3333334,
            "unit": "ns",
            "range": "± 273092885.76445943"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 324066120.3333333,
            "unit": "ns",
            "range": "± 130608561.65247196"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 454417303,
            "unit": "ns",
            "range": "± 258080370.17499524"
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
          "id": "64b6265fc274f16284325137244fd8d02f779097",
          "message": "fix(deps): update all non-major dependencies (#464)\n\n> [!NOTE]\n> This PR body was truncated due to platform limits.\n\nThis PR contains the following updates:\n\n| Package | Change |\n[Age](https://docs.renovatebot.com/merge-confidence/) |\n[Confidence](https://docs.renovatebot.com/merge-confidence/) | Type |\nUpdate |\n|---|---|---|---|---|---|\n|\n[Confluent.Kafka](https://redirect.github.com/confluentinc/confluent-kafka-dotnet)\n| `2.15.1` → `2.16.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Confluent.Kafka/2.16.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Confluent.Kafka/2.15.1/2.16.0?slim=true)\n| nuget | minor |\n|\n[Duende.AccessTokenManagement](https://redirect.github.com/duendesoftware/foss)\n([source](https://redirect.github.com/DuendeSoftware/foss)) | `4.2.0` →\n`4.3.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Duende.AccessTokenManagement/4.3.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Duende.AccessTokenManagement/4.2.0/4.3.0?slim=true)\n| nuget | minor |\n| [Hl7.Fhir.Base](https://redirect.github.com/FirelyTeam/firely-net-sdk)\n| `6.6.0` → `6.7.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Hl7.Fhir.Base/6.7.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Hl7.Fhir.Base/6.6.0/6.7.0?slim=true)\n| nuget | minor |\n| [Hl7.Fhir.R4](https://redirect.github.com/FirelyTeam/firely-net-sdk) |\n`6.6.0` → `6.7.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Hl7.Fhir.R4/6.7.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Hl7.Fhir.R4/6.6.0/6.7.0?slim=true)\n| nuget | minor |\n| [Microsoft.AspNetCore.Authentication.JwtBearer](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.12` →\n`10.0.13` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.AspNetCore.Authentication.JwtBearer/10.0.13?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.AspNetCore.Authentication.JwtBearer/10.0.12/10.0.13?slim=true)\n| nuget | patch |\n| [Microsoft.AspNetCore.Components.WebAssembly](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.12` →\n`10.0.13` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.AspNetCore.Components.WebAssembly/10.0.13?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.AspNetCore.Components.WebAssembly/10.0.12/10.0.13?slim=true)\n| nuget | patch |\n|\n[Microsoft.AspNetCore.Components.WebAssembly.DevServer](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.12` →\n`10.0.13` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.AspNetCore.Components.WebAssembly.DevServer/10.0.13?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.AspNetCore.Components.WebAssembly.DevServer/10.0.12/10.0.13?slim=true)\n| nuget | patch |\n| [Microsoft.AspNetCore.Mvc.Testing](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.12` →\n`10.0.13` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.AspNetCore.Mvc.Testing/10.0.13?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.AspNetCore.Mvc.Testing/10.0.12/10.0.13?slim=true)\n| nuget | patch |\n| [Microsoft.Extensions.Http.Polly](https://asp.net/)\n([source](https://redirect.github.com/dotnet/dotnet)) | `10.0.12` →\n`10.0.13` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.Extensions.Http.Polly/10.0.13?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.Extensions.Http.Polly/10.0.12/10.0.13?slim=true)\n| nuget | patch |\n| [Microsoft.NET.Test.Sdk](https://redirect.github.com/microsoft/vstest)\n| `18.10.1` → `18.11.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.NET.Test.Sdk/18.11.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.NET.Test.Sdk/18.10.1/18.11.0?slim=true)\n| nuget | minor |\n|\n[Microsoft.Testing.Extensions.CodeCoverage](https://redirect.github.com/microsoft/codecoverage)\n| `18.11.2` → `18.12.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.Testing.Extensions.CodeCoverage/18.12.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.Testing.Extensions.CodeCoverage/18.11.2/18.12.0?slim=true)\n| nuget | minor |\n| [OpenTelemetry](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet))\n| `1.19.1` → `1.20.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry/1.20.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry/1.19.1/1.20.0?slim=true)\n| nuget | minor |\n|\n[OpenTelemetry.Exporter.OpenTelemetryProtocol](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet))\n| `1.19.1` → `1.20.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Exporter.OpenTelemetryProtocol/1.20.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Exporter.OpenTelemetryProtocol/1.19.1/1.20.0?slim=true)\n| nuget | minor |\n| [OpenTelemetry.Extensions.Hosting](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet))\n| `1.19.1` → `1.20.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Extensions.Hosting/1.20.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Extensions.Hosting/1.19.1/1.20.0?slim=true)\n| nuget | minor |\n| [OpenTelemetry.Instrumentation.AspNetCore](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet-contrib))\n| `1.19.0` → `1.20.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Instrumentation.AspNetCore/1.20.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Instrumentation.AspNetCore/1.19.0/1.20.0?slim=true)\n| nuget | minor |\n| [OpenTelemetry.Instrumentation.Http](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet-contrib))\n| `1.19.0` → `1.20.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Instrumentation.Http/1.20.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Instrumentation.Http/1.19.0/1.20.0?slim=true)\n| nuget | minor |\n| [OpenTelemetry.Instrumentation.Runtime](https://opentelemetry.io/)\n([source](https://redirect.github.com/open-telemetry/opentelemetry-dotnet-contrib))\n| `1.19.0` → `1.20.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/OpenTelemetry.Instrumentation.Runtime/1.20.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/OpenTelemetry.Instrumentation.Runtime/1.19.0/1.20.0?slim=true)\n| nuget | minor |\n|\n[Swashbuckle.AspNetCore](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore)\n| `10.2.3` → `10.3.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Swashbuckle.AspNetCore/10.3.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Swashbuckle.AspNetCore/10.2.3/10.3.0?slim=true)\n| nuget | minor |\n| [dotnet-sdk](https://redirect.github.com/dotnet/sdk) | `10.0.401` →\n`10.0.402` |\n![age](https://developer.mend.io/api/mc/badges/age/dotnet-version/dotnet-sdk/10.0.402?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/dotnet-version/dotnet-sdk/10.0.401/10.0.402?slim=true)\n| dotnet-sdk | patch |\n| [xunit.v3.mtp-v2](https://redirect.github.com/xunit/xunit) | `4.0.1` →\n`4.0.2` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/xunit.v3.mtp-v2/4.0.2?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/xunit.v3.mtp-v2/4.0.1/4.0.2?slim=true)\n| nuget | patch |\n| [zensical](https://redirect.github.com/zensical/zensical)\n([changelog](https://zensical.org/docs/changelog/)) | `==0.0.67` →\n`==0.0.71` |\n![age](https://developer.mend.io/api/mc/badges/age/pypi/zensical/0.0.71?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/pypi/zensical/0.0.67/0.0.71?slim=true)\n| project.dependencies | patch |\n\n---\n\n### Release Notes\n\n<details>\n<summary>confluentinc/confluent-kafka-dotnet (Confluent.Kafka)</summary>\n\n###\n[`v2.16.0`](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/blob/HEAD/CHANGELOG.md#2160)\n\n#### New experimental features\n\n- Serializers and deserializers can now be supplied to a producer,\ndependent\nproducer or consumer as a *builder*, which the client invokes during its\nown\nconstruction and then owns, disposing it (and any Schema Registry client\nit\ncreated) when the client itself is disposed. See the\n[AvroGeneric](examples/AvroGeneric) example\n([#&#8203;2661](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2661))\n- Schema Registry serde builders are now available for each supported\nformat,\n  each configurable with a Schema Registry client you own or the\nconfiguration to build one from. See the\n[AvroGeneric](examples/AvroGeneric),\n[JsonSerialization](examples/JsonSerialization) and\n[Protobuf](examples/Protobuf) examples\n([#&#8203;2661](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2661))\n- Producers, consumers and admin clients can now report the id of the\nKafka\ncluster they're connected to\n([#&#8203;2661](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2661))\n- The Kafka cluster id is now propagated to Schema Registry serializers\nand\ndeserializers automatically, and resolved lazily on first use, so\nsubject\nname strategies based on the cluster no longer need it configured by\nhand.\nSee the [AvroGenericAssociation](examples/AvroGenericAssociation)\nexample\n([#&#8203;2661](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2661))\n- The serializer and deserializer builder APIs (`Set*SerializerBuilder`\nand\n`Set*DeserializerBuilder` on the producer and consumer builders, the\nSchema\nRegistry serde builders), `IClusterIdAware`, `ISerdeDisposable`,\n`SerdeExtensions`\nand the `ClusterId` client extension are **experimental**: they are\nsubject to\nchange or removal in a future release\n([#&#8203;2661](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2661))\n\n#### Enhancements\n\n- References librdkafka.redist 2.16.0. Refer to the [librdkafka v2.16.0\nrelease\nnotes](https://redirect.github.com/confluentinc/librdkafka/releases/tag/v2.16.0)\nfor more information\n- Add support for saving Azure key version with DEK\n([#&#8203;2641](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2641))\n- Pass context when clients make KEK calls to DEK Registry\n([#&#8203;2642](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2642))\n- Add support for inline validation rules\n([#&#8203;2651](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2651))\n- Add Variant, Decimal, and Timestamp CEL functions\n([#&#8203;2653](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2653))\n- Add support for linux-s390x (IBM Z), with CI coverage on s390x\n([#&#8203;2646](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2646))\n- Add support for AliCloud KMS provider with CSFLE/CSPE\n([#&#8203;2648](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2648))\n- Schema Registry examples now use the serde builder API\n([#&#8203;2661](https://redirect.github.com/confluentinc/confluent-kafka-dotnet/issues/2661))\n\n#### Fixes\n\n- Fix the CSFLE ciphertext version field being written little-endian on\nbig-endian\n  platforms, which made encrypted fields unreadable across architectures\n- Fix message `Timestamp.Type` being read as `NotAvailable` on\nbig-endian platforms\n(e.g. s390x): librdkafka's `rd_kafka_timestamp_type_t` out-parameter (a\n32-bit enum)\nwas marshalled as a pointer-sized `IntPtr`, so the value landed in the\nhigh word and\n  was truncated to `0` on big-endian\n\n</details>\n\n<details>\n<summary>DuendeSoftware/foss (Duende.AccessTokenManagement)</summary>\n\n###\n[`v4.3.0`](https://redirect.github.com/DuendeSoftware/foss/compare/4.2.0...4.3.0)\n\n[Compare\nSource](https://redirect.github.com/DuendeSoftware/foss/compare/4.2.0...4.3.0)\n\n</details>\n\n<details>\n<summary>FirelyTeam/firely-net-sdk (Hl7.Fhir.Base)</summary>\n\n###\n[`v6.7.0`](https://redirect.github.com/FirelyTeam/firely-net-sdk/releases/tag/v6.7.0):\n6.7.0\n\n##### Intro:\n\nThis release adds an opt-in `PrimitiveValueConverter` hook to the POCO\ndeserializers, makes the\nJSON parser reject a primitive whose value is supplied twice, keeps\nrepeated same-url extensions in\nsnapshots, makes the `SnapshotGenerator` report differential paths with\nan empty segment, and fixes\nFHIRPath evaluation over invalid primitive values.\n`LocalTerminologyService` now returns the full\n`$validate-code` result shape, and the terminology services accept\nrepeated `tx-resource`\nparameters. There are no breaking API changes, but several changes alter\nbehaviour or output that\ncallers may depend on: the new fatal parser issue `JSON135`, the text of\n`OperationOutcome.IssueComponent.ToString()`, warnings on inactive codes\nfrom `$validate-code`, and\n`$expand` rejecting repeated 0..1 parameters. See the behavioural notes\nbelow.\n\n**Serialization**\n\n- New `DeserializerSettings.PrimitiveValueConverter` lets callers fix a\nwrongly typed primitive while\nparsing, such as `\"valueInteger\": \"42\"` or `\"active\": \"true\"`, in both\nJson and Xml and in every\nmode, `Ostrich` included. It is called only for values that are not of\nthe type the primitive\nstores, plus `date`, `dateTime`, `instant`, `time` and `integer64`,\nwhich can be the right type in\nthe wrong format. If it declines (returns the input) or throws a\n`FormatException`,\n`InvalidCastException` or `OverflowException`, the usual error is\nreported; any other exception\n  propagates. When not set, parsing is unaffected.\n- The string overloads of the Json deserializers now encode into a\npooled buffer instead of\nallocating a fresh `byte[]` per parse, and the reader options the SDK\nuses are now public as\n`SerializationUtil.FhirJsonReaderOptions`. See issue\n[#&#8203;3598](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3598).\n- An exception thrown while reading an Xml attribute is no longer hidden\nbehind\n  \"Can only exit from a resource part.\".\n- The JSON parser now reports a fatal issue, `JSON135`\n(`PRIMITIVE_VALUE_SUPPLIED_TWICE`), when a\nprimitive gets its value from both `name` and `_name`, e.g.\n`{\"name\":\"John\",\"_name\":\"Johnny\"}`.\nThe second value used to be dropped silently, with only the non-fatal\n`JSON134`, so the\n`NoOverflow`, `Recoverable` and `BackwardsCompatible` modes accepted the\ndata loss. This applies in\neither property order and to elements of primitive arrays. `JSON134`\nstays a non-fatal error for\na primitive under `_name` when no value was set yet. See issue\n[#&#8203;3533](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3533).\n\n> **Behavioural note:** JSON that parsed before, with a non-fatal\n`JSON134`, now fails to parse in\n> every mode except `Ostrich` when both `name` and `_name` carry a\nvalue.\n\n**FHIRPath**\n\n- An unparseable `instant`, `integer64`, `integer`, `positiveInt` or\n`unsignedInt` value no longer\nmakes almost every FHIRPath expression that reaches it throw. Its value\nis now the unparsed\nstring, as it already was for `date`, `dateTime` and `time`. Before,\neven `$this is Reference`\n  threw an `InvalidOperationException` for such a node, so a sweep like\n`descendants().where($this is Reference)` failed on the whole resource.\nThe `Value` getter of the\n  POCO still throws.\n\n**Snapshot generation**\n\n- The `SnapshotGenerator` now reports a specific issue\n(`PROFILE_ELEMENTDEF_INVALID_PATH`, code\n10021\\) for a differential element whose path has an empty segment, such\nas `Observation...unit`,\nand skips that element. Such an element used to produce a phantom\nelement `Observation.` in the\nsnapshot, and any constraint on it, like a `fixedString`, was silently\nlost. The rest of the\ndifferential is still processed. See issue\n[#&#8203;3591](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3591).\n- When a differential has several extensions with the same url, e.g.\nrepeated `additional-binding`\nextensions on a binding, and the base already has one with that url, the\nsnapshot now keeps all of\nthem. The first replaces the base extension, the rest are added next to\nit. They used to overwrite\neach other, so only the last one survived. See issue\n[#&#8203;3625](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3625).\n\n**Terminology**\n\n- `LocalTerminologyService`'s ValueSet `$validate-code` now returns the\nfull result shape the HL7\nterminology ecosystem tests expect: `code`, `system`, `version` and\n`display`, plus an `issues`\nOperationOutcome with tx-issue-type codings and expressions relative to\nthe input, for both valid\nand invalid codes. A `CodeableConcept` input is echoed back, and\ninactive or non-active concepts\nget `inactive`/`status` parameters. `ValidateCodeResult` has new\nproperties to read and write\nthese (`CodeableConcept`, `NormalizedCode`, `CausedByUnknownSystem`,\n`UnknownSystem`, `Inactive`,\n`Status`), and new `Add*` extension methods put an issue in its\n`issues`. See issue\n[#&#8203;3631](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3631).\n\n> **Behavioural note:** an inactive, retired or deprecated code now\nreturns `result = true` with a\n> `message` (a `code-comment` warning). For an unknown system, `message`\nnow also says the\n  > CodeSystem could not be found.\n- The terminology services no longer reject an operation with repeated\n`tx-resource` parameters,\nwhich the tx-ecosystem IG requires servers to accept. Only repeats of\nthe operation's 0..1\nparameters are rejected; known 0..\\* and unknown parameters pass\nthrough. Each typed `*Parameters`\n  class now lists its parameters in a public `KnownParameters`.\n\n> **Behavioural note:** `$expand` now gets the same check, so a repeated\n`url`, `filter`, `count`\n  > etc. is rejected where it used to be accepted.\n\n**Other**\n\n- `PocoNode.Resolve(\"#id\")` no longer throws a `NullReferenceException`\nwhen the resource has a\ncontained resource without an `id` before the one it refers to. A\ncontained resource without an\n`id` does not match, and resolution continues. See issue\n[#&#8203;3613](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3613).\n- `OperationOutcome.IssueComponent.ToString()` now writes a space before\n`(further diagnostics:`,\nas it already did before `(at`. See issue\n[#&#8203;3621](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3621).\n\n> **Behavioural note:** this changes the rendered text of an issue.\nTests that compare against\n  > `ToString()` output need new expected strings.\n\n##### Changes:\n\n-\n[#&#8203;3634](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3634):\nVONK-10389: Allow repeated tx-resource parameters\n-\n[#&#8203;3633](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3633):\nReturn the full $validate-code result shape from LocalTerminologyService\n-\n[#&#8203;3631](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3631):\nExtend ValidateCodeResult with the full $validate-code output and fill\nit from LocalTerminologyService\n-\n[#&#8203;3627](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3627):\nVONK-10125: Add opt-in primitive value converter to the POCO\ndeserializers, pool Json string parsing\n-\n[#&#8203;3598](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3598):\nPOCO deserialization: byte/stream entry points and pooled re-encoding of\nJSON strings\n-\n[#&#8203;3626](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3626):\nKeep repeated same-url extensions in snapshots\n-\n[#&#8203;3625](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3625):\nSnapshot generator loses repeated extensions with the same URL (e.g.\nadditional-binding) when the base already has one\n-\n[#&#8203;3624](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3624):\nBump the runtime-dependencies-minor group with 2 updates\n-\n[#&#8203;3623](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3623):\nMake FHIRPath tolerate unparseable primitive values\n-\n[#&#8203;3616](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3616):\nDocument the changes for release, bump version to 6.6.0\n\n<details><summary><b>See More</b></summary>\n\n-\n[#&#8203;3622](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3622):\nAdd space before \"(further diagnostics:\" in issue ToString\n-\n[#&#8203;3619](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3619):\nAdd test for Base-typed inline children\n([#&#8203;3597](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3597))\n-\n[#&#8203;3597](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3597):\nSnapshotGenerator: inline children of a new element typed `Base` are\nemitted one level up (segment dropped from path and id)\n-\n[#&#8203;3620](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3620):\nReport and skip '..' in differential paths\n-\n[#&#8203;3618](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3618):\nReport fatal error when name and \\_name both set\n-\n[#&#8203;3533](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3533):\nJSON deserializer silently drops a primitive value when both 'name' and\n'\\_name' supply one (non-fatal)\n-\n[#&#8203;3617](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3617):\nFix Resolve NRE on contained resource w/o id\n-\n[#&#8203;3591](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3591):\nSnapshotGenerator: '..' in differential path produces phantom element\nand silently drops the constraint\n-\n[#&#8203;3576](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3576):\nfeat: add the missing NOOVERFLOW preset to both deserializers\n([#&#8203;3532](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3532))\n-\n[#&#8203;3613](https://redirect.github.com/FirelyTeam/firely-net-sdk/issues/3613):\nPocoNode.Resolve throws NullReferenceException when a contained resource\nhas no id\n\nThis list of changes was [auto\ngenerated](https://dev.azure.com/firely/firely-net-sdk/_build/results?buildId=83495\\&view=logs).</details>\n\n</details>\n\n<details>\n<summary>dotnet/dotnet\n(Microsoft.AspNetCore.Authentication.JwtBearer)</summary>\n\n###\n[`v10.0.13`](https://redirect.github.com/dotnet/dotnet/compare/v10.0.12...v10.0.13)\n\n</details>\n\n<details>\n<summary>microsoft/vstest (Microsoft.NET.Test.Sdk)</summary>\n\n###\n[`v18.11.0`](https://redirect.github.com/microsoft/vstest/releases/tag/v18.11.0)\n\n<!-- Release notes generated using configuration in .github/release.yml\nat rel/18.11 -->\n\n#### What's Changed\n\n- Fix TRX logger mangling astral-plane (non-BMP) characters by\n[@&#8203;azat-msft](https://redirect.github.com/azat-msft) in\n[#&#8203;16318](https://redirect.github.com/microsoft/vstest/pull/16318)\n- Disable the MTP testhost by default by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16337](https://redirect.github.com/microsoft/vstest/pull/16337)\n- Fix OS-blind apphost probe in MTP bridge on Unix (regression from\n[#&#8203;16201](https://redirect.github.com/microsoft/vstest/issues/16201))\nby [@&#8203;azat-msft](https://redirect.github.com/azat-msft) in\n[#&#8203;16336](https://redirect.github.com/microsoft/vstest/pull/16336)\n- Retarget the MTP client onto\nMicrosoft.Testing.Platform.ServerMode.Client.Sources by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16300](https://redirect.github.com/microsoft/vstest/pull/16300)\n- Pass arguments to PowerShell with -File instead of -command by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16363](https://redirect.github.com/microsoft/vstest/pull/16363)\n- Respect console logger verbosity from a settings file by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16044](https://redirect.github.com/microsoft/vstest/pull/16044)\n- Fix backslash normalization in VSTestCLIRunSettings on Unix by\n[@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;15795](https://redirect.github.com/microsoft/vstest/pull/15795)\n- Docs: fix typo explicitely -> explicitly by\n[@&#8203;vaibhav8a](https://redirect.github.com/vaibhav8a) in\n[#&#8203;16397](https://redirect.github.com/microsoft/vstest/pull/16397)\n- Copy acceptance test diagnostic logs into the test results directory\nby [@&#8203;nohwnd](https://redirect.github.com/nohwnd) in\n[#&#8203;16403](https://redirect.github.com/microsoft/vstest/pull/16403)\n\n#### New Contributors\n\n- [@&#8203;vaibhav8a](https://redirect.github.com/vaibhav8a) made their\nfirst contribution in\n[#&#8203;16397](https://redirect.github.com/microsoft/vstest/pull/16397)\n\n**Full Changelog**:\n<https://github.com/microsoft/vstest/compare/v18.10.1...v18.11.0>\n\n</details>\n\n<details>\n<summary>open-telemetry/opentelemetry-dotnet (OpenTelemetry)</summary>\n\n###\n[`v1.20.0`](https://redirect.github.com/open-telemetry/opentelemetry-dotnet/blob/HEAD/RELEASENOTES.md#1200)\n\nRelease details:\n[1.20.0](https://redirect.github.com/open-telemetry/opentelemetry-dotnet/releases/tag/core-1.20.0)\n\n- **Breaking change** `byte[]` attributes longer than the configured\nattribute value\nlength limit (for example `OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT`) are now\ntruncated\n  to that many bytes instead of being exported in full.\n- **Breaking change** `OTEL_EVENT_ATTRIBUTE_COUNT_LIMIT` and\n`OTEL_LINK_ATTRIBUTE_COUNT_LIMIT`\nnow fall back to `OTEL_ATTRIBUTE_COUNT_LIMIT` (then the default of 128)\nwhen\n  not set, instead of falling back to `OTEL_SPAN_ATTRIBUTE_COUNT_LIMIT`.\n- **Breaking change** Instrumentation scope attributes exported from\n`ActivitySource.Tags`\n  are now limited by `OTEL_ATTRIBUTE_COUNT_LIMIT` instead of\n  `OTEL_SPAN_ATTRIBUTE_COUNT_LIMIT`. Only applications that set\n`OTEL_SPAN_ATTRIBUTE_COUNT_LIMIT` to a different value from\n`OTEL_ATTRIBUTE_COUNT_LIMIT`\n  are affected.\n- **Breaking change** (pre-release only versions): `LoggerProvider`'s\nprotected\n`TryCreateLogger(string? name, out Logger? logger)` extensibility method\nhas\nbeen replaced by `TryCreateLogger(LoggerOptions options, out Logger?\nlogger)`.\n- Added `OpenTelemetryLoggerOptions.Version` and\n`OpenTelemetryLoggerOptions.SchemaUrl` options to set an instrumentation\n  library version and schema URL applied to logs emitted via the\n  `Microsoft.Extensions.Logging` `ILogger` integration.\n- Added `Logger.SchemaUrl` and a `LoggerProvider.GetLogger` overload\naccepting\n  a schema URL parameter using the new `LoggerOptions` type.\n- `ConsoleLogRecordExporter` now writes an `Instrumentation scope\n(Logger)` section\n  (Name, Version, and Schema URL) for each exported `LogRecord`.\n- The OTLP log exporter now exports `schema_url` on `ScopeLogs`. Log\nrecords\nemitted from a `Logger` with a schema URL set now populate\n`ScopeLogs.schema_url`\n  in exported OTLP requests.\n- Custom TLS certificate options (custom CA trust and mTLS) and the\nexperimental\ndisk retry feature are explicitly marked as not supported on\nbrowser-based\n  platforms (e.g. Blazor).\n- Suppressed informational HTTP client logs from OTLP exporters by\ndefault.\n- Added improved self-diagnostics logging to improve the support\nexperience.\n- Self-diagnostic features are explicitly disabled on browser-based\nplatforms\n  (e.g. Blazor) and are now non-functional.\n- Added `TracerProviderBuilder.ConfigureSampler` intended for library\nauthors\nwho need to influence sampling without overriding a sampler the\napplication\n  has configured.\n- Fixed `SuppressInstrumentationScope` reference counts leaking between\n  inherited asynchronous execution contexts.\n- Fixed `ResourceBuilder.CreateDefault` ignoring\n`OTEL_RESOURCE_ATTRIBUTES` and\n`OTEL_SERVICE_NAME` environment variables when the host `IConfiguration`\ndoes\n  not contain those settings.\n- Fixed a race that could incorrectly route a concurrent metric\nmeasurement to\nthe overflow point when its tag set was published as the cardinality\nlimit was\n  reached.\n- Fixed `LogRecord.CategoryName` resetting the current `Logger`'s\n`Version` to\n  `null` when set to a new value.\n- Bounded the internal cache of `Logger` scopes keyed by instrumentation\nto\n  avoid unbounded growth.\n- Fixed the `MeterProvider` and its `IServiceProvider` being kept alive\nafter the\nhost is disposed when using `Microsoft.Extensions.Diagnostics` versions\nprior\n  to .NET 9.0.\n- Fixed a race condition between recording a measurement and collecting\na\nhistogram with delta temporality that could cause an empty data point to\nbe exported.\n- Improved metric collection performance after metric streams are\nremoved by\n  compacting metric storage during collection.\n- Optimize performance of `BaggagePropagator.Inject` on .NET Framework\nusing\n  lookup tables.\n- Avoid closure allocations when serializing cached metric metadata.\n- Avoid logging malformed HTTP request headers in the exporter's\ndiagnostic log.\n- Avoid buffering the whole compressed request body in memory before\nsending it\n  on .NET Framework when using HTTP/protobuf with gzip compression.\n- Fixed log records being dropped when using\n`OtlpExportProtocol.HttpProtobuf`\nwith `IHttpClientFactory` integration if the first export of logs\nhappens while\nthe `IServiceProvider` is being disposed. The exporter now falls back to\nthe\n  default `HttpClient` instead of failing the export.\n- Dozens of other minor performance improvements.\n\n</details>\n\n<details>\n<summary>domaindrivendev/Swashbuckle.AspNetCore\n(Swashbuckle.AspNetCore)</summary>\n\n###\n[`v10.3.0`](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/releases/tag/v10.3.0)\n\n##### What's Changed\n\n- Change nullability handling for schemas with `anyOf`, `allOf`, `oneOf`\nor `type = null` by\n[@&#8203;ldeluigi](https://redirect.github.com/ldeluigi) in\n[#&#8203;3976](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/3976)\n- Handle leading slash on route prefix during index.html GET by\n[@&#8203;superstator](https://redirect.github.com/superstator) in\n[#&#8203;4084](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4084)\n- Move NRT handling to DataContractResolver by\n[@&#8203;desjoerd](https://redirect.github.com/desjoerd) in\n[#&#8203;3998](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/3998)\n- Clarify nullable behaviour by\n[@&#8203;martincostello](https://redirect.github.com/martincostello) in\n[#&#8203;4094](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4094)\n- Fix review issues for caching and encoding by\n[@&#8203;martincostello](https://redirect.github.com/martincostello) in\n[#&#8203;4093](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4093)\n- Support multiple response types per status code by\n[@&#8203;DeagleGross](https://redirect.github.com/DeagleGross) in\n[#&#8203;4100](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4100)\n- Add coverage for `AddServer` across multiple documents by\n[@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri) in\n[#&#8203;4115](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4115)\n- Fix Swagger sort key fallback for empty endpoint tags metadata by\n[@&#8203;AleksaMCode](https://redirect.github.com/AleksaMCode) in\n[#&#8203;4125](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4125)\n- Fix parameter matching to consider location when names collide by\n[@&#8203;nonsoikem](https://redirect.github.com/nonsoikem) in\n[#&#8203;4126](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4126)\n- Emit form encoding for properties of a complex form parameter by\n[@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri) in\n[#&#8203;4128](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4128)\n- Keep a property's own summary when comments come from several XML\nfiles by\n[@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri) in\n[#&#8203;4129](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4129)\n- Strip query string from relative document URLs in Swagger UI by\n[@&#8203;GabeSilvaDev](https://redirect.github.com/GabeSilvaDev) in\n[#&#8203;4152](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4152)\n- Document that System.Text.Json ignores `[EnumMember]` by\n[@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri) in\n[#&#8203;4127](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4127)\n- Drop the regex route constraint from the default Swagger route by\n[@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri) in\n[#&#8203;4130](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4130)\n- Serialize custom values in the UI `ConfigObject.AdditionalItems` by\n[@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri) in\n[#&#8203;4132](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4132)\n- Use Minimal APIs JSON options when MVC is not registered by\n[@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri) in\n[#&#8203;4116](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4116)\n- Resolve enum contracts against the underlying type under source\ngeneration by\n[@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri) in\n[#&#8203;4131](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4131)\n\n##### New Contributors\n\n- [@&#8203;ldeluigi](https://redirect.github.com/ldeluigi) made their\nfirst contribution in\n[#&#8203;3976](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/3976)\n- [@&#8203;superstator](https://redirect.github.com/superstator) made\ntheir first contribution in\n[#&#8203;4084](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4084)\n- [@&#8203;desjoerd](https://redirect.github.com/desjoerd) made their\nfirst contribution in\n[#&#8203;3998](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/3998)\n- [@&#8203;RaphaelFakhri](https://redirect.github.com/RaphaelFakhri)\nmade their first contribution in\n[#&#8203;4115](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4115)\n- [@&#8203;AleksaMCode](https://redirect.github.com/AleksaMCode) made\ntheir first contribution in\n[#&#8203;4125](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4125)\n- [@&#8203;nonsoikem](https://redirect.github.com/nonsoikem) made their\nfirst contribution in\n[#&#8203;4126](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4126)\n- [@&#8203;GabeSilvaDev](https://redirect.github.com/GabeSilvaDev) made\ntheir first contribution in\n[#&#8203;4152](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4152)\n- [@&#8203;JulianThoms](https://redirect.github.com/JulianThoms) made\ntheir first contribution in\n[#&#8203;4182](https://redirect.github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/4182)\n\n**Full Changelog**:\n<https://github.com/domaindrivendev/Swashbuckle.AspNetCore/compare/v10.2.3...v10.3.0>\n\n</details>\n\n<details>\n<summary>dotnet/sdk (dotnet-sdk)</summary>\n\n###\n[`v10.0.402`](https://redirect.github.com/dotnet/sdk/releases/tag/v10.0.402):\n.NET 10.0.13\n\n[Compare\nSource](https://redirect.github.com/dotnet/sdk/compare/v10.0.401...v10.0.402)\n\n[Release](https://redirect.github.com/dotnet/core/releases/tag/v10.0.13)\n\n#### What's Changed\n\n- Fix internal feed auth in unofficial CI pipeline by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;54362](https://redirect.github.com/dotnet/sdk/pull/54362)\n- \\[automated] Merge branch 'release/8.0.1xx' => 'release/8.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;54364](https://redirect.github.com/dotnet/sdk/pull/54364)\n- \\[release/8.0.4xx] Fix internal feed auth in unofficial CI pipeline by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;54363](https://redirect.github.com/dotnet/sdk/pull/54363)\n- Update branding to 8.0.129 by\n[@&#8203;vseanreesermsft](https://redirect.github.com/vseanreesermsft)\nin [#&#8203;54561](https://redirect.github.com/dotnet/sdk/pull/54561)\n- Update branding to 8.0.423 by\n[@&#8203;vseanreesermsft](https://redirect.github.com/vseanreesermsft)\nin [#&#8203;54562](https://redirect.github.com/dotnet/sdk/pull/54562)\n- \\[release/8.0.1xx] Update dependencies from dotnet/roslyn-analyzers by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54421](https://redirect.github.com/dotnet/sdk/pull/54421)\n- \\[release/8.0.4xx] Update dependencies from\ndotnet/source-build-externals by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54549](https://redirect.github.com/dotnet/sdk/pull/54549)\n- \\[release/8.0.4xx] Update dependencies from dotnet/arcade by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54480](https://redirect.github.com/dotnet/sdk/pull/54480)\n- \\[release/8.0.1xx] Update dependencies from dotnet/arcade by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54479](https://redirect.github.com/dotnet/sdk/pull/54479)\n- \\[release/8.0.1xx] Update dependencies from\ndotnet/source-build-externals by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54531](https://redirect.github.com/dotnet/sdk/pull/54531)\n- \\[automated] Merge branch 'release/8.0.1xx' => 'release/8.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;54568](https://redirect.github.com/dotnet/sdk/pull/54568)\n- \\[release/8.0.1xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54571](https://redirect.github.com/dotnet/sdk/pull/54571)\n- \\[release/8.0.1xx] Update dependencies from dotnet/source-build-assets\nby\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54548](https://redirect.github.com/dotnet/sdk/pull/54548)\n- \\[automated] Merge branch 'release/8.0.1xx' => 'release/8.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;54572](https://redirect.github.com/dotnet/sdk/pull/54572)\n- \\[release/8.0.4xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54588](https://redirect.github.com/dotnet/sdk/pull/54588)\n- \\[release/9.0.1xx] Update dependencies from dotnet/source-build-assets\nby\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54406](https://redirect.github.com/dotnet/sdk/pull/54406)\n- \\[release/9.0.1xx] Update dependencies from dotnet/scenario-tests by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54465](https://redirect.github.com/dotnet/sdk/pull/54465)\n- \\[release/9.0.1xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54569](https://redirect.github.com/dotnet/sdk/pull/54569)\n- \\[automated] Merge branch 'release/8.0.4xx' => 'release/9.0.1xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;54381](https://redirect.github.com/dotnet/sdk/pull/54381)\n- Merging internal commits for release/9.0.1xx by\n[@&#8203;vseanreesermsft](https://redirect.github.com/vseanreesermsft)\nin [#&#8203;54665](https://redirect.github.com/dotnet/sdk/pull/54665)\n- Merging internal commits for release/8.0.4xx by\n[@&#8203;vseanreesermsft](https://redirect.github.com/vseanreesermsft)\nin [#&#8203;54666](https://redirect.github.com/dotnet/sdk/pull/54666)\n- \\[release/9.0.1xx] Do not synchronize docx from the AAD submodule by\n[@&#8203;premun](https://redirect.github.com/premun) in\n[#&#8203;54678](https://redirect.github.com/dotnet/sdk/pull/54678)\n- \\[release/8.0.4xx] Update dependencies from dotnet/format by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54697](https://redirect.github.com/dotnet/sdk/pull/54697)\n- Merging internal commits for release/8.0.1xx by\n[@&#8203;vseanreesermsft](https://redirect.github.com/vseanreesermsft)\nin [#&#8203;54667](https://redirect.github.com/dotnet/sdk/pull/54667)\n- .NET Source-Build 9.0.118 June 2026 Updates by\n[@&#8203;dotnet-sb-bot](https://redirect.github.com/dotnet-sb-bot) in\n[#&#8203;54670](https://redirect.github.com/dotnet/sdk/pull/54670)\n- \\[automated] Merge branch 'release/8.0.1xx' => 'release/8.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;54721](https://redirect.github.com/dotnet/sdk/pull/54721)\n- Avoid possible test timeout due to very verbose output by\n[@&#8203;dsplaisted](https://redirect.github.com/dsplaisted) in\n[#&#8203;54642](https://redirect.github.com/dotnet/sdk/pull/54642)\n- \\[release/9.0.1xx] Update dependencies from dotnet/source-build-assets\nby\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54674](https://redirect.github.com/dotnet/sdk/pull/54674)\n- \\[automated] Merge branch 'release/8.0.4xx' => 'release/9.0.1xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;54671](https://redirect.github.com/dotnet/sdk/pull/54671)\n- Use truncating logger by default for all tests by\n[@&#8203;dsplaisted](https://redirect.github.com/dsplaisted) in\n[#&#8203;54795](https://redirect.github.com/dotnet/sdk/pull/54795)\n- \\[release/8.0.4xx] Update branding to 8.0.424 by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55149](https://redirect.github.com/dotnet/sdk/pull/55149)\n- \\[release/8.0.1xx] Update branding to 8.0.130 by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55150](https://redirect.github.com/dotnet/sdk/pull/55150)\n- \\[release/9.0.1xx] Update branding to 9.0.120 by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55152](https://redirect.github.com/dotnet/sdk/pull/55152)\n- \\[release/9.0.3xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54570](https://redirect.github.com/dotnet/sdk/pull/54570)\n- \\[automated] Merge branch 'release/9.0.1xx' => 'release/9.0.3xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;54750](https://redirect.github.com/dotnet/sdk/pull/54750)\n- \\[automated] Merge branch 'release/8.0.1xx' => 'release/8.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55168](https://redirect.github.com/dotnet/sdk/pull/55168)\n- Replace ubuntu22 VM and Helix images with Azure Linux 3 by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;53792](https://redirect.github.com/dotnet/sdk/pull/53792)\n- \\[release/9.0.3xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55210](https://redirect.github.com/dotnet/sdk/pull/55210)\n- Work around license scan timeout by scanning source-build-assets\nserially by [@&#8203;mthalman](https://redirect.github.com/mthalman) in\n[#&#8203;55219](https://redirect.github.com/dotnet/sdk/pull/55219)\n- \\[automated] Merge branch 'release/8.0.4xx' => 'release/9.0.1xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55158](https://redirect.github.com/dotnet/sdk/pull/55158)\n- \\[release/9.0.1xx] Update dependencies from dotnet/arcade by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54054](https://redirect.github.com/dotnet/sdk/pull/54054)\n- Fix VMR sync: update aspire remote name by\n[@&#8203;mthalman](https://redirect.github.com/mthalman) in\n[#&#8203;55234](https://redirect.github.com/dotnet/sdk/pull/55234)\n- \\[release/9.0.1xx] Update dependencies from dotnet/source-build-assets\nby\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54911](https://redirect.github.com/dotnet/sdk/pull/54911)\n- \\[release/8.0.1xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55183](https://redirect.github.com/dotnet/sdk/pull/55183)\n- \\[automated] Merge branch 'release/8.0.1xx' => 'release/8.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55207](https://redirect.github.com/dotnet/sdk/pull/55207)\n- \\[release/9.0.3xx] Update dependencies from dotnet/msbuild by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;54912](https://redirect.github.com/dotnet/sdk/pull/54912)\n- \\[release/8.0.1xx] Disable network isolation on internal PR builds;\nenforce CFSClean3 on official by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;55220](https://redirect.github.com/dotnet/sdk/pull/55220)\n- \\[release/9.0.1xx] Update dependencies from\ndotnet/source-build-externals by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55306](https://redirect.github.com/dotnet/sdk/pull/55306)\n- \\[automated] Merge branch 'release/8.0.1xx' => 'release/8.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55288](https://redirect.github.com/dotnet/sdk/pull/55288)\n- Merging internal commits for release/8.0.4xx by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55279](https://redirect.github.com/dotnet/sdk/pull/55279)\n- Merging internal commits for release/9.0.3xx by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55278](https://redirect.github.com/dotnet/sdk/pull/55278)\n- \\[release/9.0.1xx] Update dependencies from\ndotnet/source-build-externals by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55320](https://redirect.github.com/dotnet/sdk/pull/55320)\n- Merging internal commits for release/9.0.1xx by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55281](https://redirect.github.com/dotnet/sdk/pull/55281)\n- \\[automated] Merge branch 'release/9.0.1xx' => 'release/9.0.3xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55233](https://redirect.github.com/dotnet/sdk/pull/55233)\n- .NET Source-Build 9.0.119 July 2026 Updates by\n[@&#8203;dotnet-sb-bot](https://redirect.github.com/dotnet-sb-bot) in\n[#&#8203;55275](https://redirect.github.com/dotnet/sdk/pull/55275)\n- Fix internal auth by\n[@&#8203;ellahathaway](https://redirect.github.com/ellahathaway) in\n[#&#8203;55305](https://redirect.github.com/dotnet/sdk/pull/55305)\n- \\[automated] Merge branch 'release/9.0.3xx' => 'release/10.0.1xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55280](https://redirect.github.com/dotnet/sdk/pull/55280)\n- Fix missing data for ReportInstallSucces by\n[@&#8203;joeloff](https://redirect.github.com/joeloff) in\n[#&#8203;55483](https://redirect.github.com/dotnet/sdk/pull/55483)\n- \\[release/10.0.1xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55191](https://redirect.github.com/dotnet/sdk/pull/55191)\n- \\[release/10.0.4xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55506](https://redirect.github.com/dotnet/sdk/pull/55506)\n- \\[release/9.0.1xx] Update dependencies from dotnet/source-build-assets\nby\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55587](https://redirect.github.com/dotnet/sdk/pull/55587)\n- Remove IdentityModel source-build workaround by\n[@&#8203;mthalman](https://redirect.github.com/mthalman) in\n[#&#8203;55557](https://redirect.github.com/dotnet/sdk/pull/55557)\n- \\[release/9.0.3xx] Update branding to 9.0.318 by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55613](https://redirect.github.com/dotnet/sdk/pull/55613)\n- \\[release/9.0.1xx] Update dependencies from dotnet/scenario-tests by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55427](https://redirect.github.com/dotnet/sdk/pull/55427)\n- \\[release/9.0.3xx] Update dependencies from dotnet/scenario-tests by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55449](https://redirect.github.com/dotnet/sdk/pull/55449)\n- \\[release/9.0.3xx] Update dependencies from dotnet/arcade by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55332](https://redirect.github.com/dotnet/sdk/pull/55332)\n- \\[release/9.0.3xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55287](https://redirect.github.com/dotnet/sdk/pull/55287)\n- \\[release/9.0.1xx] Update dependencies from dotnet/arcade by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55331](https://redirect.github.com/dotnet/sdk/pull/55331)\n- \\[release/9.0.1xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55212](https://redirect.github.com/dotnet/sdk/pull/55212)\n- \\[release/9.0.3xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55619](https://redirect.github.com/dotnet/sdk/pull/55619)\n- \\[release/9.0.1xx] Update dependencies from dotnet/scenario-tests by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55632](https://redirect.github.com/dotnet/sdk/pull/55632)\n- \\[release/9.0.1xx] Update dependencies from dotnet/source-build-assets\nby\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55624](https://redirect.github.com/dotnet/sdk/pull/55624)\n- \\[release/9.0.1xx] Update branding to 9.0.121 by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55612](https://redirect.github.com/dotnet/sdk/pull/55612)\n- \\[automated] Merge branch 'release/8.0.4xx' => 'release/9.0.1xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55267](https://redirect.github.com/dotnet/sdk/pull/55267)\n- Localized file check-in by OneLocBuild Task: Build definition ID 140:\nBuild ID\n[`3038879`](https://redirect.github.com/dotnet/sdk/commit/3038879) by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55608](https://redirect.github.com/dotnet/sdk/pull/55608)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55593](https://redirect.github.com/dotnet/sdk/pull/55593)\n- \\[release/9.0.3xx] Update dependencies from dotnet/arcade by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55667](https://redirect.github.com/dotnet/sdk/pull/55667)\n- \\[release/9.0.3xx] Update dependencies from dotnet/scenario-tests by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55663](https://redirect.github.com/dotnet/sdk/pull/55663)\n- \\[automated] Merge branch 'release/9.0.1xx' => 'release/9.0.3xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55333](https://redirect.github.com/dotnet/sdk/pull/55333)\n- \\[release/9.0.1xx] Update dependencies from dotnet/arcade by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55666](https://redirect.github.com/dotnet/sdk/pull/55666)\n- \\[release/9.0.1xx] Update dependencies from dotnet/scenario-tests by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55678](https://redirect.github.com/dotnet/sdk/pull/55678)\n- \\[release/10.0.1xx] Update XML transform for apphost.exe by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55706](https://redirect.github.com/dotnet/sdk/pull/55706)\n- \\[release/10.0.4xx] Update XML transform for apphost.exe by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55707](https://redirect.github.com/dotnet/sdk/pull/55707)\n- Use GitHub app for pushing to the VMR during synchronization by\n[@&#8203;premun](https://redirect.github.com/premun) in\n[#&#8203;55698](https://redirect.github.com/dotnet/sdk/pull/55698)\n- \\[automated] Merge branch 'release/9.0.1xx' => 'release/9.0.3xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55674](https://redirect.github.com/dotnet/sdk/pull/55674)\n- Add a workaround for obtaining GitHub app tokens by\n[@&#8203;premun](https://redirect.github.com/premun) in\n[#&#8203;55718](https://redirect.github.com/dotnet/sdk/pull/55718)\n- \\[release/9.0.3xx] Update dependencies from dotnet/arcade by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55715](https://redirect.github.com/dotnet/sdk/pull/55715)\n- \\[release/10.0.1xx] Port template engine Helix consolidation (PR\n[#&#8203;53546](https://redirect.github.com/dotnet/sdk/issues/53546)) by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) in\n[#&#8203;55528](https://redirect.github.com/dotnet/sdk/pull/55528)\n- \\[automated] Merge branch 'release/9.0.3xx' => 'release/10.0.1xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55614](https://redirect.github.com/dotnet/sdk/pull/55614)\n- \\[release/9.0.3xx] Update dependencies from dotnet/scenario-tests by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55687](https://redirect.github.com/dotnet/sdk/pull/55687)\n- \\[automated] Merge branch 'release/9.0.1xx' => 'release/9.0.3xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55719](https://redirect.github.com/dotnet/sdk/pull/55719)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55710](https://redirect.github.com/dotnet/sdk/pull/55710)\n- \\[release/9.0.3xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55736](https://redirect.github.com/dotnet/sdk/pull/55736)\n- \\[release/9.0.3xx] Update dependencies from dotnet/templating by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55757](https://redirect.github.com/dotnet/sdk/pull/55757)\n- Revert \"\\[release/9.0.3xx] Update dependencies from dotnet/templating\"\nby [@&#8203;joeloff](https://redirect.github.com/joeloff) in\n[#&#8203;55811](https://redirect.github.com/dotnet/sdk/pull/55811)\n- \\[release/10.0.1xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55735](https://redirect.github.com/dotnet/sdk/pull/55735)\n- Merging internal commits for release/9.0.3xx by\n[@&#8203;dotnet-bot](https://redirect.github.com/dotnet-bot) in\n[#&#8203;55725](https://redirect.github.com/dotnet/sdk/pull/55725)\n- \\[release/10.0.1xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55842](https://redirect.github.com/dotnet/sdk/pull/55842)\n- Switch OneLocBuild to target release/11.0.1xx by\n[@&#8203;marcpopMSFT](https://redirect.github.com/marcpopMSFT) with\n[@&#8203;Copilot](https://redirect.github.com/Copilot) in\n[#&#8203;55852](https://redirect.github.com/dotnet/sdk/pull/55852)\n- \\[automated] Update man pages by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;56024](https://redirect.github.com/dotnet/sdk/pull/56024)\n- \\[automated] Merge branch 'release/10.0.1xx' => 'release/10.0.4xx' by\n[@&#8203;github-actions](https://redirect.github.com/github-actions)\\[bot]\nin [#&#8203;55833](https://redirect.github.com/dotnet/sdk/pull/55833)\n- \\[release/10.0.1xx] Source code updates from dotnet/dotnet by\n[@&#8203;dotnet-maestro](https://redirect.github.com/dotnet-maestro)\\[bot]\nin [#&#8203;55918](https://redirect.github.com/dotnet/sdk/pull/55918)\n- \\[release/\n\n> [!IMPORTANT]\n> ✂ PR body was truncated to here.\n\n\n</details>\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n👻 **Immortal**: This PR will be recreated if closed unmerged. Get\n[config\nhelp](https://redirect.github.com/renovatebot/renovate/discussions) if\nthat's undesired.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xNjQuMSIsInVwZGF0ZWRJblZlciI6IjQ0LjE2NC4xIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-17T13:00:48Z",
          "tree_id": "b7b33d8e0fdee3a834ca40bc2fb8a8e18ac76f9b",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/64b6265fc274f16284325137244fd8d02f779097"
        },
        "date": 1792242723173,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 113329366.8974359,
            "unit": "ns",
            "range": "± 3909962.6569781075"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1867782.5340401786,
            "unit": "ns",
            "range": "± 22463.106407686733"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1610.8161006340613,
            "unit": "ns",
            "range": "± 2.493107208725068"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 21412183.995535713,
            "unit": "ns",
            "range": "± 160688.09247612534"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 315.4042328766414,
            "unit": "ns",
            "range": "± 0.8486392433010904"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 792520198,
            "unit": "ns",
            "range": "± 716168.7857181435"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1845424871.3333333,
            "unit": "ns",
            "range": "± 5233482.237488949"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 811110289,
            "unit": "ns",
            "range": "± 8443529.925792826"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1985663265,
            "unit": "ns",
            "range": "± 7698385.682501494"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 775820461.3333334,
            "unit": "ns",
            "range": "± 2433671.3272219677"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1414092536,
            "unit": "ns",
            "range": "± 7470306.681773112"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 325833047,
            "unit": "ns",
            "range": "± 152021556.22040036"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 503019683,
            "unit": "ns",
            "range": "± 189715766.02638593"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 333515353,
            "unit": "ns",
            "range": "± 123899523.73088981"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 623622953,
            "unit": "ns",
            "range": "± 243904263.71391097"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 359971826,
            "unit": "ns",
            "range": "± 128911906.59935407"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 483440377.3333333,
            "unit": "ns",
            "range": "± 221589457.24829942"
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
          "id": "13b493cdd6cac8b0ce6c7fae037f91380d12872e",
          "message": "chore: linted chart docs (#467)",
          "timestamp": "2026-10-17T17:19:34+02:00",
          "tree_id": "3f2afeca47ae5abe2a6e30565ff57a30675bbd01",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/13b493cdd6cac8b0ce6c7fae037f91380d12872e"
        },
        "date": 1792250577929,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 112860152.95,
            "unit": "ns",
            "range": "± 2566008.4331941963"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1809127.91015625,
            "unit": "ns",
            "range": "± 40515.37529844923"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1697.3819741469163,
            "unit": "ns",
            "range": "± 2.104879678622751"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22287624.605769232,
            "unit": "ns",
            "range": "± 164255.33643059115"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 320.1488539255582,
            "unit": "ns",
            "range": "± 1.6016496003050404"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 830187425.3333334,
            "unit": "ns",
            "range": "± 4654152.258847398"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1860495224.6666667,
            "unit": "ns",
            "range": "± 33708055.64410866"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 799791537.6666666,
            "unit": "ns",
            "range": "± 8504150.892765388"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1964551726.3333333,
            "unit": "ns",
            "range": "± 15499142.036180109"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 793569786.6666666,
            "unit": "ns",
            "range": "± 2849897.973705784"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1411501639.6666667,
            "unit": "ns",
            "range": "± 6393864.752267546"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 354690706,
            "unit": "ns",
            "range": "± 108315055.05390412"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 605841670.6666666,
            "unit": "ns",
            "range": "± 342639130.89965147"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 365945541.3333333,
            "unit": "ns",
            "range": "± 129228196.04920252"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 624277411.3333334,
            "unit": "ns",
            "range": "± 289120119.63126564"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 343794815,
            "unit": "ns",
            "range": "± 124517117.507539"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 577998345.6666666,
            "unit": "ns",
            "range": "± 276849051.0273917"
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
          "id": "beb5150b5ced4f926a04580bda8989b5db8ebacd",
          "message": "chore: ignore image tag changes in README generation (#468)",
          "timestamp": "2026-10-17T18:01:17+02:00",
          "tree_id": "6911da106f2141226ae61db6fb472f82392b9df0",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/beb5150b5ced4f926a04580bda8989b5db8ebacd"
        },
        "date": 1792253056815,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 81703742.97619048,
            "unit": "ns",
            "range": "± 1906605.4819844642"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1337054.9178059895,
            "unit": "ns",
            "range": "± 33922.903103530414"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1226.9091053009033,
            "unit": "ns",
            "range": "± 3.1237876167499614"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 16908219.36213235,
            "unit": "ns",
            "range": "± 334422.2696617019"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 245.40706005096436,
            "unit": "ns",
            "range": "± 1.7666443339919835"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 605329390.3333334,
            "unit": "ns",
            "range": "± 14046030.138490247"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1500252120.3333333,
            "unit": "ns",
            "range": "± 15798629.853695994"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 657987905.3333334,
            "unit": "ns",
            "range": "± 17723833.43966661"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1521488180.6666667,
            "unit": "ns",
            "range": "± 14866366.604302524"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 605356301.6666666,
            "unit": "ns",
            "range": "± 7702941.8334260015"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1059658200.6666666,
            "unit": "ns",
            "range": "± 33813285.4015886"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 295395035.6666667,
            "unit": "ns",
            "range": "± 38112666.301068634"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 478987250.6666667,
            "unit": "ns",
            "range": "± 285886739.2980066"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 291825566.6666667,
            "unit": "ns",
            "range": "± 21671593.014245383"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 520235647,
            "unit": "ns",
            "range": "± 250449817.4311848"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 291030923,
            "unit": "ns",
            "range": "± 21677038.277592514"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 494523236.3333333,
            "unit": "ns",
            "range": "± 175983376.07406232"
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
          "id": "b8dbabef7463262de6c56bdb6da6fefcd8475da7",
          "message": "chore(master): release 3.1.1 (#466)\n\n:robot: I have created a release *beep* *boop*\n---\n\n\n##\n[3.1.1](https://github.com/miracum/fhir-pseudonymizer/compare/v3.1.0...v3.1.1)\n(2026-10-17)\n\n\n### Miscellaneous Chores\n\n* **deps:** update docker.io/library/postgres:18.6 docker digest to\n74935e7\n([#462](https://github.com/miracum/fhir-pseudonymizer/issues/462))\n([a31d735](https://github.com/miracum/fhir-pseudonymizer/commit/a31d735d7bdbcadebd0386502916c29319661225))\n* **deps:** update docker.io/mockserver/mockserver docker tag to v9\n([#465](https://github.com/miracum/fhir-pseudonymizer/issues/465))\n([e995f52](https://github.com/miracum/fhir-pseudonymizer/commit/e995f5241802323ff5efaed665fb99cc0d27dd04))\n* **deps:** update github-actions\n([#463](https://github.com/miracum/fhir-pseudonymizer/issues/463))\n([6db737f](https://github.com/miracum/fhir-pseudonymizer/commit/6db737f4a0aed606f9785ec74ddb2cffae7efd09))\n* ignore image tag changes in README generation\n([#468](https://github.com/miracum/fhir-pseudonymizer/issues/468))\n([beb5150](https://github.com/miracum/fhir-pseudonymizer/commit/beb5150b5ced4f926a04580bda8989b5db8ebacd))\n* linted chart docs\n([#467](https://github.com/miracum/fhir-pseudonymizer/issues/467))\n([13b493c](https://github.com/miracum/fhir-pseudonymizer/commit/13b493cdd6cac8b0ce6c7fae037f91380d12872e))\n\n---\nThis PR was generated with [Release\nPlease](https://github.com/googleapis/release-please). See\n[documentation](https://github.com/googleapis/release-please#release-please).",
          "timestamp": "2026-10-17T18:15:58+02:00",
          "tree_id": "3dc842ddd12eefc97f26ac0bfe21a0ea8e845e43",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/b8dbabef7463262de6c56bdb6da6fefcd8475da7"
        },
        "date": 1792253910740,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 58156688.13333333,
            "unit": "ns",
            "range": "± 677605.2671982124"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 806603.8599759615,
            "unit": "ns",
            "range": "± 1904.9216547503233"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1018.6090473447528,
            "unit": "ns",
            "range": "± 4.10559321919145"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 10349202.354166666,
            "unit": "ns",
            "range": "± 67925.30300878968"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 228.84644941183237,
            "unit": "ns",
            "range": "± 2.245992088703989"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 463403574.6666667,
            "unit": "ns",
            "range": "± 8654020.572701242"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1116198648.6666667,
            "unit": "ns",
            "range": "± 67429771.91914465"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 550730264,
            "unit": "ns",
            "range": "± 16448775.531961976"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1157828594.6666667,
            "unit": "ns",
            "range": "± 44973346.634029716"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 477952815.3333333,
            "unit": "ns",
            "range": "± 57370674.85503684"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 803570962,
            "unit": "ns",
            "range": "± 61041964.10548478"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 181177654.33333334,
            "unit": "ns",
            "range": "± 15178552.64956334"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 520241885,
            "unit": "ns",
            "range": "± 178746557.22168225"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 179298256.33333334,
            "unit": "ns",
            "range": "± 25062655.511076264"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 472286138,
            "unit": "ns",
            "range": "± 75432020.68108031"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 169500888.66666666,
            "unit": "ns",
            "range": "± 14860788.833291195"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 263743671.66666666,
            "unit": "ns",
            "range": "± 34273876.07919807"
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
          "id": "7f1853405659923f37c5211c7798903662f8bf33",
          "message": "fix: handle http client timeout transiently and fix metrics port clash (#469)",
          "timestamp": "2026-10-22T17:48:11+02:00",
          "tree_id": "2c8f0de412a8119992543a0e6960a1bddb324896",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/7f1853405659923f37c5211c7798903662f8bf33"
        },
        "date": 1792684274625,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 89893568.72727273,
            "unit": "ns",
            "range": "± 3350032.831258949"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1249866.62421875,
            "unit": "ns",
            "range": "± 21587.47163423108"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1489.3459295126108,
            "unit": "ns",
            "range": "± 11.73729467611702"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 15403061.252083333,
            "unit": "ns",
            "range": "± 248970.3175988895"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 265.0601089681898,
            "unit": "ns",
            "range": "± 4.305945734568719"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 679126711,
            "unit": "ns",
            "range": "± 4926954.232013019"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1677334929.6666667,
            "unit": "ns",
            "range": "± 22033527.188103277"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 713531259.3333334,
            "unit": "ns",
            "range": "± 58224375.81767863"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1703352454.6666667,
            "unit": "ns",
            "range": "± 7894392.678689687"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 721079983.3333334,
            "unit": "ns",
            "range": "± 54165950.64593663"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1205948323.3333333,
            "unit": "ns",
            "range": "± 8065718.654487667"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 260511234,
            "unit": "ns",
            "range": "± 117274871.41512434"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 479603058.3333333,
            "unit": "ns",
            "range": "± 154448153.4272097"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 291407247,
            "unit": "ns",
            "range": "± 80413686.37808216"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 603901317,
            "unit": "ns",
            "range": "± 301753983.91991013"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 287782001,
            "unit": "ns",
            "range": "± 90119795.31644599"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 430567595.6666667,
            "unit": "ns",
            "range": "± 211833004.30520323"
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
          "id": "11b897b597ef0ce2bba67b20c499cefd54d2ca49",
          "message": "chore(deps): update github-actions (#471)\n\nThis PR contains the following updates:\n\n| Package | Type | Update | Change |\n|---|---|---|---|\n| [astral-sh/setup-uv](https://redirect.github.com/astral-sh/setup-uv) |\naction | patch | `v10.3.0` → `v10.3.2` |\n|\n[github/codeql-action](https://redirect.github.com/github/codeql-action)\n| action | patch | `v4.39.0` → `v4.39.1` |\n| [miracum/.github](https://redirect.github.com/miracum/.github) |\nworkflow | minor | `v1.30.0` → `v1.31.2` |\n\n---\n\n### Release Notes\n\n<details>\n<summary>astral-sh/setup-uv</summary>\n\n###\n[`v10.3.2`](https://redirect.github.com/astral-sh/setup-uv/releases/tag/v10.3.2):\n🌈 update known checksums for 0.13.5\n\n[Compare\nSource](https://redirect.github.com/astral-sh/setup-uv/compare/v10.3.1...v10.3.2)\n\n#### Changes\n\n#### 🧰 Maintenance\n\n- chore: update known checksums for 0.13.5\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1103](https://redirect.github.com/astral-sh/setup-uv/issues/1103))\n\n#### 📚 Documentation\n\n- docs: update version references to v10.3.1\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1102](https://redirect.github.com/astral-sh/setup-uv/issues/1102))\n\n###\n[`v10.3.1`](https://redirect.github.com/astral-sh/setup-uv/releases/tag/v10.3.1):\n🌈 preserve cache error class names in save-cache bundle\n\n[Compare\nSource](https://redirect.github.com/astral-sh/setup-uv/compare/v10.3.0...v10.3.1)\n\n##### Changes\n\nThis release includes a fix so that multiple concurrent workflows trying\nto upload the same cache no longer warn\n\n`##[warning]Failed to save: Unable to reserve cache with key ...,\nanother job may be creating this cache.`\n\n##### 🐛 Bug fixes\n\n- fix: preserve cache error class names in save-cache bundle\n[@&#8203;dannyward630](https://redirect.github.com/dannyward630)\n([#&#8203;1093](https://redirect.github.com/astral-sh/setup-uv/issues/1093))\n\n##### 🧰 Maintenance\n\n- chore: update known checksums for 0.13.4\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1099](https://redirect.github.com/astral-sh/setup-uv/issues/1099))\n- chore: update known checksums for 0.13.2/0.13.3\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1098](https://redirect.github.com/astral-sh/setup-uv/issues/1098))\n- chore: update known checksums for 0.13.1\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1094](https://redirect.github.com/astral-sh/setup-uv/issues/1094))\n- Verify npm dependency signatures before builds\n[@&#8203;zaniebot](https://redirect.github.com/zaniebot)\n([#&#8203;1027](https://redirect.github.com/astral-sh/setup-uv/issues/1027))\n\n##### 📚 Documentation\n\n- docs: cache-dependency-glob matches relative to the working directory\n[@&#8203;flange-ipb](https://redirect.github.com/flange-ipb)\n([#&#8203;1092](https://redirect.github.com/astral-sh/setup-uv/issues/1092))\n- docs: update version references to v10.3.0\n@&#8203;[github-actions\\[bot\\]](https://redirect.github.com/apps/github-actions)\n([#&#8203;1090](https://redirect.github.com/astral-sh/setup-uv/issues/1090))\n\n##### ⬆️ Dependency updates\n\n- chore(deps): roll up Dependabot updates\n[@&#8203;eifinger](https://redirect.github.com/eifinger)\n([#&#8203;1101](https://redirect.github.com/astral-sh/setup-uv/issues/1101))\n\n</details>\n\n<details>\n<summary>github/codeql-action</summary>\n\n###\n[`v4.39.1`](https://redirect.github.com/github/codeql-action/releases/tag/v4.39.1)\n\n[Compare\nSource](https://redirect.github.com/github/codeql-action/compare/v4.39.0...v4.39.1)\n\n- Update default CodeQL bundle version to\n[2.27.3](https://redirect.github.com/github/codeql-action/releases/tag/codeql-bundle-v2.27.3).\n[#&#8203;4256](https://redirect.github.com/github/codeql-action/pull/4256)\n\n</details>\n\n<details>\n<summary>miracum/.github</summary>\n\n###\n[`v1.31.2`](https://redirect.github.com/miracum/.github/releases/tag/v1.31.2)\n\n[Compare\nSource](https://redirect.github.com/miracum/.github/compare/v1.31.1...v1.31.2)\n\n##### Bug Fixes\n\n- duplicate digests on multiple tags being pushed\n([d9d87af](https://redirect.github.com/miracum/.github/commit/d9d87af182ffe0ef4958e61dd1e697c7fd83c52f))\n\n###\n[`v1.31.1`](https://redirect.github.com/miracum/.github/releases/tag/v1.31.1)\n\n[Compare\nSource](https://redirect.github.com/miracum/.github/compare/v1.31.0...v1.31.1)\n\n##### Bug Fixes\n\n- **build:** output the digest of the pushed image on pull requests\n([9b731cc](https://redirect.github.com/miracum/.github/commit/9b731cc069e92b534c819d2b6f250723b8f2d216))\n\n###\n[`v1.31.0`](https://redirect.github.com/miracum/.github/releases/tag/v1.31.0)\n\n[Compare\nSource](https://redirect.github.com/miracum/.github/compare/v1.30.1...v1.31.0)\n\n##### Features\n\n- allow digest-pinning multiple images\n([6611917](https://redirect.github.com/miracum/.github/commit/6611917e0cbce3777825e93e3f725c98a437c060))\n\n###\n[`v1.30.1`](https://redirect.github.com/miracum/.github/releases/tag/v1.30.1)\n\n[Compare\nSource](https://redirect.github.com/miracum/.github/compare/v1.30.0...v1.30.1)\n\n##### Bug Fixes\n\n- oom on cache export due to mismatched compression codecs\n([#&#8203;225](https://redirect.github.com/miracum/.github/issues/225))\n([217f042](https://redirect.github.com/miracum/.github/commit/217f0425b4039ca66596aeb30b6461e2511a0e09))\n\n</details>\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n👻 **Immortal**: This PR will be recreated if closed unmerged. Get\n[config\nhelp](https://redirect.github.com/renovatebot/renovate/discussions) if\nthat's undesired.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xODAuMCIsInVwZGF0ZWRJblZlciI6IjQ0LjE4MC4wIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-25T15:09:48Z",
          "tree_id": "1a5104acbb8bd24998d81d3f458c4e09a8dffa5c",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/11b897b597ef0ce2bba67b20c499cefd54d2ca49"
        },
        "date": 1792941399490,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 120917512,
            "unit": "ns",
            "range": "± 2252543.9329806385"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1846272.9112723214,
            "unit": "ns",
            "range": "± 14126.97730639262"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1705.628835405622,
            "unit": "ns",
            "range": "± 2.6446935132464633"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 23370321.1875,
            "unit": "ns",
            "range": "± 89894.76611883195"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 340.47390743664334,
            "unit": "ns",
            "range": "± 0.7264606800473039"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 825307577.6666666,
            "unit": "ns",
            "range": "± 2352320.030959719"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1957732698.3333333,
            "unit": "ns",
            "range": "± 8434763.706165949"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 844041745.6666666,
            "unit": "ns",
            "range": "± 5338948.355358885"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 2009677105.3333333,
            "unit": "ns",
            "range": "± 36749585.89557638"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 786023597.3333334,
            "unit": "ns",
            "range": "± 1774261.585845879"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1430853300.3333333,
            "unit": "ns",
            "range": "± 17827275.05017055"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 337214375,
            "unit": "ns",
            "range": "± 132993839.34193763"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 509667020.3333333,
            "unit": "ns",
            "range": "± 177995157.3095605"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 326765035.3333333,
            "unit": "ns",
            "range": "± 127195534.26364395"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 624118908.3333334,
            "unit": "ns",
            "range": "± 248017463.9549889"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 337034438.6666667,
            "unit": "ns",
            "range": "± 127359191.96153584"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 450629493.6666667,
            "unit": "ns",
            "range": "± 263404792.1986944"
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
          "id": "2df37b6a0bd3e23816a408c4fed10d9a567d36c7",
          "message": "chore(deps): update all non-major dependencies (#472)\n\n> [!NOTE]\n> This PR body was truncated due to platform limits.\n\nThis PR contains the following updates:\n\n| Package | Change |\n[Age](https://docs.renovatebot.com/merge-confidence/) |\n[Confidence](https://docs.renovatebot.com/merge-confidence/) | Type |\nUpdate | Pending |\n|---|---|---|---|---|---|---|\n| [Microsoft.Extensions.Http.Resilience](https://dot.net/)\n([source](https://redirect.github.com/dotnet/extensions)) | `10.0.0` →\n`10.10.0` |\n![age](https://developer.mend.io/api/mc/badges/age/nuget/Microsoft.Extensions.Http.Resilience/10.10.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/nuget/Microsoft.Extensions.Http.Resilience/10.0.0/10.10.0?slim=true)\n| nuget | minor | `10.11.0` |\n|\n[common](https://redirect.github.com/miracum/charts/tree/master/charts/common)\n([source](https://redirect.github.com/miracum/charts/tree/HEAD/charts/common))\n| `0.1.1` → `0.2.0` |\n![age](https://developer.mend.io/api/mc/badges/age/docker/ghcr.io%2fmiracum%2fcharts%2fcommon/0.2.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/docker/ghcr.io%2fmiracum%2fcharts%2fcommon/0.1.1/0.2.0?slim=true)\n| | minor | |\n| [ghcr.io/miracum/vfps](https://redirect.github.com/miracum/vfps) |\n`v1.25.0` → `v1.26.0` |\n![age](https://developer.mend.io/api/mc/badges/age/docker/ghcr.io%2fmiracum%2fvfps/v1.26.0?slim=true)\n|\n![confidence](https://developer.mend.io/api/mc/badges/confidence/docker/ghcr.io%2fmiracum%2fvfps/v1.25.0/v1.26.0?slim=true)\n| | minor | |\n\n---\n\n### Release Notes\n\n<details>\n<summary>dotnet/extensions\n(Microsoft.Extensions.Http.Resilience)</summary>\n\n###\n[`v10.10.0`](https://redirect.github.com/dotnet/extensions/releases/tag/v10.10.0)\n\nThis month's release focuses on AI package reliability: closing gaps in\nevaluation scoring, hardening OpenAI image-option handling, and removing\nthe deprecated OpenAI Assistants API support.\n\n#### Experimental API Changes\n\n##### Removed Experimental APIs\n\n- OpenAI Assistants experimental APIs removed (was experimental under\n`OPENAI001`)\n[#&#8203;7724](https://redirect.github.com/dotnet/extensions/issues/7724)\n\n#### What's Changed\n\n##### AI\n\n- Remove OpenAI Assistants API support\n[#&#8203;7724](https://redirect.github.com/dotnet/extensions/issues/7724)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Update OpenAI package version to 2.13.0\n[#&#8203;7726](https://redirect.github.com/dotnet/extensions/issues/7726)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee)\n- OpenAI: Avoid null implicit conversions for image options\n[#&#8203;7727](https://redirect.github.com/dotnet/extensions/issues/7727)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n##### AI Evaluation\n\n- Fail closed when a quality metric has no valid score\n[#&#8203;7735](https://redirect.github.com/dotnet/extensions/issues/7735)\nby [@&#8203;thaildhe172591](https://redirect.github.com/thaildhe172591)\n- Validate path segments in Azure storage result store and response\ncache\n[#&#8203;7718](https://redirect.github.com/dotnet/extensions/issues/7718)\nby [@&#8203;Lroca88](https://redirect.github.com/Lroca88)\n\n#### Repository Infrastructure Updates\n\n- Add TfxInstaller for publishing\n[#&#8203;7695](https://redirect.github.com/dotnet/extensions/issues/7695)\nby\n[@&#8203;peterwaltonwork](https://redirect.github.com/peterwaltonwork)\n- Bump PowerShell from 7.6.4 to 7.6.5\n[#&#8203;7702](https://redirect.github.com/dotnet/extensions/issues/7702)\n- \\[Infrastructure] Update vulnerable npm dependencies\n[#&#8203;7705](https://redirect.github.com/dotnet/extensions/issues/7705)\nby [@&#8203;wtgodbe](https://redirect.github.com/wtgodbe)\n- Add Node installation for TfxInstaller\n[#&#8203;7703](https://redirect.github.com/dotnet/extensions/issues/7703)\nby\n[@&#8203;peterwaltonwork](https://redirect.github.com/peterwaltonwork)\n- Publish VSIX using publish task instead of output\n[#&#8203;7711](https://redirect.github.com/dotnet/extensions/issues/7711)\nby\n[@&#8203;peterwaltonwork](https://redirect.github.com/peterwaltonwork)\n- Bump dotnet-coverage from 18.9.0 to 18.10.0\n[#&#8203;7708](https://redirect.github.com/dotnet/extensions/issues/7708)\n- Do not validate extension during publish step\n[#&#8203;7725](https://redirect.github.com/dotnet/extensions/issues/7725)\nby\n[@&#8203;peterwaltonwork](https://redirect.github.com/peterwaltonwork)\n- Add skill for upgrading OpenAI\n[#&#8203;7728](https://redirect.github.com/dotnet/extensions/issues/7728)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee)\n- Fix source indexer stage\n[#&#8203;7694](https://redirect.github.com/dotnet/extensions/issues/7694)\nby [@&#8203;jjonescz](https://redirect.github.com/jjonescz)\n\n#### Acknowledgements\n\n- [@&#8203;Lroca88](https://redirect.github.com/Lroca88) made their\nfirst contribution in\n[#&#8203;7718](https://redirect.github.com/dotnet/extensions/issues/7718)\n- [@&#8203;thaildhe172591](https://redirect.github.com/thaildhe172591)\nmade their first contribution in\n[#&#8203;7735](https://redirect.github.com/dotnet/extensions/issues/7735)\n- [@&#8203;ANcpLua](https://redirect.github.com/ANcpLua) submitted issue\n[#&#8203;7665](https://redirect.github.com/dotnet/extensions/issues/7665)\n(resolved by\n[#&#8203;7735](https://redirect.github.com/dotnet/extensions/issues/7735))\n- [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n[@&#8203;peterwald](https://redirect.github.com/peterwald)\n[@&#8203;shyamnamboodiripad](https://redirect.github.com/shyamnamboodiripad)\nreviewed pull requests\n\n**Full Changelog**:\n<https://github.com/dotnet/extensions/compare/v10.9.0...v10.10.0>\n\n###\n[`v10.9.0`](https://redirect.github.com/dotnet/extensions/releases/tag/v10.9.0)\n\nVersion 10.9.0 is headlined by changes in these areas:\n\n- **AI:** New experimental routing APIs center on the abstract\n`RoutingChatClient` base class, with `SemanticRoutingChatClient` as a\nconcrete semantic-routing implementation. Separately, the abstract\n`FailoverChatClient` specialization and its concrete\n`OrderedFailoverChatClient` implementation add failover routing.\n- **AI Evaluation:** The generated report gains redesigned Overview,\nCases, History, and Comparison views.\n- **ASP.NET Core and HTTP diagnostics:** The release adds HTTP request\nlatency log enrichment and fixes configuration binding, response-body\nlogging, request-path redaction, and resilience package version\nhandling.\n- **Source-generated logging and service discovery:** Fixes cover\nclassification type qualification, thread-local state cleanup, and DNS\nquery suffix handling.\n\n#### Experimental API Changes\n\n##### New Experimental APIs\n\n- New experimental API: HTTP request latency log enrichment\n(`EXTEXP0013`)\n[#&#8203;7602](https://redirect.github.com/dotnet/extensions/issues/7602)\n- New experimental API: Chat client routing and failover (`MEAI001`)\n[#&#8203;7662](https://redirect.github.com/dotnet/extensions/issues/7662)\n\n#### What's Changed\n\n##### AI (`Microsoft.Extensions.AI`,\n`Microsoft.Extensions.AI.Abstractions`, and\n`Microsoft.Extensions.AI.OpenAI`)\n\n- Add extensible chat client routing\n[#&#8203;7662](https://redirect.github.com/dotnet/extensions/issues/7662)\nby [@&#8203;joshuajyue](https://redirect.github.com/joshuajyue)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Pass the request's options to the selected client\n[#&#8203;7685](https://redirect.github.com/dotnet/extensions/issues/7685)\nby [@&#8203;joshuajyue](https://redirect.github.com/joshuajyue)\n(co-authored by [@&#8203;jozkee](https://redirect.github.com/jozkee)\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- AI.Abstractions: fix ExcludeFromSchema dropped under concurrent\nAIFunction creation\n[#&#8203;7677](https://redirect.github.com/dotnet/extensions/issues/7677)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Cap OpenAI dependency version\n([b10f9c0](https://redirect.github.com/dotnet/extensions/commit/b10f9c0a081b5dbb7755b8f5592e1d3c3f550a3a))\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n**Note: Microsoft.Extensions.AI.OpenAI constrains its dependency for\nOpenAI to 2.12.x, preventing OpenAI updates to 2.13.0+ due to an\nincompatibility. We expect to release Microsoft.Extensions.AI.OpenAI\nversion 10.9.1 during the week of August 17 to address this issue.**\n\n##### HTTP Resilience and Diagnostics\n(`Microsoft.Extensions.Http.Resilience` and\n`Microsoft.Extensions.Http.Diagnostics`)\n\n- Fix Grpc.Net.ClientFactory version range check - Fixes\n[#&#8203;7565](https://redirect.github.com/dotnet/extensions/issues/7565)\n[#&#8203;7566](https://redirect.github.com/dotnet/extensions/issues/7566)\nby [@&#8203;Ghost93](https://redirect.github.com/Ghost93)\n- Fix response body logging under debugger\n[#&#8203;7678](https://redirect.github.com/dotnet/extensions/issues/7678)\nby [@&#8203;Rimobul](https://redirect.github.com/Rimobul)\n- Redact outgoing path when route is unknown\n[#&#8203;7687](https://redirect.github.com/dotnet/extensions/issues/7687)\nby [@&#8203;Rimobul](https://redirect.github.com/Rimobul)\n- Fix HTTP client logging config binding\n[#&#8203;7691](https://redirect.github.com/dotnet/extensions/issues/7691)\nby [@&#8203;Rimobul](https://redirect.github.com/Rimobul)\n\n##### ASP.NET Core Extensions\n(`Microsoft.AspNetCore.Diagnostics.Middleware`)\n\n- Rename HttpLatencyTelemetry extensions class and drop redundant TFM\nguard\n[#&#8203;7645](https://redirect.github.com/dotnet/extensions/issues/7645)\nby [@&#8203;EasyL0ver](https://redirect.github.com/EasyL0ver)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Add HTTP request latency log enricher (experimental)\n[#&#8203;7602](https://redirect.github.com/dotnet/extensions/issues/7602)\nby [@&#8203;EasyL0ver](https://redirect.github.com/EasyL0ver)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n##### Logging Source Generator (`Microsoft.Gen.Logging`)\n\n- \\[Microsoft.Gen.Logging] Clear thread-local state when logging throws\n[#&#8203;7682](https://redirect.github.com/dotnet/extensions/issues/7682)\nby [@&#8203;Rimobul](https://redirect.github.com/Rimobul)\n- \\[Microsoft.Gen.Logging] Fully qualify classification types\n[#&#8203;7689](https://redirect.github.com/dotnet/extensions/issues/7689)\nby [@&#8203;Rimobul](https://redirect.github.com/Rimobul)\n\n##### AI Evaluation (`Microsoft.Extensions.AI.Evaluation.Reporting`)\n\n- \\[Microsoft.Extensions.AI.Evaluation.Reporting] Evaluation report\nredesign\n[#&#8203;7609](https://redirect.github.com/dotnet/extensions/issues/7609)\nby [@&#8203;grafanaKibana](https://redirect.github.com/grafanaKibana)\n\n##### Project Templates (`Microsoft.McpServer.ProjectTemplates`)\n\n- Remove MCP server project template\n[#&#8203;7680](https://redirect.github.com/dotnet/extensions/issues/7680)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n**Note: The MCP server project template was migrated to the\ndotnet/aspnetcore repository, and it is now included in the .NET 10 and\n.NET 11 SDKs.**\n\n##### Service Discovery (`Microsoft.Extensions.ServiceDiscovery.Dns`)\n\n- Fix DNS SRV provider ignoring query suffix\n[#&#8203;7688](https://redirect.github.com/dotnet/extensions/issues/7688)\nby [@&#8203;Rimobul](https://redirect.github.com/Rimobul) (co-authored\nby [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n#### Test Improvements\n\n- Update Microsoft.Data.SqlClient test dependency\n([aaea3c8](https://redirect.github.com/dotnet/extensions/commit/aaea3c858a6e82962de1601d537326caa3445c80))\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n#### Repository Infrastructure Updates\n\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7621](https://redirect.github.com/dotnet/extensions/issues/7621)\n- Merge release/10.8 into main\n[#&#8203;7632](https://redirect.github.com/dotnet/extensions/issues/7632)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;jozkee](https://redirect.github.com/jozkee)\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Add agentic workflows that keep Microsoft.Extensions.AI aligned with\nOpenTelemetry GenAI semantic-conventions\n[#&#8203;7611](https://redirect.github.com/dotnet/extensions/issues/7611)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Bump brace-expansion, eslint and tfx-cli in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7648](https://redirect.github.com/dotnet/extensions/issues/7648)\n- Add a release-manager agent for the dotnet/extensions release process\n[#&#8203;7643](https://redirect.github.com/dotnet/extensions/issues/7643)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Bump CommunityToolkit.VectorData package baselines to 1.0.0 /\n1.0.0-preview\n[#&#8203;7651](https://redirect.github.com/dotnet/extensions/issues/7651)\nby [@&#8203;adamsitnik](https://redirect.github.com/adamsitnik)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Incorporate servicing release experience into release-manager\nplaybooks\n[#&#8203;7656](https://redirect.github.com/dotnet/extensions/issues/7656)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Bump dotnet-reportgenerator-globaltool from 5.5.10 to 5.5.11\n[#&#8203;7666](https://redirect.github.com/dotnet/extensions/issues/7666)\n- Bump adm-zip and azure-pipelines-task-lib in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript/azure-devops-report/tasks/PublishAIEvaluationReport\n[#&#8203;7652](https://redirect.github.com/dotnet/extensions/issues/7652)\n- Bump postcss from 8.5.15 to 8.5.23 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7660](https://redirect.github.com/dotnet/extensions/issues/7660)\n- Bump PowerShell from 7.6.3 to 7.6.4\n[#&#8203;7650](https://redirect.github.com/dotnet/extensions/issues/7650)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7644](https://redirect.github.com/dotnet/extensions/issues/7644)\n- \\[release-manager] Require package dependency closure for releases\n[#&#8203;7693](https://redirect.github.com/dotnet/extensions/issues/7693)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Optionally allow official builds to publish VSIX extension.\n[#&#8203;7692](https://redirect.github.com/dotnet/extensions/issues/7692)\nby [@&#8203;peterwald](https://redirect.github.com/peterwald)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7690](https://redirect.github.com/dotnet/extensions/issues/7690)\n\n#### Acknowledgements\n\n- [@&#8203;EasyL0ver](https://redirect.github.com/EasyL0ver) made their\nfirst contribution in\n[#&#8203;7602](https://redirect.github.com/dotnet/extensions/issues/7602)\n- [@&#8203;Ghost93](https://redirect.github.com/Ghost93) made their\nfirst contribution in\n[#&#8203;7566](https://redirect.github.com/dotnet/extensions/issues/7566)\n- [@&#8203;grafanaKibana](https://redirect.github.com/grafanaKibana)\nmade their first contribution in\n[#&#8203;7609](https://redirect.github.com/dotnet/extensions/issues/7609)\n- [@&#8203;Rimobul](https://redirect.github.com/Rimobul) made their\nfirst contribution in\n[#&#8203;7678](https://redirect.github.com/dotnet/extensions/issues/7678)\n- [@&#8203;joshuajyue](https://redirect.github.com/joshuajyue) made\ntheir first contribution in\n[#&#8203;7662](https://redirect.github.com/dotnet/extensions/issues/7662)\n- [@&#8203;Ghost93](https://redirect.github.com/Ghost93) submitted issue\n[#&#8203;7565](https://redirect.github.com/dotnet/extensions/issues/7565)\n(resolved by\n[#&#8203;7566](https://redirect.github.com/dotnet/extensions/issues/7566))\n- [@&#8203;grafanaKibana](https://redirect.github.com/grafanaKibana)\nsubmitted issue\n[#&#8203;7593](https://redirect.github.com/dotnet/extensions/issues/7593)\n(resolved by\n[#&#8203;7609](https://redirect.github.com/dotnet/extensions/issues/7609))\n- [@&#8203;jredgard](https://redirect.github.com/jredgard) submitted\nissue\n[#&#8203;7675](https://redirect.github.com/dotnet/extensions/issues/7675)\n(resolved by\n[#&#8203;7677](https://redirect.github.com/dotnet/extensions/issues/7677))\n- [@&#8203;viorelbuligadev](https://redirect.github.com/viorelbuligadev)\nsubmitted issue\n[#&#8203;7649](https://redirect.github.com/dotnet/extensions/issues/7649)\n(resolved by\n[#&#8203;7678](https://redirect.github.com/dotnet/extensions/issues/7678))\n- [@&#8203;joshuajyue](https://redirect.github.com/joshuajyue) submitted\nissue\n[#&#8203;7647](https://redirect.github.com/dotnet/extensions/issues/7647)\n(resolved by\n[#&#8203;7662](https://redirect.github.com/dotnet/extensions/issues/7662))\n- [@&#8203;jboelter](https://redirect.github.com/jboelter) submitted\nissue\n[#&#8203;7634](https://redirect.github.com/dotnet/extensions/issues/7634)\n(resolved by\n[#&#8203;7682](https://redirect.github.com/dotnet/extensions/issues/7682))\n- [@&#8203;dmeeze](https://redirect.github.com/dmeeze) submitted issue\n[#&#8203;7157](https://redirect.github.com/dotnet/extensions/issues/7157)\n(resolved by\n[#&#8203;7689](https://redirect.github.com/dotnet/extensions/issues/7689))\n- [@&#8203;ssmelov](https://redirect.github.com/ssmelov) submitted issue\n[#&#8203;7536](https://redirect.github.com/dotnet/extensions/issues/7536)\n(resolved by\n[#&#8203;7687](https://redirect.github.com/dotnet/extensions/issues/7687))\n- [@&#8203;Marcus-Kanon](https://redirect.github.com/Marcus-Kanon)\nsubmitted issue\n[#&#8203;7551](https://redirect.github.com/dotnet/extensions/issues/7551)\n(resolved by\n[#&#8203;7691](https://redirect.github.com/dotnet/extensions/issues/7691))\n- [@&#8203;vermilion](https://redirect.github.com/vermilion) submitted\nissue\n[#&#8203;7175](https://redirect.github.com/dotnet/extensions/issues/7175)\n(resolved by\n[#&#8203;7688](https://redirect.github.com/dotnet/extensions/issues/7688))\n- [@&#8203;evgenyfedorov2](https://redirect.github.com/evgenyfedorov2)\n[@&#8203;wtgodbe](https://redirect.github.com/wtgodbe)\n[@&#8203;shyamnamboodiripad](https://redirect.github.com/shyamnamboodiripad)\n[@&#8203;dariusclay](https://redirect.github.com/dariusclay)\n[@&#8203;iliar-turdushev](https://redirect.github.com/iliar-turdushev)\n[@&#8203;mariamgerges](https://redirect.github.com/mariamgerges)\n[@&#8203;PranavSenthilnathan](https://redirect.github.com/PranavSenthilnathan)\n[@&#8203;rafal-mz](https://redirect.github.com/rafal-mz)\n[@&#8203;tannergooding](https://redirect.github.com/tannergooding)\n[@&#8203;tarekgh](https://redirect.github.com/tarekgh) reviewed pull\nrequests\n\n**Full Changelog**:\n<https://github.com/dotnet/extensions/compare/v10.8.4...v10.9.0>\n\n#### Packages in this release\n\n| Package | Version |\n| ----------------------------------------------------------------- |\n-------------------------- |\n| Microsoft.AspNetCore.Testing | 10.9.0 |\n| Microsoft.Extensions.AI.Abstractions | 10.9.0 |\n| Microsoft.Extensions.AI.Evaluation.Console | 10.9.0 |\n| Microsoft.Extensions.AI.Templates | 10.9.0-preview\\.3.26411.16 |\n| Microsoft.Extensions.AmbientMetadata.Application | 10.9.0 |\n| Microsoft.Extensions.AmbientMetadata.Build | 10.9.0 |\n| Microsoft.Extensions.AuditReports | 10.9.0 |\n| Microsoft.Extensions.Caching.Hybrid | 10.9.0 |\n| Microsoft.Extensions.Compliance.Abstractions | 10.9.0 |\n| Microsoft.Extensions.DataIngestion.Abstractions |\n10.9.0-preview\\.1.26411.16 |\n| Microsoft.Extensions.DependencyInjection.AutoActivation | 10.9.0 |\n| Microsoft.Extensions.Diagnostics.ExceptionSummarization | 10.9.0 |\n| Microsoft.Extensions.ObjectPool.DependencyInjection | 10.9.0 |\n| Microsoft.Extensions.Options.Contextual | 10.9.0-preview\\.1.26411.16 |\n| Microsoft.Extensions.ServiceDiscovery.Abstractions | 10.9.0 |\n| Microsoft.Extensions.StaticAnalysis | 10.9.0 |\n| Microsoft.Extensions.TimeProvider.Testing | 10.9.0 |\n| Microsoft.Extensions.AI | 10.9.0 |\n| Microsoft.Extensions.AI.Evaluation | 10.9.0 |\n| Microsoft.Extensions.AI.OpenAI | 10.9.0 |\n| Microsoft.Extensions.AsyncState | 10.9.0 |\n| Microsoft.Extensions.Compliance.Redaction | 10.9.0 |\n| Microsoft.Extensions.Compliance.Testing | 10.9.0 |\n| Microsoft.Extensions.DataIngestion.Markdig |\n10.9.0-preview\\.1.26411.16 |\n| Microsoft.Extensions.DataIngestion.MarkItDown |\n10.9.0-preview\\.1.26411.16 |\n| Microsoft.Extensions.ServiceDiscovery | 10.9.0 |\n| Microsoft.Extensions.Telemetry.Abstractions | 10.9.0 |\n| Microsoft.Extensions.VectorData.Abstractions | 10.9.0 |\n| Microsoft.AspNetCore.AsyncState | 10.9.0 |\n| Microsoft.AspNetCore.HeaderParsing | 10.9.0 |\n| Microsoft.Extensions.AI.Evaluation.NLP | 10.9.0-preview\\.1.26411.16 |\n| Microsoft.Extensions.AI.Evaluation.Quality | 10.9.0 |\n| Microsoft.Extensions.AI.Evaluation.Reporting | 10.9.0 |\n| Microsoft.Extensions.AI.Evaluation.Safety | 10.9.0-preview\\.1.26411.16\n|\n| Microsoft.Extensions.DataIngestion | 10.9.0-preview\\.1.26411.16 |\n| Microsoft.Extensions.Diagnostics.HealthChecks.Common | 10.9.0 |\n| Microsoft.Extensions.Diagnostics.Probes | 10.9.0-preview\\.1.26411.16 |\n| Microsoft.Extensions.Diagnostics.ResourceMonitoring | 10.9.0 |\n| Microsoft.Extensions.Diagnostics.Testing | 10.9.0 |\n| Microsoft.Extensions.Resilience | 10.9.0 |\n| Microsoft.Extensions.ServiceDiscovery.Dns | 10.9.0 |\n| Microsoft.Extensions.ServiceDiscovery.Yarp | 10.9.0 |\n| Microsoft.Extensions.Telemetry | 10.9.0 |\n| Microsoft.Extensions.VectorData.ConformanceTests | 10.9.0 |\n| Microsoft.AspNetCore.Diagnostics.Middleware | 10.9.0 |\n| Microsoft.Extensions.AI.Evaluation.Reporting.Azure | 10.9.0 |\n| Microsoft.Extensions.Diagnostics.HealthChecks.ResourceUtilization |\n10.9.0 |\n| Microsoft.Extensions.Diagnostics.ResourceMonitoring.Kubernetes |\n10.9.0 |\n| Microsoft.Extensions.Hosting.Testing | 10.9.0-preview\\.1.26411.16 |\n| Microsoft.Extensions.Http.Diagnostics | 10.9.0 |\n| Microsoft.Extensions.Http.Resilience | 10.9.0 |\n\nThe `Microsoft.Agents.AI.ProjectTemplates` package is not included in\nthis release.\n\n###\n[`v10.8.0`](https://redirect.github.com/dotnet/extensions/releases/tag/v10.8.0)\n\nThis release adds new experimental APIs to\nMicrosoft.Extensions.AI.Abstractions and updates the OpenAI dependency\nto 2.12.0, alongside documentation, test, and repository maintenance.\n\n#### Experimental API Changes\n\n##### New Experimental APIs\n\n- New experimental API: `AIFunctionNameAttribute` and\n`AIParameterNameAttribute`\n[#&#8203;7610](https://redirect.github.com/dotnet/extensions/issues/7610)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- New experimental API:\n`ToolApprovalRequestContent.RequiresConfirmation` (`MEAI001`)\n[#&#8203;7549](https://redirect.github.com/dotnet/extensions/issues/7549)\nby [@&#8203;javiercn](https://redirect.github.com/javiercn) (co-authored\nby [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n#### What's Changed\n\n##### AI\n\n- Upgrade OpenAI dependency to 2.12.0\n[#&#8203;7608](https://redirect.github.com/dotnet/extensions/issues/7608)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Auto-detect audio format in OpenAISpeechToTextClient\n[#&#8203;7575](https://redirect.github.com/dotnet/extensions/issues/7575)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Fix ImageGeneratingChatClient duplicating preceding content and\ndropping following content\n[#&#8203;7624](https://redirect.github.com/dotnet/extensions/issues/7624)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n##### Vector Data\n\n- Make all test methods virtual in VectorData.ConformanceTests\n[#&#8203;7606](https://redirect.github.com/dotnet/extensions/issues/7606)\nby [@&#8203;adamsitnik](https://redirect.github.com/adamsitnik)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n#### Documentation Updates\n\n- Remove links to ai-samples repo\n[#&#8203;7574](https://redirect.github.com/dotnet/extensions/issues/7574)\nby [@&#8203;gewarren](https://redirect.github.com/gewarren)\n- Fix up docs with Copilot (MEVD)\n[#&#8203;7597](https://redirect.github.com/dotnet/extensions/issues/7597)\nby [@&#8203;gewarren](https://redirect.github.com/gewarren)\n- Fix up docs with Copilot (M.E.ServiceDiscovery)\n[#&#8203;7598](https://redirect.github.com/dotnet/extensions/issues/7598)\nby [@&#8203;gewarren](https://redirect.github.com/gewarren) (co-authored\nby [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Fix up docs with Copilot (MEAI)\n[#&#8203;7600](https://redirect.github.com/dotnet/extensions/issues/7600)\nby [@&#8203;gewarren](https://redirect.github.com/gewarren)\n- Fix up docs with Copilot\n[#&#8203;7601](https://redirect.github.com/dotnet/extensions/issues/7601)\nby [@&#8203;gewarren](https://redirect.github.com/gewarren)\n\n#### Test Improvements\n\n- Fix flaky StampedeTests and harden related test waits\n[#&#8203;7572](https://redirect.github.com/dotnet/extensions/issues/7572)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Fix SQLitePCLRaw\\.lib.e_sqlite3 vulnerability by replacing\nSemanticKernel connectors with CommunityToolkit\n[#&#8203;7579](https://redirect.github.com/dotnet/extensions/issues/7579)\nby [@&#8203;adamsitnik](https://redirect.github.com/adamsitnik)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Removing SemanticKernel Connectors dependency and replacing it\n[#&#8203;7584](https://redirect.github.com/dotnet/extensions/issues/7584)\nby [@&#8203;adamsitnik](https://redirect.github.com/adamsitnik)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Migrate to xUnit v3\n[#&#8203;7607](https://redirect.github.com/dotnet/extensions/issues/7607)\nby [@&#8203;adamsitnik](https://redirect.github.com/adamsitnik)\n(co-authored by\n[@&#8203;shyamnamboodiripad](https://redirect.github.com/shyamnamboodiripad)\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n#### Repository Infrastructure Updates\n\n- Update OTel GenAI conventions skill for standalone semconv-genai repo\n[#&#8203;7519](https://redirect.github.com/dotnet/extensions/issues/7519)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Bump dotnet-coverage from 18.7.0 to 18.8.0\n[#&#8203;7552](https://redirect.github.com/dotnet/extensions/issues/7552)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7559](https://redirect.github.com/dotnet/extensions/issues/7559)\n- Fix transitive MessagePack vulnerability in AI template AppHost\nprojects\n[#&#8203;7561](https://redirect.github.com/dotnet/extensions/issues/7561)\nby [@&#8203;adamsitnik](https://redirect.github.com/adamsitnik)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Bump esbuild,\n[@&#8203;vitejs/plugin-react](https://redirect.github.com/vitejs/plugin-react)\nand vite in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7564](https://redirect.github.com/dotnet/extensions/issues/7564)\n- Bump tmp from 0.2.6 to 0.2.7 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7569](https://redirect.github.com/dotnet/extensions/issues/7569)\n- Bump js-yaml from 4.1.1 to 4.2.0 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7570](https://redirect.github.com/dotnet/extensions/issues/7570)\n- Bump PowerShell from 7.6.2 to 7.6.3\n[#&#8203;7576](https://redirect.github.com/dotnet/extensions/issues/7576)\n- Remove duplicate 'WebAPI' classification from template\n[#&#8203;7577](https://redirect.github.com/dotnet/extensions/issues/7577)\nby [@&#8203;danroth27](https://redirect.github.com/danroth27)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7590](https://redirect.github.com/dotnet/extensions/issues/7590)\n- Eliminate redundant Correctness CI stage by merging into Build\n[#&#8203;7594](https://redirect.github.com/dotnet/extensions/issues/7594)\nby [@&#8203;adamsitnik](https://redirect.github.com/adamsitnik)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Update Agent Framework to 1.13.0\n[#&#8203;7613](https://redirect.github.com/dotnet/extensions/issues/7613)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Bump dotnet-coverage from 18.8.0 to 18.9.0\n[#&#8203;7615](https://redirect.github.com/dotnet/extensions/issues/7615)\n\n#### Acknowledgements\n\n- [@&#8203;wtgodbe](https://redirect.github.com/wtgodbe)\n[@&#8203;tarekgh](https://redirect.github.com/tarekgh)\n[@&#8203;eiriktsarpalis](https://redirect.github.com/eiriktsarpalis)\n[@&#8203;peterwald](https://redirect.github.com/peterwald)\n[@&#8203;PranavSenthilnathan](https://redirect.github.com/PranavSenthilnathan)\n[@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n[@&#8203;evgenyfedorov2](https://redirect.github.com/evgenyfedorov2)\n[@&#8203;roji](https://redirect.github.com/roji)\n[@&#8203;DeagleGross](https://redirect.github.com/DeagleGross) reviewed\npull requests\n\n**Full Changelog**:\n<https://github.com/dotnet/extensions/compare/v10.7.0...v10.8.0>\n\n###\n[`v10.7.0`](https://redirect.github.com/dotnet/extensions/releases/tag/v10.7.0)\n\nv10.7.0 graduates the\n[Microsoft.Extensions.Diagnostics.ResourceMonitoring.Kubernetes](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.ResourceMonitoring.Kubernetes)\npackage to stable. The package registers a Kubernetes-aware\n`ResourceQuotaProvider` that reads the pod's CPU and memory requests and\nlimits and exposes them to\n`Microsoft.Extensions.Diagnostics.ResourceMonitoring` as baseline and\nmaximum quotas, which then feed the request and limit dimensions of the\npublished resource utilization metrics. The companion `ResourceQuota`\nand `ResourceQuotaProvider` types in\n`Microsoft.Extensions.Diagnostics.ResourceMonitoring` graduate to stable\nin the same change so that consumers can implement custom quota\nproviders without taking an experimental dependency.\n\nOn the AI side, `Microsoft.Extensions.AI.OpenAI` moves to OpenAI 2.11.0\nand fixes a deserialization bug in `ToolJson.AdditionalProperties` so\nthat JSON Schema `additionalProperties` values shaped as sub-schema\nobjects (for example `{\"type\":\"string\"}`) are preserved instead of\nthrowing during deserialization. `HostedFileContent.SizeInBytes` and\n`HostedFileContent.CreatedAt` graduate to stable since both values are\nconsistently available across hosted-file providers, while `Purpose` and\n`Scope` remain experimental as provider-shaped vocabulary.\n`FunctionInvokingChatClient` drops a backward-compat path that\nauto-marked `ToolApprovalResponseContent` entries with\n`InformationalOnly: true`; consumers that need to continue accepting\nsessions serialized before\n[#&#8203;7468](https://redirect.github.com/dotnet/extensions/issues/7468)\ncan use the sample `ApprovalHistoryNormalizingChatClient` middleware\nadded in the test project.\n\n#### Experimental API Changes\n\n##### Now Stable\n\n- `Microsoft.Extensions.Diagnostics.ResourceMonitoring.Kubernetes`\npackage is now stable\n[#&#8203;7253](https://redirect.github.com/dotnet/extensions/issues/7253)\n- Resource Monitoring `ResourceQuota` and `ResourceQuotaProvider` APIs\nare now stable (previously `EXTEXP0008`)\n[#&#8203;7253](https://redirect.github.com/dotnet/extensions/issues/7253)\n- `HostedFileContent.SizeInBytes` and `HostedFileContent.CreatedAt` are\nnow stable (previously `MEAI001`)\n[#&#8203;7513](https://redirect.github.com/dotnet/extensions/issues/7513)\n\n#### What's Changed\n\n##### AI\n\n- Graduate HostedFileContent.SizeInBytes and HostedFileContent.CreatedAt\n[#&#8203;7513](https://redirect.github.com/dotnet/extensions/issues/7513)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Remove backward-compat InformationalOnly case from FICC; suggest\nmiddleware workaround\n[#&#8203;7538](https://redirect.github.com/dotnet/extensions/issues/7538)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Upgrade OpenAI package from 2.10.0 to 2.11.0\n[#&#8203;7544](https://redirect.github.com/dotnet/extensions/issues/7544)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Fix ToolJson.AdditionalProperties to accept sub-schema objects\n[#&#8203;7546](https://redirect.github.com/dotnet/extensions/issues/7546)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n##### Diagnostics, Health Checks, and Resource Monitoring\n\n- Move Microsoft.Extensions.Diagnostics.ResourceMonitoring.Kubernetes to\nstable\n[#&#8203;7253](https://redirect.github.com/dotnet/extensions/issues/7253)\nby [@&#8203;amadeuszl](https://redirect.github.com/amadeuszl)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n#### Repository Infrastructure Updates\n\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7521](https://redirect.github.com/dotnet/extensions/issues/7521)\n- Bump dotnet-reportgenerator-globaltool from 5.5.9 to 5.5.10\n[#&#8203;7522](https://redirect.github.com/dotnet/extensions/issues/7522)\n- Bump dotnet-coverage from 18.6.2 to 18.7.0\n[#&#8203;7530](https://redirect.github.com/dotnet/extensions/issues/7530)\n- Bump PowerShell from 7.6.1 to 7.6.2\n[#&#8203;7531](https://redirect.github.com/dotnet/extensions/issues/7531)\n- Bump qs from 6.15.1 to 6.15.2 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7532](https://redirect.github.com/dotnet/extensions/issues/7532)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7534](https://redirect.github.com/dotnet/extensions/issues/7534)\n- Bump tmp from 0.2.5 to 0.2.6 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7537](https://redirect.github.com/dotnet/extensions/issues/7537)\n\n#### Acknowledgements\n\n- [@&#8203;ericstj](https://redirect.github.com/ericstj) submitted issue\n[#&#8203;7509](https://redirect.github.com/dotnet/extensions/issues/7509)\n(resolved by\n[#&#8203;7544](https://redirect.github.com/dotnet/extensions/issues/7544))\n- [@&#8203;scottt732](https://redirect.github.com/scottt732) submitted\nissue\n[#&#8203;7540](https://redirect.github.com/dotnet/extensions/issues/7540)\n(resolved by\n[#&#8203;7546](https://redirect.github.com/dotnet/extensions/issues/7546))\n- [@&#8203;DeagleGross](https://redirect.github.com/DeagleGross)\n[@&#8203;wtgodbe](https://redirect.github.com/wtgodbe)\n[@&#8203;dariusclay](https://redirect.github.com/dariusclay)\n[@&#8203;evgenyfedorov2](https://redirect.github.com/evgenyfedorov2)\n[@&#8203;peterwald](https://redirect.github.com/peterwald)\n[@&#8203;PranavSenthilnathan](https://redirect.github.com/PranavSenthilnathan)\n[@&#8203;shyamnamboodiripad](https://redirect.github.com/shyamnamboodiripad)\n[@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n[@&#8203;tarekgh](https://redirect.github.com/tarekgh) reviewed pull\nrequests\n\n**Full Changelog**:\n<https://github.com/dotnet/extensions/compare/v10.6.0...v10.7.0>\n\n###\n[`v10.6.0`](https://redirect.github.com/dotnet/extensions/releases/tag/v10.6.0)\n\nVersion 10.6.0 stabilizes the response continuation token and\nbackground-response APIs in Microsoft.Extensions.AI.Abstractions. Most\nother AI work for May shipped in 10.5.1; this monthly release rolls\nthose changes up alongside dependency updates and a small Resource\nMonitoring cleanup.\n\n#### Experimental API Changes\n\n##### Now Stable\n\n- ResponseContinuationToken and background-response APIs are now stable\n(previously `MEAI001`)\n[#&#8203;7512](https://redirect.github.com/dotnet/extensions/issues/7512)\n\n#### What's Changed\n\n##### AI\n\n- Stabilize ResponseContinuationToken / background-response APIs\n[#&#8203;7512](https://redirect.github.com/dotnet/extensions/issues/7512)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n#### Repository Infrastructure Updates\n\n- Update version to 10.6.0\n[#&#8203;7458](https://redirect.github.com/dotnet/extensions/issues/7458)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7451](https://redirect.github.com/dotnet/extensions/issues/7451)\n- Bump follow-redirects from 1.15.11 to 1.16.0 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript/azure-devops-report/tasks/PublishAIEvaluationReport\n[#&#8203;7469](https://redirect.github.com/dotnet/extensions/issues/7469)\n- Merge release/10.5 into main\n[#&#8203;7470](https://redirect.github.com/dotnet/extensions/issues/7470)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n- Bump microsoft.visualstudio.slngen.tool from 12.0.13 to 12.0.32\n[#&#8203;7484](https://redirect.github.com/dotnet/extensions/issues/7484)\n- Bump postcss from 8.5.9 to 8.5.12 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7494](https://redirect.github.com/dotnet/extensions/issues/7494)\n- Bump dotnet-reportgenerator-globaltool from 5.5.7 to 5.5.9\n[#&#8203;7504](https://redirect.github.com/dotnet/extensions/issues/7504)\n- Rename release-notes skill to write-release-notes\n[#&#8203;7511](https://redirect.github.com/dotnet/extensions/issues/7511)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n\n#### Acknowledgements\n\n- [@&#8203;wtgodbe](https://redirect.github.com/wtgodbe)\n[@&#8203;tarekgh](https://redirect.github.com/tarekgh)\n[@&#8203;peterwald](https://redirect.github.com/peterwald)\n[@&#8203;JeremyLikness](https://redirect.github.com/JeremyLikness)\n[@&#8203;eiriktsarpalis](https://redirect.github.com/eiriktsarpalis)\n[@&#8203;ericstj](https://redirect.github.com/ericstj)\n[@&#8203;evgenyfedorov2](https://redirect.github.com/evgenyfedorov2)\nreviewed pull requests\n\n**Full Changelog**:\n<https://github.com/dotnet/extensions/compare/v10.5.2...v10.6.0>\n\n###\n[`v10.5.0`](https://redirect.github.com/dotnet/extensions/releases/tag/v10.5.0)\n\nHTTP Logging Middleware APIs in\n`Microsoft.AspNetCore.Diagnostics.Middleware` are now stable. This\nrelease also transfers `Microsoft.Extensions.VectorData.Abstractions`\nand `Microsoft.Extensions.VectorData.ConformanceTests` from the\n[Semantic Kernel](https://redirect.github.com/microsoft/semantic-kernel)\nrepository into dotnet/extensions, jumping from 10.1.0 to 10.5.0 for\nconsistent versioning. The release also delivers fixes across the AI\nlibraries, AI Evaluation, and Service Discovery.\n\n##### Breaking Changes\n\n1. **Rename `VectorStoreVectorAttribute` constructor parameter\n[#&#8203;7460](https://redirect.github.com/dotnet/extensions/issues/7460)**\n- The `Dimensions` parameter was renamed to `dimensions` (lowercase).\nThis is a source-breaking change only — binary compatibility is\npreserved.\n- If you use the named argument syntax `new\nVectorStoreVectorAttribute(Dimensions: 1536)`, update it to `new\nVectorStoreVectorAttribute(dimensions: 1536)`.\n\n##### Experimental API Changes\n\n##### Now Stable\n\n- HTTP Logging Middleware APIs are now stable (previously `EXTEXP0013`):\n`AddHttpLogEnricher<T>`, `IHttpLogEnricher`, and\n`RequestHeadersLogEnricherOptions.HeadersDataClasses`\n[#&#8203;7380](https://redirect.github.com/dotnet/extensions/issues/7380)\n\n##### What's Changed\n\n##### AI\n\n- Fix OpenAIResponsesChatClient to respect \"store\":false in responses\n[#&#8203;7417](https://redirect.github.com/dotnet/extensions/issues/7417)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n- Fix InvalidOperationException in CoalesceWebSearchToolCallContent\n[#&#8203;7419](https://redirect.github.com/dotnet/extensions/issues/7419)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n- Handle F# optional parameters in AIFunctionFactory schema generation\n[#&#8203;7439](https://redirect.github.com/dotnet/extensions/issues/7439)\nby [@&#8203;eiriktsarpalis](https://redirect.github.com/eiriktsarpalis)\n- Fix ComputerCallResponseItem using Item.Id instead of CallId\n[#&#8203;7446](https://redirect.github.com/dotnet/extensions/issues/7446)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee)\n- Fix HostedFileContent with image MIME type sent as input_file instead\nof input_image\n[#&#8203;7438](https://redirect.github.com/dotnet/extensions/issues/7438)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;copilot](https://redirect.github.com/copilot))\n- Guard Activity.Current restore with null check in OpenTelemetry\nstreaming clients\n[#&#8203;7443](https://redirect.github.com/dotnet/extensions/issues/7443)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;copilot](https://redirect.github.com/copilot))\n- Enable stateless mode in remote MCP server template (released as\nv1.2.0 on 2026-04-01)\n[#&#8203;7441](https://redirect.github.com/dotnet/extensions/issues/7441)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n\n##### Vector Data\n\n- Move Microsoft.Extensions.VectorData.Abstractions over from Semantic\nKernel\n[#&#8203;7434](https://redirect.github.com/dotnet/extensions/issues/7434)\nby [@&#8203;roji](https://redirect.github.com/roji)\n- Rename VectorStoreVectorAttribute dimensions constructor parameter\n[#&#8203;7460](https://redirect.github.com/dotnet/extensions/issues/7460)\nby [@&#8203;roji](https://redirect.github.com/roji)\n\n##### AI Evaluation\n\n- Add Path Validation for DiskBasedResponseCache and\nDiskBasedResultStore\n[#&#8203;7397](https://redirect.github.com/dotnet/extensions/issues/7397)\nby [@&#8203;peterwald](https://redirect.github.com/peterwald)\n- Update brace-expansion for CVE-2026-33750\n[#&#8203;7457](https://redirect.github.com/dotnet/extensions/issues/7457)\nby [@&#8203;SamMonoRT](https://redirect.github.com/SamMonoRT)\n\n##### ASP.NET Core Extensions\n\n- Removing experimental attribute from Http logging middleware\n[#&#8203;7380](https://redirect.github.com/dotnet/extensions/issues/7380)\nby [@&#8203;mariamgerges](https://redirect.github.com/mariamgerges)\n\n##### Service Discovery\n\n- Implement RFC6761 reserved DNS names handling\n[#&#8203;6924](https://redirect.github.com/dotnet/extensions/issues/6924)\nby [@&#8203;rzikm](https://redirect.github.com/rzikm)\n\n##### Documentation Updates\n\n- Remove per-library CHANGELOG.md files\n[#&#8203;7413](https://redirect.github.com/dotnet/extensions/issues/7413)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n\n##### Test Improvements\n\n- Fix SqliteVectorStoreWriterTests hang: reduce test record count and\nuse conditional MaxTopCount\n[#&#8203;7360](https://redirect.github.com/dotnet/extensions/issues/7360)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;copilot](https://redirect.github.com/copilot))\n\n##### Repository Infrastructure Updates\n\n- Bump minimatch and azure-pipelines-task-lib in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7361](https://redirect.github.com/dotnet/extensions/issues/7361)\n- Remove stale user-level .npmrc instead of npmAuthenticate\n[#&#8203;7366](https://redirect.github.com/dotnet/extensions/issues/7366)\nby [@&#8203;ilonatommy](https://redirect.github.com/ilonatommy)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7374](https://redirect.github.com/dotnet/extensions/issues/7374)\n- Use env vars in build.ps1 to bypass stale agent npm config\n[#&#8203;7376](https://redirect.github.com/dotnet/extensions/issues/7376)\nby [@&#8203;ilonatommy](https://redirect.github.com/ilonatommy)\n- Add a Release-Notes skill\n[#&#8203;7390](https://redirect.github.com/dotnet/extensions/issues/7390)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n- Enable CFSClean\\* policies for extensions-ci-official pipeline\n[#&#8203;7403](https://redirect.github.com/dotnet/extensions/issues/7403)\nby [@&#8203;mmitche](https://redirect.github.com/mmitche)\n- Fix CG alerts for Microsoft.Bcl.Memory\n[#&#8203;7418](https://redirect.github.com/dotnet/extensions/issues/7418)\nby [@&#8203;wtgodbe](https://redirect.github.com/wtgodbe)\n- Bump flatted from 3.3.3 to 3.4.2 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7421](https://redirect.github.com/dotnet/extensions/issues/7421)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7422](https://redirect.github.com/dotnet/extensions/issues/7422)\n- Bump picomatch from 2.3.1 to 2.3.2 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7427](https://redirect.github.com/dotnet/extensions/issues/7427)\n- Bump picomatch in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7429](https://redirect.github.com/dotnet/extensions/issues/7429)\n- Stop using Mariner 2 images\n[#&#8203;7431](https://redirect.github.com/dotnet/extensions/issues/7431)\nby [@&#8203;wtgodbe](https://redirect.github.com/wtgodbe)\n- \\[main] Update dependencies from dotnet/arcade\n[#&#8203;7435](https://redirect.github.com/dotnet/extensions/issues/7435)\n- Bump brace-expansion from 1.1.12 to 1.1.13 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7440](https://redirect.github.com/dotnet/extensions/issues/7440)\n- Bump lodash from 4.17.23 to 4.18.1 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7455](https://redirect.github.com/dotnet/extensions/issues/7455)\n- Bump vite from 6.4.1 to 6.4.2 in\n/src/Libraries/Microsoft.Extensions.AI.Evaluation.Reporting/TypeScript\n[#&#8203;7456](https://redirect.github.com/dotnet/extensions/issues/7456)\n- Correctly publish MEVD.ConformanceTests as nuget packages\n[#&#8203;7459](https://redirect.github.com/dotnet/extensions/issues/7459)\nby [@&#8203;roji](https://redirect.github.com/roji)\n- Update ApiChief baselines for MEAI and MEVD\n[#&#8203;7461](https://redirect.github.com/dotnet/extensions/issues/7461)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n- Upgrade to OpenAI 2.10.0\n[#&#8203;7450](https://redirect.github.com/dotnet/extensions/issues/7450)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n- Use shared DiagnosticIds constants for MEVD experimental APIs\n[#&#8203;7462](https://redirect.github.com/dotnet/extensions/issues/7462)\nby [@&#8203;jeffhandley](https://redirect.github.com/jeffhandley)\n\n##### Acknowledgements\n\n-\n[@&#8203;shyamnamboodiripad](https://redirect.github.com/shyamnamboodiripad)\n[@&#8203;rogerbarreto](https://redirect.github.com/rogerbarreto)\n[@&#8203;ReubenBond](https://redirect.github.com/ReubenBond)\n[@&#8203;evgenyfedorov2](https://redirect.github.com/evgenyfedorov2)\n[@&#8203;tarekgh](https://redirect.github.com/tarekgh) reviewed pull\nrequests\n\n**Full Changelog**:\n<https://github.com/dotnet/extensions/compare/v10.4.1...v10.5.0>\n\n###\n[`v10.4.0`](https://redirect.github.com/dotnet/extensions/releases/tag/v10.4.0)\n\nThis release advances the AI abstractions with new hosted file, web\nsearch, and reasoning content types, stabilizes MCP and tool approval\nAPIs, adds streaming latency metrics to OpenTelemetry instrumentation,\nand delivers bug fixes across caching, data ingestion, and resource\nmonitoring.\n\n#### Experimental API Changes\n\n##### Now Stable\n\n- MCP Server Tool Content and Function Call Approval APIs are now stable\n(previously `MEAI001`)\n[#&#8203;7299](https://redirect.github.com/dotnet/extensions/issues/7299)\n- `FakeLogCollector.GetLogsAsync(CancellationToken)` is now stable\n(previously `EXTEXP0003`)\n[#&#8203;7332](https://redirect.github.com/dotnet/extensions/issues/7332)\n\n##### New Experimental APIs\n\n- New experimental `AddExtendedHttpClientLogging` overloads with\n`wrapHandlersPipeline` parameter (`EXTEXP0013`)\n[#&#8203;7231](https://redirect.github.com/dotnet/extensions/issues/7231)\n\n##### Removed Experimental APIs\n\n- AI Tool Reduction experimental APIs removed (was experimental under\n`MEAI001`)\n[#&#8203;7353](https://redirect.github.com/dotnet/extensions/issues/7353)\n\n#### What's Changed\n\n##### AI\n\n- Add IHostedFileClient and friends\n[#&#8203;7269](https://redirect.github.com/dotnet/extensions/issues/7269)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n- Add web search tool call content\n[#&#8203;7276](https://redirect.github.com/dotnet/extensions/issues/7276)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Surface OpenAI-compatible reasoning_content as TextReasoningContent\n[#&#8203;7295](https://redirect.github.com/dotnet/extensions/issues/7295)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n- MCP/Approvals/Tool Contents stabilization\n[#&#8203;7299](https://redirect.github.com/dotnet/extensions/issues/7299)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee)\n- Implement time_to_first_chunk and time_per_output_chunk streaming\nmetrics in OpenTelemetryChatClient\n[#&#8203;7325](https://redirect.github.com/dotnet/extensions/issues/7325)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Add openai.api.type telemetry attribute to OpenAI IChatClient\nimplementations\n[#&#8203;7316](https://redirect.github.com/dotnet/extensions/issues/7316)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Update OpenTelemetry Gen AI semantic conventions to v1.40\n[#&#8203;7322](https://redirect.github.com/dotnet/extensions/issues/7322)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Fix tool definitions emission regardless of sensitivity setting\n[#&#8203;7346](https://redirect.github.com/dotnet/extensions/issues/7346)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Honor \\[Required] attribute in AI function parameter JSON schema\ngeneration\n[#&#8203;7272](https://redirect.github.com/dotnet/extensions/issues/7272)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- AddAIContentType automatically registers content type against every\nbase in the inheritance chain up to AIContent\n[#&#8203;7358](https://redirect.github.com/dotnet/extensions/issues/7358)\nby [@&#8203;jozkee](https://redirect.github.com/jozkee) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Auto-mark server-handled FunctionCallContent as InformationalOnly\n[#&#8203;7314](https://redirect.github.com/dotnet/extensions/issues/7314)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Map ReasoningEffort.None and ExtraHigh to none and xhigh in OpenAI\nIChatClient implementations\n[#&#8203;7319](https://redirect.github.com/dotnet/extensions/issues/7319)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Handle DynamicMethod reflection limitations in AIFunctionFactory\n[#&#8203;7287](https://redirect.github.com/dotnet/extensions/issues/7287)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Fix Activity.Current nulled during streaming tool invocation\n[#&#8203;7321](https://redirect.github.com/dotnet/extensions/issues/7321)\nby [@&#8203;flaviocdc](https://redirect.github.com/flaviocdc)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Handle FunctionCallOutputResponseItem in streaming response conversion\n[#&#8203;7307](https://redirect.github.com/dotnet/extensions/issues/7307)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Fix serialization of response continuation tokens\n[#&#8203;7356](https://redirect.github.com/dotnet/extensions/issues/7356)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n- Remove AI Tool Reduction experimental APIs\n[#&#8203;7353](https://redirect.github.com/dotnet/extensions/issues/7353)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n(co-authored by [@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Update OpenAI to 2.9.1\n[#&#8203;7349](https://redirect.github.com/dotnet/extensions/issues/7349)\nby [@&#8203;stephentoub](https://redirect.github.com/stephentoub)\n\n##### Telemetry and Observability\n\n- Introduce support for the Gauge metric type\n[#&#8203;7203](https://redirect.github.com/dotnet/extensions/issues/7203)\nby [@&#8203;rainsxng](https://redirect.github.com/rainsxng)\n- Update logging source generator to support generic methods\n[#&#8203;7331](https://redirect.github.com/dotnet/extensions/issues/7331)\nby [@&#8203;svick](https://redirect.github.com/svick) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Update logging source generator to match runtime PR\n[#&#8203;124589](https://redirect.github.com/dotnet/extensions/issues/124589)\n(ref readonly/params/scoped)\n[#&#8203;7333](https://redirect.github.com/dotnet/extensions/issues/7333)\nby [@&#8203;svick](https://redirect.github.com/svick) (co-authored by\n[@&#8203;Copilot](https://redirect.github.com/Copilot))\n- Promote FakeLogCollector.GetLogsAsync(CancellationToken) from\nexperimental to stable\n[#&#8203;7332](https://redirect.github.com/dotnet/extensions/issues/7332)\nby [@&#8203;Demo30](https://redirect.github.com/Demo30)\n- Remove obsolete CS1591 warning suppression from generated file\npreamble\n[#&#8203;7308](https://redirect.github.com/dotnet/extensions/issues/7308)\nby [@&#8203;luissena](https://redirect.github.com/luissena)\n\n##### HTTP Resilience and Diagnostics\n\n- Expose wrapHandlersPipeline parameter in AddExtendedHttpClientLogging\nAPI [#&#8203;7231](https:\n\n> [!IMPORTANT]\n> ✂ PR body was truncated to here.\n\n\n</details>\n\n---\n\n### Configuration\n\n📅 **Schedule**: (UTC)\n\n- Branch creation\n  - On day 1 of the month, every 3 months (`* * 1 */3 *`)\n- Automerge\n  - At any time (no schedule defined)\n\n🚦 **Automerge**: Enabled.\n\n♻ **Rebasing**: Whenever PR becomes conflicted, or you tick the\nrebase/retry checkbox.\n\n👻 **Immortal**: This PR will be recreated if closed unmerged. Get\n[config\nhelp](https://redirect.github.com/renovatebot/renovate/discussions) if\nthat's undesired.\n\n---\n\n- [ ] <!-- rebase-check -->If you want to rebase/retry this PR, check\nthis box\n\n---\n\nThis PR was generated by [Mend Renovate](https://mend.io/renovate/).\nView the [repository job\nlog](https://developer.mend.io/github/miracum/fhir-pseudonymizer).\n\n<!--renovate-debug:eyJjcmVhdGVkSW5WZXIiOiI0NC4xODAuMCIsInVwZGF0ZWRJblZlciI6IjQ0LjE4MC4wIiwidGFyZ2V0QnJhbmNoIjoibWFzdGVyIiwibGFiZWxzIjpbXX0=-->\n\nCo-authored-by: renovate[bot] <29139614+renovate[bot]@users.noreply.github.com>",
          "timestamp": "2026-10-25T15:09:55Z",
          "tree_id": "ab393541c18effd28d4ef1e157d4b1379d9fd765",
          "url": "https://github.com/miracum/fhir-pseudonymizer/commit/2df37b6a0bd3e23816a408c4fed10d9a567d36c7"
        },
        "date": 1792941478894,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizationBenchmarks.AnonymizeLargeBundleWithComplexConfig",
            "value": 115855910.27586207,
            "unit": "ns",
            "range": "± 2780498.746624475"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseAnonymizationYamlFromString",
            "value": 1727574.9526041667,
            "unit": "ns",
            "range": "± 23105.555357016856"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.HmacSha256",
            "value": 1621.1048656191144,
            "unit": "ns",
            "range": "± 3.8497151871915123"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.AnonymizerConfigurationBenchmarks.ParseHipaaAnonymizationYamlFromString",
            "value": 22700498.260416668,
            "unit": "ns",
            "range": "± 155248.23465333084"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.HashingBenchmarks.Blake3",
            "value": 315.14115575381686,
            "unit": "ns",
            "range": "± 0.7955599462967553"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 804356283,
            "unit": "ns",
            "range": "± 1286586.0557113932"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-blake3\")",
            "value": 1854672075.6666667,
            "unit": "ns",
            "range": "± 3354059.5233224966"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 792631377.3333334,
            "unit": "ns",
            "range": "± 1759330.3165970095"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"cryptoHash-hmacSha256\")",
            "value": 1962113272.6666667,
            "unit": "ns",
            "range": "± 2796789.371406852"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 796631087.3333334,
            "unit": "ns",
            "range": "± 4623556.589862649"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"bundle\", Method: \"keep\")",
            "value": 1404824552,
            "unit": "ns",
            "range": "± 4602815.065993527"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 404865725,
            "unit": "ns",
            "range": "± 69605354.95081231"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-blake3\")",
            "value": 554918781.6666666,
            "unit": "ns",
            "range": "± 312286478.61033267"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 293334950,
            "unit": "ns",
            "range": "± 146586667.02065122"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"cryptoHash-hmacSha256\")",
            "value": 622332936.3333334,
            "unit": "ns",
            "range": "± 268059016.23578495"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.ParseAndSerializeOnly(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 353100217,
            "unit": "ns",
            "range": "± 122907346.82476513"
          },
          {
            "name": "FhirPseudonymizer.Benchmarks.LargeProvenanceBenchmarks.DeIdentify(ResourceCount: 50000, Shape: \"provenance\", Method: \"keep\")",
            "value": 480348743.3333333,
            "unit": "ns",
            "range": "± 214447477.2746226"
          }
        ]
      }
    ]
  }
}
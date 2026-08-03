window.BENCHMARK_DATA = {
  "lastUpdate": 1785765383078,
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
      }
    ]
  }
}
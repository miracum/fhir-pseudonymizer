# Generating MockServer's initialization config

Because it's easier to read, the initializers are managed as YAML and converted to JSON
for MockServer.

Run:

```sh
yq -o json hack/mocks/initializer.yaml > hack/mocks/initializer.json
```

to convert.

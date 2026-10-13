# Vfps

Set `PseudonymizationService=Vfps` to create pseudonyms using
[Vfps](https://miracum.github.io/vfps/), a very fast and resource-efficient pseudonymization
service, over gRPC. The rule's `domain` (or `namespace`) is the Vfps namespace.

| Environment Variable                            | Description                                                                                                                                                                                                                        | Default |
| ----------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| `Vfps__Address`                                 | The Vfps service address, e.g. `http://vfps:8081` or `dns:///vfps-headless.vfps.svc.cluster.local:8081`. See [Vfps load balancing](#load-balancing) for which to use.                                                              | `""`    |
| `Vfps__UnsafeUseInsecureChannelCallCredentials` | If set to `true`, `CallCredentials` are applied to gRPC calls made by an insecure channel. Sending authentication headers over an insecure connection has security implications and shouldn't be done in production environments.  | `true`  |
| `Vfps__UseTls`                                  | If set to `true`, creates client-side SSL credentials loaded from disk file pointed to by the `GRPC_DEFAULT_SSL_ROOTS_FILE_PATH` environment variable. If that fails, gets the roots certificates from a well known place on disk. | `false` |
| `Vfps__Retry__Count`                            | The number of times a failed request is retried, with exponential backoff.                                                                                                                                                         | `3`     |

## Load balancing

gRPC sends every call over one long-lived HTTP/2 connection, and a regular Kubernetes Service
balances connections, not calls. With an `http://` address pointing at the Vfps Service, each FHIR
Pseudonymizer replica sends all of its calls to one Vfps pod at a time. The connection is recreated
every two minutes, so load evens out over time and newly added Vfps pods are picked up within about
that long. This is the simpler option, and enough when Vfps runs several replicas for availability
rather than throughput.

To spread individual calls across all Vfps replicas, use a `dns:///` address pointing at the Vfps
chart's headless Service (`<release>-headless`), e.g.
`dns:///vfps-headless.vfps.svc.cluster.local:8081`. The client then load-balances round-robin
across every ready pod, re-resolves the address every 15 seconds to pick up added or replaced pods,
and gives up on a connection attempt after 5 seconds instead of waiting on a pod that no longer
exists. Don't combine `dns:///` with a regular Service: it resolves to a single virtual IP, so there
is nothing to balance.

## Basic auth

| Environment Variable          | Description                                                                                                                               | Default |
| ----------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| `Vfps__Auth__Basic__Username` | The HTTP basic auth username to connect to the Vfps service. Used in the `Authorization: Basic` metadata header value for the gRPC calls. | `""`    |
| `Vfps__Auth__Basic__Password` | The HTTP basic auth password to connect to the Vfps service.                                                                              | `""`    |

## OAuth

Setting `Vfps__Auth__OAuth__TokenEndpoint` enables the OAuth client credentials flow: an access token is
fetched from the token endpoint, cached until shortly before it expires, and sent as the
`Authorization: Bearer` metadata header of every gRPC call. This is what a Vfps deployment started with
`Authorization__IsEnabled=true` expects - it only accepts bearer tokens on its gRPC API, so basic auth
does not work against such a deployment. If both are configured, OAuth takes precedence.

| Environment Variable               | Description                       | Default |
| ---------------------------------- | --------------------------------- | ------- |
| `Vfps__Auth__OAuth__TokenEndpoint` | The URL of the token endpoint     | `""`    |
| `Vfps__Auth__OAuth__ClientId`      | The client ID                     | `""`    |
| `Vfps__Auth__OAuth__ClientSecret`  | The static (shared) client secret | `""`    |
| `Vfps__Auth__OAuth__Scope`         | The scope                         | `""`    |
| `Vfps__Auth__OAuth__Resource`      | The resource                      | `""`    |

Note that the token is only attached to calls made over an insecure channel if
`Vfps__UnsafeUseInsecureChannelCallCredentials` is `true` (the default). Prefer `Vfps__UseTls=true`
instead, so the token isn't sent in plaintext.

## Access token

Vfps can also issue credentials itself. Set `Vfps__Auth__AccessToken`
to a token created in the Vfps admin UI and it is sent as the `Authorization: Bearer`
metadata header of every gRPC call.

| Environment Variable      | Description                                                      | Default |
| ------------------------- | ---------------------------------------------------------------- | ------- |
| `Vfps__Auth__AccessToken` | A Vfps-issued access token, e.g. `vfps_sat_...`. Sent unchanged. | `""`    |

Use a **service account** token (`vfps_sat_...`), created on the Vfps _Service accounts_ page,
rather than a personal one (`vfps_pat_...`): a personal token carries the access of the person who
created it and stops working when they leave. Requires `Authorization__AccessTokens__IsEnabled` on
the Vfps side.

## Namespace access

Whichever credential is used, the principal this service authenticates as needs access to each
namespace it uses: **write** access for creating pseudonyms, plus **reverse-lookup** access if
de-pseudonymization is used.

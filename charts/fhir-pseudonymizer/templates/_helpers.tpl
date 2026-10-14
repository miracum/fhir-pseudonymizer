{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "fhir-pseudonymizer.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "fhir-pseudonymizer.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "fhir-pseudonymizer.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "fhir-pseudonymizer.labels" -}}
helm.sh/chart: {{ include "fhir-pseudonymizer.chart" . }}
{{ include "fhir-pseudonymizer.matchLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "fhir-pseudonymizer.matchLabels" -}}
app.kubernetes.io/name: {{ include "fhir-pseudonymizer.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Render the anonymization config. It can be set either as a string or as an object, which is converted to YAML.
*/}}
{{- define "fhir-pseudonymizer.anonymizationConfig" -}}
{{- if kindIs "string" .Values.anonymizationConfig -}}
{{- .Values.anonymizationConfig -}}
{{- else -}}
{{- toYaml .Values.anonymizationConfig -}}
{{- end -}}
{{- end -}}

{{/*
Create the name of the service account to use
*/}}
{{- define "fhir-pseudonymizer.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "fhir-pseudonymizer.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/*
Validate the Kafka configuration, turning misconfigurations that would otherwise only result in
an idle - but healthy looking - pod into render-time errors.
*/}}
{{- define "fhir-pseudonymizer.kafka.validateValues" -}}
{{- if .Values.kafka.enabled -}}
{{- if not .Values.kafka.bootstrapServers -}}
{{- fail "kafka.enabled is set, but kafka.bootstrapServers is empty. Set it to the Kafka bootstrap servers, e.g. 'kafka:9092'." -}}
{{- end -}}
{{- range $key, $_ := .Values.kafka.client -}}
{{- if eq (lower $key) "bootstrapservers" -}}
{{- fail "Set the Kafka bootstrap servers via kafka.bootstrapServers, not via kafka.client." -}}
{{- end -}}
{{- end -}}
{{- else if .Values.kafka.topics -}}
{{- fail "kafka.topics is set, but kafka.enabled is false, so no topics would be consumed." -}}
{{- end -}}
{{- end -}}

{{/*
Whether an anonymization config is configured, from any source. Renders "true" or nothing.
*/}}
{{- define "fhir-pseudonymizer.hasAnonymizationConfig" -}}
{{- if or .Values.existingAnonymizationConfigMap (.Files.Glob "files/anonymization.yaml") .Values.anonymizationConfig -}}
true
{{- end -}}
{{- end -}}

{{/*
Whether the chart creates the ConfigMap holding the anonymization config itself. Renders "true" or nothing.
*/}}
{{- define "fhir-pseudonymizer.createAnonymizationConfigMap" -}}
{{- if and (not .Values.existingAnonymizationConfigMap) (or (.Files.Glob "files/anonymization.yaml") .Values.anonymizationConfig) -}}
true
{{- end -}}
{{- end -}}

{{/*
The name of the ConfigMap holding the anonymization config
*/}}
{{- define "fhir-pseudonymizer.anonymizationConfigMapName" -}}
{{- if .Values.existingAnonymizationConfigMap -}}
{{- tpl .Values.existingAnonymizationConfigMap . -}}
{{- else -}}
{{- include "fhir-pseudonymizer.fullname" . }}-anonymization
{{- end -}}
{{- end -}}

{{/*
The key of the anonymization config inside the ConfigMap, which is also the name of the file mounted into the container
*/}}
{{- define "fhir-pseudonymizer.anonymizationConfigKey" -}}
{{- if .Values.existingAnonymizationConfigMap -}}
{{- .Values.existingAnonymizationConfigMapKey -}}
{{- else -}}
anonymization.yaml
{{- end -}}
{{- end -}}

{{/*
Validate the anonymization config settings.
*/}}
{{- define "fhir-pseudonymizer.anonymizationConfig.validateValues" -}}
{{- if and .Values.existingAnonymizationConfigMap .Values.anonymizationConfig -}}
{{- fail "existingAnonymizationConfigMap and anonymizationConfig are both set. Use only one of them." -}}
{{- end -}}
{{- if and .Values.existingAnonymizationConfigMap (not .Values.existingAnonymizationConfigMapKey) -}}
{{- fail "existingAnonymizationConfigMap is set, but existingAnonymizationConfigMapKey is empty." -}}
{{- end -}}
{{- end -}}

# pabc

![Version: 0.1.0](https://img.shields.io/badge/Version-0.1.0-informational?style=flat-square) ![Type: application](https://img.shields.io/badge/Type-application-informational?style=flat-square) ![AppVersion: 0.0.0](https://img.shields.io/badge/AppVersion-0.0.0-informational?style=flat-square)

A helm chart for the Platform Autorisatie Beheer Component.

## Requirements

| Repository | Name | Version |
|------------|------|---------|
| https://charts.bitnami.com/bitnami | postgresql | 12.5.6 |
> [!NOTE]
> For production environments, it is recommended to use [CloudNativePG](https://github.com/cloudnative-pg/cloudnative-pg) for PostgreSQL in stead of the dependent Helm charts included here. The bundled charts are primarily intended for testing and development purposes. Also, be aware of the upcoming changes to the bitnami catalog described in this [issue](https://github.com/bitnami/containers/issues/83267).

## Values

| Key | Type | Default | Description |
|-----|------|---------|-------------|
| affinity | object | `{}` |  |
| autoscaling.enabled | bool | `false` |  |
| autoscaling.maxReplicas | int | `100` |  |
| autoscaling.minReplicas | int | `1` |  |
| autoscaling.targetCPUUtilizationPercentage | int | `80` |  |
| autoscaling.targetMemoryUtilizationPercentage | int | `80` |  |
| azureVaultSecret.contentType | string | `""` |  |
| azureVaultSecret.objectName | string | `""` |  |
| azureVaultSecret.secretName | string | `"{{ .Values.existingSecret }}"` |  |
| azureVaultSecret.vaultName | string | `nil` |  |
| existingSecret | string | `nil` |  |
| extraIngress | list | `[]` | Specify extra ingresses, for example if you have multiple ingress classes |
| fullnameOverride | string | `""` |  |
| image.pullPolicy | string | `"IfNotPresent"` |  |
| image.repository | string | `"ghcr.io/platform-autorisatie-beheer-component/pabc-api"` |  |
| image.tag | string | `""` |  |
| imagePullSecrets | list | `[]` |  |
| ingress.annotations | object | `{}` |  |
| ingress.className | string | `""` |  |
| ingress.enabled | bool | `false` |  |
| ingress.hosts | list | `[]` | ingress hosts |
| ingress.tls | list | `[]` |  |
| livenessProbe.failureThreshold | int | `6` |  |
| livenessProbe.initialDelaySeconds | int | `60` |  |
| livenessProbe.periodSeconds | int | `10` |  |
| livenessProbe.successThreshold | int | `1` |  |
| livenessProbe.timeoutSeconds | int | `5` |  |
| migrations.dataSetPath | string | `nil` | Optionally specify a path to a data set json file to insert into the database. <details> <summary>More information</summary>You can use the extraVolumes and extraVolumeMounts values to mount a data set file into the running container. The file needs to be valid according to <a href="../../PABC.MigrationService/dataset.schema.json">the json schema</a></details> |
| migrations.extraVolumeMounts | list | `[]` | Optionally specify extra list of additional volumeMounts, for example to trust extra ca certificates. |
| migrations.extraVolumes | list | `[]` | Optionally specify extra list of additional volumes, for example to trust extra ca certificates. |
| migrations.image.pullPolicy | string | `"IfNotPresent"` |  |
| migrations.image.repository | string | `"ghcr.io/platform-autorisatie-beheer-component/pabc-migrations"` |  |
| migrations.image.tag | string | `""` |  |
| migrations.restartPolicy | string | `"Never"` |  |
| nameOverride | string | `""` |  |
| nodeSelector | object | `{}` |  |
| pdb.create | bool | `false` |  |
| pdb.maxUnavailable | string | `""` |  |
| pdb.minAvailable | int | `1` |  |
| persistence.enabled | bool | `true` |  |
| persistence.existingClaim | string | `nil` |  |
| persistence.size | string | `"1Gi"` |  |
| persistence.storageClassName | string | `""` |  |
| podAnnotations | object | `{}` |  |
| podLabels | object | `{}` |  |
| podSecurityContext.fsGroup | int | `1000` |  |
| postgresql.auth.database | string | `""` |  |
| postgresql.auth.password | string | `""` |  |
| postgresql.auth.postgresPassword | string | `""` |  |
| postgresql.auth.username | string | `""` |  |
| postgresql.enabled | bool | `true` |  |
| postgresql.image.repository | string | `"bitnamilegacy/postgresql"` |  |
| postgresql.metrics.enabled | bool | `false` |  |
| postgresql.metrics.image.repository | string | `"bitnamilegacy/postgres-exporter"` |  |
| postgresql.primary.persistence.enabled | bool | `true` |  |
| postgresql.primary.persistence.size | string | `"8Gi"` |  |
| postgresql.volumePermissions.image.repository | string | `"bitnamilegacy/os-shell"` |  |
| readinessProbe.failureThreshold | int | `6` |  |
| readinessProbe.initialDelaySeconds | int | `30` |  |
| readinessProbe.periodSeconds | int | `10` |  |
| readinessProbe.successThreshold | int | `1` |  |
| readinessProbe.timeoutSeconds | int | `5` |  |
| replicaCount | int | `1` |  |
| resources | object | `{}` |  |
| securityContext.capabilities.drop[0] | string | `"ALL"` |  |
| securityContext.readOnlyRootFilesystem | bool | `false` |  |
| securityContext.runAsNonRoot | bool | `true` |  |
| securityContext.runAsUser | int | `1000` |  |
| service.port | int | `80` |  |
| service.type | string | `"ClusterIP"` |  |
| serviceAccount.annotations | object | `{}` |  |
| serviceAccount.automountServiceAccountToken | bool | `true` |  |
| serviceAccount.create | bool | `true` |  |
| serviceAccount.name | string | `""` |  |
| settings.apiKeys | list | `[]` | List of issued API keys. <details> <summary>More information </summary>The API keys can have any arbitrary value. Calls to the API must include an 'X-API-KEY' header with one of these API keys as the value.</details> |
| settings.aspnetcore.environment | string | `""` |  |
| settings.aspnetcore.forwardedHeadersEnabled | bool | `true` |  |
| settings.aspnetcore.httpPorts | string | `""` |  |
| settings.database.host | string | `""` |  |
| settings.database.name | string | `"pabc"` |  |
| settings.database.password | string | `""` |  |
| settings.database.port | int | `5432` |  |
| settings.database.username | string | `""` |  |
| tolerations | list | `[]` |  |

----------------------------------------------
Autogenerated from chart metadata using [helm-docs v1.14.2](https://github.com/norwoodj/helm-docs/releases/v1.14.2)

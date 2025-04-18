CAS Adapter
=
[![ci-cas-adapter](https://github.com/bcgov/cas-interface-service/actions/workflows/ci-cas-adapter.yml/badge.svg)](https://github.com/bcgov/cas-interface-services/actions/workflows/ci-cas-adapter.yml)
[![cd-cas-adapter](https://github.com/bcgov/cas-interface-service/actions/workflows/cd-cas-adapter.yml/badge.svg)](https://github.com/bcgov/cas-interface-services/actions/workflows/cd-cas-adapter.yml)
[![codecov](https://codecov.io/gh/bcgov/cas-interface-service/branch/main/graph/badge.svg?token=d5woacAxGD)](https://codecov.io/gh/bcgov/cas-interface-services)

An adapter to access a collection of CAS APIs

Technology Stack
-

| Layer | Technology |
|-------|------------|
| Microservices | C Sharp - Dotnet 9 |
| Authentication | OAuth |
| Container Platform | OpenShift 4 |
| Logging | Splunk, and Serilog |
| CI/CD Pipeline | GitHub Actions, Kubernetes Pipelines (Tekton) |

Repository Map
--------------

- **test**: Source for unit tests
- **.openshift**: Various OpenShift related material, including instructions for setup and templates.

Installation
------------
This application is meant to be deployed to RedHat OpenShift version 4. Full instructions to deploy to OpenShift are in the `.openshift` directory.


Run Docker
-
```
docker build . -t cas-adapter`
docker run --rm -p 8080:8080 --name cas-adapter cas-adapter
```

Developer Prerequisites
-----------------------

**Public Application**
- .Net SDK (9.0)
- .NET Core IDE such as Visual Studio or VS Code
- JAG VPN with access to MS Dynamics

**DevOps**
- RedHat OpenShift tools
- Docker/Podman
- A familiarity with GitHub Actions and Tekton Pipelines

DevOps Process
-------------

## Github Actions

There are two main categories of Github Actions used in this project:

1. Continuous Integration - these pipelines are used to integrate code from a Fork into the trunk "main"
2. Continuous Delivery - these pipelines are used to assist in building code for delivery (deployment) in OpenShift.

An example of other Github Actions also used in the project is the stats action Code Cov uses.

### Configuration Required for Github Actions

1. OPENSHIFT_NAMESPACE - set to the full project identifier where images are stored. For example proj-tools.

2. DOCKER_USERNAME - the username for a Service Account with access to read / write OPENSHIFT_NAMESPACE images

Note that this username must have the following role bindings set:

`oc policy add-role-to-user system:image-builder system:serviceaccount:<namespace>:<username>`

3. OPENSHIFT_PASSWORD - the TOKEN from the OpenShift secret for the username in OPENSHIFT_USERNAME

4. OPENSHIFT_REGISTRY - the hostname for the public image repository.  You can get this by viewing the details for an image in the given project; only put the hostname portion.

## Tekton Pipelines

NOTE: Tekton Pipelines are not implemented yet.

There is also a series of Tekton (Kubernetes) pipelines:

### Promotion to TEST
To promote code to TEST, login to OpenShift and start the Kubernetes Pipeline for Promote to Test.

### Promotion to PROD
To promote code to PROD, login to OpenShift and start the Kubernetes Pipeline for Promote to Prod. Not that this pipeline will also make a backup of the current PROD deployment.

### Restore PROD from backup
If you wish to revert to the previous PROD deployment, login to OpenShift and start the Kubernetes Pipeline for Restore PROD from Backup.

Resources
-

[Proxies, Policies, and Redirects](https://emcr.atlassian.net/wiki/spaces/DRP/pages/238649445/API+Policies+Proxies+and+Redirects)

Contribution
------------

Please report any [issues](https://github.com/bcgov/https://github.com/bcgov/rsbc-dmf/issues).

[Pull requests](https://github.com/bcgov/rsbc-dmf/pulls) are always welcome.

If you would like to contribute, please see our [contributing](CONTRIBUTING.md) guidelines.

Please note that this project is released with a [Contributor Code of Conduct](CODE_OF_CONDUCT.md). By participating in this project you agree to abide by its terms.

License
-------

    Copyright 2022 Province of British Columbia

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at 

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.

Maintenance
-----------

This repository is maintained by [BC Attorney General]( https://www2.gov.bc.ca/gov/content/governments/organizational-structure/ministries-organizations/ministries/justice-attorney-general ).



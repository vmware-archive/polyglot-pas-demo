# Loan Application Service

This service is the entry point for getting a loan from the bank. It relies on the backing loan approval service to complete the process. 

- .NET
- Visual Studio solution
- Steeltoe
- Spring Cloud Config Client
- Spring Cloud Registry Client

## External config

The application has very little internal configuration declared (appsetting.json). Instead it following a config-first model where it relys on an eternal config service to provide everything. The config can be found in the [config](https://github.com/pivotal/polyglot-pas-demo/tree/master/config) folder of this repo.

## Building and Running locally

To run locally you will need to start up a few docker images that house Spring Cloud Config and Spring Cloud Registry. Refer to the [Steeltoe DockerFiles](https://github.com/SteeltoeOSS/Dockerfiles) to get started. 

Once those services are running on the default ports, simply "Start Debugging" the app in Visual Studio and good things will happen.

## Building and Running in the Cloud

To build the application with all the necessary cloud dependencies use, first publish the app from Visual Studio to a local folder. The you'll have two choices for publishing...

1. If you would like to use the free [Pivotal Web Services (PWS)](https://run.pivotal.io) then update the Ci/cf-stage.yml manifest to target Linux, and push.

2. If your company's Pivotal Application Service (PAS) foundation has the Windows feature installed (PASW), then push using the provided manifest.

Either way, you will need the [`cf` command line client](https://docs.run.pivotal.io/cf-cli/cf-help.html) installed.

```bash
cf push <application-name> -f <manifest-file>
```

> Sample `cf-` *stage* and *prod* `.yml` manifest files for Cloud Foundry have been provided.
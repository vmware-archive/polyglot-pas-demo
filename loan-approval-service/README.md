# Loan Checker Service

This service can be used to check and approve Loan Applications. People asking for too much money can't
have any. People on the naughty list can't have any money either. It's that simple.

- Java
- Maven
- Spring Boot
- Spring Cloud Config Client
- Spring Cloud Registry Client

When running in the cloud, the configuration (max loan amount and naughty list) is externalised here:

[link]

## Building and Running locally

You don't need the Spring Cloud Services to be available in order to run locally. A maven profile called `simcloud` has been provided to ensure that sensible configuration is used.

```bash
mvn clean install # to clean, build and test the service
mvn spring-boot:run -P simcloud # to run the service on localhost
```

If you have HTTPIE installed, you can test the service with simple calls like this one:

```bash
http POST localhost/check id=D816D833-467A-4737-BE75-D2F894A9F804 name=Trunp amount=1000 status=PENDING
http POST localhost/check id=`uuidgen` name=Wilcock amount=1000 status=PENDING
http GET localhost/statuses
```

You can smoke test this local server using the smoke test script provided by first setting `localhost` as the `URL` variable as shown below.

## Building and Running in the Cloud

To build the application with all the necessary cloud dependencies use `mvn clean install`.

```bash
mvn clean install
```

To run the application in the cloud, use Pivotal Web Services (PWS) or your company's Pivotal Application Service (PAS) foundation. You will need the `cf` command line client installed.

```bash
cf push <application-name> -f <manifest-file>
```

> Sample *test* and *prod* `manifest.yml` files for Cloud Foundry have been provided.

The manifest will control what gets deployed. When developing and building the JAR locally, I use `local-manifest.yml` to push it to my PWS/PAS environment. When running in the cloud the application expects certain services to be present, namely Spring Cloud Config and Spring Cloud Registry. The expected names for these service instances is contain in the `manifest.yml`.

## Smoke Testing Any Environment

You can easily smoke test any environment (local or cloud) using the `/ci/tasks/smoke-test-loan-checker.sh` script (bash). This script just needs a single environment variable `URL` to be set to the environment URL that you wish to test.

```bash
env URL=loan-checker-prod.apps.cloudyazure.io ./ci/tasks/smoke-test-loan-checker.sh
env URL=localhost ./ci/tasks/smoke-test-loan-checker.sh
```

> To use this script on a locally hosted environment, use `env URL=localhost ...`.

## Performing CI with Concourse

Two sample pipelines have been included, one for pushing to `test` and one for promoting to `prod`. They are just simple samples, not too realistic. You'll need to add a file called `private.yml` containing keys, endpoints and other missing settings that are custom to your environment in order to use them with your Concourse platform.

[1]: http://dolszewski.com/spring/spring-boot-properties-per-maven-profile/
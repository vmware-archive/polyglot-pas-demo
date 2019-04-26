# Loan Checker Service

This service can be used to check and approve Loan Applications. People asking for too much money can't
have any. People on the naughty list can't have any money either. It's that simple.

- Java
- Spring Boot 2.1.4
- Spring Cloud Config Client
- Spring Cloud Registry Client
- Maven

When running in the cloud, the configuration (max loan amount and naughty list) is externalised here:

[link]

## Running locally

When running locally (and by default) the POM.xml will instruct maven to exclude certain classes and dependencies that are
only required when running in the cloud. Profiles are used to acheive this. The "local" profile is used by default.

```bash
mvn clean install
mvn spring-boot:run
```

If you have HTTPIE installed, you can test the service with simple calls like this one:

```bash
http POST :8080/check id=D816D833-467A-4737-BE75-D2F894A9F804 name=Trunp amount=1000 status=PENDING
http GET :8080/statuses
```

## Building for the cloud

```bash
mvn clean install -P cloud -DskipTests
```

## Running in the cloud

```bash
cf push <application-name> -f <manifest-file>

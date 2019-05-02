# Polyglot PAS Demo

This code shows how microservices built using .Net or Java can easily use and Spring Cloud Services to solve cloud related architectural challenges such as service discovery or externalised configuration.

This repository is organised into two pieces, each containing an application. The .Net application is called "Loan Applications" and is contained within the `loan-application-service` folder. The Java application is called "Loan Checker" and is stored in the `loan-approval-service` folder.

## Loan Applications (.Net)

You can explore the .Net code in the `loan-application-service` folder [here][1].

## Loan Checker (Java)

You can explore the Java code in the `loan-approval-service` folder [here][2].

## Configuration

The Spring Cloud Config Server we have used to externalise our configuration expects to find that configuration in a Git repository. The configuration used by the Java and .Net applications can be found in [this `config` folder][3]. The configuration is organised into profiles, specifically a `test` profile and a `prod` profile. This is to illustrate just one of the many ways in which your configuration can be managed.

[1]: /loan-application-service
[2]: /loan-approval-service
[3]: /config

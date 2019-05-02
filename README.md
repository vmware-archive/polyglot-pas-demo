# Polyglot PAS Demo

This code shows how microservices built using .Net or Java can easily use and Spring Cloud Services to solve cloud related architectural challenges such as service discovery or externalised configuration.

This repository is organised into two pieces, each containing an application. The .Net application is called "Loan Applications" and is contained within the `loan-application-service` folder. The Java application is called "Loan Checker" and is stored in the `loan-approval-service` folder.

## Loan Applications (.Net)

You can explore the .Net code in the `loan-application-service` folder [here][1].

## Loan Checker (Java)

You can explore the Java code in the `loan-approval-service` folder [here][2].

## Configuration

The Spring Cloud Config Server we have used to externalise our configuration expects to find that configuration in a Git repository. The configuration used by the Java and .Net applications can be found in the `config` folder [here][3]. This configuration is for both apps and is organised into profiles, specifically a `test` profile and a `prod` profile. This is to illustrate just one of the many ways in which your configuration can be organised and managed.

## Notes: Running these applications on PAS

The `config-server.json` files (for example [this one][4]) can be used to tell the config server which Git repository you want to use as your config backing store. To create a config-server on Pivotal Application Server (or on [Pivotal Web Services][5]) you can use the services in the marketplace and commission your services using the `cf` command line client as follows...

```bash
cf create-service p-config-server standard config -c config-server.json # May differ depending on your PAS environment
```

> Don't forget to bind your services to your apps using `cf bind-service <app-name> <service-name>` after they have been created. 
 
The applications in this demo do spell out their service requirements clearly in their Cloud Foundry `manifest.yml` files, [like this one][6].

You can sign up for Pivotal Web Services at [run.pivotal.io][5]. You can get free credit and it doesn't require a credit card. 

[1]: /loan-application-service
[2]: /loan-approval-service
[3]: /config
[4]: /config/test/config-server.json
[5]: https://run.pivotal.io
[6]: /loan-approval-service/test-manifest.yml

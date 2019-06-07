# Java & .Net - Better Together

A MicroSoft Build 2019 presentation by [David Dieruf][14] and [Ben Wilcock][15].

* The Video of the MS Build talk is [here][5].
* The slide deck accompanying the talk are [here][13].

## Introduction

This code shows how polyglot microservices built using .Net and Java can easily coexist thanks to [Spring][17], [Spring Cloud Services][10], [Steeltoe][11], and [Pivotal Application Service (PAS)][7].

In this code we offer technical guidance on how to...

* [externalize configuration][18] in .Net and Java microservices using Spring Cloud Config Server
* register and discover services dynamically at runtime in .Net and Java using Spring Cloud Service Registry

## Prerequisites

This code is intended for use with [Pivotal Application Service (PAS)][7] with both the [Spring Cloud Services broker][10] and the [Azure Services broker][8] installed. 

> You can try [PAS for free for 90 days on Azure][9].

> **Pivotal Application Service:** [PAS][7] is a truly polyglot IaaS abstraction layer for both Windows and Linux based applications. it offers a uniquely unified developer experience so that programmers working in many different languages can get their apps into production fast.

## Understanding the code layout

This repository is organized into two distinct pieces. The .Net application is called "Loan Applications" and is contained within the `/loan-application-service` folder. The Java application is called "Loan Checker" and is stored in the `/loan-approval-service` folder.

In the [talk][5] and the [accompanying slides][13] we discuss how a merger between a fictitious bank and its supplier has brought these two applications together under one roof and we illustrate their 'before and after' architectural landscapes.

### Loan Applications (.Net)

You can explore the .Net code in the `/loan-application-service` folder [here][1]. The Loan Applications service is the API that the outside world will use to request a loan from our fictitious bank.

### Loan Checker (Java)

You can explore the Java code in the `/loan-approval-service` folder [here][2]. The Loan Checker application acts as a quality gate, checking that loans are not being given to people on te 'naughty list'.

## Configuration

Spring Cloud Config Server expects to find its client's configuration files in a Git repository. In this case, the configuration used by both the Java and the .Net applications can be found in the `/config` folder of this repository ([here][3]). This configuration is organised into "profiles", specifically a `test` profile and a `prod` profile in order to illustrate just one of the many ways in which Spring Cloud Config can manage your configuration.

## Running these applications on PAS

The assumption being made is that you already have access to a PAS instance with all the necessary pre-requisites installed (see above for details). 

We're also assuming that you're comfortable using the `cf` command, and have used it before to comission services and deploy applications. If you have not, it really is super simple - [try this guide][12] or talk to your Pivotal account team for help.

Creating the AzureDB and Spring Cloud Registry services is fairly straight-forward (just remember to use the service names in the `manifest.yml` files for each app for a smooth experience). 

Spring Cloud Config Server demands a more specific configuration step, as detailed below.

### Telling Spring Cloud Config Server where the config is stored

To create a Spring Cloud Config Server instance correctly, you can use the `cf` command line client as follows...

```bash
cf create-service p-config-server standard config -c config-server.json
```

The `config-server.json` file (for example [this one][4]) is used to tell the config server which Git repository location you want to use as your config's 'backing store'. 

The documentation for Spring Cloud Services (registry and config server) can be found [here][16].

> **Note:** The applications in this demo spell out their service requirements (and the service-names expected) in their Cloud Foundry `manifest.yml` files ([like this one][6]). Use these predefined service names when you commission your services for a more enjoyable deployment experience.

We hope you enjoyed [the talk][5] and get something useful from this code sample. If you would like to contact us or give us feedback, you can find [David Dieruf][14] and [Ben Wilcock][15] on Twitter.

[1]: /loan-application-service
[2]: /loan-approval-service
[3]: /config
[4]: /config/test/config-server.json
[5]: https://mybuild.techcommunity.microsoft.com/sessions/77161
[6]: /loan-approval-service/test-manifest.yml
[7]: https://pivotal.io/platform/pivotal-application-service
[8]: https://pivotal.io/platform/services-marketplace/data-management/microsoft-azure
[9]: https://azuremarketplace.microsoft.com/en-us/marketplace/apps/pivotal.pivotal-cloud-foundry?tab=PlansAndPrice
[10]: https://pivotal.io/platform/services-marketplace/microservices-management/spring-cloud-services
[11]: https://pivotal.io/platform/services-marketplace/microservices-management/steeltoe
[12]: https://pivotal.io/platform/pcf-tutorials/getting-started-with-pivotal-cloud-foundry
[13]: https://www.slideshare.net/BenWilcock1/java-and-net-together-at-scale-microservices-architecture
[14]: https://twitter.com/DierufDavid
[15]: https://twitter.com/benbravo73
[16]: https://docs.pivotal.io/spring-cloud-services/2-0/common/index.html
[17]: https://spring.io]
[18]: https://12factor.net/config
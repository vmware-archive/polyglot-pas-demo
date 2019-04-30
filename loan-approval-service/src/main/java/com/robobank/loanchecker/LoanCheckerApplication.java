package com.robobank.loanchecker;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.cloud.client.discovery.EnableDiscoveryClient;

@SpringBootApplication
@EnableDiscoveryClient
@EnableConfigurationProperties
public class LoanCheckerApplication {

	public static void main(String[] args) {
		SpringApplication.run(LoanCheckerApplication.class, args);
	}

}

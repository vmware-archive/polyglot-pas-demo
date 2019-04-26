package com.robobank.loanchecker;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.client.discovery.EnableDiscoveryClient;

@SpringBootApplication
@EnableDiscoveryClient
public class CloudLoanCheckerApplication {

	public static void main(String[] args) {
		SpringApplication.run(CloudLoanCheckerApplication.class, args);
	}

}

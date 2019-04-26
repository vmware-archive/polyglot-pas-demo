package com.robobank.loanchecker;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.properties.EnableConfigurationProperties;

@SpringBootApplication
@EnableConfigurationProperties
public class LocalLoanCheckerApplication {

	public static void main(String[] args) {
		SpringApplication.run(LocalLoanCheckerApplication.class, args);
	}

}

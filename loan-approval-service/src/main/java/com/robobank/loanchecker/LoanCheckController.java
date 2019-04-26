package com.robobank.loanchecker;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
public class LoanCheckController {

  private static final Logger LOGGER = LoggerFactory.getLogger(LoanCheckController.class);

  @Value("${max-loan-amount:0}")
  Long maxLoanAmount;

  @Value("${loans.blacklist}")
  List<String> blacklist;

  @PostMapping("/check")
  public LoanApplication checkLoanApplication(@RequestBody LoanApplication application){

    LOGGER.info("The maximum loan is: {}", maxLoanAmount);
    LOGGER.info("These people are barred: {}", blacklist);
    LOGGER.info("Received the LoanApplication: {}", application);

    if(null == application.getStatus() || !application.getStatus().equals(Statuses.PENDING)){
      RuntimeException e = new IllegalStateException("The application must have a Status of PENDING, yours was: " + application.getStatus());
      LOGGER.error(e.getMessage(), e);
      throw e;
    }

    if (application.getAmount() > maxLoanAmount){
      application.setStatus(Statuses.REJECTED.name());
      return application;
    }

    if (blacklist.contains(application.getName().toLowerCase())){
      application.setStatus(Statuses.REJECTED.name());
      return application;
    }

    application.setStatus(Statuses.APPROVED.name());
    return application;

  }



}

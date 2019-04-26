package com.robobank.loanchecker;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
public class LoanCheckController {

  private static final Logger LOGGER = LoggerFactory.getLogger(LoanCheckController.class);

  private LoanCheckService checkingService;

  @Autowired
  public LoanCheckController(LoanCheckService checkingService) {
    this.checkingService = checkingService;
  }

  @PostMapping("/check")
  public LoanApplication checkLoanApplication(@RequestBody LoanApplication application){
    LOGGER.info("Received the LoanApplication: {}", application);
    return checkingService.validateLoan(application);
  }

  @GetMapping("/statuses")
  public List<LoanApplication> getAllStatuses(){
    return checkingService.getAll();
  }

}
